using System.Net;
using Core;
using Data;
using Microsoft.Extensions.Logging;
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
	
	protected void AssertOk<T>(ApiResponse<T>? resp)
		where T : class
	{
		Assert.That(resp, Is.Not.Null);
		Assert.That(resp?.Success, Is.True);
		Assert.That(resp?.Errors, Is.Null);
		Assert.That(resp?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
	}
	
	protected void AssertBadRequest<T>(ApiResponse<T>? resp)
		where T : class
	{
		Assert.That(resp, Is.Not.Null);
		Assert.That(resp?.Success, Is.False);
		Assert.That(resp?.Errors, Is.Not.Empty);
		Assert.That(resp?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}
	
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
}
