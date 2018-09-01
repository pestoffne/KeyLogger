// Encoding: UTF-8
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace Foo
{
 static class Decryptor
 {
    const string password = "(\x23, \x12, \x19\x94)";
    static readonly byte[] salt = Encoding.UTF8.GetBytes("fb$UWvZmTQ");

    static void Main(string[] argv)
    {
/*
      // Распаковка архива с расшифровкой каждого файла в нём. Падает в DecryptData.
      if (argv.Length == 2)
      {
        string archive_filename = argv[0];
        string output_dir = argv[1];
        Console.WriteLine("Unzipping and decrypting {0} ...", archive_filename);

        if (!Directory.Exists(output_dir))
        {
          Directory.CreateDirectory(output_dir);
        }

        using (ZipArchive archive = ZipFile.Open(archive_filename, ZipArchiveMode.Read))
        {
          foreach (ZipArchiveEntry entry in archive.Entries)
          {
            string output_file = output_dir + "/" + entry.FullName;
            Console.WriteLine("Debug: output_file = {0}", output_file);

            using (Stream entry_stream = entry.Open())
            {
              using (Stream output_stream = new FileStream(output_file, FileMode.CreateNew, FileAccess.Write))
              {
                Console.WriteLine("DecryptData start.");
                Foo.Cryptography.DecryptData(entry_stream, output_stream, password, salt);
                Console.WriteLine("DecryptData stop.");
              }
            }
          }
        }
      }

      Console.WriteLine("Использование: Decryptor.exe <zip_архив_с_шифрованными_файлами> <папка_результат>");
*/

      // Распаковка архива в папку с файлами, расшифровка каждого файла. Работает нормально
      if (argv.Length == 3)
      {
        string archive_filename = argv[0];
        string intermediate_dir = argv[1];
        string output_dir = argv[2];

        if (Directory.Exists(intermediate_dir))
        {
          Console.WriteLine("Удаление директории с её содержимого: {0}", intermediate_dir);
          Directory.Delete(intermediate_dir, true);
        }

        ZipFile.ExtractToDirectory(archive_filename, intermediate_dir);

        if (Directory.Exists(output_dir))
        {
          Console.WriteLine("Директория уже существует: {0}", output_dir);
          return;
        }

        Directory.CreateDirectory(output_dir);

        foreach (string encrypted_file in Directory.GetFiles(intermediate_dir))
        {
          string output_file = output_dir + "/" + new FileInfo(encrypted_file).Name;

          using (Stream encrypted_stream = new FileStream(encrypted_file, FileMode.Open, FileAccess.Read))
          {
            using (Stream output_stream = new FileStream(output_file, FileMode.CreateNew, FileAccess.Write))
            {
              Console.WriteLine("DecryptData start.");
              Foo.Cryptography.DecryptData(encrypted_stream, output_stream, password, salt);
              Console.WriteLine("DecryptData stop.");
            }
          }
        }

        Console.WriteLine("Decrypted.");
        return;
      }

      Console.WriteLine("Использование: Decryptor.exe <zip_архив_с_шифрованными_файлами> <промежуточная_папка> <папка_результат>");
    }
  }
}
