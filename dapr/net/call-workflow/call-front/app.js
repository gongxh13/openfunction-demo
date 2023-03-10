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
    try {
        const data = req.body;
        const wrokflowType = data.wrokflowType;
        const instanceId = data.instanceId;
        const type = data.type;
        const inputData = data.inputData;
        const daprAppId = data.daprAppId;
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
                    console.log("Failed to start workflow.");
                    break;
                }
                console.log("Successfully start workflow " + instanceId);
                break;
            case "terminate":
                response = await fetch(`${workflowurl}/${wrokflowType}/${instanceId}/terminate`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "dapr-app-id": daprAppId
                    }
                });
                if (!response.ok) {
                    console.log("Failed to stop workflow.");
                    break;
                }
                console.log("Successfully stop workflow " + instanceId);
                break;
            case "status":
                response = await fetch(`${workflowurl}/${wrokflowType}/${instanceId}`, {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json"
                    }
                });
                if (!response.ok) {
                    console.log("Failed to get workflow status.");
                    break;
                }
                console.log("Successfully get workflow " + instanceId);
                break;
            default:
                console.log("Not support call type " + type);
                break;
        }
        try {
            let resultJson = await response.json();
            console.log(resultJson);
            res.status(200).send(resultJson);
        } catch (e) {
            try {
                let resultText = await response.text();
                console.log(resultText);
                res.status(200).send(resultText);
            } catch (e) {
                console.error(e);
                res.status(500).send(e);
            }
        }
    } catch (e) {
        console.error(e);
        res.status(500).send(e);
    } finally {
    }

})

app.listen(port, () => console.log(`Node App listening on port ${port}!`));