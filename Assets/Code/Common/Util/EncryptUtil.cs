using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using System.Globalization;
namespace Common.Util
{
    public class EncryptUtil
    {
        #region DES
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encodeStr"></param>
        /// <param name="key">8字符</param>
        /// <returns></returns>
        public static string DESEncryptString(string encodeStr, string key)
        {
            byte[] data = Encoding.UTF8.GetBytes(encodeStr);
            DESCryptoServiceProvider DESCSP = new DESCryptoServiceProvider();
            DESCSP.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DESCSP.IV = ASCIIEncoding.ASCII.GetBytes(key);
            ICryptoTransform desencrypt = DESCSP.CreateEncryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return BitConverter.ToString(result);
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decodeStr"></param>
        /// <param name="key">8字符</param>
        /// <returns></returns>
        public static string DESDecryptString(string decodeStr, string key)
        {
            string[] strs = decodeStr.Split("-".ToCharArray());
            byte[] data = new byte[strs.Length];
            for (int i = 0, count = strs.Length; i < count; i++)
            {
                data[i] = byte.Parse(strs[i], NumberStyles.HexNumber);
            }
            DESCryptoServiceProvider DESCSP = new DESCryptoServiceProvider();
            DESCSP.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DESCSP.IV = ASCIIEncoding.ASCII.GetBytes(key);
            ICryptoTransform desencrypt = DESCSP.CreateDecryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// DES加密bytes
        /// </summary>
        /// <param name="encodeStr"></param>
        /// <param name="key">8字符</param>
        /// <returns></returns>
        public static byte[] DESEncryptBytes(byte[] bytes, string key)
        {
            DESCryptoServiceProvider DESCSP = new DESCryptoServiceProvider();
            DESCSP.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DESCSP.IV = ASCIIEncoding.ASCII.GetBytes(key);
            //DESCSP.Mode = CipherMode.ECB;
            ICryptoTransform desencrypt = DESCSP.CreateEncryptor();
            byte[] result = desencrypt.TransformFinalBlock(bytes, 0, bytes.Length);
            return result;
        }

        /// <summary>
        /// DES解密bytes
        /// </summary>
        /// <param name="decodeStr"></param>
        /// <param name="key">8字符</param>
        /// <returns></returns>
        public static byte[] DESDecryptBytes(byte[] bytes, string key)
        {
            DESCryptoServiceProvider DESCSP = new DESCryptoServiceProvider();
            DESCSP.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DESCSP.IV = ASCIIEncoding.ASCII.GetBytes(key);
            //DESCSP.Mode = CipherMode.ECB;
            ICryptoTransform desencrypt = DESCSP.CreateDecryptor();
            byte[] result = desencrypt.TransformFinalBlock(bytes, 0, bytes.Length);
            return result;
        }
        #endregion

        #region AES
        /// <summary>
        /// AES加密bytes
        /// </summary>
        /// <param name="encodeStr"></param>
        /// <param name="key">128、192、256位</param>
        /// <returns></returns>
        public static byte[] AESEncryptBytes(byte[] bytes, string key)
        {
            byte[] keys = ASCIIEncoding.ASCII.GetBytes(key);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keys;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] result = cTransform.TransformFinalBlock(bytes, 0, bytes.Length);
            return result;
        }

        /// <summary>
        /// AES解密bytes
        /// </summary>
        /// <param name="decodeStr"></param>
        /// <param name="key">128、192、256位(字符串长度16,24,32)</param>
        /// <returns></returns>
        public static byte[] AESDecryptBytes(byte[] bytes, string key)
        {
            byte[] keys = ASCIIEncoding.ASCII.GetBytes(key);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keys;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] result = cTransform.TransformFinalBlock(bytes, 0, bytes.Length);
            return result;
        }
        #endregion

        #region blow fish
        /// <summary>
        /// BlowFish加密bytes
        /// </summary>
        /// <param name="encodeStr"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] BlowFishEncryptBytes(byte[] bytes, string key)
        {
            byte[] keys = ASCIIEncoding.ASCII.GetBytes(key);
            BlowFishCS.BlowFish blowFish = new BlowFishCS.BlowFish(keys);
            byte[] result = blowFish.Encrypt_ECB(bytes);
            return result;
        }

        /// <summary>
        /// BlowFish解密bytes
        /// </summary>
        /// <param name="decodeStr"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] BlowFishDecryptBytes(byte[] bytes, string key)
        {
            byte[] keys = ASCIIEncoding.ASCII.GetBytes(key);
            BlowFishCS.BlowFish blowFish = new BlowFishCS.BlowFish(keys);
            byte[] result = blowFish.Decrypt_ECB(bytes);
            return result;
        }
        #endregion

