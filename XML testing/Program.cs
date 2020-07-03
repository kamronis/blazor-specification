using System;
using System.Linq;
using System.Xml.Linq;

namespace XML_testing
{
    class Program
    {
        static void Main(string[] args)
        {
            XElement db = XElement.Load(@"C:\Users\Kamroni\Desktop\Test_cassete_20200629\meta\Test_cassete_20200629_current.fog");
            var count = db.Elements("photo-doc").;

            Console.WriteLine(count);
        }
    }
}
