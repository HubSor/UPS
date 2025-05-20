namespace Services.Domain;
public interface IPasswordService
{
	byte[] GenerateSalt();
	byte[] GenerateHash(string password, byte[] salt);
	void FakeGenerateHash();
}
