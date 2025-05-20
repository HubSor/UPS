using MassTransit;
using Messages.Responses;
using Moq;

namespace Helpers;
public class MockConsumeContext<O, R> : Mock<ConsumeContext<O>>
	where O : class
	where R : class
{
	public MockConsumeContext(O order, ICollection<ApiResponse<R>> responses)
	{
		Setup(x => x.Message).Returns(order);
		Setup(x => x.RespondAsync(It.IsAny<ApiResponse<R>>())).Callback<ApiResponse<R>>(responses.Add);
	}
}
