# Dapr workflows
This is change from [official demo](https://github.com/dapr/dotnet-sdk/tree/master/examples/Workflow) to k8s environment.

You can see in ./order-processor/ Dockerfile.

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

## Deploy workflow pods
```
kubectl apply -f deploy.yaml
```

## Deploy workflow component
```
kubectl apply -f workflow.yaml
```

## Access workflow
### You can use the following command to create a pod in the cluster and access the function from the pod:
```
kubectl run curl --rm --image=radial/busyboxplus:curl -i --tty
```
### call start workflow
note where `1234` is you start workflow instance id, please replace it
```
curl -i -X POST http://localhost:3500/v1.0-alpha1/workflows/dapr-workflow/OrderProcessingWorkflow/1234/start \
  -H "Content-Type: application/json" \
  -d '{ "input" : {"Name": "Paperclips", "TotalCost": 99.95, "Quantity": 1}}'
```

### Next, send an HTTP request to get the status of the workflow that was started:
```
curl -i -X GET http://localhost:3500/v1.0-alpha1/workflows/dapr-workflow/OrderProcessingWorkflow/1234
```