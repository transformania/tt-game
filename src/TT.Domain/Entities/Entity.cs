using System;

namespace TT.Domain.Entities
{
    public class Entity : Entity<int>
    { }

    public class Entity<TKey> :
        IEquatable<Entity<TKey>>,
        IEquatable<BaseDTO<Entity<TKey>, TKey>>
    {
        public virtual TKey Id { get; protected set; }
        protected IDomainRepository Repository { get; set; }

        public Entity()
        {
            Repository = DomainRegistry.Repository;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Entity<TKey>)obj)
                || obj.GetType() == typeof(BaseDTO<Entity<TKey>, TKey>) && Equals((BaseDTO<Entity<TKey>, TKey>)obj);
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

        public bool Equals(Entity<TKey> other)
        {
            return Id.Equals(other.Id);
        }

        public bool Equals(BaseDTO<Entity<TKey>, TKey> other)
        {
            return Id.Equals(other.Id);
        }
    }
}