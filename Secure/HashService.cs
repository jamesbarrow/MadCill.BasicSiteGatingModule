using System;

namespace MadCill.BasicSiteGatingModule.Secure
{
    public class HashService : IEncryptionService
    {
        //public string Decrypt(string textToDecrypt)
        //{
        //    throw new NotImplementedException();
        //}

        public string Encrypt(string textToEncrypt)
        {
            if (String.IsNullOrEmpty(textToEncrypt))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        public bool IsMatch(string plainTextToMatch, string encryptedString)
        {
            var hashCheckPassword = Encrypt(plainTextToMatch);
            return hashCheckPassword == encryptedString;
        }
    }
}
