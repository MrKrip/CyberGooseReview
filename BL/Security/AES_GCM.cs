using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Security
{
    public class AES_GCM
    {
        public static string Decrypt(string chipertext, IConfiguration _config, byte[] salt, byte[] tag)
        {
            string output = string.Empty;
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("Keys:AES-GCM"));
            AesGcm Aes = new AesGcm(key);
            var chipertextBytes = Convert.FromBase64String(chipertext);
            var plaintext = new byte[chipertextBytes.Length];
            Aes.Decrypt(salt, chipertextBytes, tag, plaintext);
            output = Encoding.ASCII.GetString(plaintext);
            return output;

        }

        public static string Encrypt(string plaintext, IConfiguration _config, byte[] salt, byte[] tag)
        {
            string output = string.Empty;
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("Keys:AES-GCM"));
            AesGcm Aes = new AesGcm(key);
            var plaintextBytes = Encoding.ASCII.GetBytes(plaintext);
            var ciphertext = new byte[plaintextBytes.Length];
            Aes.Encrypt(salt, plaintextBytes, ciphertext, tag);
            output = Convert.ToBase64String(ciphertext);
            return output;
        }
    }
}
