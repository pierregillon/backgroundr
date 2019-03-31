namespace backgroundr.domain
{
    public interface IEncryptor
    {
        string Encrypt(string clearText);
        string Decrypt(string cipherText);
    }
}