using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace VisionClient.Utility
{
    internal interface IXMLCredentials
    {
        (string, string, bool) LoadCredentials();
        void SaveCredentials(string email, string password, bool keepLoggedIn);
    }

    internal class XMLCredentials : IXMLCredentials
    {
        public (string, string, bool) LoadCredentials()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Directory.GetCurrentDirectory() + @"\VisionConfig.xml");
                if (doc is null) return (string.Empty, string.Empty, false);

                RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Vision");
                byte[]? aesKey = key?.GetValue("Key") as byte[];
                byte[]? aesIV = key?.GetValue("IV") as byte[];

                if (aesKey is null || aesIV is null) return (string.Empty, string.Empty, false);

                var aes = Aes.Create();
                aes.Key = aesKey;
                aes.IV = aesIV;

                var email = DecryptAES(doc?.SelectSingleNode("//Config/Credentials/Email")?.InnerText, aes);
                var password = DecryptAES(doc?.SelectSingleNode("//Config/Credentials/Password")?.InnerText, aes);
                bool.TryParse(doc?.SelectSingleNode("//Config/Credentials/AutoLogin")?.InnerText, out bool autologin);

                return (email, password, autologin);
            }
            catch (Exception)
            {
                return (string.Empty, string.Empty, false);
            }
        }

        public void SaveCredentials(string email, string password, bool keepLoggedIn)
        {
            try
            {
                var directory = Directory.GetCurrentDirectory() + @"\VisionConfig.xml";
                if (!File.Exists(directory)) CreateConfig(directory);

                XDocument doc = XDocument.Load(directory);
                Aes aes = Aes.Create();

                if (doc is null || doc?.Root is null) return;


                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    var emailXML = doc.Root.Descendants("Email").FirstOrDefault();
                    emailXML?.SetValue(EncryptAES(email, aes));

                    var passwordXML = doc.Root.Descendants("Password").FirstOrDefault();
                    passwordXML?.SetValue(EncryptAES(password, aes));

                    RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Vision");
                    registryKey.SetValue("Key", aes.Key);
                    registryKey.SetValue("IV", aes.IV);
                }

                var loadCredentialsXML = doc.Root.Descendants("AutoLogin").FirstOrDefault();
                loadCredentialsXML?.SetValue(keepLoggedIn);


                doc.Save(directory);
            }
            catch (Exception) { }
        }

        private void CreateConfig(string directory)
        {
            new XDocument(
                    new XElement("Config",
                        new XElement("Credentials",
                            new XElement("Email", ""),
                            new XElement("Password", ""),
                            new XElement("AutoLogin", "")
                        )
                    )
            ).Save(directory);
        }

        private string EncryptAES(string plainText, SymmetricAlgorithm aes)
        {
            byte[] encrypted;

            ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    encrypted = ms.ToArray();
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        private string DecryptAES(string? encryptedText, SymmetricAlgorithm aes)
        {
            if (string.IsNullOrEmpty(encryptedText)) return string.Empty;

            string? decrypted = null;
            byte[] cipher = Convert.FromBase64String(encryptedText);

            ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(cipher))
            {
                using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        decrypted = sr.ReadToEnd();
                    }
                }
            }
            return decrypted;
        }
    }
}
