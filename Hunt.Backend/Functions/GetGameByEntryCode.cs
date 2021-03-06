using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

using System.Threading.Tasks;
using Hunt.Common;

namespace Hunt.Backend.Functions
{
	public static class GetGameByEntryCode
	{
        [FunctionName(nameof(GetGameByEntryCode))]
		async public static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(GetGameByEntryCode))]
		HttpRequestMessage req, TraceWriter log)
		{
            using (var analytic = new AnalyticService(new RequestTelemetry
            {
                Name = nameof(GetGameByEntryCode)
            }))
            {
                try
                {
                    var kvps = req.GetQueryNameValuePairs();
                    var email = kvps.FirstOrDefault(kvp => kvp.Key == "email").Value;
                    var entryCode = kvps.FirstOrDefault(kvp => kvp.Key == "entryCode").Value;

                    var existingGame = CosmosDataService.Instance.GetOngoingGame(email);

                    if (existingGame != null)
					{
						var data = new Event("Attempt to lookup game by entry code failed due to ongoing game");
						data.Add("code", entryCode).Add("email", email);
						await EventHubService.Instance.SendEvent(data);
						return req.CreateErrorResponse(HttpStatusCode.Conflict, "User already has an ongoing game");
					}

					var openGame = CosmosDataService.Instance.GetGameByEntryCode(entryCode);
					var outcome = openGame == null ? "failed" : "succeeded";

					{
						var data = new Event($"Attempt to lookup game by entry code {outcome}");
						data.Add("code", entryCode).Add("email", email);
						await EventHubService.Instance.SendEvent(data, openGame);
					}

					return req.CreateResponse(HttpStatusCode.OK, openGame);
                }
                catch (Exception e)
                {
                    // track exceptions that occur
                    analytic.TrackException(e);
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message, e);
                }
            }
		}
	}
}