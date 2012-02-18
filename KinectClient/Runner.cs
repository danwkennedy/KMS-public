using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Threading;
using Microsoft.Kinect;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace KinectClient
{
    /// <summary>
    /// The entry point for the program
    /// </summary>
    class Runner
    {
        static void Main(string[] args)
        {
            // We are expecting exactly two arguments (both are required)
            if (args.Length != 2)
            {
                Console.WriteLine("Both pipes and sockets are expecting two arguments");
            }
            else
            {
                // Start a pipe client
                KinectClient client = new KinectClientPipe(args[0], args[1]);
                client.Start();
            }
        }
    }
}
