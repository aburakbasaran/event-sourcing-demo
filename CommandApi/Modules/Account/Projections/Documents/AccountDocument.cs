using System;

namespace CommandApi.Modules.Account.Projections
{
    public class AccountDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}