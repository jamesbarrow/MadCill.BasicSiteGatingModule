using System;
using System.IO;
using System.Security.Cryptography;

namespace MadCill.BasicSiteGatingModule.Secure
{
    public class SimpleEncryptionService : IEncryptionService
    {
        private string _key;
        private string _iv;

        public SimpleEncryptionService(string key, string iv)
        {
            _key = key;
            _iv = iv;
        }

        public string Encrypt(string textToEncrypt)
        {
            try
            {
                string ToReturn = "";
                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }

        //public string Decrypt(string textToDecrypt)
        //{
        //    try
        //    {
        //        string ToReturn = "";
        //        byte[] _ivByte = { };
        //        _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
        //        byte[] _keybyte = { };
        //        _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
        //        MemoryStream ms = null; CryptoStream cs = null;
        //        byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
        //        inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
        //        using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
        //        {
        //            ms = new MemoryStream();
        //            cs = new CryptoStream(ms, des.CreateDecryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
        //            cs.Write(inputbyteArray, 0, inputbyteArray.Length);
        //            cs.FlushFinalBlock();
        //            Encoding encoding = Encoding.UTF8;
        //            ToReturn = encoding.GetString(ms.ToArray());
        //        }
        //        return ToReturn;
        //    }
        //    catch (Exception ae)
        //    {
        //        throw new Exception(ae.Message, ae.InnerException);
        //    }
        //}

        public bool IsMatch(string plainTextToMatch, string encryptedString)
        {
            var checkPassword = Encrypt(plainTextToMatch);
            return checkPassword == encryptedString;
        }
    }
}
