using System.IO;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;

namespace API.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _autoMapper;
        private readonly ILogger<PaymentController> _logger;
        private const string WebHooksSecret = "";

        public PaymentController(IPaymentService paymentService, IMapper autoMapper,
                                 ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _autoMapper = autoMapper;
            this._logger = logger;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket == null) return BadRequest(new APiResponse(400, "Problem with ur basket"));
          
            return Ok(basket);
        }

        [HttpPost("webHook")]
        public async Task<ActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WebHooksSecret);
            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded:", intent.Id);
                    // update order status
                    order = await _paymentService.UpdateOrderOnPaymentSucceeded(intent.Id);
                    _logger.LogInformation("Order updated to payment received", order?.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent=(PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed:", intent.Id);
                    // update order status
                    order = await _paymentService.UpdateOrderOnPaymentFailed(intent.Id);
                    _logger.LogInformation("Order updated to payment failed", order?.Id);
                    break;
            }

            return new EmptyResult(); // we need to confirm to stripe that we receive that event
        }
    }
}
