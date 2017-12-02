using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrackMe;

namespace NameGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var form = new Form1();

            //assembly has been recompiled to make all its methods public
            form.eval_a(new List<string> { "123", "234"}.ToArray());

            form.eval_a();
        }
    }
}
