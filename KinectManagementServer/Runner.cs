using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Threading;

namespace KinectManagementServer
{

    class Runner
    {
        /// <summary>
        /// The entry point for the Kinect Management Server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            KinectManagementServer KMS = new KinectManagementServer();
            KMS.Run();
        }
    }
}
