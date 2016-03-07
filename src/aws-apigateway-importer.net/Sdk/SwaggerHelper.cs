﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiGatewayImporter.Sdk
{
    public static class SwaggerHelper
    {
        public static string GetProducesContentType(IEnumerable<string> apiProduces, IEnumerable<string> methodProduces)
        {
            if (methodProduces != null && methodProduces.Any())
            {
                if (methodProduces.Any(t => t.Equals(Constants.DEFAULT_PRODUCES_CONTENT_TYPE, StringComparison.OrdinalIgnoreCase)))
                {
                    return Constants.DEFAULT_PRODUCES_CONTENT_TYPE;
                }

                return methodProduces.FirstOrDefault();
            }

            if (apiProduces != null && apiProduces.Any())
            {
                if (apiProduces.Any(t => t.Equals(Constants.DEFAULT_PRODUCES_CONTENT_TYPE, StringComparison.OrdinalIgnoreCase)))
                {
                    return Constants.DEFAULT_PRODUCES_CONTENT_TYPE;
                }

                return apiProduces.FirstOrDefault();
            }

            return Constants.DEFAULT_PRODUCES_CONTENT_TYPE;
        }

        public static string GenerateSchema(Schema model, string modelName, IDictionary<string, Schema> definitions)
        {
            try
            {
                var modelSchema = JsonConvert.SerializeObject(model);
                var models = JsonConvert.SerializeObject(definitions);
                var schema = new SchemaTransformer().Flatten(modelSchema, models);

                return schema;
            }
            catch (IOException e)
            {
                throw new ArgumentException("Could not process model", e);
            }
        }
    }
}
