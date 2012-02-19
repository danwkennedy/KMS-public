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

        #region Vars

        #endregion

        #region Init

        static void Main(string[] args)
        {
            
        }

        private void InitKinectClients()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                foreach (KinectSensor sensor in KinectSensor.KinectSensors)
                {
                    if (sensor.Status == KinectStatus.Connected)
                    {
                        PipeServer server = new PipeServer("D:\\git\\2KinectTechDemo\\KinectClient\\bin\\Debug\\KinectClient.exe", sensor.UniqueKinectId);

                        Thread kinectThread = new Thread(new ThreadStart(server.ThreadProc));
                        //threads.Add(kinectThread);
                        //skeletonDict[sensor.UniqueKinectId] = new Skeleton[2];
                        kinectThread.Start();
                    }
                }
            }
        }

        private void InitGesture()
        {

        }

        private void InitSocketServer()
        {

        }

        #endregion

    }
}
