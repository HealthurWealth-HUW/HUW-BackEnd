ALTER TABLE CheckOutUserProductTransactions
ADD ExpectedDeliveryDate DATETIME NULL;


ALTER TABLE UserProductTransactions
ADD ExpectedDeliveryDate DATETIME NULL;

ALTER TABLE UserProductTransactions
ADD
    GST_Percentage DECIMAL(5,2),
    GST_Amount DECIMAL(18,2),
    CGST DECIMAL(18,2),
    SGST DECIMAL(18,2),
    IGST DECIMAL(18,2),
    BaseAmount DECIMAL(18,2);


ALTER TABLE Cart
ADD 
    Has_Promo_Code BIT NOT NULL 
        CONSTRAINT DF_Cart_HasPromoCode DEFAULT 0,
    Promo_Code_ID BIGINT NULL,
    Promo_Code_Amount DECIMAL(10,2) NULL;


ALTER TABLE Cart
ADD CONSTRAINT FK_Cart_Coupon
FOREIGN KEY (Promo_Code_ID)
REFERENCES Tbl_Coupon_Info (Coupon_Id);

ALTER TABLE UserProductTransactions
ADD ProductOriginalCost DECIMAL(18,2);

--Sproc changes for expected delivery date

SP_Coupon_ValidateAndApply

SP_Checkout_UpsertAndCalculate

SP_Checkout_ConfirmCOD

SP_Payment_ConfirmOnlineSuccess

SP_GetCouponByCode

SP_Cart_ApplyCoupon

fn_GetShippingCharge

fn_CalculateGST

SP_UserOrderDetails

