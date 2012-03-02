using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            UnityThread client = new UnityThread();
            client.ThreadProc();
        }
    }
}
