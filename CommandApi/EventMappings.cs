using Domain;
using static Core.EventStore.EventTypeMapper;
namespace CommandApi
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
           Map<V1.AccountCreated>("accountCreated");
           Map<V1.Deposited>("deposited");
           Map<V1.Withdrew>("withdrew");
           Map<V1.ChangedOwner>("changedOwner");
           Map<V1.AccountClosed>("accountClosed");
           Map<V1.WithdrewFix>("withdrewFix");
           
           Map<AccountSnapshot>("accountSnapshot");
        }
    }
}