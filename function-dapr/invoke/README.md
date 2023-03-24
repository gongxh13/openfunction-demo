# Introduction
This demo will use openfunction to call openfunction use dapr service invoke. We has two function from and to, we will use from to call to service by dapr sdk.

# Build and push image
## Build from function
### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=function-dapr-invoke-from
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
### Build
```
pack build ${IMAGE_TAG} --trust-builder --path ./from/ --builder openfunction/builder-go:v2.4.0 --env FUNC_NAME="function-dapr-invoke-from"  --env FUNC_CLEAR_SOURCE=true
docker push ${IMAGE_TAG}
```

## Build to function
### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=function-dapr-invoke-to
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
### Build
```
pack build ${IMAGE_TAG} --trust-builder --path ./to/ --builder openfunction/builder-go:v2.4.0 --env FUNC_NAME="function-dapr-invoke-to"  --env FUNC_CLEAR_SOURCE=true
docker push ${IMAGE_TAG}
```

# Deployment
## Deployment from function
`kubedtl apply -f deploy-from.yaml`
## Deployment to function
`kubedtl apply -f deploy-to.yaml`

# Access function
## You can observe the process of a function with the following command:
```
kubectl get functions.core.openfunction.io
```
## You can use the following command to create a pod in the cluster and access the function from the pod:
```
kubectl run curl --rm --image=radial/busyboxplus:curl -i --tty
```
## Access the function via URL
```
curl http://function-dapr-invoke-from.default.svc.cluster.local/trigger/
```

## See from function log
```
kubectl logs -f \
  $(kubectl get po -l \
  openfunction.io/serving=$(kubectl get functions function-dapr-invoke-from -o jsonpath='{.status.serving.resourceRef}') \
  -o jsonpath='{.items[0].metadata.name}') \
  function
```

## See to function log
```
kubectl logs -f \
  $(kubectl get po -l \
  openfunction.io/serving=$(kubectl get functions function-dapr-invoke-to -o jsonpath='{.status.serving.resourceRef}') \
  -o jsonpath='{.items[0].metadata.name}') \
  function
```