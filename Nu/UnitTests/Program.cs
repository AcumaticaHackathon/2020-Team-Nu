// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Newtonsoft.Json

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();
        }

        static async Task InvokeRequestResponseService()
        {
            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
            };
            using (var client = new HttpClient(handler))
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "WebServiceInput0",
                            new List<Dictionary<string, string>>(){
                                new Dictionary<string, string>(){
                                    {
                                        "Inventory ID", "AACOMPUT01"
                                    },
                                    {
                                        "Type", "CS"
                                    },
                                    {
                                        "Currency", "EUR"
                                    },
                                    {
                                        "Customer ID", "CARIBBEAN"
                                    },
                                    {
                                        "Warehouse", "RETAIL"
                                    },
                                    {
                                        "UOM", "EA"
                                    },
                                    {
                                        "Order Qty", "10"
                                    },
                                    {
                                        "Price", "500"
                                    }
                                }
                            }
                        }
                    },
                    GlobalParameters = new Dictionary<string, string>()
                };
                const string apiKey = "GvcgTv0LoUq75WSlF2EXLH9txHFaFSw8"; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("http://40.78.54.203:80/api/v1/service/salesorders-price-prediction-pip/score");

                // WARNING: The 'await' statement below can result in a deadlock
                // if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false)
                // so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)
                var requestString = JsonConvert.SerializeObject(scoreRequest);
                var content = new StringContent(requestString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(string.Empty, content).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var predictionResult = result.Substring(result.IndexOf("Scored Labels", StringComparison.Ordinal)).Substring(16);
                    var remove = predictionResult.Remove(predictionResult.Length - 4);
                    var result2 = decimal.Parse(remove.Split('.').First());
                    //Prices.Add(predictionResult);
                }
                else
                {
                    Console.WriteLine($"The request failed with status code: {response.StatusCode}");

                    // Print the headers - they include the requert ID and the timestamp,
                    // which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}