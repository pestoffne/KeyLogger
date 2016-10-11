using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _Keys
{
    struct Letter
    {
        public char Rus;
        public char Eng;

        public Letter(char eng, char rus)
        {
            Eng = eng;
            Rus = rus;
        }
    }

    class Symbol
    {
        private SortedDictionary<Keys, Letter> symbols = new SortedDictionary<Keys,Letter>();

        private void Create()
        {
            symbols.Add(Keys.D0, new Letter('0', '0'));
            symbols.Add(Keys.D1, new Letter('1', '1'));
            symbols.Add(Keys.D2, new Letter('2', '2'));
            symbols.Add(Keys.D3, new Letter('3', '3'));
            symbols.Add(Keys.D4, new Letter('4', '4'));
            symbols.Add(Keys.D5, new Letter('5', '5'));
            symbols.Add(Keys.D6, new Letter('6', '6'));
            symbols.Add(Keys.D7, new Letter('7', '7'));
            symbols.Add(Keys.D8, new Letter('8', '8'));
            symbols.Add(Keys.D9, new Letter('9', '9'));

            symbols.Add(Keys.A, new Letter('a', 'ф'));
            symbols.Add(Keys.B, new Letter('b', 'и'));
            symbols.Add(Keys.C, new Letter('c', 'с'));
            symbols.Add(Keys.D, new Letter('d', 'в'));
            symbols.Add(Keys.E, new Letter('e', 'у'));
            symbols.Add(Keys.F, new Letter('f', 'а'));
            symbols.Add(Keys.G, new Letter('g', 'п'));
            symbols.Add(Keys.H, new Letter('h', 'р'));
            symbols.Add(Keys.I, new Letter('i', 'ш'));
            symbols.Add(Keys.J, new Letter('j', 'о'));
            symbols.Add(Keys.K, new Letter('k', 'л'));
            symbols.Add(Keys.L, new Letter('l', 'д'));
            symbols.Add(Keys.M, new Letter('m', 'ь'));
            symbols.Add(Keys.N, new Letter('n', 'т'));
            symbols.Add(Keys.O, new Letter('o', 'щ'));
            symbols.Add(Keys.P, new Letter('p', 'з'));
            symbols.Add(Keys.Q, new Letter('q', 'й'));
            symbols.Add(Keys.R, new Letter('r', 'к'));
            symbols.Add(Keys.S, new Letter('s', 'ы'));
            symbols.Add(Keys.T, new Letter('t', 'е'));
            symbols.Add(Keys.U, new Letter('u', 'г'));
            symbols.Add(Keys.V, new Letter('v', 'м'));
            symbols.Add(Keys.W, new Letter('w', 'ц'));
            symbols.Add(Keys.X, new Letter('x', 'ч'));
            symbols.Add(Keys.Y, new Letter('y', 'н'));
            symbols.Add(Keys.Z, new Letter('z', 'я'));

            symbols.Add(Keys.Space, new Letter('_', '_'));

            symbols.Add(Keys.Shift, new Letter('S', 'S'));
            symbols.Add(Keys.LShiftKey, new Letter('S', 'S'));
            symbols.Add(Keys.RShiftKey, new Letter('S', 'S'));
            symbols.Add(Keys.Control, new Letter('C', 'C'));
            symbols.Add(Keys.Enter, new Letter('E', 'E'));
        }

        private static Symbol _instance;

        private Symbol() { Create(); }

        public static Symbol Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Symbol();
                }
                return _instance;
            }
        }

        public string GetRus(Keys k)
        {
            try
            {
                return symbols[k].Rus.ToString();
            }
            catch
            {
                //return " " + k.ToString() + " ";
                return "";
            }
        }

        public string GetEng(Keys k)
        {
            try
            {
                return symbols[k].Eng.ToString();
            }
            catch
            {
                //return " " + k.ToString() + " ";
                return "";
            }
        }
    }
}
