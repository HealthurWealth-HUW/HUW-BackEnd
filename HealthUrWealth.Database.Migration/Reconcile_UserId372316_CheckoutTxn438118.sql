/*
    MANUAL RECONCILIATION - single customer, single order.
    -----------------------------------------------------
    Incident: UserId 372316 (Ravi Kumar, mobile 8897131448) paid via Razorpay
    (UPI) on 2026-07-12 14:23 IST - payment captured, pay_TCX3OdTpNUQCPz,
    Bank RRN 224650994957, amount INR 1,104.00 - but the browser never called
    back to POST /api/v1/Payments/success, so PaymentTransactions /
    UserProductTransactions were never written. The checkout snapshot
    (PaymentTransactionId 437609) sat OrderStatus='Open' for 3 days untouched,
    then got marked 'Stale' on 2026-07-15 when the user reopened checkout,
    which created a fresh, still-open snapshot: PaymentTransactionId 438118
    (same cart, same items, same amount - verified against CheckOutUserProductTransactions).

    This script closes out checkout 438118 exactly the way
    SP_Payment_ConfirmOnlineSuccess would have on 2026-07-12, using the real
    Razorpay payment as the gateway reference, and marks the order as a
    reconciled/manual entry in Comments for audit purposes.

    Verified before writing this script (read-only queries against
    db_Zon_Huw_Live):
      - CheckOutPaymentTransactions: PaymentTransactionId 438118, UserId 372316,
        OrderStatus = 'Open', TxnAmount = 1104.00, PaymentMode = 'Online'.
      - CheckOutUserProductTransactions: ProductId 93378 ("Kl Glow Rich Foaming
        FaceWash"), Quantity 3, ProductCost 368.00 => 3 * 368 = 1104.00 (matches
        the Razorpay captured amount exactly).
      - Cart 2784: still Status = 'ACTIVE' (never checked out) - CartItem still
        has ProductId 93378 x 3.
      - No row anywhere in PaymentTransactions references pay_TCX3OdTpNUQCPz or
        RRN 224650994957 (confirmed via LIKE search across the whole table).

    Safe to review and run once. NOT idempotent by design - the guard block
    below aborts cleanly if checkout 438118 is no longer in the expected state
    (e.g. someone already reconciled it, or the customer somehow completed
    checkout again through the app), so it will not double-insert if re-run
    after already succeeding once (OrderStatus will no longer be 'Open').

    Product stock (Products.Quantity / IsSold) is intentionally NOT touched -
    see the commented-out section near the bottom. Current live data already
    shows Quantity = 0 / IsSold = 0 for ProductId 93378, which appears to be
    unrelated existing stock state, not something this script should override
    without the merchandising team's input. Decide separately and rerun as
    a distinct action if a decrement is actually required.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE
    @UserId               BIGINT       = 372316,
    @CheckoutTxnId        BIGINT       = 438118,
    @GatewayTransactionId NVARCHAR(200) = 'pay_TCX3OdTpNUQCPz',
    @BankRRN              NVARCHAR(100) = '224650994957',
    @PaymentMode          NVARCHAR(50)  = 'Online',
    @ExpectedTxnAmount    DECIMAL(18,2) = 1104.00;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE
        @FinalPaymentTxnId BIGINT,
        @CouponId          BIGINT,
        @CouponAmount      DECIMAL(18,2),
        @HasPromo          BIT,
        @SubTotal          DECIMAL(18,2),
        @ShippingCharges   DECIMAL(18,2),
        @FinalAmount       DECIMAL(18,2),
        @ActualTxnAmount   DECIMAL(18,2);

    -----------------------------------------------------------------
    -- Guard: re-validate the checkout is exactly what we inspected.
    -- Abort (no changes) if anything has moved since the investigation.
    -----------------------------------------------------------------
    SELECT
        @CouponId        = Promo_Code_ID,
        @CouponAmount    = ISNULL(Promo_Code_Amount, 0),
        @HasPromo        = ISNULL(Has_Promo_Code, 0),
        @ShippingCharges = ISNULL(ShippingCharges, 0),
        @ActualTxnAmount = TxnAmount
    FROM CheckOutPaymentTransactions
    WHERE PaymentTransactionId = @CheckoutTxnId
      AND UserId = @UserId
      AND OrderStatus = 'Open';

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Aborting: checkout %d for UserId %d is no longer Open. Nothing was changed - re-investigate before re-running.', 16, 1, @CheckoutTxnId, @UserId);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @ActualTxnAmount <> @ExpectedTxnAmount
    BEGIN
        RAISERROR('Aborting: checkout %d amount (%s) does not match the expected Razorpay amount. Nothing was changed - re-investigate before re-running.', 16, 1, @CheckoutTxnId, @ActualTxnAmount);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Subtotal from the checkout snapshot items (mirrors SP_Payment_ConfirmOnlineSuccess step 2)
    SELECT @SubTotal = SUM(ProductCost * Quantity)
    FROM CheckOutUserProductTransactions
    WHERE PaymentTransactionId = @CheckoutTxnId;

    IF @SubTotal IS NULL SET @SubTotal = 0;

    SET @FinalAmount = @SubTotal + @ShippingCharges - @CouponAmount;
    IF @FinalAmount < 0 SET @FinalAmount = 0;

    -----------------------------------------------------------------
    -- Insert Payment (mirrors SP_Payment_ConfirmOnlineSuccess step 5)
    -----------------------------------------------------------------
    INSERT INTO PaymentTransactions
    (
        UserId, PaymentMode, TxnStatus, TxnRefNo, PGTxnId, PaymentStatus, CreatedOn,
        CurrencyCode, CurrencySymbol, CurrencyValue,
        TxnAmount, TxnMessage, Authorized, OrderCurrentStatus,
        ShippingCharges, Has_Promo_Code, Promo_Code_ID, Promo_Code_Amount,
        IsCartTampered, Comments
    )
    SELECT
        UserId, @PaymentMode, 'SUCCESS', @BankRRN, @GatewayTransactionId, 1, CreatedOn, -- CreatedOn kept as the real Jul 12 payment time, not SYSDATETIME()
        CurrencyCode, CurrencySymbol, CurrencyValue,
        @FinalAmount, 'Transaction Successful through Payment Gateway (manual reconciliation)',
        1, 6, @ShippingCharges, @HasPromo, @CouponId,
        CASE WHEN @HasPromo = 1 THEN @CouponAmount ELSE NULL END,
        IsCartTampered,
        'Manually reconciled ' + CONVERT(varchar, SYSDATETIME(), 120) + ' - Razorpay payment captured 2026-07-12 14:23 IST but /api/v1/Payments/success was never invoked (checkout went stale before client confirmation). See incident thread for details.'
    FROM CheckOutPaymentTransactions
    WHERE PaymentTransactionId = @CheckoutTxnId;

    SET @FinalPaymentTxnId = SCOPE_IDENTITY();

    -----------------------------------------------------------------
    -- Move Items (mirrors SP_Payment_ConfirmOnlineSuccess step 6)
    -----------------------------------------------------------------
    INSERT INTO UserProductTransactions
    (
        PaymentTransactionId, UserId, ProductId, SubProductId, Quantity,
        ProductOriginalCost, ProductCost, ProductDiscountPercentage,
        ProductDiscountCost, CouponDiscountAmount,
        ShippingAddressId, BillingAddressId, IsActive, IsDeleted,
        CreatedOn, UpdatedOn, CreatedBy, UpdatedBy, Status,
        CurrencyCode, CurrencyValue, CurrencySymbol,
        OrdersReturnReason, OrdersReturnAction, ReplacementTransactionId,
        IsReplaced, ExpectedDeliveryDate,
        GST_Percentage, GST_Amount, CGST, SGST, IGST, BaseAmount
    )
    SELECT
        @FinalPaymentTxnId, cup.UserId, cup.ProductId, cup.SubProductId, cup.Quantity,
        p.ProductOriginalCost, (cup.ProductCost * cup.Quantity),
        ISNULL(cup.ProductDiscountPercentage, 0),
        ISNULL(cup.ProductDiscountCost, 0),
        CASE
            WHEN ISNULL(chk.Promo_Code_Amount, 0) > 0 AND @SubTotal > 0
            THEN ROUND(((cup.ProductCost * cup.Quantity) / @SubTotal) * chk.Promo_Code_Amount, 2)
            ELSE 0
        END,
        cup.ShippingAddressId, cup.BillingAddressId, 1, 0,
        chk.CreatedOn, SYSDATETIME(), cup.CreatedBy, cup.UpdatedBy, 0, -- CreatedOn kept as the real Jul 12 checkout time
        chk.CurrencyCode, chk.CurrencyValue, chk.CurrencySymbol,
        cup.OrdersReturnReason, cup.OrdersReturnAction,
        cup.ReplacementTransactionId, cup.IsReplaced, cup.ExpectedDeliveryDate,
        p.GST, gst.GST_Amount, gst.CGST, gst.SGST, gst.IGST, gst.BaseAmount
    FROM CheckOutUserProductTransactions cup
    INNER JOIN CheckOutPaymentTransactions chk ON chk.PaymentTransactionId = cup.PaymentTransactionId
    INNER JOIN Products p ON p.ProductId = cup.ProductId
    INNER JOIN UserAddresses ua ON ua.UserAddressId = cup.ShippingAddressId
    CROSS APPLY dbo.fn_CalculateGST((cup.ProductCost * cup.Quantity), p.GST, ua.StateName) gst
    WHERE cup.PaymentTransactionId = @CheckoutTxnId;

    -----------------------------------------------------------------
    -- Close Checkout (mirrors SP_Payment_ConfirmOnlineSuccess step 7)
    -----------------------------------------------------------------
    UPDATE CheckOutPaymentTransactions
    SET OrderStatus = 'Closed', UpdatedOn = SYSDATETIME()
    WHERE PaymentTransactionId = @CheckoutTxnId;

    -----------------------------------------------------------------
    -- Stock decrement - INTENTIONALLY DISABLED. See header note above.
    -- Uncomment only after confirming with the merchandising/inventory
    -- team that this unit should still be deducted from current stock.
    -----------------------------------------------------------------
    -- UPDATE p
    -- SET
    --     p.Quantity = CASE WHEN p.Quantity - cup.Quantity < 0 THEN 0 ELSE p.Quantity - cup.Quantity END,
    --     p.IsSold   = CASE WHEN p.Quantity - cup.Quantity <= 0 THEN 1 ELSE 0 END
    -- FROM Products p
    -- INNER JOIN (
    --     SELECT ProductId, SUM(Quantity) AS Quantity
    --     FROM CheckOutUserProductTransactions
    --     WHERE PaymentTransactionId = @CheckoutTxnId
    --     GROUP BY ProductId
    -- ) cup ON cup.ProductId = p.ProductId;

    -----------------------------------------------------------------
    -- Clear Cart (mirrors SP_Payment_ConfirmOnlineSuccess step 9)
    -- Cart 2784 has been left ACTIVE with these same 3 units since Jul 12.
    -----------------------------------------------------------------
    DELETE ci
    FROM CartItem ci
    INNER JOIN Cart c ON c.CartId = ci.CartId
    WHERE c.UserId = @UserId AND c.Status = 'ACTIVE';

    UPDATE Cart
    SET Status = 'CHECKED_OUT', UpdatedOn = SYSDATETIME()
    WHERE UserId = @UserId AND Status = 'ACTIVE';

    -----------------------------------------------------------------
    -- Attach prescriptions (mirrors SP_Payment_ConfirmOnlineSuccess step 10) - no-op if none
    -----------------------------------------------------------------
    UPDATE Tbl_Presecription_Info
    SET Payment_Transaction_Id = @FinalPaymentTxnId
    WHERE CartId IN (SELECT CartId FROM Cart WHERE UserId = @UserId)
      AND Payment_Transaction_Id IS NULL;

    COMMIT TRANSACTION;

    SELECT
        @FinalPaymentTxnId AS NewPaymentTransactionId,
        @CheckoutTxnId     AS ReconciledCheckoutTxnId,
        @FinalAmount       AS FinalAmount;

    PRINT 'Reconciliation complete. Verify with the SELECTs below before considering this done.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH
GO

-----------------------------------------------------------------
-- Post-run verification - run these manually after the block above
-- to confirm the new rows look right before notifying the customer.
-----------------------------------------------------------------
-- SELECT * FROM PaymentTransactions WHERE UserId = 372316 ORDER BY PaymentTransactionId DESC;
-- SELECT * FROM UserProductTransactions WHERE UserId = 372316 ORDER BY TransactionId DESC;
-- SELECT * FROM CheckOutPaymentTransactions WHERE UserId = 372316 ORDER BY PaymentTransactionId DESC;
-- SELECT * FROM Cart WHERE UserId = 372316;
