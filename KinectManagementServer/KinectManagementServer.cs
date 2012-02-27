using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Threading;
using System.Runtime.Remoting.Contexts;
using System.Runtime.CompilerServices;
using KinectManagementServer;
using Utils;
using UnityInterface;

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

        /// <summary>
        /// A delegate method to allow for the Gesture Module to update the events list
        /// </summary>
        /// <param name="e">The update args with the list of new GestureEvents to be sent to the Unity Interface</param>
        public delegate void OnCompleted(GestureCompletedArgs e);

        #region Vars

        /// <summary>
        /// A List of all the threads running Kinect PipeServers 
        /// </summary>
        private List<Thread> pipeThreads;
        private List<PipeServer> pipes;

        /// <summary>
        /// A list of all the threads running Gesture Modules
        /// </summary>
        private List<Thread> gestureThreads;
        //private List<GestureThread> gestures;
        private Dictionary<string, GestureThread.MainCall> gestureModules;
        //TODO: finish => private List<> gestures;

        /// <summary>
        /// A dictionary mapping the KinectId (string) and the SkeletonId (int) to a player
        /// </summary>
        private Dictionary<string, Dictionary<int, Player>> players;

        private int gestureCount = 0;
        private List<GestureEvent> gestureEvents;

        private UnityThread unity;
        private UnityThread.MainCall unityInterface;

        private Thread unityThread;

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
                        PipeServer server = new PipeServer("D:\\git\\2KinectTechDemo\\KinectClient\\bin\\Release\\KinectClient.exe", sensor.UniqueKinectId, SkeletonUpdate, NoSkeletons);
                        pipes.Add(server);
                        Thread kinectThread = new Thread(new ThreadStart(server.ThreadProc));
                        kinectThread.IsBackground = true;
                        pipeThreads.Add(kinectThread);
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
            foreach (PipeServer server in pipes)
            {
                // create a new Gesture object: must be associated with a kinectId, a set of Players and a callback method
                // add it to gestures
                // create a thread with a starting method in the Gesture object
                // start the thread
                GestureThread gesture = new GestureThread(this.OnGestureCompleted, server.KinectId);
                //gestures.Add(gesture);
                gestureModules.Add(server.KinectId, gesture.Worker);
                Thread gestureThread = new Thread(new ThreadStart(gesture.ThreadProc));
                gestureThread.Start();
            }
        }

        /// <summary>
        /// TODO: comment here
        /// </summary>
        private void InitSocketServer()
        {
            unity = new UnityThread();
            unityInterface = unity.Worker;
            unityThread = new Thread(new ThreadStart(unity.ThreadProc));
            unityThread.Start();

        }

        #endregion

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public KinectManagementServer()
        {
            pipeThreads = new List<Thread>();
            pipes = new List<PipeServer>();

            gestureThreads = new List<Thread>();
            //gestures = new List<GestureThread>();
            gestureModules = new Dictionary<string, GestureThread.MainCall>();

            players = new Dictionary<string, Dictionary<int, Player>>();
            gestureEvents = new List<GestureEvent>();
        }

        /// <summary>
        /// Runs the KMS
        /// </summary>
        public void Run()
        {
            InitializeComponents();

            while (true) ;
        }

        #region Pipe Event Handling

        /// <summary>
        /// Receives a List of Skeletons and organizes the data into Players
        /// </summary>
        /// <param name="e">The event arguments holding the KinectId and List of Skeletons</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SkeletonUpdate(SkeletonUpdateArgs e)
        {
#if (PIPE_DEBUG || DEBUG_ALL)
            Console.WriteLine("[Main] Received skeletons from {0}", e.KinectId);
#endif

            foreach (Skeleton skeleton in e.Skeletons)
            {
                try
                {
                    Player p = players[e.KinectId][skeleton.TrackingId];
                    p.Skeleton = skeleton;
                }
                catch (KeyNotFoundException e1)
                {
                    if (players.Count < (pipeThreads.Count * 2))
                    {
                        AddPlayer(new Player(players.Count, e.KinectId, skeleton));
#if(PLAYER_DEBUG)
                        Console.WriteLine("Adding new player");
#endif
                    }
                }
            }

            // Call to gesture module
            if (players[e.KinectId].Values.Count > 0) {
                GestureModuleArgs args = new GestureModuleArgs(players[e.KinectId].Values.ToList<Player>());
                gestureModules[e.KinectId].BeginInvoke(args, null, null);
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void OnGestureCompleted(GestureCompletedArgs e)
        {
            
            gestureEvents.AddRange(e.Events);
            gestureCount++;

            if (gestureCount >= pipeThreads.Count && gestureEvents.Count > 0)
            {

//#if (DEBUG)
//                foreach (GestureEvent s in gestureEvents)
//                {
//                    Console.WriteLine("[Event] " + s.Type);
//                }
//#endif
                //Console.WriteLine(unityThread.ThreadState);
                unityInterface.Invoke(new UnityModuleArgs(gestureEvents));

                gestureEvents.Clear();
                gestureCount = 0;
            }
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
#if (DEBUG)
                    Console.WriteLine("[Client] Removing Player number {0}", players[KID][keys[i]].PlayerId);
#endif
                    players[KID].Remove(keys[i]);
                }
                finally { }
            }
        }

        #endregion
    }
}
