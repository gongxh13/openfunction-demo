## Pub sub
This demo will use dapr pub and sub. I use redis for pubsub message brokers.

## Build and push docker image
### Bulid sub
#### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=dapr-go-pubsub-sub
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
```shell
docker build -t ${IMAGE_TAG} -f ./sub/Dockerfile ./sub/
docker push ${IMAGE_TAG}
```

### Bulid pub
#### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=dapr-go-pubsub-pub
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
```shell
docker build -t ${IMAGE_TAG} -f ./pub/Dockerfile ./pub/
docker push ${IMAGE_TAG}
```

## Deployment
### Deploy pubsub component
Use `kubectl apply -f deploy-redis.yaml` deploy a pubsub-redis name component.
### Deploy subscriber
`kubectl apply -f subscription.yaml`
### Deploy sub
`kubectl apply -f deploy-sub.yaml`
### Deploy pub
`kubectl apply -f deploy-pub.yaml`