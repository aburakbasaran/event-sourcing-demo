using System;

namespace Domain
{
    public static class V1
    {
        public class AccountCreated
        {
            public string Owner { get; set; }
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Deposited
        {
            public double Amount { get; set; }
            public Guid Id { get; set; }
            public string Description { get; set; }
        }

        public class Withdrew
        {
            public Guid Id { get; set; }
            public double Amount { get; set; }
            public string Description { get; set; }
            public DateTime ChangedAt { get; set; }
        }

        public class ChangedOwner
        {
            public Guid Id { get; set; }
            public string OldOwner { get; set; }
            public string NewOwner { get; set; }
        }

        public class AccountClosed
        {
            public Guid Id { get; set; }
            public string Owner { get; set; }
            public DateTime ClosedAt { get; set; }
        }
        
        public class WithdrewFix
        {
            public Guid Id { get; set; }
            public double Amount { get; set; }
            public string Description { get; set; }
            public DateTime ChangedAt { get; set; }
            public Guid WithdrewId { get; set; }
        }
    }
}