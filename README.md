# openfunction-demo
## Introduction
This repo is used to record openfunction demo.
Refer to [official example link](https://github.com/openFunction/samples/tree/release-0.6).
## Prerequisites
### Has a kubernete cluster
### [Install OpenFunction](https://openfunction.dev/docs/getting-started/installation/#install-openfunction)
### Registry Credential
Follow [this guide](https://github.com/OpenFunction/samples/blob/release-0.6/Prerequisites.md#registry-credential) to create a registry credential.
### install pack
Follow [this guide](https://buildpacks.io/docs/tools/pack/#install) install pack cli in your build image machine.
## Limitation
### Docker hub limitation
Due to the company's internal network restrictions, the automatic build of openfunction cannot be fully automated at present, so the code is manually built and uploaded to myself docker hub. If you want to replace your own docker hub, you need to go to the docker hub official website to register an account and know the account password, and then repackage the demo and publish it to your own hub warehouse.