        #region Triple DES CBC模式**
        /// <summary>  
        /// DES3 CBC模式加密  
        /// </summary>  
        /// <param name="data">明文的byte数组</param>  
        /// <param name="iv">IV</param>  
        /// <param name="key">32字符密钥</param>  
        /// <returns>密文的byte数组</returns>  
        public static byte[] TripleDesCBCEncryptBytes(byte[] data, byte[] iv, byte[] key)
        {
            //复制于MSDN  
            try
            {
                // Create a MemoryStream.  
                MemoryStream mStream = new MemoryStream();
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.CBC;             //默认值  
                tdsp.Padding = PaddingMode.PKCS7;       //默认值  
                // Create a CryptoStream using the MemoryStream   
                // and the passed key and initialization vector (IV).  
                CryptoStream cStream = new CryptoStream(mStream,
                    tdsp.CreateEncryptor(key, iv),
                    CryptoStreamMode.Write);
                // Write the byte array to the crypto stream and flush it.  
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                // Get an array of bytes from the   
                // MemoryStream that holds the   
                // encrypted data.  
                byte[] ret = mStream.ToArray();
                // Close the streams.  
                cStream.Close();
                mStream.Close();
                // Return the encrypted buffer.  
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
        /// <summary>  
        /// DES3 CBC模式解密  
        /// </summary>  
        /// <param name="data">密文的byte数组</param>  
        /// <param name="iv">IV</param>  
        /// <param name="key">32字符密钥</param>  
        /// <returns>明文的byte数组</returns>  
        public static byte[] TripleDesCBCDecryptBytes(byte[] data, byte[] iv, byte[] key)
        {
            try
            {
                // Create a new MemoryStream using the passed   
                // array of encrypted data.  
                MemoryStream msDecrypt = new MemoryStream(data);
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.CBC;
                tdsp.Padding = PaddingMode.PKCS7;
                // Create a CryptoStream using the MemoryStream   
                // and the passed key and initialization vector (IV).  
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    tdsp.CreateDecryptor(key, iv),
                    CryptoStreamMode.Read);
                // Create buffer to hold the decrypted data.  
                byte[] fromEncrypt = new byte[data.Length];
                // Read the decrypted data out of the crypto stream  
                // and place it into the temporary buffer.  
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                //Convert the buffer into a string and return it.  
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
        #endregion

        #region Triple DES ECB模式
        /// <summary>  
        /// DES3 ECB模式加密  
        /// </summary>  
        /// <param name="data">byte数组</param>  
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>  
        /// <param name="key">32字符密钥</param>  
        /// <returns>密文的byte数组</returns>  
        public static byte[] TripleDesECBEncryptBytes(byte[] data, byte[] iv, byte[] key)
        {
            try
            {
                // Create a MemoryStream.  
                MemoryStream mStream = new MemoryStream();
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;
                // Create a CryptoStream using the MemoryStream   
                // and the passed key and initialization vector (IV).  
                CryptoStream cStream = new CryptoStream(mStream,
                    tdsp.CreateEncryptor(key, iv),
                    CryptoStreamMode.Write);
                // Write the byte array to the crypto stream and flush it.  
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                // Get an array of bytes from the   
                // MemoryStream that holds the   
                // encrypted data.  
                byte[] ret = mStream.ToArray();
                // Close the streams.  
                cStream.Close();
                mStream.Close();
                // Return the encrypted buffer.  
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
        /// <summary>  
        /// DES3 ECB模式解密  
        /// </summary>  
        /// <param name="data">密文的byte数组</param>  
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>  
        /// <param name="key">32字符密钥</param>  
        /// <returns>明文的byte数组</returns>  
        public static byte[] TripleDesECBDecryptBytes(byte[] data, byte[] iv, byte[] key)
        {
            try
            {
                // Create a new MemoryStream using the passed   
                // array of encrypted data.  
                MemoryStream msDecrypt = new MemoryStream(data);
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;
                // Create a CryptoStream using the MemoryStream   
                // and the passed key and initialization vector (IV).  
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    tdsp.CreateDecryptor(key, iv),
                    CryptoStreamMode.Read);
                // Create buffer to hold the decrypted data.  
                byte[] fromEncrypt = new byte[data.Length];
                // Read the decrypted data out of the crypto stream  
                // and place it into the temporary buffer.  
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                //Convert the buffer into a string and return it.  
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
        #endregion

        #region MD5
        /// <summary>
        /// 实现对一个文件md5的读取，path为文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string HashFile2MD5(string path)
        {
            try
            {
                FileStream get_file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash_byte = get_md5.ComputeHash(get_file);
                string result = System.BitConverter.ToString(hash_byte);
                result = result.Replace("-", "");
                get_file.Close();
                return result;
            }
            catch (System.Exception e)
            {
                return "ERROR:" + e;
            }
        }

        public static string String2MD5(string content)
        {
            byte[] bytes = Encoding.Default.GetBytes(content.Trim());
            return Bytes2MD5(bytes);
        }

        public static string Bytes2MD5(byte[] bytes)
        {
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash_byte = get_md5.ComputeHash(bytes);
                //System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                //byte[] hash_byte = md5.ComputeHash(data);
                StringBuilder sb = new StringBuilder();
                for (int i = 0, length = hash_byte.Length; i < length; i++)
                {
                    sb.Append(hash_byte[i].ToString("X2"));
                }
                //string result = System.BitConverter.ToString(hash_byte);
                hash_byte = null;
                string result = sb.ToString();
                result = result.Replace("-", "");
                return result;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 移位
        public static byte[] PlusExcursionBytes(byte[] bytes, int excursion)
        {
            for (int i = 0, length = bytes.Length; i < length; i++)
            {
                byte b = bytes[i];
                int t = b + excursion;
                if (t > 255)
                    t = t - 255;
                bytes[i] = (byte)t;
            }
            return bytes;
        }

        public static byte[] MinusExcursionBytes(byte[] bytes, int excursion)
        {
            for (int i = 0, length = bytes.Length; i < length; i++)
            {
                byte b = bytes[i];
                int t = b - excursion;
                if (t < 0)
                    t = t + 255;
                bytes[i] = (byte)t;
            }
            return bytes;
        }
        #endregion
    }
}