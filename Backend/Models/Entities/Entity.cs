using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public abstract class Entity<T>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public T Id { get; set; } = default!;
	}
	
	public abstract class DictEntity<T>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public T Id { get; set; } = default!;
	}
}
