using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class AffineCipher
    {
        private string _input;
        private KeyValuePair<int,int> _key;
        private Alphabet _alphabet;
        public enum ReversedSign { plus, minus }
        public static Random rnd = new Random();

        public AffineCipher(string input, Alphabet alphabet)
        {
            _input = input;
            _alphabet = alphabet;
            _key = GenerateKeyPair(); /*new KeyValuePair<int, int>(48, 35);*/
        }

        public string GetInput
        {
            get { return _input; }
        }
        public KeyValuePair<int,int> GetKey
        {
            get { return _key; }
            set { _key = value; }
        }
        public Alphabet GetAlphabet
        {
            get { return _alphabet; }
        }

        private KeyValuePair<int, int> GenerateKeyPair()  //Згенерувати ключ
        {
            var a = rnd.Next(1, GetAlphabet.GetLenght()-1);      // згенерувати випадкове число "а" в межах від [1; довжина алфавіту-1)
            var secondCondition = gcd(a, GetAlphabet.GetLenght());    // Пошук найбільшого спільного дільника (а та довжини алфавіту)
            while (secondCondition != 1)    // поки НСД(a,n) не рівне 1, генерувати інше випадкове число
            {
                a = rnd.Next(1, GetAlphabet.GetLenght()-1);
                secondCondition = gcd(a, GetAlphabet.GetLenght());
            }
            var s = rnd.Next(0, GetAlphabet.GetLenght()-1);     // згенерувати довільне число на проміжку від [0; довжина алфавіту-1)
            return new KeyValuePair<int, int>(a, s);    // повернути пару чисел
        }
        private int gcd(int a, int b)   //Знайти НСД
        {
            if (a == 0)
                return b;

            return gcd(b % a, a);
        }

        private int modInverse(int a, int b, Enum e)   //Зайти обернене число за модулем
        {
            int b0 = b;
            int y = 0, x = 1;

            if (b == 1)
                return 0;

            while (a > 1)
            {
                int q = a / b;    // q - коефіцієнт
                int t = b;

                // b тепер - решта; обчислення за алгоритмом Евкліда
                b = a % b;
                a = t;
                t = y;

                // Оновити x та y 
                y = x - q * y;
                x = t;
            }

            // Зробити x позитивним 
            if (x < 0 && e.Equals(ReversedSign.plus))
            {
                x += b0;
            }

            return x;
        }

        public  string Encrypt()   // Зашифрувати вхідний текст
        {
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.WriteAllText(Path.Combine(executableLocation, "AffineKey.txt"), string.Join(",", GetKey.Key, GetKey.Value));
            var ciphered = "";   // результуюча змінна для запису зашифрованого тексту
            //var a = "";
            //var p = GetAlphabet.GetLenght();   
            for (int i = 0; i < GetInput.Length; i++)
            {
                ciphered +=GetAlphabet.GetLetterFromIndex(((GetKey.Key * GetAlphabet.GetIndexFromAlphabet(GetInput[i])) + GetKey.Value) % GetAlphabet.GetLenght());    //обчислення індексу зашифрованого символу та визначення символу за індексом
                //a += " "+GetAlphabet.GetIndexFromAlphabet(GetInput[i]);
                
            }
            return ciphered;
        }

        public string Decrypt()   // Розшифтуватиы
        {
            var decrypted = "";
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var primaryKey = File.ReadAllText(Path.Combine(executableLocation, "AffineKey.txt")).Split(',').ToList();
            GetKey = new KeyValuePair<int, int>(Convert.ToInt32(primaryKey[0]), Convert.ToInt32(primaryKey[1]));  // отримати ключ
            for (int i = 0; i < GetInput.Length; i++)
            {
          
                var index = GetAlphabet.GetIndexFromAlphabet(GetInput[i]) < GetKey.Value ? GetAlphabet.GetIndexFromAlphabet(GetInput[i]) + GetAlphabet.GetLenght() : GetAlphabet.GetIndexFromAlphabet(GetInput[i]);    // перевірка умови для y
                decrypted += GetAlphabet.GetLetterFromIndex((modInverse(GetKey.Key, GetAlphabet.GetLenght(), ReversedSign.plus) * (index - GetKey.Value)) % GetAlphabet.GetLenght());  // обчислення розшифрованого індексу та запис до результуючого рядка

            }
            return decrypted;

        }

    }
}
