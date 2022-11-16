using System.Security.Cryptography;
using System.Text;

namespace BLL.Security
{
    public class Make_MD5
    {
        public static string GetHash(string input)
        {
            var md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            string hash = sBuilder.ToString();

            return hash;
        }
    }
}
