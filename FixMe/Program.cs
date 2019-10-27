using System;
using System.Text;
using System.Threading.Tasks;
using Core.EventStore;
using Domain;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using static Core.EventStore.EventTypeMapper;

namespace FixMe
{
    class Program
    {
        private static IEventStoreConnection eventStoreConnection;

        static void Main(string[] args)
        {
            eventMaps();
            eventStoreConnection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@127.0.0.1:1113"), "Fix-App");
            eventStoreConnection.ConnectAsync();
            eventStoreConnection.SubscribeToAllFrom(new Position(0L,0L), 
                CatchUpSubscriptionSettings.Default,
                EventAppeared);
            Console.WriteLine("Let me fix mate!");
            Console.ReadKey();
        }
        private static void EventAppeared(EventStoreCatchUpSubscription s, ResolvedEvent e)
        {
            if (e.Event.EventType != "withdrew") return;
            
            var o = e.Deserialze();
                var withdrew = o as V1.Withdrew;
                var withdrewFix = new V1.WithdrewFix
                {
                    Id = withdrew.Id,
                    Amount = new Account().CalculateFee(withdrew.Amount),
                    Description = "my fix",
                    ChangedAt = DateTime.Now,
                    WithdrewId = e.Event.EventId
                };
                var data = new EventData(
                    Guid.NewGuid(),
                    EventTypeMapper.GetTypeName(withdrewFix.GetType()),
                    true,
                    Encoding.Default.GetBytes(JsonConvert.SerializeObject(withdrewFix)), null);

                eventStoreConnection.AppendToStreamAsync(e.OriginalStreamId, ExpectedVersion.Any, data);

                Console.WriteLine($"Last position of fix=> {e.OriginalPosition.Value.CommitPosition}");
        }

        private static void eventMaps()
        {
            Map<V1.Withdrew>("withdrew");
            Map<V1.WithdrewFix>("withdrewFix");
        }
    }
}