using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.Common.Exceptions;
using HealthUrWelath.Application.Notifications.Interfaces;
using HealthUrWelath.Application.Notifications.Models;
using HealthUrWelath.Application.Notifications.Templates;
using HealthUrWelath.Application.Orders.Interfaces;
using HealthUrWelath.Application.Payments.Interfaces;
using HealthUrWelath.Application.Users.Interfaces;
using MediatR;
using Microsoft.Data.SqlClient;
using System.Numerics;
using System.Text;

namespace HealthUrWelath.Application.Payments.Commands
{
    public static class ConfirmOnlinePayment
    {
        public sealed record Command(
    long PaymentTransactionId,
    string GatewayTransactionId,
    string PaymentMode
) : IRequest<long>;

        public sealed class Handler
        : IRequestHandler<Command, long>
        {
            private readonly IPaymentRepository _repo;
            private readonly IUserRepository _userRepo;
            private readonly IOrderRepository _orderRepo;

            private readonly IUserContext _user;
            private readonly INotificationService _notify;

            public Handler(IPaymentRepository repo,
                    IUserContext user, INotificationService notify, IUserRepository userRepo, IOrderRepository orderRepo)
            {
                _repo = repo;
                _user = user;
                _notify = notify;
                _userRepo = userRepo;
                _orderRepo = orderRepo;
            }

            public async Task<long> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                long PaymentTransactionId;
                try
                {
                    // Confirm Payment
                    PaymentTransactionId = await _repo.ConfirmOnlinePaymentAsync(
                           _user.UserId,
                           request.PaymentTransactionId,
                           request.GatewayTransactionId,
                           request.PaymentMode
                       );
                }
                catch (SqlException ex) when (ex.Number == 50000)
                {
                    // SP_Payment_ConfirmOnlineSuccess raises this when no matching Pending
                    // row exists for this id - already confirmed elsewhere, or an unknown/
                    // stale id. A clean 409 instead of an unhandled 500.
                    throw new AppException(ex.Message, 409);
                }

                // Get user
                var user = await _userRepo.GetByIdAsync(
                   _user.UserId);

                //Get Order Details 
                var orderDetails = await _orderRepo.GetOrdersDetails(PaymentTransactionId, _user.UserId);

                var productHtml = new StringBuilder();

                foreach (var item in orderDetails.Items)
                {
                    var gstHtml = new StringBuilder();

                    var gstAmount = item.GST;

                    if (orderDetails.ShippingAddress.StateName?.Trim().Equals("telangana", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        gstHtml.Append($"CGST: {gstAmount / 2:0.00} <br/> SGST: {gstAmount / 2:0.00}");
                    }
                    else
                    {
                        gstHtml.Append($"IGST: {gstAmount:0.00}");
                    }

                    productHtml.Append($@"
                                    <tr>
                                        <td>{item.ProductName}</td>
                                        <td>Qty: {item.Quantity}</td>
                                        <td>{gstHtml}</td>
                                        <td>₹ {item.Total:0.00}</td>
                                    </tr>
                                ");
                }

                string promoAmount = orderDetails.Order.CouponDiscount.ToString("0.00");

                string total = (orderDetails.Items.Sum(x => x.Total)).ToString("0.00");

                string shipping = orderDetails.Order.ShippingCharges.ToString("0.00");
                string grandTotal = orderDetails.Order.GrandTotal.ToString("0.00");

                // Notify user
                await _notify.SendAsync(new NotificationRequest
                {
                    Subject = $"Order Confirmation - Your Order with Healthurwealth.com [{PaymentTransactionId}] has been successfully placed!",
                    EmailTemplateKey = EmailTemplateKeys.PaymentSuccess,
                    SmsTemplateKey = SmsTemplateKeys.PaymentSuccess,
                    Mobile = user.MobileNo,
                    Email = user.EmailId,
                    Channel = NotificationChannel.Email,
                    Tokens =
                        {
                            ["FirstName"] = user.FirstName,
                            ["PaymentMode"] = orderDetails.Order.PaymentMode,
                            ["OrderNo"] = PaymentTransactionId.ToString(),

                            ["ProductList"] = productHtml.ToString(),
                            ["Total"] = total,
                            ["ShippingCharges"] = shipping,
                            ["PromocodeAmount"] = promoAmount,
                            ["GrandTotal"] = grandTotal,

                            ["StreetAddress1"] = orderDetails.ShippingAddress.StreetAddress1,
                            ["StreetAddress2"] = orderDetails.ShippingAddress.StreetAddress2,
                            ["LandMark"] = orderDetails.ShippingAddress.LandMark,
                            ["City"] = orderDetails.ShippingAddress.City,
                            ["State"] = orderDetails.ShippingAddress.StateName,
                            ["PinCode"] = orderDetails.ShippingAddress.PinCode,
                        }
                });

                return PaymentTransactionId;
            }
        }
    }
}