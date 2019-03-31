namespace backgroundr.infrastructure
{
    public interface IEncrypter
    {
        string Encrypt(string text);
        string Decrypt(string cipherText);
    }
}