using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LabsCryptography
{
    class ElHamalKey
    {
        private List<List<int>> _keys;
        private static Random rnd = new Random();

        public ElHamalKey()
        {
            _keys = GetKeys();
        }

        public List<List<int>> GetElHamalKeys
        {
            get { return _keys; }
        }

        private List<List<int>> GetKeys()   // Згенерувати ключ.
        {
            var p = rnd.Next(10, 100);
            while (!IsPrime(p))
            {
                p = rnd.Next(10, 100);
            }
            var g = GetPrimitiveRoot(p);
            var x = rnd.Next(2, /*p - 1*/20);
            var y = BinarPowByModule(g, x, p);

            return new List<List<int>> { new List<int> { p, g, (int)y }, new List<int> { x } };
        }
        private int GetPrimitiveRoot(int p)  // відшукати первісний корінь
        {
            var result = 0;
            for (int i = 2; i < p; i++)
            {
                if (gcd(i, p) == 1)
                {
                    result = i;
                    break;
                }
            }
            return result;
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

        private int gcd(int a, int b)
        {
             
            if (a == 0)   // Все дільться на  0   
                return b;
            if (b == 0)
                return a;
       
            if (a == b)    // основа 
                return a;
             
            if (a > b)       // а - більше
                return gcd(a - b, b);
            return gcd(a, b - a);
        }
    }
}
