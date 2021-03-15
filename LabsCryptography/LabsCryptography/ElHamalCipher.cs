using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class ElHamalCipher
    {
        private string _input;
        private List<int> _key;
        private static Random rnd = new Random();


        public ElHamalCipher(string input, List<int> key)
        {
            _input = input;

            _key = key; /*new KeyValuePair<int, int>(48, 35);*/
        }

        public string GetInput
        {
            get { return _input; }
        }
        public List<int> GetKey
        {
            get { return _key; }
        }

        private List<int> SplitIntoBlocks(string input, int p) // Розбити на блоки в числовому форматі
        {
            var binaryInput = string.Join("", input.Select(x => Convert.ToString(GetIndexFromAlphabet(x), 2).PadLeft(GetAlphabetLenght(), '0'))); // перевести в бінарний стрінг
            var limitLength = Convert.ToString(p, 2).Length - 1;
            var splitedBinaryBlocks = SplitIntoMaxBlocks(binaryInput, limitLength);
            splitedBinaryBlocks.Add(binaryInput.Substring(String.Join("", splitedBinaryBlocks.Select(x => x)).Length, binaryInput.Length - String.Join("", splitedBinaryBlocks.Select(x => x)).Length));

            return splitedBinaryBlocks.Where(x => x != "").Select(x => Convert.ToInt32(x, 2)).ToList();
        }

        private List<string> SplitIntoMaxBlocks(string str, int chunkSize) // Розбити бінарний стрінг на максимально можливу кількість блоків (після треба додати те, що залишилося)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize)).ToList();
        }

        public string Encrypt() // Зашифрувати, 5 - 26 букв в алфавіті - при переводі в двійкову 11010!!
        {
            var blocks = SplitIntoBlocks(GetInput, GetKey[0]);  // блоки
            //var k = /*rnd.Next(2, publicKey[0] - 1)*/8;
            var cipheredBlocks = new List<Tuple<int, int>>();  // результуючий список пари чисел
            for (int i = 0; i < blocks.Count; i++)  // цикл, для обчислення пари шифроелементів для кожного блоку
            {
                var k = rnd.Next(2, /*publicKey[0]-1*/10);
                cipheredBlocks.Add(new Tuple<int, int>((int)BinarPowByModule(GetKey[1], k, GetKey[0]), (int)((BigInteger)(blocks[i] * (BigInteger)Math.Pow(GetKey[2], k)) % GetKey[0])));
            }
            var result = string.Join("", cipheredBlocks.Select(x => x.ToString()));  // додати до результуючого рядка тримані пари чисел
            return result;
        }
        private BigInteger BinarPowByModule(BigInteger number, BigInteger pow, BigInteger module)  // Бінарне піднесення до степеня по модулю.
        {
            BigInteger res = 1;
            number = number % module;

            while (pow > 0)
            {
                if ((pow & 1) == 1)
                    res = (res * number) % module;
                pow = pow >> 1;
                number = (number * number) % module;
            }
            return res;
        }

        private BigInteger modInverse(BigInteger a, BigInteger n)   // Обчислити обернене.
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        public  string Decrypt(List<int> PublicKey) // Розшифрувати
        {
            var blocks = new List<string>();     // блоки пари чисел (string)
            Regex regex = new Regex(@"\(.*?\)");
            MatchCollection matches = regex.Matches(GetInput);
            for (int i = 0; i < matches.Count; i++)
            {
                blocks.Add(matches[i].ToString());
            }

            var changedBlocks = new List<List<int>>();  // блоки пари чисел

            foreach (var a in blocks)
            {
                var t = a.Trim(')', '(').Replace(", ", "#").Split('#').ToList();
                changedBlocks.Add(t.Select(x => int.Parse(x)).ToList());
            }


            var decryptedBlocks = new List<int>();  // блоки з розшированими значеннями
            foreach (var item in changedBlocks)
            {
                decryptedBlocks.Add((int)((item[1] * modInverse(BigInteger.Pow(item[0], GetKey[0]), PublicKey[0])) % PublicKey[0]));
            }
            var result = Decr(decryptedBlocks, new KeyValuePair<int, int>(0, PublicKey[0]));   // розбиття блоків на кінцеві значення
            return result;
        }

        private string Decr(List<int> decryptedBlocks, KeyValuePair<int, int> SecretKey)
        {

            var mainPart = decryptedBlocks.Take(decryptedBlocks.Count - 1).Select(x => Convert.ToString(x, 2).PadLeft(GetPublicKeyLenght(SecretKey) - 1, '0')).ToList();
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
            var ch = index - 1 < alphabet.Length && index != 0 ? alphabet.ElementAt(index - 1) : ' ';
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
