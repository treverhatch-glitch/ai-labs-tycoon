using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace AiLabs.Rules.Core
{
    public static class Digest
    {
        /// <summary>SHA-256 of the text's UTF-8 bytes, as lowercase hex.</summary>
        public static string Sha256Hex(string text)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
                var sb = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
                return sb.ToString();
            }
        }
    }
}
