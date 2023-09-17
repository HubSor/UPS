using System.Security.Cryptography;

namespace Services;
public class PasswordService : IPasswordService
{
	private static readonly int iterations = 65536;
	public void FakeGenerateHash()
	{
		GenerateHash("testowe hasło", GenerateSalt());
	}

	public byte[] GenerateHash(string password, byte[] salt)
	{
		return new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256).GetBytes(64);
	}

	public byte[] GenerateSalt()
	{
		return RandomNumberGenerator.GetBytes(32);
	}
}
