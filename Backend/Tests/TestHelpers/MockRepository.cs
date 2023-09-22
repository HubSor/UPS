using Core;
using MockQueryable.Moq;
using Moq;

namespace Helpers;
public class MockRepository<TEntity> : Mock<IRepository<TEntity>>
	where TEntity : class
{
	public ICollection<TEntity> Entities { get; set; } = new List<TEntity>();
	
	public MockRepository()
	{
		Setup(x => x.GetAll()).Returns(Entities.BuildMock());
		Setup(x => x.AddAsync(It.IsAny<TEntity>())).Callback(Entities.Add);
		Setup(x => x.DeleteAsync(It.IsAny<TEntity>())).Callback<TEntity>(e => Entities.Remove(e));
		Setup(x => x.UpdateAsync(It.IsAny<TEntity>())).Callback<TEntity>(e => 
		{
			var item = Entities.First(x => x == e);
			Entities.Remove(item);
			Entities.Add(e);
		});
	}
}
