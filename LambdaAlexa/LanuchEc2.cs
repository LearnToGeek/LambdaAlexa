using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaAlexa
{
    class LanuchEc2
    {
        public const string INVOCATION_NAME = "EC2 Service";

        public async Task<SkillResponse> LaunchMyEC2(SkillRequest skillRequest, ILambdaContext context)
        {
            var requestType = skillRequest.GetRequestType();
            if (requestType == typeof(IntentRequest))
            {
                AmazonS3Client amazonS3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);

                string responseBody = "";

                try
                {
                    GetObjectRequest s3request = new GetObjectRequest
                    {
                        BucketName = "geekyprofile",
                        Key = "Ravi.txt"
                    };
                    using (GetObjectResponse response = await amazonS3Client.GetObjectAsync(s3request))
                    using (Stream responseStream = response.ResponseStream)
                    using (StreamReader reader = new StreamReader(responseStream))
                    {

                        responseBody = reader.ReadToEnd(); // Now you process the response body.
                    }
                }
                catch (AmazonS3Exception e)
                {
                    responseBody = String.Format("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                }
                catch (Exception e)
                {
                    responseBody = String.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                }
                return MakeSkillResponse($"{responseBody}", true);

            }
            else
            {
                return MakeSkillResponse($"There is some problem GeekyRV", true);
            }


        }

        private SkillResponse MakeSkillResponse(string outputSpeech, bool shouldEndSession,
            string repromtText = "Ask alexa about your address")
        {
            var response = new ResponseBody
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = new PlainTextOutputSpeech { Text = outputSpeech }
            };

            if (repromtText != null)
            {
                response.Reprompt = new Reprompt() { OutputSpeech = new PlainTextOutputSpeech() { Text = repromtText } };
            }

            var skillResponse = new SkillResponse
            {
                Response = response,
                Version = "1.0"

            };

            return skillResponse;
        }
    }
}
