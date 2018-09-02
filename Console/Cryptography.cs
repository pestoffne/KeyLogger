// Encoding: UTF-8
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Foo
{
  static class Cryptography
  {
    public static void EncryptData
      (Stream input_stream, Stream output_stream, string password, byte[] salt)
    {
      Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);
      byte[] key = pdb.GetBytes(24);  // 16 или 24
      byte[] iv  = pdb.GetBytes(8);   // только 8

      TripleDES tdes = new TripleDESCryptoServiceProvider();

      using (Stream encrypt_stream = new CryptoStream
        (output_stream, tdes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
      {
        byte[] buffer = new byte[tdes.BlockSize];
        long input_file_length = input_stream.Length;
        long i = 0;

        while (i < input_file_length)
        {
          int buffer_count = input_stream.Read(buffer, 0, tdes.BlockSize);
          encrypt_stream.Write(buffer, 0, buffer_count);
          i += buffer_count;
        }
      }
    }

/*
    // При каких-то условиях завистает в конце потока
    // input_stream может быть только FileStream, иначе нельзя получить Length
    public static void DecryptData
      (Stream input_stream, Stream output_stream, string password, byte[] salt)
    {
      Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);
      byte[] key = pdb.GetBytes(24);  // 16 или 24
      byte[] iv  = pdb.GetBytes(8);   // только 8

      TripleDES tdes = new TripleDESCryptoServiceProvider();

      using (Stream decrypt_stream = new CryptoStream
        (output_stream, tdes.CreateDecryptor(key, iv), CryptoStreamMode.Write))
      {
        byte[] buffer = new byte[tdes.BlockSize];
        long input_file_length = input_stream.Length;
        long i = 0;

        while (i < input_file_length)
        {
          int buffer_count = input_stream.Read(buffer, 0, tdes.BlockSize);
          decrypt_stream.Write(buffer, 0, buffer_count);
          i += buffer_count;
        }
      }
    }
*/

    public static void DecryptData2
      (Stream input_stream, Stream output_stream, string password, byte[] salt)
    {
      Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);
      byte[] key = pdb.GetBytes(24);  // 16 или 24
      byte[] iv  = pdb.GetBytes(8);   // только 8

      TripleDES tdes = new TripleDESCryptoServiceProvider();
      int block_size = tdes.BlockSize;
      char[] buffer = new char[block_size];

      using (Stream decrypt_stream = new CryptoStream
        (output_stream, tdes.CreateDecryptor(key, iv), CryptoStreamMode.Write))
      {
        input_stream.CopyTo(decrypt_stream);
      }
    }
  }
}
