ALTER TABLE Tbl_Presecription_Info
ADD CartId BIGINT NULL;

Modified StoredProcedures:
=================================
[SP_Payment_ConfirmOnlineSuccess]

[SP_Checkout_ConfirmCOD]

SP_Cart_GetItems

SP_Checkout_GetSummary


--------------------------------------------------
-- PaymentTransactions
--------------------------------------------------
IF COL_LENGTH('PaymentTransactions', 'IsCartTampered') IS NULL
BEGIN
    ALTER TABLE PaymentTransactions
    ADD IsCartTampered BIT NOT NULL 
        CONSTRAINT DF_Payment_IsCartTampered DEFAULT 0;
END
GO

--------------------------------------------------
-- CheckOutPaymentTransactions
--------------------------------------------------
IF COL_LENGTH('CheckOutPaymentTransactions', 'IsCartTampered') IS NULL
BEGIN
    ALTER TABLE CheckOutPaymentTransactions
    ADD IsCartTampered BIT NOT NULL 
        CONSTRAINT DF_Checkout_IsCartTampered DEFAULT 0;
END
GO

UPDATE dbo.PaymentTransactions
SET IsCartTampered = 0
WHERE IsCartTampered IS NULL;


UPDATE dbo.CheckOutPaymentTransactions
SET IsCartTampered = 0
WHERE IsCartTampered IS NULL;