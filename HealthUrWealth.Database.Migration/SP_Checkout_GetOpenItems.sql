USE [staging_huw]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Returns the checkout snapshot items for the current OPEN checkout.
-- The frontend MUST use this instead of GET /cart on the checkout page
-- so that product list and totals are always from the same snapshot.
CREATE OR ALTER PROCEDURE [dbo].[SP_Checkout_GetOpenItems]
(
    @UserId BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Find the open checkout for this user
    DECLARE @CheckoutTxnId BIGINT;

    SELECT TOP 1 @CheckoutTxnId = PaymentTransactionId
    FROM CheckOutPaymentTransactions
    WHERE UserId = @UserId
      AND OrderStatus = 'Open'
    ORDER BY CreatedOn DESC;

    IF @CheckoutTxnId IS NULL
    BEGIN
        -- No open checkout — return empty result set with correct columns
        SELECT
            CAST(0 AS BIGINT)       AS ProductId,
            CAST('' AS NVARCHAR(500)) AS ProductName,
            CAST(NULL AS NVARCHAR(500)) AS ProductImgUrl,
            CAST(0 AS INT)          AS Quantity,
            CAST(0 AS DECIMAL(18,2)) AS UnitPrice,
            CAST(0 AS DECIMAL(18,2)) AS TotalPrice,
            CAST(0 AS DECIMAL(18,2)) AS DiscountPercentage,
            CAST(0 AS DECIMAL(18,2)) AS DiscountPerUnit
        WHERE 1 = 0;
        RETURN;
    END

    SELECT
        cup.ProductId,
        p.ProductName,
        p.ProductImgUrl,
        cup.Quantity,
        cup.ProductCost                          AS UnitPrice,
        (cup.ProductCost * cup.Quantity)         AS TotalPrice,
        ISNULL(cup.ProductDiscountPercentage, 0) AS DiscountPercentage,
        ISNULL(cup.ProductDiscountCost, 0)       AS DiscountPerUnit
    FROM CheckOutUserProductTransactions cup
    INNER JOIN Products p ON p.ProductId = cup.ProductId
    WHERE cup.PaymentTransactionId = @CheckoutTxnId;
END
GO
