
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SmsFunctionApp
{

    public static class SendTextMessage
    {
        private const string accountSid = "AC06f7198c97981979eae77590684f6158";
        private const string authToken = "85a4faaf7baed75ba4a96e0aae1f1a65";

        [FunctionName("SendTextMessage")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("SmsFunctionApp.SendTextMessage() processing a request.");

            TwilioClient.Init(accountSid, authToken);

            string phone = req.Query["phone"];
            string message = req.Query["message"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            phone = phone ?? data?.phone;
            message = message ?? data?.message;

            // twilio specific code

            var messageObject = MessageResource.Create(
                from: new Twilio.Types.PhoneNumber("+18509188268"),
                body: message,
                to: new Twilio.Types.PhoneNumber(phone));

            if (phone != null && message != null)
            {
                return (ActionResult)new OkObjectResult($"Message id: {messageObject.Sid}\nSent to: {phone}\nBody: {message}");
            }
            else
            {
                return new BadRequestObjectResult("Please pass 'phone' and 'message' in query string OR request body");
            }
        }
    }
}
