/*
    Fixes two confirmed IDOR (insecure direct object reference) gaps found in the
    order-details flow, where an authenticated user could view another user's
    order data just by guessing/incrementing a numeric order id:

      1. SP_Order_GetStatusTimeline had no UserId filter at all — any logged-in
         user could read any other user's order status timeline.
      2. SP_UserOrderDetails filtered the order summary by UserId, but its
         shipping-address and order-items result sets filtered only by
         PaymentTransactionId — so a request for someone else's orderId
         returned Order = null but a real shipping address and item list
         belonging to the victim.

    Safe to re-run (CREATE OR ALTER).
*/

CREATE OR ALTER PROCEDURE [dbo].[SP_Order_GetStatusTimeline]
(
    @PaymentTransactionId BIGINT,
    @UserId                BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PaymentTransactionId,

        CreatedOn AS OrderPlacedDate,

        CASE WHEN Authorized = 1
             THEN CreatedOn
        END AS ProcessingDate,

        CASE WHEN Dispatched = 1
             THEN DispatchedDate
        END AS DispatchedDate,

        CASE WHEN Delivered = 1
             THEN DeliveredDate
        END AS DeliveredDate,

        CASE WHEN TxnStatus = 'Transaction Failed'
             THEN UpdatedOn
        END AS CancelledDate,

        OrderCurrentStatus

    FROM PaymentTransactions
    WHERE PaymentTransactionId = @PaymentTransactionId
      AND UserId = @UserId;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_UserOrderDetails]
