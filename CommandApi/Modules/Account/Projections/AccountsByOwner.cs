using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Projections;
using Domain;
using Raven.Client.Documents.Session;

namespace CommandApi.Modules.Account.Projections
{
    public class AccountsByOwner : Projection
    {
        private readonly Func<IDocumentSession> getSession;
        public AccountsByOwner(Func<IDocumentSession> session)=> getSession = session;
        private static string DocumentId(string id) => $"AccountsByOwner/{id}";
        public override async Task Handle(object e)
        {
            using (var session = getSession())
            {
                switch (e)
                {
                    case V1.AccountCreated view:
                        
                        var documentId = DocumentId(view.Owner);
                        var document = session.Load<AccountsByOwnerDocument>(documentId);
                        if (document == null)
                        {
                             document = new AccountsByOwnerDocument
                            {
                                Id = DocumentId(view.Owner),
                                Owner = view.Owner,
                                Accounts = new List<AccountDocument>()
                            };     
                            session.Store(document); 
                        }
                        document.Accounts.Add(new AccountDocument
                        {
                            Id = view.Id,
                            Name = view.Name
                        });
                       
                        session.Store(document);
                        break;
                    case V1.ChangedOwner view:
                        documentId = DocumentId(view.OldOwner);
                        //delete account from old owner's document
                        AccountDocument accountDocument=null;
                        session.Update<AccountsByOwnerDocument>(documentId, doc =>
                        {
                            accountDocument = doc.Accounts.FirstOrDefault(q => q.Id == view.Id);
                            if (accountDocument != null)
                                doc.Accounts.Remove(accountDocument);
                        });
                        //add account to new owner's document or create the document if needed
                        document = session.Load<AccountsByOwnerDocument>(DocumentId(view.NewOwner)) ?? 
                                   new AccountsByOwnerDocument
                                    {
                                        Id = DocumentId(view.NewOwner),
                                        Owner = view.NewOwner,
                                        Accounts = new List<AccountDocument>()
                                    };
                        document.Accounts.Add(accountDocument);
                        session.Store(document);
                        break;
                    case V1.AccountClosed view:
                        
                        documentId = DocumentId(view.Owner);
                        session.Update<AccountsByOwnerDocument>(documentId, doc =>
                        {
                            var removedDoc = doc.Accounts.FirstOrDefault(q => q.Id == view.Id);
                            if(removedDoc!=null)
                                doc.Accounts.Remove(removedDoc);
                        });
                        
                        break;
                }
                session.SaveChanges();
                
            }
        }
    }
}