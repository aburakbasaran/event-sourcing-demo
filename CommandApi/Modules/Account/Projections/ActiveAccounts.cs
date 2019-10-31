using System;
using System.Threading.Tasks;
using Core;
using Core.Projections;
using Domain;
using Raven.Client.Documents.Session;

namespace CommandApi.Modules.Account.Projections
{
    public class ActiveAccounts : Projection
    {
        private readonly Func<IDocumentSession> getSession;
        public ActiveAccounts(Func<IDocumentSession> session)=> getSession = session;
        private static string DocumentId(Guid id) => $"ActiveAccount/{id}";
        public override async Task Handle(object e)
        {
            using (var session = getSession())
            {
                switch (e)
                {
                    case V1.AccountCreated view:
                        var document = new ActiveAccountDocument
                        {
                            Id = DocumentId(view.Id),
                            Owner = view.Owner,
                            Name = view.Name,
                            CurrentBalance = 0L,
                        };
                        session.Store(document);
                        break;
                    
                    case V1.Deposited view:
                        session.Update<ActiveAccountDocument>(DocumentId(view.Id), doc =>
                        {
                            doc.CurrentBalance += view.Amount;
                        });
                        break; 
                    case V1.Withdrew view:
                        session.Update<ActiveAccountDocument>(DocumentId(view.Id), doc =>
                        {
                            doc.CurrentBalance -= view.Amount;
                        });
                        break;  
                    #region DO NOT OPEN
                    /*
                    case V1.WithdrewFix view:
                        session.Update<ActiveAccountDocument>(DocumentId(view.Id), doc =>
                        {
                            Console.WriteLine($"Withdrew=>{doc.CurrentBalance}");
                            doc.CurrentBalance -= view.Amount;
                        });
                        break;  
                    */ 
                    #endregion
                    case V1.ChangedOwner view:
                        session.Update<ActiveAccountDocument>(DocumentId(view.Id), doc =>
                            {
                                doc.Owner = view.NewOwner;
                            });
                        break;   
                    case V1.AccountClosed view:
                        session.Delete(DocumentId(view.Id));
                        break;
                }

                session.SaveChanges();
                
            }
        }
    }
}