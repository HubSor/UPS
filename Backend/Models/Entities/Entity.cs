using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public abstract class Entity<T>
    {
        [Key]
        public T Id { get; set; } = default!;
    }
}
