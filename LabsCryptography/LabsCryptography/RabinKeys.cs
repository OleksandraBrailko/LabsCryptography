using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class RabinKeys
    {

        private List<KeyValuePair<int, int>> _keys;
        private static Random rnd = new Random();

        public RabinKeys()
        {
            _keys = GetKeys();
        }

        public List<KeyValuePair<int, int>> GetRabinKeys
        {
            get { return _keys; }
        }

        private List<KeyValuePair<int, int>> GetKeys()   // Згенерувати ключ.
        {
            var p = rnd.Next(1000, 3000)/*ConvertByteArrayToInt(Random512Bits())*/;
            while (!IsPrime(p) || p % 4 != 3)
            {
                p = rnd.Next(1000, 3000)/*ConvertByteArrayToInt(Random512Bits())*/;
            }
            var q = rnd.Next(1000, 3000)/*ConvertByteArrayToInt(Random512Bits())*/;
            while (!IsPrime(q) || q % 4 != 3 || p == q)
            {
                q = rnd.Next(1000, 3000)/*ConvertByteArrayToInt(Random512Bits())*/;
            }
            var n = p * q;
            return new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(2, n), new KeyValuePair<int, int>(p, q) };
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
  

    }
}
