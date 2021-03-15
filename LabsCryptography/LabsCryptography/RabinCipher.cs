using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class RabinCipher
    {
        private string _input;
        private KeyValuePair<int, int> _key;
        public RabinCipher(string input, KeyValuePair<int, int> key)
        {
            _input = input;

            _key = key; 
        }

        public string GetInput
        {
            get { return _input; }
        }
        public KeyValuePair<int, int> GetKey
        {
            get { return _key; }
        }






        private List<BigInteger> EuclidExtended(BigInteger a, BigInteger b)
        {
            BigInteger s = 0;
            BigInteger x = 1;
            BigInteger t = 1;
            BigInteger y = 0;
            BigInteger r = b;
            BigInteger gcd = a;
            while (!r.Equals(0))
            {
                BigInteger q = gcd / r;
                BigInteger tr = r;
                r = gcd - (q * r);
                gcd = tr;

                BigInteger ts = s;
                s = x - (q * s);
                x = ts;

                BigInteger tt = t;
                t = y - (q * t);
                y = tt;
            }
            return new List<BigInteger> { gcd, x, y };
        }

       private List<int> GetOptions(int p, int q, int C)
        {
            var parameters = EuclidExtended(p, q);
            var n = p * q;
            var r = BinarPowByModule(C, (p + 1) / 4, p);
            var s = BinarPowByModule(C, (q + 1) / 4, q);



            var x = mod((int)(parameters[1] * p * s + parameters[2] * q * r), n);
            var y = mod((int)(parameters[1] * p * s - parameters[2] * q * r), n);
            return new List<int> { x, n - x, y, n - y };
        }

        private int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
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


        private  List<int> SplitIntoBlocks(string input, KeyValuePair<int, int> key) // Розбити на блоки в числовому форматі
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
            return string.Join(" ", blocks.Select(x => BinarPowByModule(x, GetKey.Key, GetKey.Value)));
        }

        private  List<List<int>> GetAllPossibleCombos(
          List<List<int>> strings)
        {
            IEnumerable<IEnumerable<int>> combos = new int[][] { new int[0] };

            foreach (var inner in strings)
                combos = from c in combos
                         from i in inner
                         select c.Append(i);

            return combos.Select(x => x.ToList()).ToList();
        }


        public string Decrypt() // Розшифрувати
        {
            //var blocks = GetInput.Select(x => Convert.ToInt32(x)).ToList(); // зашифровані блоки

            var blocks = GetInput.Split(' ').Select(x=> int.Parse(x)).ToList();

            var decryptedBlocks1 = blocks.Select(x => GetOptions(GetKey.Key, GetKey.Value, x).ToList()).ToList();

            var r = GetAllPossibleCombos(decryptedBlocks1);

            var result = "";
            foreach (var a in r)
            {
                result += GetMessages(a, new KeyValuePair<int, int>(2,GetKey.Key*GetKey.Value));
                result += "\n\n\n";
            }

            return result;
        }

        private string GetMessages(List<int> decryptedBlocks, KeyValuePair<int, int> SecretKey)
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
