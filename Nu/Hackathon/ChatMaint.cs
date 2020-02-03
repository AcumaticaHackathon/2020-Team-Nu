using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.SO;
using PX.SM;

namespace Hackathon
{
    public class ChatMaint : PXGraph<ChatMaint>
    {
        public SelectFrom<Conversation>.View Conversation;

        public SelectFrom<ChatMessage>
            .Where<ChatMessage.conversationId.IsEqual<Conversation.conversationId.FromCurrent>>.View Messages;

        public SelectFrom<CRActivity>.View Activities;

        public SelectFrom<Users>.View Users;

        public List<decimal?> Prices
        {
            get;
            set;
        }

        public IEnumerable CreateActivity(PXAdapter adapter)
        {
            var activity = CreateChat();
            Activities.Insert(activity);
            Activities.Cache.PersistInserted(activity);
            return adapter.Get();
        }

        public IEnumerable<decimal?> GetPrediction(SOOrder salesOrder)
        {
            Prices = new List<decimal?>();
            var lineItems = SelectFrom<SOLine>
                .Where<SOLine.orderNbr.IsEqual<P.AsString>>.View
                .Select(this, salesOrder.OrderNbr).FirstTableItems;
            foreach (var lineItem in lineItems)
            {
                InvokeRequestResponseService(lineItem, salesOrder.CuryID).Wait();
            }
            return Prices;
        }

        private async Task InvokeRequestResponseService(SOLine lineItem, string currency)
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            using var client = new HttpClient(handler);
            var scoreRequest = new
            {
                Inputs = new Dictionary<string, List<Dictionary<string, string>>>
                {
                    {
                        "WebServiceInput0", new List<Dictionary<string, string>>
                        {
                            new Dictionary<string, string>
                            {
                                {
                                    "Inventory ID", lineItem.InventoryID.ToString()
                                },
                                {
                                    "Type", lineItem.OrderType
                                },
                                {
                                    "Currency", currency
                                },
                                {
                                    "Customer ID", lineItem.CustomerID.ToString()
                                },
                                {
                                    "Warehouse", lineItem.SiteID.ToString()
                                },
                                {
                                    "UOM", lineItem.UOM
                                },
                                {
                                    "Order Qty", ((int) lineItem.OrderQty.GetValueOrDefault()).ToString()
                                },
                                {
                                    "Price", lineItem.UnitPrice.ToString()
                                }
                                ////{
                                ////    "Inventory ID", "AACOMPUT01"
                                ////},
                                ////{
                                ////    "Type", "CS"
                                ////},
                                ////{
                                ////    "Currency", "EUR"
                                ////},
                                ////{
                                ////    "Customer ID", "CARIBBEAN"
                                ////},
                                ////{
                                ////    "Warehouse", "RETAIL"
                                ////},
                                ////{
                                ////    "UOM", "EA"
                                ////},
                                ////{
                                ////    "Order Qty", "10"
                                ////},
                                ////{
                                ////    "Price", "500"
                                ////},
                            }
                        }
                    }
                },
                GlobalParameters = new Dictionary<string, string>()
            };
            const string apiKey = "GvcgTv0LoUq75WSlF2EXLH9txHFaFSw8"; // Replace this with the API key for the web service
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.BaseAddress = new Uri("http://40.78.54.203:80/api/v1/service/salesorders-price-prediction-pip/score");
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
                Prices.Add(result2);
            }
            else
            {
                Console.WriteLine($"The request failed with status code: {response.StatusCode}");
                Console.WriteLine(response.Headers.ToString());
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
            }
        }

        private CRActivity CreateChat()
        {
            return new CRActivity
            {
                Type = "C",
                RefNoteID = Conversation.Current.RelatedEntityNoteId,
                Body = Messages.Current.Message
            };
        }

        private class PredictionResult
        {
            public decimal ScoredLabels
            {
                get;
                set;
            }
        }
    }
}