(
    @PaymentTransactionId BIGINT,
    @UserId BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    /* ----------------------------------------------------
       1️⃣ ORDER SUMMARY
    ---------------------------------------------------- */

    SELECT
        PT.PaymentTransactionId AS OrderId,
        PT.CreatedOn            AS OrderDate,

        --------------------------------------------------
        -- PAYMENT
        --------------------------------------------------
        PT.PaymentMode,
        PT.TxnStatus,

        --------------------------------------------------
        -- FINAL PAYABLE AMOUNT
        -- (Already includes Shipping & Coupon)
        --------------------------------------------------
        ISNULL(PT.TxnAmount, 0)       AS GrandTotal,

        --------------------------------------------------
        -- SHIPPING
        --------------------------------------------------
        ISNULL(PT.ShippingCharges, 0) AS ShippingCharges,

        --------------------------------------------------
        -- GST SUMMARY
        --------------------------------------------------
        ISNULL(GSTAgg.CGST, 0) AS CGST,
        ISNULL(GSTAgg.SGST, 0) AS SGST,
        ISNULL(GSTAgg.IGST, 0) AS IGST,
        ISNULL(GSTAgg.GST, 0)  AS GST,

        --------------------------------------------------
        -- PRODUCT DISCOUNT
        --------------------------------------------------
        ISNULL(PDAgg.ProductDiscount, 0) AS ProductDiscount,

        --------------------------------------------------
        -- COUPON DISCOUNT
        --------------------------------------------------
        ISNULL(PT.Promo_Code_Amount, 0) AS CouponDiscount,

        --------------------------------------------------
        -- TOTAL SAVINGS
        --------------------------------------------------
        ISNULL(PDAgg.ProductDiscount, 0)
        + ISNULL(PT.Promo_Code_Amount, 0) AS TotalDiscount,

        --------------------------------------------------
        -- TAMPER FLAG
        --------------------------------------------------
        ISNULL(PT.IsCartTampered, 0) AS IsCartTampered

    FROM PaymentTransactions PT

    --------------------------------------------------
    -- GST AGGREGATION
    --------------------------------------------------
    OUTER APPLY
    (
        SELECT
            SUM(ISNULL(UPT.CGST,0))      AS CGST,
            SUM(ISNULL(UPT.SGST,0))      AS SGST,
            SUM(ISNULL(UPT.IGST,0))      AS IGST,
            SUM(ISNULL(UPT.GST_Amount,0)) AS GST
        FROM UserProductTransactions UPT
        WHERE UPT.PaymentTransactionId = PT.PaymentTransactionId
          AND UPT.IsDeleted = 0
    ) GSTAgg

    --------------------------------------------------
    -- PRODUCT DISCOUNT AGGREGATION
    --------------------------------------------------
    OUTER APPLY
    (
        SELECT
            SUM(
                ISNULL(UPT.ProductDiscountCost,0)
                * ISNULL(UPT.Quantity,1)
            ) AS ProductDiscount
        FROM UserProductTransactions UPT
        WHERE UPT.PaymentTransactionId = PT.PaymentTransactionId
          AND UPT.IsDeleted = 0
    ) PDAgg

    WHERE PT.PaymentTransactionId = @PaymentTransactionId
      AND PT.UserId = @UserId;


    /* ----------------------------------------------------
       2️⃣ SHIPPING ADDRESS
       (IDOR fix: only return the address when this order
        actually belongs to @UserId)
    ---------------------------------------------------- */

    SELECT TOP 1
        UA.UserAddressId AS ShippingAddressId,

        UA.StreetAddress1,
        UA.StreetAddress2,
        UA.LandMark,

        UA.City,
        UA.StateName,
        UA.CountryId,

        UA.PinCode

    FROM UserProductTransactions UPT

    INNER JOIN UserAddresses UA
        ON UA.UserAddressId = UPT.ShippingAddressId

    WHERE UPT.PaymentTransactionId = @PaymentTransactionId
      AND EXISTS (
          SELECT 1 FROM PaymentTransactions PT
          WHERE PT.PaymentTransactionId = UPT.PaymentTransactionId
            AND PT.UserId = @UserId
      );


    /* ----------------------------------------------------
       3️⃣ ORDER ITEMS
       (IDOR fix: only return items when this order
        actually belongs to @UserId)
    ---------------------------------------------------- */

    SELECT
        --------------------------------------------------
        -- PRODUCT
        --------------------------------------------------
        P.ProductId,
        P.ProductName,
        P.ProductImgUrl,

        --------------------------------------------------
        -- QUANTITY
        --------------------------------------------------
        UPT.Quantity,

        --------------------------------------------------
        -- ORIGINAL PRICE (MRP)
        --------------------------------------------------
        UPT.ProductOriginalCost AS OriginalPrice,

        --------------------------------------------------
        -- FINAL UNIT PRICE
        --------------------------------------------------
               CASE
        WHEN ISNULL(UPT.Quantity,0) > 0
        THEN ISNULL(UPT.ProductCost,0) / UPT.Quantity
        ELSE ISNULL(UPT.ProductCost,0)
        END AS Price,


        --------------------------------------------------
        -- PRODUCT DISCOUNT
        --------------------------------------------------
        ISNULL(UPT.ProductDiscountPercentage,0)
            AS ProductDiscountPercentage,

        ISNULL(UPT.ProductDiscountCost,0)
            AS ProductDiscountPerUnit,

        (
            ISNULL(UPT.ProductDiscountCost,0)
            * ISNULL(UPT.Quantity,1)
        ) AS ProductDiscountTotal,

        --------------------------------------------------
        -- COUPON DISCOUNT
        --------------------------------------------------
        ISNULL(UPT.CouponDiscountAmount,0)
            AS CouponDiscount,

        --------------------------------------------------
        -- GST
        --------------------------------------------------
        ISNULL(UPT.GST_Percentage,0)
            AS GSTPercentage,

        ISNULL(UPT.GST_Amount,0)
            AS GST,

        ISNULL(UPT.CGST,0)
            AS CGST,

        ISNULL(UPT.SGST,0)
            AS SGST,

        ISNULL(UPT.IGST,0)
            AS IGST,

        --------------------------------------------------
        -- FINAL LINE TOTAL
        -- (After Coupon Discount)
        --------------------------------------------------
        (
        ISNULL(UPT.ProductCost,0)
        - ISNULL(UPT.CouponDiscountAmount,0)
        ) AS Total,


        --------------------------------------------------
        -- DELIVERY
        --------------------------------------------------
        UPT.ExpectedDeliveryDate

    FROM UserProductTransactions UPT

    INNER JOIN Products P
        ON P.ProductId = UPT.ProductId

    WHERE UPT.PaymentTransactionId = @PaymentTransactionId
      AND UPT.IsDeleted = 0
      AND EXISTS (
          SELECT 1 FROM PaymentTransactions PT
          WHERE PT.PaymentTransactionId = UPT.PaymentTransactionId
            AND PT.UserId = @UserId
      );

END
GO
