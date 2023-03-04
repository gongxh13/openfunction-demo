# Introduction
In this case, we will create two functions: function-input and function-output. function-output will send the content to the kafka after a HTTP request received. The function-input will watch the kafka and consume it. Two function will has same java code.
## Prerequirment
Follow [this guide](https://github.com/OpenFunction/samples/blob/release-0.6/Prerequisites.md#kafka) to install a Kafka server named kafka-openfunction-demo and a Topic named function-dapr-java-binding-topic.

## Build
### Build function-output
#### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=function-dapr-java-binding-output
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
```shell
pack build ${IMAGE_TAG} --trust-builder --path ./ --builder openfunction/builder-java:v2-18 --env FUNC_NAME="dev.openfunction.samples.OpenFunctionImpl"  --env FUNC_CLEAR_SOURCE=true
docker push ${IMAGE_TAG}
```
### Build function-input
#### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=function-dapr-java-binding-input
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
```shell
pack build ${IMAGE_TAG} --trust-builder --path ./ --builder openfunction/builder-java:v2-18 --env FUNC_NAME="dev.openfunction.samples.OpenFunctionImpl"  --env FUNC_CLEAR_SOURCE=true
docker push ${IMAGE_TAG}
```

## Deployment
### Deploy the binding output:

```shell
kubectl apply -f deploy-output.yaml
```

### Deploy the binding input:

```shell
kubectl apply -f deploy-input.yaml
```

## Access demo
### Check the current function status:
```
kubectl get functions.core.openfunction.io
```
```
kubectl get function function-front -o=jsonpath='{.status.addresses}'
```
### You can use the following command to create a pod in the cluster and access the function from the pod:
```
kubectl run curl --rm --image=radial/busyboxplus:curl -i --tty
```
### Access the function via URL
```
curl -d '{"message":"Awesome OpenFunction!"}' -H "Content-Type: application/json" -X POST http://function-output.default.svc.cluster.local/
```

### Query function-output's log:
```
kubectl logs -f \
  $(kubectl get po -l \
  openfunction.io/serving=$(kubectl get functions function-output -o jsonpath='{.status.serving.resourceRef}') \
  -o jsonpath='{.items[0].metadata.name}') \
  function
```

### Query function-input's log:
```
kubectl logs -f \
  $(kubectl get po -l \
  openfunction.io/serving=$(kubectl get functions function-input -o jsonpath='{.status.serving.resourceRef}') \
  -o jsonpath='{.items[0].metadata.name}') \
  function
```