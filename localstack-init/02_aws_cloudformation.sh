#!/bin/bash

echo "--- AWS Cloudformation Localstack ---"

echo "############ Execuing Cloudformation ############"
awslocal cloudformation deploy --stack-name health-hub-local --template-file /cloudformation/cloudformation.yml --capabilities CAPABILITY_NAMED_IAM

# echo "########### Describing Resources ###########"
# awslocal cloudformation describe-stack-resources --stack-name health-hub-local