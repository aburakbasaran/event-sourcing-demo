using System.Collections.Generic;

namespace CommandApi.Modules.Account.Projections
{
    public class AccountsByOwnerDocument
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public List<AccountDocument> Accounts { get; set; }
    }
}