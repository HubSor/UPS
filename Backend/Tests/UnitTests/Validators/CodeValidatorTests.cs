using NUnit.Framework;
using ProductsMicro.Validators;

namespace UnitTests.Validators;

[TestFixture]
public class CodeValidatorTests
{
	protected CodeValidator validator = new();
	
	[TestCase("TEST12")]
	[TestCase("TEST")]
	[TestCase("A")]
	[TestCase("12465")]
	[TestCase("A-5")]
	[TestCase("TEST-1")]
	[TestCase("A---5")]
	[TestCase("---")]
	[TestCase("")]
	public void Validate_Valid(string password)
	{
		var result = validator.Validate(password);
		Assert.That(result.IsValid, Is.True);
	}
	
	[TestCase("abcdefgh")]
	[TestCase("!qazxswedC")]
	[TestCase("test12")]
	[TestCase("---a")]
	[TestCase("test_1")]
	[TestCase("TEST123")]
	public void Validate_Invalid(string password)
	{
		var result = validator.Validate(password);
		Assert.That(result.IsValid, Is.False);
	}
}