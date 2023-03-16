using Dapr.Workflow;
using DurableTask.Core.Exceptions;
using InventoryApp.Activities;
using InventoryApp.Models;

namespace InventoryApp.Workflows
{
    public class InventoryWorkflow : Workflow<OrderPayload, InventoryResult>
    {
        public override async Task<InventoryResult> RunAsync(WorkflowContext context, OrderPayload order)
        {
            string orderId = context.InstanceId;

            // Determine if there is enough of the item available for purchase by checking the inventory
            InventoryResult result = await context.CallActivityAsync<InventoryResult>(
                nameof(ReserveInventoryActivity),
                new InventoryRequest(RequestId: orderId, order.Name, order.Quantity));
            
            // If there is insufficient inventory, fail and let the user know 
            if (!result.Success)
            {
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Insufficient inventory for {order.Name}"));
            } else {
                // Let them know inventory is enough
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Order {orderId} count is enough!"));
            }

            // End the workflow with a success result
            return result;
        }
    }
}
