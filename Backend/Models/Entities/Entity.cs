using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public abstract class Entity
    {
        [Key]
        public long Id { get; set; }
    }
}
