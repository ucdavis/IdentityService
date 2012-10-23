using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Dapper;
using IdentityService.Controllers;
using IdentityService.Core.Domain;
using IdentityService.Services;
using UCDArch.Core;

namespace IdentityService.App_Start
{
    class MyAuthenticationFiter : IActionFilter
    {
        private readonly IDbService _dbService = SmartServiceLocator<IDbService>.GetService();

        public bool AllowMultiple
        {  // There can just be one in the context:
            get { return false; }
        }

        public Task<HttpResponseMessage> ExecuteActionFilterAsync(
            HttpActionContext actionContext, 
            CancellationToken cancellationToken, 
            Func<Task<HttpResponseMessage>> continuation)
        {
            //Example: Provide an HTTP header like this: Authorization: Basic bXlVc2VybmFtZTpteVBhc3N3b3Jk,
            //where the authorization parameter is the application token present in the database.

            var auth = actionContext.Request.Headers.Authorization;
            if (!actionContext.Request.Headers.Contains("Authorization"))
            {
                return ErrorResponse("Authorization header is missing from request!");
            }
            else if (actionContext.Request.Headers.Authorization.Parameter == null)
            {
                return ErrorResponse("Authentication token is missing from Authorization header!");
            }
           
            var authToken = actionContext.Request.Headers.Authorization.Parameter; // In our case, this will simply be the Authentication token supplpied, as
            // opposed to the typical base 64 encoded username:password combination.
            var authScheme = actionContext.Request.Headers.Authorization.Scheme; // Just a test to verify that the authorization Scheme was read from the header. 

            IQueryable<AuthToken> tokens;
            using (var conn = _dbService.GetConnection())
            {
                var appName = (ConfigurationManager.AppSettings["AppName"] as string != null
                                   ? ConfigurationManager.AppSettings["AppName"]
                                   : "IdentityService");
                tokens = conn.Query<AuthToken>("SELECT * FROM TokenV").AsQueryable().Where(t => t.Name == appName);
            }

            if (!tokens.Any())
            {
               // Not authorized
                return ErrorResponse("Authentication token not present or not active in database!");
            }
            else {
                // Check if token provided matches token returned from database
                var token = tokens.FirstOrDefault();

                if (!token.Active)
                {
                    // Inactive token:
                    return ErrorResponse("Authentication token is not active!");
                }                   
                else if (!token.Token.Equals(authToken))
                {
                    // Not authorized
                    return ErrorResponse("Authentication token does not match value in database!");
                }
            }

            // Sample base 64 encode/decode logic
            //byte[] authBytes = Encoding.UTF8.GetBytes("myUsername:myPassword".ToCharArray());
            //var ahv = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

            //var myCharArray = new char[100];
            //int bytesUsed;
            //int charsUsed;
            //bool completed;
            //var decodedBytes = Convert.FromBase64String(ahv.Parameter);
            //Decoder decoder = new UTF8Encoding().GetDecoder();
            //decoder.Convert(decodedBytes, 0, decodedBytes.Count(), myCharArray, 0, 100, true, out bytesUsed, out charsUsed, out completed);
            //var result = new string(myCharArray,0,charsUsed);

            // Otherwise everything is OK
            var task = continuation();
            return task;
        }
        /// <summary>
        /// Construct and return an HTTP response with the appropriate JSON error message.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        private Task<HttpResponseMessage> ErrorResponse(string reason = null)
        {
            return Task.Factory.StartNew<HttpResponseMessage>(() =>
                                                                  {
                                                                      const string defaultReason = "Missing Authorization Header or Bad or Missing Authentication Token";
                var errorMessage = new ErrorMessage()
                {
                    Error =
                        (string.IsNullOrEmpty(reason) ? defaultReason : reason)
                };
                var retval = new HttpResponseMessage(
                        HttpStatusCode.Forbidden)
                {
                    Content =
                        new ObjectContent(
                        errorMessage.GetType(),
                        errorMessage,
                        new JsonMediaTypeFormatter
                            ())
                };
                return retval;
            });
        }
    }
}
