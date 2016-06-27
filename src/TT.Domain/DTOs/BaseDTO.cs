using System;
using TT.Domain.Entities;

namespace TT.Domain.DTOs
{
    /// <summary>
    /// <para>The base class for all DTOs. The class contains the key for the corresponding entity.</para>
    /// <para>Allows strongly typed equality checks between other DTOs of the same type and corresponding entities.</para>
    /// </summary>
    /// <typeparam name="Entity">The Entity that corresponds with the DTO.</typeparam>
    /// <typeparam name="TKey">The key type for the Entity. The types must match.</typeparam>
    public class BaseDTO<Entity, TKey> :
        IEquatable<BaseDTO<Entity, TKey>>,
        IEquatable<Entity> where Entity : Entity<TKey>
    {
        public TKey Id { get; set; }

        public bool Equals(BaseDTO<Entity, TKey> other)
        {
            return other.Id.Equals(Id);
        }

        public bool Equals(Entity other)
        {
            return other.Id.Equals(Id);
        }
        
        public static bool operator ==(BaseDTO<Entity, TKey> left, BaseDTO<Entity, TKey> right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(null, left) || ReferenceEquals(null, right)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(BaseDTO<Entity, TKey> left, BaseDTO<Entity, TKey> right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return (obj.GetType() == typeof(BaseDTO<Entity, TKey>) && Equals((BaseDTO<Entity, TKey>)obj)) // equals this 
                 || obj.GetType() == typeof(Entity) && Equals((Entity)obj); // or equals that
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}