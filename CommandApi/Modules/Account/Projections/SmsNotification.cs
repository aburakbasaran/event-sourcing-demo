using System;
using System.Threading.Tasks;
using Core;
using Domain;

namespace CommandApi.Modules.Account.Projections
{
    public class SmsNotification : Projection
    {
        public override Task Handle(object e)
        {
            switch (e)
            {
                case V1.AccountClosed x:
                    Console.WriteLine($"SmsService.Send({x.Owner},\"Your account has been closed.\")");
                    break;
            }

            return Task.CompletedTask;
        }
    }
}