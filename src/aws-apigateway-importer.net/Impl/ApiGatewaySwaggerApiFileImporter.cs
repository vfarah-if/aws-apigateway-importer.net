﻿using System.IO;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AWS.APIGateway.Impl
{
    public class ApiGatewaySwaggerApiFileImporter : ISwaggerApiFileImporter
    {
        private readonly ISwaggerApiImporter importer;
        ILog log = LogManager.GetLogger(typeof(ApiGatewaySwaggerApiFileImporter));

        public ApiGatewaySwaggerApiFileImporter(ISwaggerApiImporter importer)
        {
            this.importer = importer;
        }

        public string ImportApi(string filePath)
        {
            log.InfoFormat("Attempting to create API from Swagger definition. Swagger file: {0}", filePath);

            var swagger = Import(filePath);
            return importer.CreateApi(swagger, Path.GetFileName(filePath));
        }

        public void UpdateApi(string apiId, string filePath)
        {
            log.InfoFormat("Attempting to update API from Swagger definition. API identifier: {0} Swagger file: {1}", apiId, filePath);

            var swagger = Import(filePath);
            importer.UpdateApi(apiId, swagger);
        }

        public void Deploy(string apiId, string deploymentStage)
        {
            importer.Deploy(apiId, deploymentStage);
        }

        public void DeleteApi(string apiId)
        {
            importer.DeleteApi(apiId);
        }

        private static SwaggerDocument Import(string filePath)
        {
            var serializer = new JsonSerializer {ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };
    
            var sr = new StreamReader(filePath);
            return serializer.Deserialize<SwaggerDocument>(new JsonTextReader(sr));
        }
    }
}
