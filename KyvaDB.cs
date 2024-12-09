using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	public class KyvaDB : IDisposable
	{
		private Dictionary<string, string> _database;
		private string _filePath;
		private readonly byte[] _key;
		private const string _defaultPassword = "KyvaKyvaKyvaKyva";

		public KyvaDB(string filePath, string password)
		{
			_filePath = filePath;
			_key = Encoding.UTF8.GetBytes(password.PadRight(16).Substring(0, 16));
			_database = LoadDatabase();
		}

		public KyvaDB(string filePath)
		{
			_filePath = filePath;
			_key = Encoding.UTF8.GetBytes(_defaultPassword);
			_database = LoadDatabase();
		}

		private Dictionary<string, string> LoadDatabase()
		{
			if (!File.Exists(_filePath))
				return new Dictionary<string, string>();

			using (Aes aes = Aes.Create())
			{
				aes.Mode = CipherMode.ECB;
				aes.Padding = PaddingMode.PKCS7;
				aes.Key = _key;
				using (FileStream fs = new FileStream(_filePath, FileMode.Open))
				using (ICryptoTransform decryptor = aes.CreateDecryptor())
				using (CryptoStream cs = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
				using (BinaryReader br = new BinaryReader(cs))
				{
					int count = br.ReadInt32();
					byte[] encryptedJson = br.ReadBytes(count);
					string json = DecryptBson(encryptedJson);
					return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
				}
			}
		}

		private void SaveDatabase()
		{
			string json = JsonConvert.SerializeObject(_database);
			byte[] encryptedJ = EncryptBson(json);
			using (Aes aes = Aes.Create())
			{
				aes.Mode = CipherMode.ECB;
				aes.Padding = PaddingMode.PKCS7;
				aes.Key = _key;
				using (FileStream fs = new FileStream(_filePath, FileMode.Create))
				using (ICryptoTransform encryptor = aes.CreateEncryptor())
				using (CryptoStream cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Write))
				using (BinaryWriter bw = new BinaryWriter(cs))
				{
					bw.Write(encryptedJ.Length);
					bw.Write(encryptedJ);
				}
			}
		}

		private byte[] EncryptBson(string plainText)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Mode = CipherMode.ECB;
				aes.Padding = PaddingMode.PKCS7;
				aes.Key = _key;

				ICryptoTransform encryptor = aes.CreateEncryptor();
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
					using (StreamWriter sw = new StreamWriter(cs))
						sw.Write(plainText);
					return ms.ToArray();
				}
			}
		}

		private string DecryptBson(byte[] bson)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Mode = CipherMode.ECB;
				aes.Padding = PaddingMode.PKCS7;
				aes.Key = _key;

				ICryptoTransform decryptor = aes.CreateDecryptor();
				using (MemoryStream ms = new MemoryStream(bson))
				using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
				using (StreamReader sr = new StreamReader(cs))
					return sr.ReadToEnd();
			}
		}

		public void Add(string key, string value)
		{
			if (_database.ContainsKey(key))
			{
				Console.WriteLine("Key already exists. Use Update method to change the value.");
			}
			else
			{
				_database[key] = value;
				Console.WriteLine($"Added: {key} = {value}");
				SaveDatabase();
			}
		}

		public string Get(string key)
		{
			if (_database.TryGetValue(key, out string value))
			{
				return value;
			}
			else
			{
				return "Key not found.";
			}
		}

		public void Update(string key, string value)
		{
			if (_database.ContainsKey(key))
			{
				_database[key] = value;
				Console.WriteLine($"Updated: {key} = {value}");
				SaveDatabase();
			}
			else
			{
				Console.WriteLine("Key not found. Use Add method to insert a new key-value pair.");
			}
		}

		public void Delete(string key)
		{
			if (_database.Remove(key))
			{
				Console.WriteLine($"Deleted key: {key}");
				SaveDatabase();
			}
			else
			{
				Console.WriteLine("Key not found.");
			}
		}

		public void DisplayAll()
		{
			Console.WriteLine("Current Database:");
			foreach (var kvp in _database)
			{
				Console.WriteLine($"{kvp.Key} = {kvp.Value}");
			}
		}

		public void Dispose()
		{
			SaveDatabase();
		}
	}
}
