using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Threading;
using System.Runtime.Remoting.Contexts;
using System.Runtime.CompilerServices;

namespace KinectManagementServer
{
    class KinectManagementServer
    {

        /// <summary>
        /// A delegate method to allow for the Pipe threads to update the skeleton 
        /// data of the players
        /// </summary>
        /// <param name="e">The update args with the KinectId associated with the calling thread and a List of Skeletons to update</param>
        public delegate void UpdateSkeletons(SkeletonUpdateArgs e);

        #region Vars

        private List<Thread> kinectPipes;
        private List<Thread> gestures;

        private Dictionary<string, Dictionary<int, Player>> players;

        #endregion

        #region Init

        void InitializeComponents()
        {
            InitKinectClients();
            InitGesture();
            InitSocketServer();
        }

        /// <summary>
        /// Checks the number of connected sensors and sets up a PipeServer to handle each sensor.
        /// </summary>
        private void InitKinectClients()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                foreach (KinectSensor sensor in KinectSensor.KinectSensors)
                {
                    if (sensor.Status == KinectStatus.Connected)
                    {
                        PipeServer server = new PipeServer("D:\\git\\2KinectTechDemo\\KinectClient\\bin\\Debug\\KinectClient.exe", sensor.UniqueKinectId, SkeletonUpdate, NoSkeletons);
                        Thread kinectThread = new Thread(new ThreadStart(server.ThreadProc));
                        kinectThread.IsBackground = true;
                        kinectPipes.Add(kinectThread);
                        kinectThread.Start();
                        players.Add(sensor.UniqueKinectId, new Dictionary<int, Player>());
                    }
                }
            }
        }

        /// <summary>
        /// TODO: comment here
        /// </summary>
        private void InitGesture()
        {

        }

        /// <summary>
        /// TODO: comment here
        /// </summary>
        private void InitSocketServer()
        {

        }

        #endregion

        public KinectManagementServer()
        {
            kinectPipes = new List<Thread>();
            players = new Dictionary<string, Dictionary<int, Player>>();
        }

        public void Run()
        {
            InitializeComponents();

            while (true)
            {

            }
        }

        #region Pipe Event Handling

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SkeletonUpdate(SkeletonUpdateArgs e)
        {
            Console.WriteLine("[Main] Received skeletons from {0}", e.KinectId);

            foreach (Skeleton skeleton in e.Skeletons)
            {
                try
                {
                    Player p = players[e.KinectId][skeleton.TrackingId];
                }
                catch (KeyNotFoundException e1)
                {
                    if (players.Count < (kinectPipes.Count * 2))
                    {
                        AddPlayer(new Player(players.Count, e.KinectId, skeleton));
                        Console.WriteLine("Adding new player");
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void NoSkeletons(SkeletonUpdateArgs e)
        {
            RemovePlayerList(e.KinectId);
        }

        #endregion


        #region Player Management

        private void AddPlayer(Player p)
        {
            players[p.KinectId][p.Skeleton.TrackingId] = p;
        }

        private void RemovePlayerList(string KID)
        {
            int[] keys = players[KID].Keys.ToArray<int>();

            for (int i = 0; i < keys.Length; i++)
            {
                try
                {
                    Console.WriteLine("[Client] Removing Player number {0}", players[KID][keys[i]].PlayerId);
                    players[KID].Remove(keys[i]);
                }
                finally { }
            }
        }

        #endregion
    }
}
