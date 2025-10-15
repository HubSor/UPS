using System.Security.Claims;
using Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;

namespace TestHelpers;

public class MockHttpContextAccessor : Mock<IHttpContextAccessor>
{
	public bool SignedIn { get; set; }
	public ICollection<Claim> Claims { get; set; } = [];
	
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
			
		var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
		claimsPrincipalMock
			.Setup(s => s.FindFirst(It.IsAny<string>()))
			.Returns<string>(type => Claims.FirstOrDefault(c => c.Type == type));
		claimsPrincipalMock
			.Setup(s => s.Claims)
			.Returns(() => Claims);	
		
		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext
			.Setup(s => s.RequestServices).Returns(serviceProviderMock.Object);
		mockHttpContext
			.Setup(s => s.User).Returns(claimsPrincipalMock.Object);
			
		Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
	}
	
	public void SetClaims(User user) 
	{
		Claims = user.GetClaims();
	}
}
