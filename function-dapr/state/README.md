# Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=function-dapr-state
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
# Build
```
pack build ${IMAGE_TAG} --trust-builder --path ./ --builder openfunction/builder-java:v2-18 --env FUNC_NAME="com.openfunction.StateClient"  --env FUNC_CLEAR_SOURCE=true
```
# Deployment
## Create and configure a state store
1. Follow [these steps](https://docs.dapr.io/getting-started/tutorials/configure-state-pubsub/#step-1-create-a-redis-store) to create a Redis store.
2. Apply the redis.yaml file and observe that your state store was successfully configured!
```
kubectl apply -f ./deploy/redis.yaml
```
3. Deploy application
```
kubectl apply -f deploy.yaml
```
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
curl http://function-dapr-state.default.svc.cluster.local/test
```