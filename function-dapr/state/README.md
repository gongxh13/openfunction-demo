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
curl http://function-java-hello-world.default.svc.cluster.local/test
```