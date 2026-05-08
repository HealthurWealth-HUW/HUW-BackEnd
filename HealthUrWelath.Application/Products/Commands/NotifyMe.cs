using HealthUrWelath.Application.Notifications.Interfaces;
using HealthUrWelath.Application.Notifications.Models;
using HealthUrWelath.Application.Notifications.Templates;
using MediatR;
using static System.Net.WebRequestMethods;

namespace HealthUrWelath.Application.Products.Commands
{
    public class NotifyMe
    {
        public sealed record Command(
            long ProductId,
            string Name,
            string MobileNumber,
            string Email
        ) : IRequest;

        public sealed class Handler : IRequestHandler<Command>
        {
            private readonly IProductReadRepository _productRepo;
            private readonly INotificationService _notify;

            public Handler(
                IProductReadRepository productRepo, INotificationService notify)
            {
                _productRepo = productRepo;
                _notify = notify;
            }

            public async Task Handle(Command request, CancellationToken ct)
            {
                var mobile = Convert.ToInt64(request.MobileNumber);

                await _productRepo.NotifyMeInsertAsync(
                    request.ProductId,
                    request.Name,
                    mobile,
                    request.Email);

                var product = await _productRepo.GetProductByIdAsync(request.ProductId);

                var NotifymeRequest = new NotificationRequest
                {
                    EmailTemplateKey = EmailTemplateKeys.NotifyMe_Admin,
                    //SmsTemplateId = SmsTemplateIds.OtpLogin,
                   // Mobile = user.Mobile,
                   // Email = user.Email,
                    Channel = NotificationChannel.Both,
                    Tokens =
                    {
                    //    ["FirstName"] = user.FirstName,
                     //   ["OTP"] = otp
                    }
                };
                await _notify.SendAsync(NotifymeRequest);

               //await _email.SendNotifyMeMailAsync(
               //     request.Name,
               //     request.MobileNumber,
               //     product.ProductName,
               //     product.ProductImgUrl,
               //     product.ProductCost);
            }
        }
    }
}
