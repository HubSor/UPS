using NUnit.Framework;

namespace UnitTests.Validators;

[TestFixture]
public class PasswordValidatorTests
{
	protected PasswordValidator validator = new();
	
	[TestCase("abcdefgH1")]
	[TestCase("!qazxsw23edC")]
	[TestCase("!546565bG")]
	[TestCase("admin1232546464GFBHTRTRNYYTRJHYT%$^$%#$#@$!$Grgtgtrthtrhrth")]
	public void Validate_Valid(string password)
	{
		var result = validator.Validate(password);
		Assert.That(result.IsValid, Is.True);
	}
	
	[TestCase("abcdefgh")]
	[TestCase("!qazxswedc")]
	[TestCase("!546565")]
	[TestCase("test")]
	[TestCase("admin")]
	[TestCase("admin1232546464GFBHTRTRNYYTRJHYT%$^$%#$#@$!$Grgtgtrthtrhrth54675756767")]
	public void Validate_Invalid(string password)
	{
		var result = validator.Validate(password);
		Assert.That(result.IsValid, Is.False);
	}
}