# Dapr workflows
This is change from [official demo](https://github.com/dapr/quickstarts/tree/master/workflows/csharp/sdk) to k8s environment.

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