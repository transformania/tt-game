using System;
using TT.Domain.Entities;

namespace TT.Domain
{
    /// <summary>
    /// <para>The base class for all DTOs. The class contains the key for the corresponding entity.</para>
    /// <para>Allows strongly typed equality checks between other DTOs of the same type and corresponding entities.</para>
    /// </summary>
    /// <typeparam name="TEntity">The Entity type that corresponds with the DTO.</typeparam>
    /// <typeparam name="TKey">The key type for the Entity. The types must match.</typeparam>
    public class BaseDTO<TEntity, TKey> :
        IEquatable<BaseDTO<TEntity, TKey>>,
        IEquatable<TEntity> where TEntity : Entity<TKey>
    {
        public TKey Id { get; set; }

        public bool Equals(BaseDTO<TEntity, TKey> other)
        {
            return other.Id.Equals(Id);
        }

        public bool Equals(TEntity other)
        {
            return other.Id.Equals(Id);
        }
        
        public static bool operator ==(BaseDTO<TEntity, TKey> left, BaseDTO<TEntity, TKey> right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(null, left) || ReferenceEquals(null, right)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(BaseDTO<TEntity, TKey> left, BaseDTO<TEntity, TKey> right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return (obj.GetType() == typeof(BaseDTO<TEntity, TKey>) && Equals((BaseDTO<TEntity, TKey>)obj)) // equals this
                 || obj.GetType() == typeof(TEntity) && Equals((TEntity)obj); // or equals that
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}