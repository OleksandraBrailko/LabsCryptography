using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class RSAKeys
    {
        private List<KeyValuePair<int, int>> _keys;
        private static Random rnd = new Random();

        public RSAKeys()
        {
            _keys = GetKeys();
        }

        public List<KeyValuePair<int, int>> GetRSAKeys
        {
            get { return _keys; }
        }
          

        public List<KeyValuePair<int, int>> GetKeys()   // Згенерувати ключ.
        {

            var p = rnd.Next(10, 100)/*ConvertByteArrayToInt(Random512Bits())*/;
            while (!IsPrime(p))
            {
                p = rnd.Next(10, 100)/*ConvertByteArrayToInt(Random512Bits())*/;
            }
            var q = rnd.Next(10, 100)/*ConvertByteArrayToInt(Random512Bits())*/;
            while (!IsPrime(q) || q==p)
            {
                q = rnd.Next(10, 100)/*ConvertByteArrayToInt(Random512Bits())*/;
            }

            var n = p * q;
            var eulerFunction = (p - 1) * (q - 1);
            var e = rnd.Next(2, eulerFunction);
            while (!IsCoprime(e, eulerFunction) || !IsCoprime(e, modInverse(e, eulerFunction)) || !IsPrime(e))
            {
                e = rnd.Next(2, eulerFunction);
            }
            var d = modInverse(e, eulerFunction);
            return new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(e, n), new KeyValuePair<int, int>(d, n) };
        }

        private bool IsPrime(int number)  //  Перевірити чи число є простим
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }
        private bool IsCoprime(int a, int b)  // Перевірити чи числа є взаємно простими.
        {
            return a == b
                   ? a == 1
                   : a > b
                        ? IsCoprime(a - b, b)
                        : IsCoprime(b - a, a);
        }

        private int modInverse(int a, int n)   // Обчислити обернене.
        {
            int i = n, v = 0, d = 1;
            while (a > 0)
            {
                int t = i / a, x = a;
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
    }
}
