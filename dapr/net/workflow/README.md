# Dapr workflows
This is change from [official demo](https://github.com/dapr/quickstarts/tree/master/workflows/csharp/sdk) to k8s environment.

You can see in ./order-processor/ Dockerfile.

## Build and push image
Build and push the workflow order-processor image.
### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=dapr-workflow-order-processor
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
### Build
```shell
cd order-processor
docker build -t ${IMAGE_TAG} -f Dockerfile .
docker push ${IMAGE_TAG}
```

## Deploy
```
kubectl apply -f deploy.yaml
```