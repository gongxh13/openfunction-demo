# Dapr workflows
This demo will start mutiple workflow and use one workflow to call others.
Order workflow is the main workflow for create workflow, and inventory is another workflow. When create a order, it will query inventory workflow to check whether is enough.
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
Build and push the inventory workflow image.
### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=dapr-distribute-workflow-inventory
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
### Build
```shell
docker build -t ${IMAGE_TAG} -f ./inventory/Dockerfile ./inventory/
docker push ${IMAGE_TAG}
```

Build and push the order workflow image.
### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=dapr-distribute-workflow-order
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
### Build
```shell
docker build -t ${IMAGE_TAG} -f ./order/Dockerfile ./order/
docker push ${IMAGE_TAG}
```

## Deploy inventory workflow pods
```
kubectl apply -f deploy-inventory.yaml
```
## Deploy order workflow pods
```
kubectl apply -f deploy-order.yaml
```