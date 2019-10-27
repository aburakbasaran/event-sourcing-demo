using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public abstract class Aggregate
    {
        private readonly IList<object> changes = new List<object>();

        public Guid Id { get; protected set; } = Guid.Empty;
        public long Version { get; protected set; } = -1;

        protected abstract void When(object e);
        protected abstract void EnsureValidState();

        public void Apply(object e)
        {
            When(e);
            EnsureValidState();
            changes.Add(e);
        }

        public void Load(object[] history)
        {
            foreach (var e in history)
            {
                When(e);
                Version++;   
            }
        }

        public object[] GetChanges() => changes.ToArray();
        
        public int ChangesCount() => changes.Count;

        public void ClearChanges() => changes.Clear();
    }
}