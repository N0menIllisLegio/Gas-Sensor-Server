using System.Security.Cryptography;
using System.Text;

namespace Gss.Core.Helpers
{
  internal static class CryptoHelper
  {
    public static byte[] GetHash(string inputString)
    {
      using var algorithm = SHA256.Create();
      return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
      var stringBuilder = new StringBuilder();

      foreach (byte hashedByte in GetHash(inputString))
      {
        stringBuilder.Append(hashedByte.ToString("X2"));
      }

      return stringBuilder.ToString();
    }
  }
}
