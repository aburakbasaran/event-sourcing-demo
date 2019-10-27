using System;
using System.Threading.Tasks;
using Core;
using Domain;

namespace CommandApi.Modules.Account
{
    public class ApplicationService 
    {
        private readonly IRepository repository;

        public ApplicationService(IRepository repository)
        {
            this.repository = repository;
        }

        public  Task Handle(Commands.V1.CreateAccount command) => 
            repository.SaveAsync(Domain.Account.Create(command.Id,command.Owner,command.Name));

        public  Task Handle(Commands.V1.Deposit command) => 
            HandleForUpdate(command.Id,account=>account.Deposit(command.Amount,command.Description));

        public  Task Handle(Commands.V1.Withdraw command) => 
            HandleForUpdate(command.Id, account=>account.Withdraw(command.Amount,command.Description));

        public  Task Handle(Commands.V1.TransferAccount command) => 
            HandleForUpdate(command.Id, account=>account.ChangeOwner(command.NewOwner));
        public  Task Handle(Commands.V1.CloseAccount command) =>
            HandleForUpdate(command.Id, account=>account.Close());

        private async Task HandleForUpdate(Guid aggregateId, Action<Domain.Account> handle)
        {
            var aggregate = await repository.GetByIdAsync<Domain.Account>(aggregateId);
            handle(aggregate);
            await repository.SaveAsync(aggregate);
        }
        
    }
}
