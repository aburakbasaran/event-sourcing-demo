using System;

namespace Domain
{
    public abstract class CommandBase
    {
        public Guid Id { get; set; }
    }
    
    public static class Commands
    {
        public static class V1
        {
            public class CreateAccount :CommandBase
            {
                public string Owner { get; set; }
                public string Name { get; set; }
            }
            public class Deposit:CommandBase
            {
                public double Amount { get; set; }
                public string Description { get; set; }
            }
            public class Withdraw:CommandBase
            {
                public double Amount { get; set; }
                public string Description { get; set; }
            }
            public class TransferAccount:CommandBase
            {
                public string NewOwner { get; set; }
            }
            public class CloseAccount:CommandBase
            {
            }
        }
    }
}
