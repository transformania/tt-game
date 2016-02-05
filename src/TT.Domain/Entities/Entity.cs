using Highway.Data;

namespace TT.Domain.Entities
{
    public class Entity : Entity<int>
    { }

    public class Entity<TKey>
    {
        public virtual TKey Id { get; protected set; }
        protected IRepository Repository { get; set; }

        public Entity()
        {
            Repository = DomainRegistry.Repository;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Entity<TKey>)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !Equals(left, right);
        }

        protected bool Equals(Entity<TKey> other)
        {
            return Id.Equals(other.Id);
        }
    }
}