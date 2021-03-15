using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class RSA
    {
        private string _input;
        private KeyValuePair<int, int> _key;



        public RSA(string input, KeyValuePair<int,int> key)
        {
            _input = input;

            _key = key; /*new KeyValuePair<int, int>(48, 35);*/
        }

        public string GetInput
        {
            get { return _input; }
        }
        public KeyValuePair<int, int> GetKey
        {
            get { return _key; }
        }


       private BigInteger BinarPowByModule(BigInteger number, BigInteger pow, BigInteger module)  // Бінарне піднесення до степеня по модулю.
        {
            BigInteger res = 1;
            number = number % module;

            while (pow > 0)
            {
                if ((pow & 1) == 1)  // перевірка степені на парність/непарність
                    res = (res * number) % module;    // якщо степінь непарна, результую змінну змінити
                pow = pow >> 1;   // поділти на 2 степінь
                number = (number * number) % module;    // обчислити нове значення числа
            }
            return res;
        }

        private List<int> SplitIntoBlocks(string input, KeyValuePair<int, int> key) // Розбити на блоки в числовому форматі
        {
            var binaryInput = string.Join("", input.Select(x => Convert.ToString(GetIndexFromAlphabet(x), 2).PadLeft(GetAlphabetLenght(), '0'))); // перевести в бінарний стрінг
            var limitLength = Convert.ToString(key.Value, 2).Length - 1;
            var splitedBinaryBlocks = SplitIntoMaxBlocks(binaryInput, limitLength);
            splitedBinaryBlocks.Add(binaryInput.Substring(String.Join("", splitedBinaryBlocks.Select(x => x)).Length, binaryInput.Length - String.Join("", splitedBinaryBlocks.Select(x => x)).Length));

            return splitedBinaryBlocks.Where(x => x != "").Select(x => Convert.ToInt32(x, 2)).ToList();
        }

       private List<string> SplitIntoMaxBlocks(string str, int chunkSize) // Розбити бінарний стрінг на максимально можливу кількість блоків (після треба додати те, що залишилося)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize)).ToList();
        }

        public  string Encrypt() // Зашифрувати, 5 - 26 букв в алфавіті - при переводі в двійкову 11010!!
        {
            var blocks = SplitIntoBlocks(GetInput, GetKey);
            var cipheredText = new List<long>(); // результуючий список зашифрованих елементів

            foreach (var item in blocks)
            {
                cipheredText.Add((int)BinarPowByModule(item, GetKey.Key, GetKey.Value));  // обчислення зашифрованого значення
            }
            return /*string.Join("", blocks.Select(x => (char)BinarPowByModule(x, GetKey.Key, GetKey.Value)));*/string.Join("", cipheredText.Select(x => Convert.ToString(x).PadLeft(GetKey.Value.ToString().Length, '0')));
        }

        public  string Decrypt() // Розшифрувати
        {
            var cipheredBlocks = SplitIntoMaxBlocks(GetInput, Convert.ToString(GetKey.Value).Length).Select(x => Convert.ToInt64(x)).ToList(); // розбити шифротекст на блоки дожиною, як і довжина ключа


            var decryptedBlocks = new List<long>();  // результуючий список розшированих блоків
            foreach (var item in cipheredBlocks)
            {
                decryptedBlocks.Add((int)BinarPowByModule(item, GetKey.Key, GetKey.Value)); // додати елемент до списку після виконання операції XOR 
            }


            //var blocks = GetInput.Select(x => Convert.ToInt32(x)).ToList();  // Розбити вхідний текст на блоки

            //var decryptedBlocks = blocks.Select(x => (int)BinarPowByModule(x, GetKey.Key, GetKey.Value)).ToList();  // обчислити розшифровані знічення 

            var mainPart = decryptedBlocks.Take(decryptedBlocks.Count - 1).Select(x => Convert.ToString(x, 2).PadLeft(GetPublicKeyLenght(GetKey) - 1, '0')).ToList();
            var remainder = Convert.ToString(decryptedBlocks.Last(), 2);

            var mainPart2 = SplitIntoMaxBlocks(string.Join("", mainPart.Select(x => x)), GetAlphabetLenght());
            var remainder2 = string.Join("", mainPart.Select(x => x))
                .Substring(string.Join("", mainPart.Select(x => x)).Count() - (string.Join("", mainPart.Select(x => x)).Count() - string.Join("", mainPart2.Select(x => x)).Count()), string.Join("", mainPart.Select(x => x)).Count() - string.Join("", mainPart2.Select(x => x)).Count());

            var difference = GetAlphabetLenght() - (remainder2.Count() + remainder.Count());

            var lastElement = new List<string>();

            if (difference < 0)
            {
                var rest = remainder2.Insert(remainder2.Length, remainder.PadLeft(10 - remainder2.Count(), '0'));
                lastElement = SplitIntoMaxBlocks(rest, GetAlphabetLenght());
            }
            else
            {
                lastElement.Add(remainder2.Insert(remainder2.Length, remainder.PadLeft(difference + remainder.Count(), '0')));
            }


            mainPart2.AddRange(lastElement);


            var res = string.Join("", mainPart2.Select(x => GetLetterFromIndex(Convert.ToInt32(x, 2))));
            return string.Join("", mainPart2.Select(x => GetLetterFromIndex(Convert.ToInt32(x, 2))));
        }


        private int GetIndexFromAlphabet(char ch)
        {
            var alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ,.!?'-абвгґдеєжзиіїйклмнопрстуфхцчшщьюя1234567890\r\n";
            //var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ,.!?";
            return alphabet.IndexOf((ch)) + 1;
        }
        private char GetLetterFromIndex(int index)
        {
            var alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ,.!?'-абвгґдеєжзиіїйклмнопрстуфхцчшщьюя1234567890\r\n";
            //var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ,.!?";
            var ch = index-1<alphabet.Length && index != 0 ? alphabet.ElementAt(index - 1): ' ';
            return ch;
        }
        private int GetAlphabetLenght()
        {
            return Convert.ToString("АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ,.!?'-абвгґдеєжзиіїйклмнопрстуфхцчшщьюя1234567890\r\n".Length, 2).Length;
        }
        public static int GetPublicKeyLenght(KeyValuePair<int, int> publicKey)
        {
            return Convert.ToString(publicKey.Value, 2).Length;
        }

    
}
}
