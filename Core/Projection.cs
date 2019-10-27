using System;
using System.Threading.Tasks;

namespace Core
{
    public abstract class Projection
    {
        private readonly Type type;

        protected Projection() => type = GetType();

        public abstract Task Handle(object e);
        
        public override string ToString() => type.Name;

        public static implicit operator string(Projection self) => self.ToString();
    }
}
