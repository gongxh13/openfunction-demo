using Dapr.Workflow;
using Dapr.Client;
using DurableTask.Core.Exceptions;
using OrderApp.Activities;
using OrderApp.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace OrderApp.Workflows
{
    public class OrderWorkflow : Workflow<OrderPayload, OrderResult>
    {
        ILogger logger;
        WorkflowEngineClient workflowEngineClient;

        public OrderWorkflow()
        {
            this.logger = loggerFactory.CreateLogger<UpdateInventoryActivity>();
            var builder = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {});
            using var host = builder.Build();
            this.workflowEngineClient = host.Services.GetRequiredService<WorkflowEngineClient>();
        }

        public OrderWorkflow(ILoggerFactory loggerFactory, WorkflowEngineClient workflowEngineClient)
        {
            this.logger = loggerFactory.CreateLogger<UpdateInventoryActivity>();
            this.workflowEngineClient = workflowEngineClient;
        }

        public override async Task<OrderResult> RunAsync(WorkflowContext context, OrderPayload order)
        {
            string orderId = context.InstanceId;

            // Notify the user that an order has come through
            await context.CallActivityAsync(
                nameof(NotifyActivity),
                new Notification($"Received order {orderId} for {order.Quantity} {order.Name} at ${order.TotalCost}"));
            

            // Determine if there is enough of the item available for purchase by checking the inventory
            // InventoryResult result = await context.CallActivityAsync<InventoryResult>(
            //     nameof(ReserveInventoryActivity),
            //     new InventoryRequest(RequestId: orderId, order.Name, order.Quantity));
            
            // call inventory workflow 
            string result = await workflowEngineClient.ScheduleNewWorkflowAsync(
                name: "InventoryWorkflow",
                instanceId: orderId,
                input: order);
            this.logger.LogInformation(
                "Checking inventory for order '{orderId}' result is {result}",
                orderId,
                result);
            // If there is insufficient inventory, fail and let the user know 
            // if (!result.Success)
            // {
            //     // End the workflow here since we don't have sufficient inventory
            //     await context.CallActivityAsync(
            //         nameof(NotifyActivity),
            //         new Notification($"Insufficient inventory for {order.Name}"));
            //     return new OrderResult(Processed: false);
            // }

            // There is enough inventory available so the user can purchase the item(s). Process their payment
            await context.CallActivityAsync(
                nameof(ProcessPaymentActivity),
                new PaymentRequest(RequestId: orderId, order.Name, order.Quantity, order.TotalCost));

            try
            {
                // There is enough inventory available so the user can purchase the item(s). Process their payment
                await context.CallActivityAsync(
                    nameof(UpdateInventoryActivity),
                    new PaymentRequest(RequestId: orderId, order.Name, order.Quantity, order.TotalCost));                
            }
            catch (TaskFailedException)
            {
                // Let them know their payment was processed
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Order {orderId} Failed! You are now getting a refund"));
                return new OrderResult(Processed: false);
            }

            // Let them know their payment was processed
            await context.CallActivityAsync(
                nameof(NotifyActivity),
                new Notification($"Order {orderId} has completed!"));

            // End the workflow with a success result
            return new OrderResult(Processed: true);
        }
    }
}
