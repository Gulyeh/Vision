using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

namespace VisionClient.Utility
{

    public static class XMLCredentials
    {
        public static (string, string, bool) LoadCredentials()
        {
            try
            {
                XmlDocument doc = new();
                doc.Load(Directory.GetCurrentDirectory() + @"\VisionConfig.xml");
                if (doc is null) return (string.Empty, string.Empty, false);

                RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Vision");

                if (key?.GetValue("Key") is not byte[] aesKey ||
                    key?.GetValue("IV") is not byte[] aesIV) return (string.Empty, string.Empty, false);

                var aes = Aes.Create();
                aes.Key = aesKey;
                aes.IV = aesIV;

                var email = DecryptAES(doc?.SelectSingleNode("//Config/Credentials/Email")?.InnerText, aes);
                var password = DecryptAES(doc?.SelectSingleNode("//Config/Credentials/Password")?.InnerText, aes);
                _ = bool.TryParse(doc?.SelectSingleNode("//Config/Credentials/AutoLogin")?.InnerText, out bool autologin);

                return (email, password, autologin);
            }
            catch (Exception)
            {
                return (string.Empty, string.Empty, false);
            }
        }

        public static void SaveCredentials(string email, string password, bool keepLoggedIn)
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

        private static void CreateConfig(string directory)
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

        private static string EncryptAES(string plainText, SymmetricAlgorithm aes)
        {
            byte[] encrypted;

            ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new())
            {
                using CryptoStream cs = new(ms, enc, CryptoStreamMode.Write);
                using (StreamWriter sw = new(cs))
                {
                    sw.Write(plainText);
                }

                encrypted = ms.ToArray();
            }

            return Convert.ToBase64String(encrypted);
        }

        private static string DecryptAES(string? encryptedText, SymmetricAlgorithm aes)
        {
            if (string.IsNullOrEmpty(encryptedText)) return string.Empty;

            string? decrypted = null;
            byte[] cipher = Convert.FromBase64String(encryptedText);

            ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new(cipher))
            {
                using CryptoStream cs = new(ms, dec, CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                decrypted = sr.ReadToEnd();
            }
            return decrypted;
        }
    }
}
