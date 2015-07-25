using System.Text;
using System;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;


/// <summary>
/// Get the Gravatar image of an email
/// </summary>
class GravatarImage
{
    private const string _url = "http://www.gravatar.com/avatar/";
    /// <summary>
    /// Get the URL of the image
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="size">The size of the image (1 - 600)</param>
    /// <param name="rating">The MPAA style rating(g, pg, r, x)</param>
    /// <returns>The image URL</returns>
    public static string GetURL(string email, int size, string rating)
    {
        email = email.ToLower();
        email = getMd5Hash(email);

        if (size < 1 | size > 600)
        {
            throw new ArgumentOutOfRangeException("size",
                "The image size should be between 20 and 80");
        }

        rating = rating.ToLower();
        if (rating != "g" & rating != "pg" & rating != "r" & rating != "x")
        {
            throw new ArgumentOutOfRangeException("rating",
                "The rating can only be one of the following: g, pg, r, x");
        }

        return _url + email + "&s=" + size.ToString() + "&r=" + rating;
    }

    /// <summary>
    /// Get the URL of the image
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="size">The size of the image (20 - 80)</param>
    /// <returns>The image URL</returns>
    public static string GetURL(string email, int size)
    {
        return GetURL(email, size, "g");
    }

    /// <summary>
    /// Get the URL of the image
    /// </summary>
    /// <param name="email">The email address</param>
    /// <returns>The image URL</returns>
    public static string GetURL(string email)
    {
        return GetURL(email, 80, "g");
    }

    /// <summary>
    /// Hash an input string and return the hash as a 32 character hexadecimal string
    /// </summary>
    /// <param name="input">The string to hash</param>
    /// <returns>The MD5 hash</returns>
    public static string getMd5Hash(string input)
    {
        var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
        IBuffer buff = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8);
        var hashed = alg.HashData(buff);
        var res = CryptographicBuffer.EncodeToHexString(hashed);
        return res;
    }
}

