using System.Collections;
using System.Linq;
using Microsoft.AspNet.SignalR.Infrastructure;
using PX.Data;
using PX.Objects.SO;

namespace Hackathon
{
    public class SalesOrderEntryExtension : PXGraphExtension<SOOrderEntry>
    {
        public PXAction<SOOrder> Predict;

        [InjectDependency]
        public IConnectionManager SignalRConnectionManager
        {
            get;
            set;
        }

        [PXButton]
        public IEnumerable predict(PXAdapter adapter)
        {
            ////var chatMaint = PXGraph.CreateInstance<ChatMaint>();
            ////var prices = chatMaint.GetPrediction(Base.Document.Current);
            ////var message = prices.Aggregate(string.Empty, (current, price) => string.Concat(current, $"{price}, "));
            ////var hubContext = SignalRConnectionManager.GetHubContext<ChatHub>();
            ////hubContext.Clients.All.broadcastMessage(message);
            var chatMaint = PXGraph.CreateInstance<ChatMaint>();
            var prices = chatMaint.GetPrediction(Base.Document.Current);
            var message = prices.Aggregate(string.Empty, (current, price) => string.Concat(current, $"{price}, "));
            var messageActual = Base.Transactions.SelectMain().Select(t => t.UnitPrice).Aggregate(string.Empty,
                (current, price) => string.Concat(current, $"{(int)price.GetValueOrDefault()}, "));
            var hubContext = SignalRConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.All.broadcastMessage(
                $"Sales Order is taken off hold. Actual prices are ${messageActual} but predicted unit prices are: {message}");
            return adapter.Get();
        }

        public void _(Events.RowPersisted<SOOrder> args)
        {
            if (args.Row == null)
            {
                return;
            }
            if (args.Row.Status == SOOrderStatus.Open && args.TranStatus == PXTranStatus.Completed)
            {
                var chatMaint = PXGraph.CreateInstance<ChatMaint>();
                var prices = chatMaint.GetPrediction(args.Row);
                var message = prices.Aggregate(string.Empty, (current, price) => string.Concat(current, $"{price}, "));
                var messageActual = Base.Transactions.SelectMain().Select(t => t.UnitPrice).Aggregate(string.Empty,
                    (current, price) => string.Concat(current, $"{(int) price.GetValueOrDefault()}, "));
                var hubContext = SignalRConnectionManager.GetHubContext<ChatHub>();
                hubContext.Clients.All.broadcastMessage(
                    $"Sales Order is taken off hold. Actual prices are ${messageActual} but predicted unit prices are: {message}");
            }
        }
    }
}