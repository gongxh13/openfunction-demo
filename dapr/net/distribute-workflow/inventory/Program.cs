using Dapr.Client;
using Dapr.Workflow;
using InventoryApp.Activities;
using InventoryApp.Models;
using InventoryApp.Workflows;
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
        options.RegisterWorkflow<InventoryWorkflow>();

        // These are the activities that get invoked by the workflow(s).
        options.RegisterActivity<NotifyActivity>();
        options.RegisterActivity<ReserveInventoryActivity>();
    });
});

// Dapr uses a random port for gRPC by default. If we don't know what that port
// is (because this app was started separate from dapr), then assume 4001.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DAPR_GRPC_PORT")))
{
    Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", "4001");
}

Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("*** Welcome to the Dapr Workflow console app sample!");
Console.WriteLine("*** Using this app, you can place orders that start workflows.");
Console.WriteLine("*** Ensure that Dapr is running in a separate terminal window using the following command:");
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("        dapr run --dapr-grpc-port 4001 --app-id wfapp");
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

var baseInventory = new List<InventoryItem>
{
    new InventoryItem(Name: "Paperclips", PerItemCost: 5, Quantity: 100),
    new InventoryItem(Name: "Cars", PerItemCost: 15000, Quantity: 100),
    new InventoryItem(Name: "Computers", PerItemCost: 500, Quantity: 100),
};

// Populate the store with items
await RestockInventory(daprClient, baseInventory);

// Start the input loop
while (true)
{
}

static async Task RestockInventory(DaprClient daprClient, List<InventoryItem> inventory)
{
    Console.WriteLine("*** Restocking inventory...");
    foreach (var item in inventory)
    {
        Console.WriteLine($"*** \t{item.Name}: {item.Quantity}");
        await daprClient.SaveStateAsync(storeName, item.Name.ToLowerInvariant(), item);
    }
}
