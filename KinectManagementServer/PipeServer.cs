using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO.Pipes;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.ComponentModel;
//using KinectManagementServer;
using Utils;

namespace KinectManagementServer
{
    class PipeServer
    {
        private string clientProcess;
        private string kinectId;

        private KinectManagementServer.UpdateSkeletons update;
        private KinectManagementServer.UpdateSkeletons empty;

        public PipeServer(string client, string kid, KinectManagementServer.UpdateSkeletons _update, KinectManagementServer.UpdateSkeletons _empty)
        {
            this.clientProcess = client;
            this.kinectId = kid;
            this.update = _update;
            this.empty = _empty;
        }

        #region Main

        /// <summary>
        /// The main entry point for the spawned thread. 
        /// If this method is allowed to complete, the thread will be disposed and all 
        /// memory lost.
        /// </summary>
        public void ThreadProc()
        {
            AnonymousPipeServerStream pipeServer =
                    new AnonymousPipeServerStream(PipeDirection.In,
                    HandleInheritability.Inheritable);

            Process childProcess = new Process();
            childProcess.StartInfo.FileName = this.clientProcess;
            childProcess.StartInfo.Arguments += " " + pipeServer.GetClientHandleAsString();
            childProcess.StartInfo.Arguments += " " + kinectId;

#if (DEBUG)
            Console.WriteLine("[Thread] Setting up Kinect with ID: {0}", KinectSensor.KinectSensors[0].UniqueKinectId);
#endif

            childProcess.StartInfo.UseShellExecute = false;
            childProcess.Start();

            pipeServer.DisposeLocalCopyOfClientHandle();

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Skeleton[] skeletonData;

                while (pipeServer.IsConnected)
                {
                    try
                    {
                        skeletonData = (Skeleton[]) formatter.Deserialize(pipeServer);
                        List<Skeleton> toUpdate = new List<Skeleton>();

                        foreach (Skeleton s in skeletonData)
                        {
                            if (s.TrackingState == SkeletonTrackingState.Tracked)
                            {
#if (DEBUG)
                                Console.WriteLine("[Child] Tracking skeleton with ID: {0}", s.TrackingId);
#endif
                                toUpdate.Add(s);
                            }
                        }

                        if (toUpdate.Count > 0)
                        {
                            UpdateSkeletons(toUpdate);
                        }
                        else
                        {
                            EmptyFrame();
                        }
                    }
                    catch (SerializationException e)
                    {
#if (DEBUG)
                        Console.WriteLine("[Thread] Exception: {0}", e.Message);
#else
                        throw e;
#endif
                    }
                }
            }
            catch (IOException e)
            {
#if (DEBUG)
                Console.WriteLine("[Thread] Error: {0}", e.Message);
#else
                throw e;
#endif
            }
        }

        #endregion

        #region Event Handlers

        private void UpdateSkeletons(List<Skeleton> skeletons)
        {
            SkeletonUpdateArgs args = new SkeletonUpdateArgs(kinectId, skeletons);
            update.Invoke(args);
        }

        private void EmptyFrame()
        {
            SkeletonUpdateArgs args = new SkeletonUpdateArgs(kinectId, null);
            empty.Invoke(args);
        }

        #endregion

        #region Getters / Setters

        public string KinectId
        {
            get
            {
                return this.kinectId;
            }
        }

        #endregion

    }
}
