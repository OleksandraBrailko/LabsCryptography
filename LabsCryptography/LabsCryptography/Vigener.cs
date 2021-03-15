using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class Vigener
    {
        private string _input;
        private string _key;
        private Alphabet _alphabet;

        public Vigener(string input, string key, Alphabet alphabet)
        {
            _input = input;
            _key = key;
            _alphabet = alphabet;
        }

        public string GetInput
        {
            get { return _input; }
        }
        public string GetKey
        {
            get { return _key; }
        }
        public Alphabet GetAlphabet
        {
            get { return _alphabet; }
        }

        public string Encrypt()          // Шифрування алгоритмом Віженера
        {
            string encrypted = "";          // результуючий рядок
            var kCounter = 0;          // змінна для відліку порядкового номера символа в ключовому слові
            for (int i = 0; i < GetInput.Length; i++)
            {
                kCounter = kCounter == GetKey.Length ? 0 : kCounter;          // якщо останній символ ключа - на наступному кроці повернутися до початкового
                int encrChar = (GetAlphabet.GetIndexFromAlphabet(GetInput[i]) + GetAlphabet.GetIndexFromAlphabet(GetKey[kCounter])) % GetAlphabet.GetLenght();          // обчислити новий індекс
                encrypted += GetAlphabet.GetLetterFromIndex(encrChar);          // відшукати літеру за індексом
                kCounter++;
            }
            return encrypted;
        }

        public string Decrypt()          // Дешифрування
        {
            string changedKey = "";          // результуюча змінна для нового ключа
            foreach (var ch in GetKey)
            {
                changedKey += GetAlphabet.GetLetterFromIndex((GetAlphabet.GetLenght() - GetAlphabet.GetIndexFromAlphabet(ch)) % GetAlphabet.GetLenght());          // обчислення симовлу нового ключа
            }
            var obj = new Vigener(GetInput, changedKey, GetAlphabet);          
            return obj.Encrypt();          // повернення розшифрованого тексту
        }

    }
}
