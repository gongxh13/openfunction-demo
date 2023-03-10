# Dapr workflows
This is change from [official demo](https://github.com/dapr/dotnet-sdk/tree/master/examples/Workflow) to k8s environment.

In this example, we use a node appliction to receive user order request, after receive order request, send it by dapr workflow api to order-processor.
order-processor is real workflow to deal order request.
## Prerequeirement
deploy redis in clusters.
```
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install redis bitnami/redis --set image.tag=6.2
```
Apply the redis.yaml file and observe that your state store was successfully configured!
kubectl apply -f redis.yaml


## Build and push image
Build and push the workflow order-processor image.
### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=dapr-call-workflow-order-processor
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
### Build
```shell
cd order-processor
docker build -t ${IMAGE_TAG} -f Dockerfile .
docker push ${IMAGE_TAG}
```

Build and push the call front image.
### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=dapr-call-workflow-front
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
### Build
```shell
cd call-front
docker build -t ${IMAGE_TAG} -f Dockerfile .
docker push ${IMAGE_TAG}
```

## Deploy workflow pods
```
kubectl apply -f deploy-workflow.yaml
```
## Deploy call front pods
```
kubectl apply -f deploy-front.yaml
```

## Note: dapr 1.10 version can not register workflow components by yaml self, so not need execute Below code. Default workflow component name is dapr.
### Deploy workflow component
```
kubectl apply -f workflow.yaml
```

## Access workflow
### Forword node front service to local host
```
kubectl port-forward service/dapr-call-workflow-front 8080:80
```
### check node front is start
```
curl http://localhost:8080/ports
```
### call start workflow
Make note of the "1234" in the commands below. This represents the unique identifier for the workflow run and can be replaced with any identifier of your choosing.
```
curl -i -X POST http://localhost:8080/trigger \
  -H "Content-Type: application/json" \
  -d '{"wrokflowType": "OrderProcessingWorkflow", "instanceId": "1234", "type": "start", "inputData" : {"Name": "Paperclips", "TotalCost": 99.95, "Quantity": 1}}'
```

### Next, send an HTTP request to get the status of the workflow that was started:
```
curl -i -X POST http://localhost:8080/trigger \
  -H "Content-Type: application/json" \
  -d '{"wrokflowType": "OrderProcessingWorkflow", "instanceId": "1234", "type": "status"}'
```