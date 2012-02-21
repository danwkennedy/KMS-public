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
    /// <summary>
    /// Controls the main thread for the KMS.
    /// This class will run the main thread of the KMS. It will 
    /// spawn a new thread for each Kinect sensor found on the USB 
    /// bus as well as a gesture thread for each sensor. This class 
    /// will also be responsible for assigning players to skeletons.
    /// </summary>
    class KinectManagementServer
    {

        /// <summary>
        /// A delegate method to allow for the Pipe threads to update the skeleton 
        /// data of the players
        /// </summary>
        /// <param name="e">The update args with the KinectId associated with the calling thread and a List of Skeletons to update</param>
        public delegate void UpdateSkeletons(SkeletonUpdateArgs e);

        public delegate void OnCompleted();

        #region Vars

        /// <summary>
        /// A List of all the threads running Kinect PipeServers 
        /// </summary>
        private List<Thread> kinectPipes;

        /// <summary>
        /// A list of all the threads running Gesture Modules
        /// </summary>
        private List<Thread> gestures;

        /// <summary>
        /// A dictionary mapping the KinectId (string) and the SkeletonId (int) to a player
        /// </summary>
        private Dictionary<string, Dictionary<int, Player>> players;

        #endregion

        #region Init

        /// <summary>
        /// Initializes all the components in the correct order (order matters here)
        /// </summary>
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

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public KinectManagementServer()
        {
            kinectPipes = new List<Thread>();
            players = new Dictionary<string, Dictionary<int, Player>>();
        }

        /// <summary>
        /// Runs the KMS
        /// </summary>
        public void Run()
        {
            InitializeComponents();

            while (true)
            {

            }
        }

        #region Pipe Event Handling

        /// <summary>
        /// Receives a List of Skeletons and organizes the data into Players
        /// </summary>
        /// <param name="e">The event arguments holding the KinectId and List of Skeletons</param>
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

        /// <summary>
        /// If no tracked Skeletons are found in the current SkeletonFrame, this method 
        /// will be called to ensure that all Skeletons associated with the Frame's KinectId 
        /// are removed.
        /// </summary>
        /// <param name="e">The event arguments holding the KinectId (no need for Skeleton data)</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void NoSkeletons(SkeletonUpdateArgs e)
        {
            RemovePlayerList(e.KinectId);
        }

        #endregion

        #region Gesture Event Handling

        public void OnCompleted()
        {

        }

        #endregion


        #region Player Management

        /// <summary>
        /// Adds a given player to the Player list according to the Player's KinectId and 
        /// Skeleton TrackingId
        /// </summary>
        /// <param name="p">The Player to add to the List of Players</param>
        private void AddPlayer(Player p)
        {
            players[p.KinectId][p.Skeleton.TrackingId] = p;
        }

        /// <summary>
        /// Removes all players from the list associated with the given KinectId
        /// </summary>
        /// <param name="KID">The KinectId or list to remove</param>
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
