using HealthUrWealth.Api.Contracts.Auth;
using HealthUrWelath.Application.Authentication.Commands;
using HealthUrWelath.Application.Authentication.Dtos;
using HealthUrWelath.Application.Payments.Commands;
using HealthUrWelath.Application.Payments.Dtos;
using HealthUrWelath.Application.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("failed")]
        public async Task<IActionResult> PaymentFailed(
            [FromBody] PaymentFailedDto request)
        {
            await _mediator.Send(
                new PaymentFailed.Command(
                    request.PaymentTransactionId,
                    request.PaymentMode,
                    request.GatewayTransactionId
                )
            );

            return Ok(new
            {
                Status = "FAILED",
                Message = "Payment marked as failed"
            });
        }

        [HttpPost("success")]
        public async Task<IActionResult> ConfirmSuccess(
    [FromBody] PaymentSuccessRequestDto request)
        {
            var finalTxnId = await _mediator.Send(
                new ConfirmOnlinePayment.Command(
                    request.PaymentTransactionId,
                    request.GatewayTransactionId,
                    request.PaymentMode
                ));

            return Ok(new
            {
                Message = "Payment Successful",
                PaymentTransactionId = finalTxnId
            });
        }

    //    [HttpPost("success")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> PayuSuccessAsync()
    //    {
    //        var form = Request.Form;

    //        var status = form["status"].ToString();
    //        var txnid = form["txnid"].ToString();
    //        var mihpayid = form["mihpayid"].ToString(); // Gateway Transaction ID
    //        var amount = form["amount"].ToString();
    //        var email = form["email"].ToString();
    //        var firstname = form["firstname"].ToString();
    //        var productinfo = form["productinfo"].ToString();
    //        var receivedHash = form["hash"].ToString();

    //        Your config values
    //       var key = _configuration["PayU:Key"];
    //        var salt = _configuration["PayU:Salt"];

    //        Always format amount exactly like request
    //        var formattedAmount = Convert.ToDecimal(amount)
    //            .ToString("0.00", CultureInfo.InvariantCulture);

    //        Build reverse hash sequence
    //        var reverseHashSequence = string.Join("|", new[]
    //        {
    //    salt,
    //    status,
    //    "", "", "", "", "", "", "", "", "", "",  // 10 empty fields
    //    email?.Trim(),
    //    firstname?.Trim(),
    //    productinfo?.Trim(),
    //    formattedAmount,
    //    txnid,
    //    key
    //});

    //        Generate SHA512
    //        using var sha512 = SHA512.Create();
    //        var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(reverseHashSequence));
    //        var calculatedHash = BitConverter.ToString(hashBytes)
    //            .Replace("-", "")
    //            .ToLower();

    //        Compare
    //        if (calculatedHash != receivedHash.ToLower())
    //        {
    //            Hash mismatch → possible tampering
    //            return BadRequest("Invalid hash");
    //        }

    //        Hash verified ✔
    //         Save transaction to DB here
    //        var finalTxnId = await _mediator.Send(
    //            new ConfirmOnlinePayment(
    //               Convert.ToInt64(txnid),
    //               mihpayid,
    //                "Payumoney"
    //            ));

    //        var redirectUrl = $"https://staging.healthurwealth.com/Home/Processtransaction?txnid={12345}&gtid={23455}&status={"success"}";

    //        return Content("PAYU SUCCESS RECEIVED");

    //        return Redirect(redirectUrl); // THIS sends 302
    //    }

        [HttpPost("payu/initiate")]
        public async Task<IActionResult> InitiatePayUPayment()
        {
            var result = await _mediator.Send(new InitiatePayU.Query());
            return Ok(result);
        }
    }
}
