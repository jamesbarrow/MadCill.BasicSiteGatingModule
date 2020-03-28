using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadCill.BasicSiteGatingModule.Secure
{
    public interface IEncryptionService
    {
        string Encrypt(string textToEncrypt);

        //string Decrypt(string textToDecrypt);

        bool IsMatch(string plainTextToMatch, string encryptedString);
    }
}
