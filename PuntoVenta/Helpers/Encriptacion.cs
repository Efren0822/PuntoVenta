using System.Security.Cryptography;
using System.Text;

namespace PuntoVenta.Helpers
{
    public class Encriptacion
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("TuClaveDeEncriptacion"); 
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("TuVectorDeInicializacion"); 

        public static string Encriptar(string texto)
        {
            using (Aes aesAlg = Aes.Create("AES"))
            {
                if (aesAlg == null)
                    throw new InvalidOperationException("No se pudo crear una instancia de AES.");

                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(texto);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string Desencriptar(string textoCifrado)
        {
            byte[] cipherText = Convert.FromBase64String(textoCifrado);

            using (Aes aesAlg = Aes.Create("AES"))
            {
                if (aesAlg == null)
                    throw new InvalidOperationException("No se pudo crear una instancia de AES.");

                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
        }
    }
}

