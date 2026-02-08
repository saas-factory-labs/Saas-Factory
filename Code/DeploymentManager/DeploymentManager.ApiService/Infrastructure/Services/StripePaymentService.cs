//namespace DeploymentPortal.ApiService.Infrastructure.Services
//{
//    public class StripePaymentService
//{
//        private readonly StripeConfiguration _stripeConfiguration;
//    private readonly ILogger<StripePaymentService> _logger;

//    public StripePaymentService(StripeConfiguration stripeConfiguration, ILogger<StripePaymentService> logger)
//        {
//        _stripeConfiguration = stripeConfiguration;
//        _logger = logger;
//    }

//    public async Task<StripePaymentResponse> Charge(StripePaymentRequest request)
//        {
//        try
//            {
//            var options = new ChargeCreateOptions
//            {
//                Amount = request.Amount,
//                Currency = request.Currency,
//                Source = request.Token,
//                Description = request.Description
//            };

//            var service = new ChargeService();
//                service.ApiKey = _stripe
//            };
//        }
//}
//}



