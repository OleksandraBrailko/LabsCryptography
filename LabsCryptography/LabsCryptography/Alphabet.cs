using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class Alphabet
    {

        private string _alphabet;

        public Alphabet() // Встановлення початкового стану об'єкта 
        {
            _alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ,.!?'-абвгґдеєжзиіїйклмнопрстуфхцчшщьюя1234567890\r\n";
        }
        public string GetAlphabet
        {
            get { return _alphabet; }
        }
        public int GetIndexFromAlphabet(char ch) // Пошук індексу заданої літери в алфавіті
        {
            var result = GetAlphabet.Contains(ch) ? GetAlphabet.IndexOf((ch)) : (int)ch;
            return result;
        }
        public char GetLetterFromIndex(int index) // Пошук літери за заданим індексом
        {
            var result = GetAlphabet.Count() - 1 < index ? (char)index : GetAlphabet.ElementAt(index);
            return result;
        }
        public int GetLenght()
        {
            return GetAlphabet.Length;
        }


    }
}
