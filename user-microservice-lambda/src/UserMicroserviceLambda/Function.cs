using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UserMicroserviceLambda
{
    public class UserMicroservice
    {
        private HttpStatusCode retValue = HttpStatusCode.Accepted;
        public APIGatewayProxyResponse FunctionHandler(UserMicroserviceModel reqObj, ILambdaContext context)
        {
            //Process the data. e.g. save to database (not shown below)
            context.Logger.Log(reqObj.Address);
            context.Logger.Log(reqObj.Country);
            retValue = HttpStatusCode.OK;
            // End Lambda processing logic 
            var response = CreateResponse(retValue);
            return response;
        }
        
        private APIGatewayProxyResponse CreateResponse(HttpStatusCode httpStatusCode)
        {
            int statusCode = (int)httpStatusCode;
            
            var response = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = "Processed",
                Headers = new Dictionary<string, string>
                { 
                    { "Content-Type", "application/json" }, 
                    { "Access-Control-Allow-Origin", "*" } 
                }
            };
            return response;
        }
    }
}