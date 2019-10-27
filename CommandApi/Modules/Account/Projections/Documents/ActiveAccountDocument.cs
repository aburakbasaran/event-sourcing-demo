using System.Globalization;

namespace CommandApi.Modules.Account.Projections
{
    public class ActiveAccountDocument
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public double CurrentBalance { get; set; }
    }
}
