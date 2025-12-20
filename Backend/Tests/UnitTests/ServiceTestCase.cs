using System.Runtime.CompilerServices;
using Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Moq;
using NUnit.Framework;
using Services.Application;
using TestHelpers;

namespace UnitTests;

[TestFixture]
public abstract class ServiceTestCase<C, O, R>
	where C : BaseApplicationService
	where O : class
	where R : class
{
	protected C service = default!;
	protected Mock<IUnitOfWork> mockUnitOfWork = new();
	protected Mock<ILogger<C>> mockLogger = new();
	protected MockHttpContextAccessor mockHttpContextAccessor = new();
	
	[OneTimeSetUp]
	public async Task SuperOneTimeSetUp()
	{
		await OneTimeSetUp();
	}
	
	protected virtual Task OneTimeSetUp() => Task.CompletedTask;
	
	[SetUp]
	public async Task SuperSetUp()
	{
		await SetUp();
	}
	
	protected virtual Task SetUp() => Task.CompletedTask;
	
	[TearDown]
	public async Task SuperTearDown()
	{
		await TearDown();
	}
	
	protected virtual Task TearDown() => Task.CompletedTask;

	protected IRepository<T> GetMockRepo<T>()
		where T : class
	{
		var mockRepo = new MockRepository<T>();
		return mockRepo.Object;
	}
}
