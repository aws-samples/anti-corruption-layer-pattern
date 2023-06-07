using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Stage = Amazon.CDK.AWS.APIGateway.Stage;
using StageProps = Amazon.CDK.AWS.APIGateway.StageProps;

namespace CdkUserMicroservice
{
    public class CdkUserMicroserviceStack : Stack
    {
        internal CdkUserMicroserviceStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            #region iamroles
            var iamLambdaRole = new Role(this,"MicroserviceLambdaExecutionRole", new RoleProps
            {
                RoleName = "MicroserviceLambdaExecutionRole",
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });
            iamLambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("CloudWatchLogsFullAccess"));
            iamLambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AWSXrayFullAccess"));
            iamLambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("CloudWatchLambdaInsightsExecutionRolePolicy"));
            
            iamLambdaRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new [] {"cloudwatch:PutMetricData"},
                Resources = new [] {"*"}
            }));

            var iamApiGatewayRole = new Role(this, "apiRole", new RoleProps
            {
                RoleName = "apiRole",
                AssumedBy = new ServicePrincipal("apigateway.amazonaws.com")
            });
            iamApiGatewayRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new []{"lambda:InvokeFunction"},
                Resources = new []{"*"}
            }));
            #endregion iamroles
            
            #region LambdaFunctions
            var userMicroServiceLambda = new Function(this,"UserMicroserviceLambda", new FunctionProps
            {
                FunctionName = "UserMicroserviceLambda",
                Runtime = Runtime.DOTNET_6,
                Handler = "UserMicroserviceLambda::UserMicroserviceLambda.UserMicroservice::FunctionHandler",
                Role = iamLambdaRole,
                Code = Code.FromAsset("lambdas/UserMicroserviceLambda.zip"),
                Timeout = Duration.Seconds(300),
                Tracing = Tracing.ACTIVE
            });
            #endregion
            
            #region API Gateway
            var api = new RestApi(this, "MicroserviceAPI", new RestApiProps
            {
                RestApiName = "MicroserviceAPI",
                Description = "This API fronts the microservices",
                CloudWatchRole = true
            });

            var userResource =  api.Root.AddResource("user");
            var userMicroserviceIntegration =  new LambdaIntegration(userMicroServiceLambda, new LambdaIntegrationOptions
            {
                Proxy = false,
                PassthroughBehavior = PassthroughBehavior.WHEN_NO_TEMPLATES,
                //Integration request
                RequestTemplates = new Dictionary<string, string>
                {
                    ["application/json"] = "#set($inputRoot = $input.path(\'$\')) { \"UserId\":$inputRoot.UserId, \"Address\":\"$inputRoot.Address\", \"City\" : \"$inputRoot.City\", \"State\" : \"$inputRoot.State\", \"ZipCode\" : $inputRoot.ZipCode,  \"Country\" : \"$inputRoot.Country\"}"
                },
                //Integration response
                IntegrationResponses = new IIntegrationResponse[]
                {
                    new IntegrationResponse
                    {
                        StatusCode = "200",
                        ResponseTemplates = new Dictionary<string, string>
                        {
                            { "application/json", "" } 
                        }
                    }
                }
            });
            var anyMethod = userResource.AddMethod("ANY", userMicroserviceIntegration, new MethodOptions
            {
                //Method response
                MethodResponses = new[]
                {
                    new MethodResponse
                    {
                        StatusCode = "200", ResponseModels = new Dictionary<string, IModel>()
                        {
                            ["application/json"] =Model.EMPTY_MODEL
                        }
                    }
                }
            });
            
            var devDeploy = new Deployment(this, "Deployment", new DeploymentProps { Api = api });
            var devStage = new Stage(this, "dev", new StageProps
            {
                Deployment = devDeploy,
                StageName = "dev",
                TracingEnabled = true,
                MetricsEnabled = true,
                LoggingLevel = MethodLoggingLevel.INFO
            });
            api.DeploymentStage = devStage;
      
            var mockIntegration = new MockIntegration(new IntegrationOptions
            {
                //Integration request
                RequestTemplates = new Dictionary<string, string>
                {
                    ["application/json"] = "{ \"statusCode\": \"200\" }"
                },
                //Integration response
                IntegrationResponses = new IIntegrationResponse[]
                {
                    new IntegrationResponse
                    {
                        StatusCode = "200",
                        ResponseTemplates = new Dictionary<string, string>
                        {
                            { "application/json", "" } 
                        }
                    }
                }
            });
            var mockMethod = userResource.AddMethod("OPTIONS", mockIntegration, new MethodOptions
            {
                //Method response
                MethodResponses = new[]
                {
                    new MethodResponse
                    {
                        StatusCode = "200", ResponseModels = new Dictionary<string, IModel>()
                        {
                            ["application/json"] = Model.EMPTY_MODEL
                        }
                    }
                }
            });
            #endregion
        }
    }
}
