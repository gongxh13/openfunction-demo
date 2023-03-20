## build and push docker image

### Create and configure a state store
1. Follow [these steps](https://docs.dapr.io/getting-started/tutorials/configure-state-pubsub/#step-1-create-a-redis-store) to create a Redis store.
If redis can not start because volumn, please execute: `kubectl apply -f redis-pv.yaml`
2. Apply the redis.yaml file and observe that your state store was successfully configured!
```
kubectl apply -f redis.yaml
```

### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=dapr-java-state
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
```shell
docker build -t ${IMAGE_TAG} -f ./code/Dockerfile ./code/
docker push ${IMAGE_TAG}
```

### Deploy the producer:
```shell
kubectl apply -f deploy.yaml
```