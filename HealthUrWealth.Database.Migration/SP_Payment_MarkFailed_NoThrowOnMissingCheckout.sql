/*
    SP_Payment_MarkFailed previously raised RAISERROR('Invalid or already processed
    checkout', 16, 1) when no matching Open checkout was found for the given
    UserId/PaymentTransactionId. That RAISERROR surfaced to the API as an unhandled
    Microsoft.Data.SqlClient.SqlException (500), even though the payment gateway
    calling /api/v1/Payments/failed for an already-processed or unknown checkout is
    an expected, non-fatal condition (duplicate callback, stale txn id, race with
    another request).

    Fix: no longer RAISERROR. Instead return NULL for PaymentTransactionId so the
    application layer can log it and return success without treating it as an error.

    Safe to re-run (CREATE OR ALTER).
*/

CREATE OR ALTER PROCEDURE [dbo].[SP_Payment_MarkFailed]
(
    @UserId BIGINT,
    @PaymentTransactionId BIGINT,
    @PaymentMode NVARCHAR(50),
    @PGTxnId NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @FinalPaymentTxnId BIGINT;

        /* 1) Validate checkout exists and is still open.
              Not an error condition - just nothing to mark failed. */
        IF NOT EXISTS (
            SELECT 1
            FROM CheckOutPaymentTransactions
            WHERE PaymentTransactionId = @PaymentTransactionId
              AND UserId = @UserId
              AND OrderStatus = 'Open'
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT CAST(NULL AS BIGINT) AS PaymentTransactionId;
            RETURN;
        END

        /* 2) Insert failed payment record */
        INSERT INTO PaymentTransactions
        (
            UserId,
            PaymentMode,
            TxnStatus,
            TxnMessage,
            PaymentStatus,
            CreatedOn,
            PGTxnId,
            CurrencyCode,
            CurrencySymbol,
            CurrencyValue,
            TxnAmount
        )
        SELECT
            UserId,
            @PaymentMode,
            'FAILED',
            'Transaction Failed through ' + @PaymentMode,
            3, -- legacy canceled enum
            SYSDATETIME(),
            @PGTxnId,
            CurrencyCode,
            CurrencySymbol,
            CurrencyValue,
            TxnAmount
        FROM CheckOutPaymentTransactions
        WHERE PaymentTransactionId = @PaymentTransactionId;

        SET @FinalPaymentTxnId = SCOPE_IDENTITY();

        /* 3) Update checkout status */
        UPDATE CheckOutPaymentTransactions
        SET
            OrderStatus = 'Failed',
            UpdatedOn = SYSDATETIME()
        WHERE PaymentTransactionId = @PaymentTransactionId;

        COMMIT TRANSACTION;

        SELECT @FinalPaymentTxnId AS PaymentTransactionId;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
