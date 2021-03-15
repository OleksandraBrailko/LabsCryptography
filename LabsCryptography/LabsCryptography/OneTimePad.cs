using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class OneTimePad
    {
        //    private string _input;
        //    private string _key;
        //    private Alphabet _alphabet;
        //    enum Values
        //    {
        //        eight = 8,
        //        four = 4
        //    }

        //    public OneTimePad(string input, string key, Alphabet alphabet)
        //    {
        //        _input = input;
        //        _key = key;
        //        _alphabet = alphabet;
        //    }

        //    public string GetInput
        //    {
        //        get { return _input; }
        //    }
        //    public string GetKey
        //    {
        //        get { return _key; }
        //    }
        //    public Alphabet GetAlphabet
        //    {
        //        get { return _alphabet; }
        //    }

        //    private List<string> SplitBinaryString(string input, Values val)
        //    {
        //        var binarySplited = input.Select((c, i) => new { c, i })
        //                    .GroupBy(x => x.i / (int)val)
        //                    .Select(x => String.Join("", x.Select(y => y.c)))
        //                    .ToList();
        //        return binarySplited;
        //    }
        //    private List<int> SplitBinaryStringToDigit(List<string> input)
        //    {
        //        var result = input.Select(x => Convert.ToInt32(x, 2)).ToList();
        //        return result;
        //    }

        //    private string ToBinary(int[] data, Values val)
        //    {
        //        return string.Join("", data.Select(x => Convert.ToString(x, 2).PadLeft((int)val, '0')));
        //    }

        //    private List<int> GetDataToEncrypt(string input)
        //    {
        //        var a = ToBinary(input.Select(x => GetAlphabet.GetIndexFromAlphabet(x)).ToArray(), Values.eight);
        //        var result = SplitBinaryStringToDigit(SplitBinaryString(ToBinary(input.Select(x => GetAlphabet.GetIndexFromAlphabet(x)).ToArray(), Values.eight), Values.eight));
        //        return result;
        //    }
        //    private List<int> Xor(List<int> input, List<int> key)
        //    {
        //        List<int> xor = new List<int>();
        //        for (int i = 0; i < input.Count; i++)
        //        {
        //            xor.Add(input[i] ^ key[i % key.Count]);
        //        }
        //        return xor;
        //    }
        //    public string EncryptDecrypt()
        //    {
        //        var preraredText = GetDataToEncrypt(GetInput);
        //        var preparedKey = GetDataToEncrypt(GetKey);
        //        var result = string.Join("", SplitBinaryString(ToBinary(Xor(preraredText, preparedKey).ToArray(), Values.eight), Values.eight).Select(x => Convert.ToInt32(x, 2)).ToList().Select(x => GetAlphabet.GetLetterFromIndex(x)));
        //        return result;
        //    }
        //}


        private string _input;
        private string _key;

        public OneTimePad(string input, string key)
        {
            _input = input;
            _key = key;

        }

        public string GetInput
        {
            get { return _input; }
        }
        public string GetKey
        {
            get { return _key; }
        }
        private int GetKeyLenght(string key)
        {
            var binaryKey = string.Join("", key.Select(x => Convert.ToString(GetIndexFromAlphabet(x), 2)));

            return binaryKey.Length;
        }

        private List<long> SplitIntoBlocks(string input, int limitLength) // Розбити на блоки в числовому форматі
        {
            var binaryInput = string.Join("", input.Select(x => Convert.ToString(GetIndexFromAlphabet(x), 2).PadLeft(GetAlphabetLenght(), '0'))); // перевести в бінарний рядок, вирівняти
            var splitedBinaryBlocks = SplitIntoMaxBlocks(binaryInput, limitLength); // розбити на блоки з довжини ключа (максимальну кількість)
            splitedBinaryBlocks.Add(binaryInput.Substring(String.Join("", splitedBinaryBlocks.Select(x => x)).Length, binaryInput.Length - String.Join("", splitedBinaryBlocks.Select(x => x)).Length)); // додати залишковий блок
            return splitedBinaryBlocks.Where(x => x != "").Select(x => Convert.ToInt64(x, 2)).ToList(); // кожний елемент перевести в десяткову систему числення
        }

        private List<string> SplitIntoMaxBlocks(string str, int chunkSize) // Розбити бінарний стрінг на максимально можливу кількість блоків (після треба додати те, що залишилося)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize)).ToList();
        }

        public string Encrypt() // Шифрування
        {
            var keyLength = GetKeyLenght(GetKey);  // довжина кюча
            var blocks = SplitIntoBlocks(GetInput, keyLength); // розбити на блоки з довжиною, що рівня довжині ключа (результат - в десятковій системі)

            var binaryKey = Convert.ToInt64(string.Join("", GetKey.Select(x => Convert.ToString(GetIndexFromAlphabet(x), 2))), 2);  // перевести ключ в двійкову послідовність, а потім в десяткову
            var cipheredText = new List<long>(); // результуючий список зашифрованих елементів

            foreach (var item in blocks)
            {
                cipheredText.Add(item ^ binaryKey);  // операція XOR
            }
             
            return string.Join("", cipheredText.Select(x => Convert.ToString(x).PadLeft(binaryKey.ToString().Length, '0')));  // результуючий рядок (послідовність десяткових числе, вирівняних до довжини ключа)
        }

        public string Decrypt() // Розшифрувати
        {
            var binaryKey = Convert.ToInt64(string.Join("", GetKey.Select(x => Convert.ToString(GetIndexFromAlphabet(x), 2))), 2); // перевести ключ в двйкову послідовність та тримати число в десятковій системі
            var cipheredBlocks = SplitIntoMaxBlocks(GetInput, Convert.ToString(binaryKey).Length).Select(x => Convert.ToInt64(x)).ToList(); // розбити шифротекст на блоки дожиною, як і довжина ключа


            var decryptedBlocks = new List<long>();  // результуючий список розшированих блоків
            foreach (var item in cipheredBlocks)
            {
                decryptedBlocks.Add(item ^ binaryKey); // додати елемент до списку після виконання операції XOR 
            }

            var result = Decr(decryptedBlocks, GetKey);  // розшифрувати розшированы блоки
            return result;
        }

        private string Decr(List<long> decryptedBlocks, string key) // Розшифрувати блоки  
        {

            var mainPart = decryptedBlocks.Take(decryptedBlocks.Count - 1).Select(x => Convert.ToString(x, 2).PadLeft(GetKeyLenght(key), '0')).ToList(); // розбити на блоки довжиною, як і довжина ключа (окрім останнього елелменту)
            var remainder = Convert.ToString(decryptedBlocks.Last(), 2); // залишок - останній елемент (теж перевести у двійкову)

            var mainPart2 = SplitIntoMaxBlocks(string.Join("", mainPart.Select(x => x)), GetAlphabetLenght()); // розбити основну части на блоки з довжину, як і дожина алфавіту 
            var remainder2 = string.Join("", mainPart.Select(x => x))
                .Substring(string.Join("", mainPart.Select(x => x)).Count() - (string.Join("", mainPart.Select(x => x)).Count() - string.Join("", mainPart2.Select(x => x)).Count()), string.Join("", mainPart.Select(x => x)).Count() - string.Join("", mainPart2.Select(x => x)).Count()); // залишкові елементи

            var difference = GetAlphabetLenght() - (remainder2.Count() + remainder.Count()); // різниця, якої не вистачає для отримання останнього елемента (елементів)

            var lastElement = new List<string>(); // результуюча змінна залишкових елементів

            if (difference < 0)
            {
                var rest = remainder2.Insert(remainder2.Length, remainder.PadLeft(10 - remainder2.Count(), '0'));
                lastElement = SplitIntoMaxBlocks(rest, GetAlphabetLenght());
            }
            else
            {
                lastElement.Add(remainder2.Insert(remainder2.Length, remainder.PadLeft(difference + remainder.Count(), '0')));
            }


            mainPart2.AddRange(lastElement); // додати останні елементи


            //var res = string.Join("", mainPart2.Select(x => GetLetterFromIndex(Convert.ToInt32(x, 2))));
            return string.Join("", mainPart2.Select(x => GetLetterFromIndex(Convert.ToInt32(x, 2))));  // повернути розшифровані блоки
        }

        private int GetIndexFromAlphabet(char ch)
        {
            var alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ,.!?'-абвгґдеєжзиіїйклмнопрстуфхцчшщьюя1234567890\r\n";
            //var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ,.!?";
            return alphabet.IndexOf((ch)) + 1;
        }
        private char GetLetterFromIndex(long index)
        {
            var alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ,.!?'-абвгґдеєжзиіїйклмнопрстуфхцчшщьюя1234567890\r\n";
            //var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ,.!?";
            var ch = index - 1 < alphabet.Length && index != 0 ? alphabet.ElementAt((int)index - 1) : ' ';
            return ch;
        }
        private int GetAlphabetLenght()
        {
            return Convert.ToString("АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ,.!?'-абвгґдеєжзиіїйклмнопрстуфхцчшщьюя1234567890\r\n".Length, 2).Length;
        }
    }
}
