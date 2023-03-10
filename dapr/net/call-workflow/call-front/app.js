const express = require('express');
const bodyParser = require('body-parser');
require('isomorphic-fetch');

const app = express();
app.use(bodyParser.json());

// These ports are injected automatically into the container.
const workflowName = "dapr";
const daprPort = process.env.DAPR_HTTP_PORT ?? "3500"; 
const daprGRPCPort = process.env.DAPR_GRPC_PORT ?? "50001";

const workflowurl = `http://localhost:${daprPort}/v1.0-alpha1/workflows/${workflowName}`;
const port = process.env.APP_PORT ?? "3000";

app.get('/ports', (_req, res) => {
    console.log("DAPR_HTTP_PORT: " + daprPort);
    console.log("DAPR_GRPC_PORT: " + daprGRPCPort);
    res.status(200).send({DAPR_HTTP_PORT: daprPort, DAPR_GRPC_PORT: daprGRPCPort })
});

app.post('/trigger', async (req, res) => {
    const data = req.body;
    const wrokflowType = data.wrokflowType;
    const instanceId = data.instanceId;
    const type = data.type;
    const inputData = data.inputData;
    let response;
    switch (type) {
        case "start":
            response = await fetch(`${workflowurl}/${wrokflowType}/${instanceId}/start`, {
                method: "POST",
                body: JSON.stringify(inputData),
                headers: {
                    "Content-Type": "application/json"
                }
            });
            if (!response.ok) {
                throw "Failed to start workflow.";
            }
            console.log("Successfully start workflow " + instanceId);
            console.log(response.json());
            console.log(response.text());
            break;
        case "terminate":
            response = await fetch(`${workflowurl}/${wrokflowType}/${instanceId}/terminate`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                }
            });
            if (!response.ok) {
                throw "Failed to stop workflow.";
            }
            console.log("Successfully stop workflow " + instanceId);
            console.log(response.json());
            console.log(response.text());
            break;
        case "status":
            response = await fetch(`${workflowurl}/${wrokflowType}/${instanceId}`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                }
            });
            if (!response.ok) {
                throw "Failed to get workflow status.";
            }
            console.log("Successfully get workflow " + instanceId);
            console.log(response.json());
            console.log(response.text());
            break;
        default:
            console.log("Not support call type " + type);
            break;
    }
    console.log(response);
    console.log(response);
    res.status(200).send(response.json());
})

app.listen(port, () => console.log(`Node App listening on port ${port}!`));