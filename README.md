# Amazon API Gateway Importer.Net 
##### Based on [Amazon API Gateway Importer][aws-apigateway-importer]
The **Amazon API Gateway Importer** lets you create or update [Amazon API Gateway][service-page] APIs from a Swagger (aka [OpenAPI Specification][oai]) representation. 

To learn more about API Gateway, please see the [service documentation][service-docs] or the [API documentation][api-docs].

[service-page]: http://aws.amazon.com/api-gateway/
[service-docs]: http://docs.aws.amazon.com/apigateway/latest/developerguide/
[api-docs]: http://docs.aws.amazon.com/apigateway/api-reference
[aws-apigateway-importer]: https://github.com/awslabs/aws-apigateway-importer
[oai]: https://github.com/OAI/OpenAPI-Specification

## Usage

### Prerequisites

#### Credentials
This tool requires AWS credentials to be configured in at least one of the locations specified by the [default credential provider chain](http://docs.aws.amazon.com/AWSSdkDocsNET/V2/DeveloperGuide/net-dg-config-creds.html).

It will look for configured credentials in environment variables, Java system properties, [AWS SDK/CLI](http://aws.amazon.com/cli) profile credentials, and EC2 instance profile credentials.

### Import and deploy a new API

```sh
--create path/to/swagger.json --deploy path/to/deployment.json
```

### Update an existing API and deploy it to a stage

```sh
--update API_ID path/to/swagger.json --deploy path/to/deployment.json  
```

### Merge to an existing API and deploy it to a stage

```sh
--merge API_ID path/to/swagger.json --deploy path/to/deployment.json  
```

### Deploy an existing API to a stage

```sh
--update API_ID --deploy path/to/deployment.json  
```

### Provsion a API key for an existing API & stage

```sh
--update API_ID --prov API_KEY_NAME STAGE_NAME 
```

### Wipe an existing API

```sh
--wipe API_ID
```

### Delete an existing API

```sh
--delete API_ID
```

### List Commands

```sh
--list apis
--list keys
```

### API Gateway Extension Example

You can fully define an API Gateway API in Swagger using the `x-amazon-apigateway-auth` and `x-amazon-apigateway-integration` extensions.

Defined on an Operation:

```json

"x-amazon-apigateway-auth" : {
    "type" : "aws_iam"
},
"x-amazon-apigateway-integration" : {
   "type" : "aws",
   "uri" : "arn:aws:apigateway:us-east-1:lambda:path/2015-03-31/functions/arn:aws:lambda:us-east-1:MY_ACCT_ID:function:helloWorld/invocations",
   "httpMethod" : "POST",
   "credentials" : "arn:aws:iam::MY_ACCT_ID:role/lambda_exec_role",
   "requestTemplates" : {
       "application/json" : "json request template 2",
       "application/xml" : "xml request template 2"
   },
   "requestParameters" : {
       "integration.request.path.integrationPathParam" : "method.request.querystring.latitude",
       "integration.request.querystring.integrationQueryParam" : "method.request.querystring.longitude"
   },
   "cacheNamespace" : "cache-namespace",
   "cacheKeyParameters" : [],
   "responses" : {
       "2\\d{2}" : {
           "statusCode" : "200",
           "responseParameters" : {
               "method.response.header.test-method-response-header" : "integration.response.header.integrationResponseHeaderParam1"
           },
           "responseTemplates" : {
               "application/json" : "json 200 response template",
               "application/xml" : "xml 200 response template"
           }
       },
       "default" : {
           "statusCode" : "400",
           "responseParameters" : {
               "method.response.header.test-method-response-header" : "'static value'"
           },
           "responseTemplates" : {
               "application/json" : "json 400 response template",
               "application/xml" : "xml 400 response template"
           }
       }
   }
}
```

Deployment Configuration :

```json
{
    "description": "DEPLOYMENT_DESCRIPTION",
    "stageName": "STAGE_NAME",
    "stageDescription": "STAGE_DESCRIPTION",
    "logging": {
        "enabled": true,
        "cloudwatchRoleArn": "arn:aws:iam::MY_ACCT_ID:role/ExampleCloudWatch",
        "metricsEnabled": true,
        "loggingLevel": "Info",
        "dataTraceEnabled": false
    },
    "caching": {
        "enabled": false,
        "clusterSize": "CLUSTER_SIZE",
        "ttlInSeconds": 5,
        "dataEncrypted": false
    },
    "throttling": {
        "rateLimit": 500,
        "burstLimit": 1000
    }
}
```