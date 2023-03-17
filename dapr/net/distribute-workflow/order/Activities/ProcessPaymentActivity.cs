using Dapr.Client;
using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using OrderApp.Models;

namespace OrderApp.Activities
{
    public class ProcessPaymentActivity : WorkflowActivity<PaymentRequest, object>
    {
        readonly ILogger logger;

        public ProcessPaymentActivity(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<ProcessPaymentActivity>();
        }

        public override async Task<object> RunAsync(WorkflowActivityContext context, PaymentRequest req)
        {
            this.logger.LogInformation(
                "Processing payment: {requestId} for {amount} {item} at ${currency}",
                req.RequestId,
                req.Amount,
                req.ItemName,
                req.Currency);

            this.logger.LogInformation(
                "Payment for request ID '{requestId}' processed successfully",
                req.RequestId);

            return null;
        }
    }
}
