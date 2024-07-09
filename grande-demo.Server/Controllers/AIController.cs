using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Azure.Identity;
using Azure.Core;
using Azure.Storage;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Azure.Storage.Blobs;

namespace grande_demo.Server.Controllers
{
    [Route("api/[controller]")]
    public class AIController : Controller
    {

        private readonly string containerName = "test-container";


        private readonly IConfiguration _config;
        private string openAIEndpoint;
        private string openAIDeploymentName;
        private string openAI_APIVersion;
        private AccessToken accessToken;
        private string[] requestScopeContext;

        public AIController(IConfiguration config)
        {
            _config = config;
            openAIEndpoint = _config["OpenAI:Endpoint"];
            openAIDeploymentName = _config["OpenAI:DeploymentId"];
            openAI_APIVersion = _config["OpenAI:ApiVersion"];
            requestScopeContext = new[] { _config["OAuth2:RequestScopeContext"] };
        }


        public class GetResponseInput
        {
            public string request { get; set; }
        }

        private static DefaultAzureCredential GetAzureCredential()
        {
            var credentialOptions = new DefaultAzureCredentialOptions
            {
                ExcludeAzureCliCredential = false,
                ExcludeVisualStudioCredential = false,
                ExcludeVisualStudioCodeCredential = false,
                ExcludeSharedTokenCacheCredential = false,
                //ExcludeManagedIdentityCredential = true, //Only use this if you want to exclude Managed Identity (Often if you have issues with signing in, or if you are using environment variables)
            };
            var credential = new DefaultAzureCredential(true);
            return credential;
        }

        private static AccessToken GetToken(string[] context)
        {
            var credential = GetAzureCredential();
            var tokenRequestContext = new TokenRequestContext(context);
            AccessToken accessToken = credential.GetToken(tokenRequestContext);
            return accessToken;
        }

        [HttpPost("GetResponse")]
        public IActionResult GetResponse([FromBody] GetResponseInput input)
        {
            string request = input.request;

            try
            {
                var token = GetToken(requestScopeContext);
                string requestURI = openAIEndpoint + "openai/deployments/" + openAIDeploymentName + "/chat/completions?api-version=" + openAI_APIVersion;
                var myReq = (HttpWebRequest)WebRequest.Create(requestURI);
                //add bearer token to the request
                myReq.Headers.Add("Authorization", token.Token);
                myReq.ContentType = "application/json";
                myReq.Method = "POST";
                var messages = new List<ChatMessage>
                {
                    new ChatMessage { role = "user", content = request }
                };
                var data = new
                {
                    temperature = 0.8,
                    presence_penalty = -0.1,
                    frequency_penalty = -0.1,
                    messages = messages
                };
                var json = JsonConvert.SerializeObject(data);
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                myReq.ContentLength = bytes.Length;
                using (var reqStream = myReq.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }
                var myResp = (HttpWebResponse)myReq.GetResponse();
                using (var streamReader = new StreamReader(myResp.GetResponseStream()))
                {
                    var response = streamReader.ReadToEnd();
                    var result = JsonConvert.DeserializeObject<dynamic>(response);
                    var sb = new StringBuilder();
                    var responseContent = result.choices[0].message.content.ToString();
                    sb.Append(responseContent);
                    sb.Replace("\n", " ");
                    sb.Replace("\r", " ");
                    sb.Replace("\t", " ");
                    sb.Replace("\"", "");
                    return Ok(new { response = sb.ToString().Trim() });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("CreateTestBlob")]
        public IActionResult CreateTestBlob()
        {
            var accountName = _config["Blob:AccountName"];
            // TODO: Replace <storage-account-name> with your actual storage account name
            var blobServiceClient = new BlobServiceClient(
                    new Uri("https://" + accountName + ".blob.core.windows.net"),
                    GetAzureCredential());


            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.CreateBlobContainer(containerName);

            // Create a unique name for the blob
            string blobName = "TestBlob";

            // Get a reference to a blob

            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            //Creat stream for string
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, World!"));

            //upload string content to the blob
            blobClient.Upload(stream);


            return Ok();
        }

        [HttpGet("GetTestBlob")]
        public IActionResult GetTestBlob()
        {
            var accountName = _config["Blob:AccountName"];
            // TODO: Replace <storage-account-name> with your actual storage account name
            var blobServiceClient = new BlobServiceClient(
                    new Uri("https://" + accountName + ".blob.core.windows.net"),
                    GetAzureCredential());

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create a unique name for the blob
            string blobName = "TestBlob";
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            var blobDownloadInfo = blobClient.Download();

            //read the entire content to string
            var blobContent = new StreamReader(blobDownloadInfo.Value.Content).ReadToEnd();

            return Ok(blobContent);
        }

        [HttpPost("DeleteTestBlob")]
        public IActionResult DeleteTestBlob()
        {
            var accountName = _config["Blob:AccountName"];
            // TODO: Replace <storage-account-name> with your actual storage account name
            var blobServiceClient = new BlobServiceClient(
                    new Uri("https://" + accountName + ".blob.core.windows.net"),
                    GetAzureCredential());

            // Create the container and return a container client object
            blobServiceClient.DeleteBlobContainer(containerName);

            return Ok();
        }


    }

    public class AuthResponse
    {
        public string token { get; set; }
        public string region { get; set; }
    }

    public class ChatMessage
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
