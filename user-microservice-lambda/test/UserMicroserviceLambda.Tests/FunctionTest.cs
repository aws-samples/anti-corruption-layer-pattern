using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using UserMicroserviceLambda;

namespace UserMicroserviceLambda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestUserMicroserviceFunction()
        {
            // Arrange
            var function = new UserMicroservice();
            var context = new TestLambdaContext();
            var request = new UserMicroserviceModel
            {
                UserId = 12345,
                Address = "475 Sansome St,10th floor",
                City = "San Francisco",
                State = "California",
                ZipCode = 94111,
                Country = "United States"
            };

            // Act
            var response = function.FunctionHandler(request, context);

            // Assert
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("Processed", response.Body);
            Assert.True(response.Headers.ContainsKey("Content-Type"));
            Assert.Equal("application/json", response.Headers["Content-Type"]);
        }
    }
}
