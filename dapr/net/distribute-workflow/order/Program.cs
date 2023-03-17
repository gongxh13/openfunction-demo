using Dapr.Client;
using Dapr.Workflow;
using OrderApp.Activities;
using OrderApp.Models;
using OrderApp.Workflows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

const string storeName = "statestore";

// The workflow host is a background service that connects to the sidecar over gRPC
var builder = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
{
    services.AddDaprWorkflow(options =>
    {
        // Note that it's also possible to register a lambda function as the workflow
        // or activity implementation instead of a class.
        options.RegisterWorkflow<OrderWorkflow>();

        // These are the activities that get invoked by the workflow(s).
        options.RegisterActivity<NotifyActivity>();
        options.RegisterActivity<ProcessPaymentActivity>();
        options.RegisterActivity<UpdateInventoryActivity>();
    });
});

// Dapr uses a random port for gRPC by default. If we don't know what that port
// is (because this app was started separate from dapr), then assume 4001.

Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("*** Welcome to the Dapr Workflow console app sample!");
Console.WriteLine("*** Using this app, you can place orders that start workflows.");
Console.WriteLine("*** Ensure that Dapr is running in a separate terminal window using the following command:");
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine();
Console.ResetColor();

// Start the app - this is the point where we connect to the Dapr sidecar to
// listen for workflow work-items to execute.
using var host = builder.Build();
host.Start();

using var daprClient = new DaprClientBuilder().Build();

// Wait for the sidecar to become available
while (!await daprClient.CheckHealthAsync())
{
    Thread.Sleep(TimeSpan.FromSeconds(5));
}

// Wait one more second for the workflow engine to finish initializing.
// This is just to make the log output look a little nicer.
Thread.Sleep(TimeSpan.FromSeconds(1));

// NOTE: WorkflowEngineClient will be replaced with a richer version of DaprClient
//       in a subsequent SDK release. This is a temporary workaround.
WorkflowEngineClient workflowClient = host.Services.GetRequiredService<WorkflowEngineClient>();

var orderInfo = new OrderPayload("Cars", 99, 1);
string result = await workflowClient.ScheduleNewWorkflowAsync(
    name: nameof(OrderWorkflow),
    input: orderInfo);
Console.WriteLine("111 " + result);
// Start the input loop
while (true)
{
    // // Get the name of the item to order and make sure we have inventory
    // string items = string.Join(", ", baseInventory.Select(i => i.Name));
    // // Console.WriteLine($"Enter the name of one of the following items to order [{items}].");
    // // Console.WriteLine("To restock items, type 'restock'.");
    // string itemName = Console.ReadLine()?.Trim();
    // if (string.IsNullOrEmpty(itemName))
    // {
    //     continue;
    // }
    // else if (string.Equals("restock", itemName, StringComparison.OrdinalIgnoreCase))
    // {
    //     await RestockInventory(daprClient, baseInventory);
    //     continue;
    // }

    // InventoryItem item = baseInventory.FirstOrDefault(item => string.Equals(item.Name, itemName, StringComparison.OrdinalIgnoreCase));
    // if (item == null)
    // {
    //     Console.ForegroundColor = ConsoleColor.Yellow;
    //     Console.WriteLine($"We don't have {itemName}!");
    //     Console.ResetColor();
    //     continue;
    // }

    // Console.WriteLine($"How many {itemName} would you like to purchase?");
    // string amountStr = Console.ReadLine().Trim();
    // if (!int.TryParse(amountStr, out int amount) || amount <= 0)
    // {
    //     Console.ForegroundColor = ConsoleColor.Yellow;
    //     Console.WriteLine($"Invalid input. Assuming you meant to type '1'.");
    //     Console.ResetColor();
    //     amount = 1;
    // }

    // // Construct the order with a unique order ID
    // string orderId = $"{itemName.ToLowerInvariant()}-{Guid.NewGuid().ToString()[..8]}";
    // double totalCost = amount * item.PerItemCost;


    // // Start the workflow using the order ID as the workflow ID
    // Console.WriteLine($"Starting order workflow '{orderId}' purchasing {amount} {itemName}");

    // // Wait for the workflow to complete
    // WorkflowState state = await workflowClient.GetWorkflowStateAsync(
    //     instanceId: orderId,
    //     getInputsAndOutputs: true);
    // while (!state.IsWorkflowCompleted)
    // {
    //     Thread.Sleep(TimeSpan.FromSeconds(1));
    //     state = await workflowClient.GetWorkflowStateAsync(
    //         instanceId: orderId,
    //         getInputsAndOutputs: true);
    // }

    // if (state.RuntimeStatus == WorkflowRuntimeStatus.Completed)
    // {
    //     OrderResult result = state.ReadOutputAs<OrderResult>();
    //     if (result.Processed)
    //     {
    //         Console.ForegroundColor = ConsoleColor.Green;
    //         Console.WriteLine($"Order workflow is {state.RuntimeStatus} and the order was processed successfully.");
    //         Console.ResetColor();
    //     }
    //     else
    //     {
    //         Console.WriteLine($"Order workflow is {state.RuntimeStatus} but the order was not processed.");
    //     }
    // }
    // else if (state.RuntimeStatus == WorkflowRuntimeStatus.Failed)
    // {
    //     // WorkflowEngineClient doesn't expose a way to get error information.
    //     // For that, we resort to DaprClient. The experience will be improved in the next release.
    //     GetWorkflowResponse response = await daprClient.GetWorkflowAsync(
    //         instanceId: orderId,
    //         workflowComponent: "dapr",
    //         workflowName: nameof(OrderProcessingWorkflow),
    //         CancellationToken.None);
            
    //     string failureDetails = await GetWorkflowFailureDetails(daprClient, orderId);
    //     Console.ForegroundColor = ConsoleColor.Red;
    //     Console.WriteLine($"The workflow failed - {failureDetails}");
    //     Console.ResetColor();
    // }

    // Console.WriteLine();
}

