using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();

            using (var c = new ServiceReference1.Service1Client())
            {
                Console.WriteLine(c.GetData(1));
            }

            Console.ReadLine();
        }
    }
}
