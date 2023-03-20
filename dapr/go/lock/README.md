# Lock
## Deployment
### install lock components
`kubectl apply -f lock.yaml`

### Build and deploy lock app

Build and push the producer image.
#### Environment
```
IMAGE_REPOSITORY=gongxh/
IAMGE_NAME=dapr-go-lock
IMAGE_VERSION=latest
IMAGE_TAG=${IMAGE_REPOSITORY}${IAMGE_NAME}:${IMAGE_VERSION}
```
```shell
docker build -t ${IMAGE_TAG} -f ./code/Dockerfile ./code/
docker push ${IMAGE_TAG}
```

#### Deploy the producer:

```shell
kubectl apply -f deploy.yaml
```