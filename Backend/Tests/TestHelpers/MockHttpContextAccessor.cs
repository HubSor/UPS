using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Helpers;
public class MockHttpContextAccessor : Mock<IHttpContextAccessor>
{
	public bool SignedIn { get; set; }
	public MockHttpContextAccessor()
	{
		var authenticationServiceMock = new Mock<IAuthenticationService>();
		authenticationServiceMock
			.Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
			.Returns(Task.CompletedTask)
			.Callback(() => SignedIn = true);

		var serviceProviderMock = new Mock<IServiceProvider>();
		serviceProviderMock
			.Setup(s => s.GetService(typeof(IAuthenticationService)))
			.Returns(authenticationServiceMock.Object);
		
		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext
			.Setup(s => s.RequestServices).Returns(serviceProviderMock.Object);
			
		Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
	}
}
