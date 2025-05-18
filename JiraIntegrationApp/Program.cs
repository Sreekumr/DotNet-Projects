//using System;
//using System.Text;
//using System.Threading.Tasks;
//using RestSharp;

//namespace JiraIntegration
//{
//    class Program
//    {
//        static async Task Main(string[] args)
//        {
//            Console.WriteLine("helloq");
//            string email = "sreekumarms1731@gmail.com";
//            string apiToken = "";
//            string jiraDomain = "sreekumarms1731.atlassian.net";
//            string projectKey = "CPG";

//            var client = new RestClient($"https://{jiraDomain}/rest/api/3/issue");

//            var request = new RestRequest();
//            request.Method = Method.Post;

//            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{apiToken}"));
//            request.AddHeader("Authorization", $"Basic {credentials}");
//            request.AddHeader("Content-Type", "application/json");

//            var body = new
//            {
//                fields = new
//                {
//                    summary = "Bug created via C# console app",
//                    issuetype = new { name = "Bug" },
//                    project = new { key = projectKey }
//                }
//            };

//            request.AddJsonBody(body);

//            var response = await client.ExecuteAsync(request);

//            Console.WriteLine($"Status: {response.StatusCode}");
//            Console.WriteLine($"Response: {response.Content}");
//        }
//    }
//}


using System;
//using System.Net.Http.Headers;
//using System.Text;

//namespace JiraIntegrationApp
//{
//    class Program
//    {
//        static async Task Main(string[] args)
//        {
//            Console.WriteLine("helloq");
//            string jiraBaseUrl = "https://sreekumarms1731.atlassian.net/";
//            string username = "sreekumarms1731@gmail.com";
//            string apiToken = "";
//            string projectKey = "TEST";
//            string issueSummary = "Sample Issue summary";



//            string createIssueUrl = $"{jiraBaseUrl}/rest/api/3/issue";

//            //var issueData = new
//            //{
//            //    fields = new
//            //    {
//            //        project = new { key = projectKey },
//            //        summary = issueSummery,
//            //        description = issueDescription,
//            //        issuetype = new { name = "Task" } // or "Task", "Story", etc.
//            //    }
//            //};

//            var issueData = new
//            {
//                fields = new
//                {
//                    project = new { key = projectKey },
//                    summary = issueSummary,
//                    description = new
//                    {
//                        type = "doc",
//                        version = 1,
//                        content = new[]
//            {
//                new {
//                    type = "paragraph",
//                    content = new[] {
//                        new {
//                            type = "text",
//                            text = "This is a description in Atlassian Document Format." +
//                            "Lorem is a placeholder text often used in design and publishing, derived from a corrupted Latin phrase. Its meaning is essentially \"pain itself\" or a shortened version of \"dolorem ipsum\". It's commonly used as a placeholder to fill space and allow designers to focus on layout and typography without being distracted by actual content. "
//                        }
//                    }
//                }
//            }
//                    },
//                    issuetype = new { name = "Task" }
//                }
//            };

//            string jsonPayload = System.Text.Json.JsonSerializer.Serialize(issueData);

//            using (HttpClient client = new HttpClient())
//            {

//                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{apiToken}"));
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

//                var json = System.Text.Json.JsonSerializer.Serialize(issueData);
//                var content = new StringContent(json, Encoding.UTF8, "application/json");

//                try
//                {
//                    HttpResponseMessage response = await client.PostAsync(createIssueUrl, content);

//                    if (response.IsSuccessStatusCode)
//                    {
//                        string responseBody = await response.Content.ReadAsStringAsync();
//                        Console.WriteLine("Issue created successfully");
//                        Console.WriteLine($"Response: {responseBody}");
//                    }
//                    else
//                    {
//                        Console.WriteLine($"Error: {response.StatusCode}");
//                        string errorResponse = await response.Content.ReadAsStringAsync();
//                        Console.WriteLine($"Error Response: {errorResponse}");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Exception: {ex.Message}");
//                }


//            }

//        }  
//    }
//}

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JiraIntegrationApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("hello");
            // Replace these with your actual values
            string jiraBaseUrl = "https://sreekumarms1731.atlassian.net/";     // No trailing slash
            string username = "@gmail.com";
            string apiToken = "";
            string projectKey = "TEST";
            string issueSummary = "Sample Issue with Screenshot Attachment";

            // Your image data URL (truncated example)
            string dataUrl = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAusB9YyD39sAAAAASUVORK5CYII=";


            string createIssueUrl = $"{jiraBaseUrl}/rest/api/3/issue";

            var issueData = new
            {
                fields = new
                {
                    project = new { key = projectKey },
                    summary = issueSummary,
                    description = new
                    {
                        type = "doc",
                        version = 1,
                        content = new[]
                        {
                        new {
                            type = "paragraph",
                            content = new[] {
                                new {
                                    type = "text",
                                    text = "This issue contains an image attached via data URL."
                                }
                            }
                        }
                    }
                    },
                    issuetype = new { name = "Task" }
                }
            };

            string jsonPayload = JsonSerializer.Serialize(issueData);

            using (HttpClient client = new HttpClient())
            {
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{apiToken}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(createIssueUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Issue created successfully.");
                        Console.WriteLine($"Response: {responseBody}");

                        using var doc = JsonDocument.Parse(responseBody);
                        string? issueKey = doc.RootElement.GetProperty("key").GetString();

                        if (!string.IsNullOrEmpty(issueKey))
                        {
                            await UploadAttachmentFromDataUrl(jiraBaseUrl, issueKey, username, apiToken, dataUrl);
                        }
                        else
                        {
                            Console.WriteLine("Issue key not found in response.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Issue creation failed: {response.StatusCode}");
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(errorResponse);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception during issue creation: {ex.Message}");
                }
            }
        }

        static async Task UploadAttachmentFromDataUrl(string baseUrl, string issueKey, string username, string apiToken, string dataUrl)
        {
            string uploadUrl = $"{baseUrl}/rest/api/3/issue/{issueKey}/attachments";

            using (HttpClient uploadClient = new HttpClient())
            {
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{apiToken}"));
                uploadClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                uploadClient.DefaultRequestHeaders.Add("X-Atlassian-Token", "no-check");

                // Parse data URL
                string mimeType = dataUrl.Substring(5, dataUrl.IndexOf(';') - 5);
                string base64Data = dataUrl.Substring(dataUrl.IndexOf(',') + 1);
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                using (var form = new MultipartFormDataContent())
                {
                    var byteContent = new ByteArrayContent(imageBytes);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

                    form.Add(byteContent, "file", "screenshot.png");

                    try
                    {
                        var uploadResponse = await uploadClient.PostAsync(uploadUrl, form);
                        if (uploadResponse.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Image attached successfully.");
                            var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                            Console.WriteLine(uploadResult);
                        }
                        else
                        {
                            Console.WriteLine($"Image upload failed: {uploadResponse.StatusCode}");
                            var uploadError = await uploadResponse.Content.ReadAsStringAsync();
                            Console.WriteLine(uploadError);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception during image upload: {ex.Message}");
                    }
                }
            }
        }
    }
}
