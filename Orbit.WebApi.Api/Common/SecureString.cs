using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Orbit.WebApi.Api.Common
{
    /// <summary>
    /// A class for encrypting and decrypting password.
    /// </summary>
    internal class SecureString
    {
        /// <summary>
        ///     Secure key for encryption.
        /// </summary>
        private static string securityKey = ConfigurationManager.AppSettings["SecureStringKey"] ?? "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        ///     This method used for encrypting password 
        /// </summary>
        /// <param name="stringToEncrypt">password string</param>
        /// <returns>encrypted password string</returns>
        public static string Encrypt(string stringToEncrypt)
        {
            if (stringToEncrypt.Trim() != string.Empty)
            {
                byte[] keyArray;
                byte[] encryptArray = UTF8Encoding.UTF8.GetBytes(stringToEncrypt);

                // This class used for encrypt secure key
                MD5CryptoServiceProvider cryptoServiceProvideMD5 = new MD5CryptoServiceProvider();
                keyArray = cryptoServiceProvideMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));
                cryptoServiceProvideMD5.Clear();

                TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                tripleDES.Key = keyArray;
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;

                ICryptoTransform cryToTransform = tripleDES.CreateEncryptor();
                byte[] resultArray = cryToTransform.TransformFinalBlock(encryptArray, 0, encryptArray.Length);
                tripleDES.Clear();

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            else
            {
                return stringToEncrypt;
            }
        }

        /// <summary>
        ///     This method used for decrypting password     
        /// </summary>
        /// <param name="stringToDecrypt">encrypted password string</param>
        /// <returns>decrypted password string</returns>
        public static string Decrypt(string stringToDecrypt)
        {
            byte[] keyArray;
            stringToDecrypt = stringToDecrypt.Replace(" ", "+");
            byte[] decryptArray = Convert.FromBase64String(stringToDecrypt);

            // This class used for encrypt secure key
            MD5CryptoServiceProvider cryptoServiceProvideMD5 = new MD5CryptoServiceProvider();
            keyArray = cryptoServiceProvideMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));
            cryptoServiceProvideMD5.Clear();

            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = keyArray;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;

            ICryptoTransform cryToTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cryToTransform.TransformFinalBlock(decryptArray, 0, decryptArray.Length);
            tripleDES.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}