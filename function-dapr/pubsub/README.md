# Autoscaling service based on queue depth
## 
## Deployment
### install Kafka
Follow [this guide](https://github.com/OpenFunction/samples/blob/release-0.6/Prerequisites.md#kafka) to install a Kafka server named kafka-openfunction-demo and a Topic named function-dapr-pubsub-topic.

To configure the autoscaling demo we will deploy two functions: `subscriber` which will be used to process messages of the `function-dapr-pubsub-topic` queue in Kafka, and the `producer`, which will be publishing messages.

### Build and deploy Producer

Build and push the producer image.
#### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=function-dapr-pub
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
```shell
cd producer
docker build -t ${IMAGE_TAG} -f Dockerfile.producer .
docker push ${IMAGE_TAG}
```

#### Deploy the producer:

```shell
kubectl apply -f deploy-pub.yaml
```

### Deploy Subscriber

#### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=function-dapr-sub
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
```shell
cd subscriber
pack build ${IMAGE_TAG} --trust-builder --path ./ --builder openfunction/builder-go:v2.4.0 --env FUNC_NAME="Subscriber"  --env FUNC_CLEAR_SOURCE=true
docker push ${IMAGE_TAG}
```

#### Deploy the subscriber:

```shell
kubectl apply -f deploy-sub.yaml
```