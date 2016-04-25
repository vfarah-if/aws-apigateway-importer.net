﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Importer.Swagger.Impl
{
    public class SwaggerApiFileImporter : ISwaggerApiFileImporter
    {
        private readonly ISwaggerApiImporter importer;
        ILog log = LogManager.GetLogger(typeof(SwaggerApiFileImporter));

        public SwaggerApiFileImporter(ISwaggerApiImporter importer)
        {
            this.importer = importer;
        }

        public string ImportApi(string filePath)
        {
            log.InfoFormat("Attempting to create API from Swagger definition. Swagger file: {0}", filePath);

            var swagger = Import<SwaggerDocument>(filePath);
            return importer.CreateApi(swagger, Path.GetFileName(filePath));
        }

        public void UpdateApi(string apiId, string filePath)
        {
            log.InfoFormat("Attempting to update API from Swagger definition. API identifier: {0} Swagger file: {1}", apiId, filePath);

            var swagger = Import<SwaggerDocument>(filePath);
            importer.UpdateApi(apiId, swagger);
        }

        public void MergeApi(string apiId, string filePath)
        {
            log.InfoFormat("Attempting to merge API from Swagger definition. API identifier: {0} Swagger file: {1}", apiId, filePath);

            var swagger = Import<SwaggerDocument>(filePath);
            importer.MergeApi(apiId, swagger);
        }

        public void Deploy(string apiId, string deploymentConfigFilePath)
        {
            log.InfoFormat("Attempting to deploy API. API identifier: {0}" , apiId);

            var config = Import<DeploymentConfig>(deploymentConfigFilePath);
            importer.Deploy(apiId, config);
        }

        public void DeleteApi(string apiId)
        {
            log.InfoFormat("Attempting to delete API. API identifier: {0}", apiId);

            importer.DeleteApi(apiId);
        }

        public string ProvisionApiKey(string apiId, string name, string stage)
        {
            log.InfoFormat("Attempting to provision API Key. API identifier: {0}", apiId);

            return importer.ProvisionApiKey(apiId, name, stage);
        }

        public void DeleteApiKey(string key)
        {
            log.InfoFormat("Attempting to delete API key {0}", key);

            importer.DeleteApiKey(key);
        }

        public void WipeApi(string apiId)
        {
            log.InfoFormat("Attempting to Wipe API. API identifier: {0}", apiId);

            importer.WipeApi(apiId);
        }

        public IDictionary<string, string> ListApis()
        {
            log.InfoFormat("Listing APIs");

            return importer.ListApis();
        }

        public IDictionary<string, Key> ListKeys()
        {
            log.InfoFormat("Listing Keys");

            return importer.ListKeys();
        }

        public IEnumerable<string> ListOperations(string filePath)
        {
            log.InfoFormat("Attempting list all operations : Swagger file: {0}", filePath);

            var swagger = Import<SwaggerDocument>(filePath);

            return swagger.Paths.Select(path => GetOperations(path.Key, path.Value)).ToList();
        }

        public string ExportAsSwagger(string exportApiId, string stage)
        {
            log.InfoFormat("Exporting API {0}", exportApiId);

            return importer.ExportAsSwagger(exportApiId, stage);
        }

        private static T Import<T>(string filePath)
        {
            var serializer = new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };
    
            var sr = new StreamReader(filePath);
            return serializer.Deserialize<T>(new JsonTextReader(sr));
        }

        private string GetOperations(string path, PathItem pathItem)
        {
            var ops = new List<string>();

            if (pathItem.Head != null) ops.Add("HEAD");
            if (pathItem.Get != null) ops.Add("GET");
            if (pathItem.Post != null) ops.Add("POST");
            if (pathItem.Put != null) ops.Add("PUT");
            if (pathItem.Patch != null) ops.Add("PATCH");
            if (pathItem.Delete != null) ops.Add("DELETE");
            if (pathItem.Options != null) ops.Add("OPTIONS");
            
            return $"{string.Join(",", ops)} {path}";
        }
    }
}
