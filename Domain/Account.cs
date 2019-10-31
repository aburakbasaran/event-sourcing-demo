using System;
using System.Linq;
using Core;

namespace Domain
{
    public class Account : Aggregate
    {
        #region Properties & Ctor
        public Account()
        {
        }
        private string Owner { get; set; }
        private string Name { get; set; }
        private double CurrentBalance { get; set; }
        private bool IsDisabled { get; set; }
        
        #endregion
        public static Account Create(Guid id,string owner,string accountName)
        {
            var bankAccount = new Account();
            
            bankAccount.Apply(new V1.AccountCreated
            {
                Owner = owner,
                Id = id,
                Name= accountName
            });
          
            return bankAccount;
        }

        public void Deposit(double amount,string description)
        {
            Apply(new V1.Deposited
            {
                Id = Id,
                Amount   = amount,
                Description=description
            });
        }

        public void Withdraw(double amount,string description)
        {
            Apply(new V1.Withdrew
            {
                Id = Id,
                Amount = amount,
                Description = description,
                ChangedAt = DateTime.Now
            });
        }

        public void ChangeOwner(string newOwner)
        {
            Apply(new V1.ChangedOwner
            {
                Id=Id,
                NewOwner = newOwner
            });
        }
        public void Close()
        {
            Apply(new V1.AccountClosed
            {
                Id=Id,
                ClosedAt = DateTime.UtcNow,
                Owner = Owner
            });
        }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case V1.AccountCreated x:
                    Id = x.Id;
                    Owner = x.Owner;
                    Name = x.Name;
                    CurrentBalance = 0;
                    IsDisabled = false;
                    break;
                case V1.Deposited x:
                    CurrentBalance += x.Amount;
                    break;
                case V1.Withdrew x:
                    CurrentBalance -= x.Amount;
                    break;
                case V1.ChangedOwner x:
                    Owner = x.NewOwner;
                    break;
                case V1.AccountClosed x:
                    IsDisabled = true;
                    break;
            }
        }
       
        protected override void EnsureValidState()
        {
            if (CurrentBalance <0)
                throw new Exception($"Balance can not be zero or less. Current Balance is {CurrentBalance}");
        }

        public override string ToString()
        {
            return $"Id:{Id}, Owner:{Owner}, CurrentBalance is {CurrentBalance}";
        }

        #region Fee
        public double CalculateFee(double amount)
        {
            var fee = 0D;
            
            if (amount > 10)
            {
                fee=  1.40;
            }else if (amount > 1 && amount < 100)
            {
                fee= 0.40;
            }
            return fee;
        }
        

        #endregion
           
        #region snp-acc
        /*
        public Snapshot TakeSnapshot()
        {
            var snapshot = new AccountSnapshot(Id, Version)
            {
                Name = Name, Owner = Owner, CurrentBalance = CurrentBalance
            };
            return snapshot;
        }

        public void ApplySnapshot(Snapshot snapshot)
        {
            var s = (AccountSnapshot) snapshot;
            Id = s.AggregateId;
            Version = s.Version;
            CurrentBalance = s.CurrentBalance;
            Name = s.Name;
            Owner = s.Owner;
        }

        public Func<bool> SnapshotFrequency()
        {
            var frequency = 4;
            return () => Version % frequency==0;
        }
        */
        #endregion
    }

    #region snp-acc
    public class AccountSnapshot : Snapshot
    {
        public AccountSnapshot(Guid aggregateId,long version):base(Guid.NewGuid(),aggregateId,version )
        {
        }
        public string Name { get; set; }
        public double CurrentBalance { get; set; }
        public string Owner { get; set; }
    }
    #endregion
}