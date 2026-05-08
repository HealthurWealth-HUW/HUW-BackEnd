ALTER TABLE Cart
ADD CartVersion INT DEFAULT 1;

ALTER TABLE CheckOutPaymentTransactions
ADD CartVersion INT ;


[SP_Cart_UpdateQuantity]

[SP_Cart_RemoveItem]

[SP_Cart_ApplyCoupon] 

[SP_Cart_AddOrUpdateItem] 

-- Fix Cart table
UPDATE Cart
SET CartVersion = 1
WHERE CartVersion IS NULL;

-- Fix Checkout table
UPDATE CheckOutPaymentTransactions
SET CartVersion = 1
WHERE CartVersion IS NULL;