using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //new TestListener().Test1();
            new TestVisitor().Test1();
            //new TestReadSkeleton().Test1();
        }
    }
}
