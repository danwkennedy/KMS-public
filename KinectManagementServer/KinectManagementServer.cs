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

        #region Delegates

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

        #endregion

        #region Vars

        /// <summary>
        /// A List of all the threads running Kinect PipeServers 
        /// </summary>
        private List<Thread> pipeThreads;

        /// <summary>
        /// Holds a reference to each pipe server in use
        /// </summary>
        private List<PipeServer> pipes;

        /// <summary>
        /// A list of all the threads running Gesture Modules
        /// </summary>
        private List<Thread> gestureThreads;

        /// <summary>
        /// Maps a gesture thread to a given Kinect device
        /// </summary>
        private Dictionary<string, GestureThread.MainCall> gestureModules;
        //TODO: finish => private List<> gestures;

        /// <summary>
        /// A dictionary mapping the KinectId (string) and the SkeletonId (int) to a player
        /// </summary>
        private Dictionary<string, Dictionary<int, Player>> players;

        /// <summary>
        /// Keeps track of which player number is currently free
        /// </summary>
        private Stack<int> playerIndices = new Stack<int>(new int[] {3, 2, 1, 0});

        /// <summary>
        /// Makes the main thread wait until all gesture threads have returned before calling the UnityInterface
        /// </summary>
        private int gestureCount = 0;

        /// <summary>
        /// Keeps track of all of the gesture events for one frame
        /// </summary>
        private List<GestureEvent> gestureEvents;

        /// <summary>
        /// Holds a reference to the Unity Interface
        /// </summary>
        private UnityThread unity;

        /// <summary>
        /// Holds a reference to the interrupt delegate in the UnityInterface
        /// </summary>
        private UnityThread.MainCall unityInterface;

        /// <summary>
        /// Holds a reference to the thread managing the Unity Interface
        /// </summary>
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
            InitUnityInterface();
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
        /// Initializes a Gesture Thread for each Kinect Thread and 
        /// pairs a Gesture module with a Kinect.
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
        /// Initializes a thread that will handle the 
        /// socket interface with Unity.
        /// </summary>
        private void InitUnityInterface()
        {
            unity = new UnityThread();
            unityInterface = unity.Worker;
            unityThread = new Thread(new ThreadStart(unity.ThreadProc));
            unityThread.Start();
        }

        #endregion

        #region Runner

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
            Wait();
        }

        /// <summary>
        /// Puts the main thread in a waiting state.
        /// The thread will exit this state when one of 
        /// its delegate functions is invoked.
        /// </summary>
        private void Wait()
        {
            while (true) ;
        }

        #endregion

        #region Pipe Event Handling

        /// <summary>
        /// Receives a List of Skeletons and organizes the data into Players
        /// </summary>
        /// <param name="e">The event arguments holding the KinectId and List of Skeletons</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SkeletonUpdate(SkeletonUpdateArgs e)
        {
            // Check to see that the number of skeletons has gone down
            if (e.Skeletons.Count < players[e.KinectId].Values.Count && e.Skeletons.Count == 1)
            {
                // If so, find the missing player and remove it
                Player[] playerArray = players[e.KinectId].Values.ToArray<Player>();
                
                for (int i = 0; i < playerArray.Length; i++ )
                {
                    if (playerArray[i].Skeleton.TrackingId != e.Skeletons[0].TrackingId)
                    {
                        RemovePlayer(playerArray[i]);
                    }
                }
            }

            // Iterate through the skeletons
            foreach (Skeleton skeleton in e.Skeletons)
            {
                // Try to update the skeleton associated with the new skeleton's tracking id
                try
                {
                    Player p = players[e.KinectId][skeleton.TrackingId];
                    p.Skeleton = skeleton;
                }
                catch (KeyNotFoundException e1)
                {
                    // If no skeleton is found and there is room, add a new player
                    if (players.Count < (pipeThreads.Count * 2))
                    {
                        //AddPlayer(new Player(playerIndex, e.KinectId, skeleton));
                        AddPlayer(skeleton, e.KinectId);
                    }
                }
            }

            // Call to gesture module
            if (players[e.KinectId].Values.Count > 0)
            {
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
        private void AddPlayer(Skeleton skeleton, string KID)
        {
            Player p = new Player(playerIndices.Pop(), KID, skeleton);
            players[p.KinectId][p.Skeleton.TrackingId] = p;
            gestureEvents.Add(new GestureEvent("addPlayer", p.PlayerId));

            #if(DEBUG)
                        Console.WriteLine("Adding Player{0}", p.PlayerId);
            #endif
        }

        /// <summary>
        /// Remve a given Player from the Player list.
        /// </summary>
        /// <param name="p">The player to remove</param>
        private void RemovePlayer(Player p)
        {
            players[p.KinectId].Remove(p.Skeleton.TrackingId);
            playerIndices.Push(p.PlayerId);
            gestureEvents.Add(new GestureEvent("removePlayer", p.PlayerId));

            #if(DEBUG)
                        Console.WriteLine("Removing player{0}", p.PlayerId);
            #endif
        }

        /// <summary>
        /// Removes all players from the list associated with the given KinectId
        /// </summary>
        /// <param name="KID">The KinectId or list to remove</param>
        private void RemovePlayerList(string KID)
        {
            // Get all the keys to remove
            int[] keys = players[KID].Keys.ToArray<int>();

            // Iterate and remove each key
            for (int i = 0; i < keys.Length; i++)
            {

#if (DEBUG)
                Console.WriteLine("[Client] Removing Player{0}", players[KID][keys[i]].PlayerId);
#endif

                playerIndices.Push(players[KID][keys[i]].PlayerId);
                gestureEvents.Add(new GestureEvent("removePlayer", players[KID][keys[i]].PlayerId));

                players[KID].Remove(keys[i]);
            }
        }

        #endregion

    }
}
