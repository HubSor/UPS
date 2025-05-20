using System.Net;
using Data;
using Helpers;
using MassTransit;
using Messages.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace UnitTests;

[TestFixture]
public abstract class ConsumerTestCase<C, O, R>
	where C : class
	where O : class
	where R : class
{
	protected C consumer = default!;
	protected ICollection<ApiResponse<R>> responses = new List<ApiResponse<R>>();
	protected Mock<IUnitOfWork> mockUnitOfWork = new();
	protected Mock<ILogger<C>> mockLogger = new();
	protected MockHttpContextAccessor mockHttpContextAccessor = new();
	
	protected void AssertOk()
	{
		var resp = responses.FirstOrDefault();
		Assert.That(resp, Is.Not.Null);
		Assert.That(resp?.Success, Is.True);
		Assert.That(resp?.Errors, Is.Null);
		Assert.That(resp?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
	}
	
	protected void AssertBadRequest()
	{
		var resp = responses.FirstOrDefault();
		Assert.That(resp, Is.Not.Null);
		Assert.That(resp?.Success, Is.False);
		Assert.That(resp?.Errors, Is.Not.Empty);
		Assert.That(resp?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}
	
	protected ConsumeContext<O> GetConsumeContext(O order)
	{
		return new MockConsumeContext<O, R>(order, responses).Object;
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
		responses.Clear();
		await TearDown();
	}
	
	protected virtual Task TearDown() => Task.CompletedTask;
}
