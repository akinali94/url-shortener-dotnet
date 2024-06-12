using System.Text;

namespace url_shortener_dotnet.Domain.Helpers;

public class Base58Encoder
{
    private const string Base58Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    public string Encode(long input)
    {
        if (input == 0) 
            return Base58Alphabet[0].ToString();

        var result = new StringBuilder();
        while (input > 0)
        {
            var remainder = (int)(input % 58);
            input /= 58;
            result.Insert(0, Base58Alphabet[remainder]);
        }

        return result.ToString();
    }
}