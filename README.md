# build docker image with kaniko and jenkins running on kubernetes

## Prerequisite

- [x] a running kubernetes cluster , you can use minikube as well；
- [x] a running and configured  jenkins in kubernetes
- [x] username and password for  accessing source code repository, if you are using github , just pass token instead.
- [x] credentials like database connection strings  you wanna pass to during  building image
- [x] docker-registry secret infomation in order to push image to your registry

## What is kaniko & Why kaniko?

we can not easily access to docker deamon since Docker runtime support is removed in a future release(currently planned for the 1.22 release in late 2021) of Kubernetes,
here is [kanilo](https://github.com/GoogleContainerTools/kaniko) come to save the day.kaniko is a tool to build container images from a Dockerfile, inside a container or Kubernetes cluster.

## Quick Start

1 apply following command which contains  docker-registry auth info to cluster

```
kubectl --namespace default \
create secret \
docker-registry regcred \
--docker-server https://your.registry.url \
--docker-username your_user_name \
--docker-password your_password \
--docker-email your_email_address
```

2 Store your  credentials  to jenkins secret text
those credentials will be used in Jenkinsfile like below

```
environment {            
   SERVICE_NAME = "shop-api"           
   REGISTRY = "registry.cn-hangzhou.aliyuncs.com/hyper/${SERVICE_NAME}" 
   // credencials database connectionstring 
    StoreConnection = credentials('StoreConnection') 
    IdentityConnection = credentials('IdentityConnection') 
    GitHub = credentials('GitHub')
                       }
```

3 Set up a jenkins pipeline ,specify  build with Jenkinsfile, then pull current repository
