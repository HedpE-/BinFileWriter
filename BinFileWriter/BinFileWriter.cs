/*
 * Created by Hugo Gonçalves
 * hugo.goncalves@vodafone.com
 * Date: 28/10/2016
 */
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinFileWriter
{
	/// <summary>
	/// Class takes an object and creates an array of that object to write/read from a binary file
	/// </summary>
	public class BinFileWriter
	{
		private static object userData;
		private static object[] userFile;
		private static int incArray = 0;
		private static string filePathUserFile;
		private static int encryptionType;
		private static string encryptionKey;
		private static int fileEncryptionType;
		private static int fileMaxRecords;
		/// <summary>
		/// Init the class by setting the required variables based on input types
		/// </summary>
		/// <param name="obj">Obj type to add to the array to write/read</param>
		/// <param name="filePath">Path to the binary file to read/write.</param>
		/// <param name="pathType">Path type:
		/// 0 - Hard Disk
		/// 1 - Shared Drive
		/// 2 - HTTP
		/// 3 - HTTPS
		/// 4 - FTP
		/// 5 - SFTP
		/// </param>
		/// <param name="encryptType">Encryption type for key:
		/// 0 - SHA1
		/// 1 - SHA256
		/// 2 - SHA384
		/// 3 - SHA512
		/// 4 - MD5
		/// </param>
		/// <param name="encryptKey">Key to encrypt the file</param>
		/// <param name="fileEncryptType">Encryption type file:
		/// 0 - DES
		/// 1 - AES
		/// </param>
		/// <param name="maxRecords">Max objects inside the array</param>
		public void initClass(object obj,string filePath,int pathType, int encryptType, string encryptKey, int fileEncryptType, int maxRecords)
		{
			if(encryptType < 0 || encryptType > 4)
				return;
			if(fileEncryptType < 0 || fileEncryptType > 1)
				return;
			userData = obj;
			filePathUserFile = filePath;
			userFile = InitializeArray<object>(maxRecords);
			encryptionType = encryptType;
			encryptionKey = encryptKey;
			fileEncryptionType = fileEncryptType;
			fileMaxRecords = maxRecords;
		}
		
		public void sizeAlteration(int maxRecords)
		{
			Array.Resize<Object>(ref userFile, maxRecords);
		} 
		
		/// <param name="data">Data to add to object before writing</param>
		public static void addTo(object data)
		{
			userFile[incArray] = data;
			incArray++;
		}
		
		public static bool writeFile()
		{
			string hashedKey = CalcHash(encryptionKey,encryptionType);
			var fsEncrypted  = new FileStream(filePathUserFile,FileMode.Create,FileAccess.Write);
			switch(fileEncryptionType)
			{
				case 0	:	var DES = new DESCryptoServiceProvider();
							ICryptoTransform desencrypt = DES.CreateEncryptor();
							var DESCr = new DESCryptoServiceProvider();
							DESCr.Key = ASCIIEncoding.ASCII.GetBytes(hashedKey);
							DESCr.IV = ASCIIEncoding.ASCII.GetBytes(hashedKey);	
							var cryptostream = new CryptoStream(fsEncrypted,desencrypt,CryptoStreamMode.Write);
							cryptostream.Write(ObjectToByteArray(userFile),0,userFile.Length);
							return true;
				case 1	:	var AES = new AesCryptoServiceProvider();
							ICryptoTransform desencrypt1 = AES.CreateEncryptor();
							var AESCr = new DESCryptoServiceProvider();
							AESCr.Key = ASCIIEncoding.ASCII.GetBytes(hashedKey);
							AESCr.IV = ASCIIEncoding.ASCII.GetBytes(hashedKey);	
							var cryptostream1 = new CryptoStream(fsEncrypted,desencrypt1,CryptoStreamMode.Write);
							cryptostream1.Write(ObjectToByteArray(userFile),0,userFile.Length);
							return true;
														
			}
			return false;
		}
		
		public static object readFile()
		{
			string hashedKey = CalcHash(encryptionKey,encryptionType);
			var fsRead  = new FileStream(filePathUserFile,FileMode.Open,FileAccess.Read);
			byte[] buffer = null;
			switch(fileEncryptionType)
			{
				case 0	:	var DES = new DESCryptoServiceProvider();
							ICryptoTransform desdecrypt = DES.CreateDecryptor();
							var DESCr = new DESCryptoServiceProvider();
							DESCr.Key = ASCIIEncoding.ASCII.GetBytes(hashedKey);
							DESCr.IV = ASCIIEncoding.ASCII.GetBytes(hashedKey);	
							var cryptostream = new CryptoStream(fsRead,desdecrypt,CryptoStreamMode.Read);
							cryptostream.Read(buffer,0,userFile.Length);
							ByteArrayToObject(buffer);
							return userFile;
				case 1	:	var AES = new AesCryptoServiceProvider();
							ICryptoTransform desdecrypt1 = AES.CreateDecryptor();
							var AESCr = new DESCryptoServiceProvider();
							AESCr.Key = ASCIIEncoding.ASCII.GetBytes(hashedKey);
							AESCr.IV = ASCIIEncoding.ASCII.GetBytes(hashedKey);	
							var cryptostream1 = new CryptoStream(fsRead,desdecrypt1,CryptoStreamMode.Write);
							cryptostream1.Read(buffer,0,userFile.Length);
							userFile = ByteArrayToObject(buffer);
							return userFile;
														
			}
			return false;
		}
		
		private T[] InitializeArray<T>(int length) where T : new()
		{
		    T[] array = new T[length];
		    for (int i = 0; i < length; ++i)
		    {
		        array[i] = new T();
		    }
		
		    return array;
		}
		
		private static string CalcHash(string input, int type)
		{
			var hasher0 = SHA1.Create();
			var hasher1 = SHA256.Create();
			var hasher2 = SHA384.Create();
			var hasher3 = SHA512.Create();
			var hasher4 = MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = null;
			switch(type)
			{
					case 0 : hash = hasher0.ComputeHash(inputBytes); break;
					case 1 : hash = hasher1.ComputeHash(inputBytes); break;
					case 2 : hash = hasher2.ComputeHash(inputBytes); break;
					case 3 : hash = hasher3.ComputeHash(inputBytes); break;
					case 4 : hash = hasher4.ComputeHash(inputBytes); break;
			}
			var sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}		
		
		private static byte[] ObjectToByteArray(object obj)
		{
		    if(obj == null)
		        return null;
		    var bf = new BinaryFormatter();
		    using (var ms = new MemoryStream())
		    {
		        bf.Serialize(ms, obj);
		        return ms.ToArray();
		    }
		}
		
		private static Object[] ByteArrayToObject(byte[] arrBytes)
		{
		    var memStream = new MemoryStream();
		    var binForm = new BinaryFormatter();
		    memStream.Write(arrBytes, 0, arrBytes.Length);
		    memStream.Seek(0, SeekOrigin.Begin);
		    var obj = (Object[]) binForm.Deserialize(memStream);
		    return obj;
		}
	}
}
