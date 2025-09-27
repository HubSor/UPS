using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
	public interface IEntity {}
	
	public abstract class Entity<T> : IEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public T Id { get; set; } = default!;
	}
	
	public abstract class DictEntity<T> : IEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public T Id { get; set; } = default!;
	}
}
