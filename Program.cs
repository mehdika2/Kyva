using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//byte[] key = Encoding.UTF8.GetBytes("123456789012345A");
			//byte[] iv = Encoding.UTF8.GetBytes("123456789012345Z");

			//using (Aes aes = Aes.Create())
			//{
			//	aes.GenerateKey();
			//	aes.GenerateIV();

			//	using (ICryptoTransform encryptor = aes.CreateEncryptor(key, iv))
			//	using (FileStream fs = new FileStream("key.kv", FileMode.Create))
			//	using (CryptoStream cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Write))
			//	{
			//		cs.Write(BitConverter.GetBytes(aes.Key.Length), 0, sizeof(int));
			//		cs.Write(BitConverter.GetBytes(aes.IV.Length), 0, sizeof(int));
			//		cs.Write(aes.Key, 0, aes.Key.Length);
			//		cs.Write(aes.IV, 0, aes.IV.Length);
			//	}

			//	using (ICryptoTransform decryptor = aes.CreateDecryptor(key, iv))
			//	using (FileStream fs = new FileStream("key.kv", FileMode.Open))
			//	using (CryptoStream cs = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
			//	{
			//		byte[] keysLenght = new byte[sizeof(int) * 2];
			//		cs.Read(keysLenght, 0, sizeof(int) * 2);
			//		int keyLenght = BitConverter.ToInt32(keysLenght, 0);
			//		int ivLength = BitConverter.ToInt32(keysLenght, sizeof(int));
			//		cs.Read(aes.Key, 0, keyLenght);
			//		cs.Read(aes.IV, 0, ivLength);
			//	}
			//}


			KyvaDB db = new KyvaDB("MyDatabase.db", "MyP4ssw0rd$#!@83");

			db.Add("Hello", "Use");

			db.DisplayAll();

			db.Dispose();




			//// Specify the file path for the database
			//string filePath = "database.json";
			//KyvaDB db = new KyvaDB(filePath, key, iv);

			//// Add some key-value pairs
			//db.Add("name", "Alice");
			//db.Add("age", "30");
			//db.DisplayAll();

			//// Retrieve a value
			//Console.WriteLine(db.Get("name")); // Output: Alice

			//// Update a value
			//db.Update("age", "31");
			//db.DisplayAll();

			//// Delete a key-value pair
			//db.Delete("name");
			//db.DisplayAll();
		}
	}
}
