using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using GestureModule;
using System.Runtime.CompilerServices;
using KinectManagementServer;


namespace KinectManagementServer
{

    /// <summary>
    /// Responsible for managing a Gesture Module
    /// </summary>
    class GestureThread
    {

        #region Vars

        /// <summary>
        /// A delegate method type used by the Scheduler to send 
        /// instructions to its child threads.
        /// </summary>
        /// <param name="e">The list of Players to process</param>
        public delegate void MainCall(GestureModuleArgs e);

        /// <summary>
        /// An instance of the GestureModule
        /// </summary>
        private GestureModule.GestureModuleClass gestureModule;

        /// <summary>
        /// A callback method to be called after processing Player data
        /// </summary>
        private KinectManagementServer.OnCompleted completed;

        /// <summary>
        /// The unique Id of the Kinect sensor associated with this thread
        /// </summary>
        private string kinectId;

        #endregion

        #region Init

        /// <summary>
        /// Creates an instance of GestureThread. Each instance requires a referrence to an 
        /// OnCompleted method from the Scheduler as well as a KinectId.
        /// </summary>
        /// <param name="threadEvent">A reference to an OnCompleted method in the Scheduler</param>
        /// <param name="_kinectId">The unique Id for the Kinect sensor associated with this thread</param>
        public GestureThread(KinectManagementServer.OnCompleted threadEvent, string _kinectId)
        {
            completed = threadEvent;
            kinectId = _kinectId;
        }

        /// <summary>
        /// Initializes the Gesture Module
        /// </summary>
        private void InitializeGestureModule()
        {
            gestureModule = new GestureModule.GestureModuleClass();
        }

        #endregion

        #region Runner

        /// <summary>
        /// The main entry point for the thread, initializes the 
        /// Gesture Module and waits for instructions from the 
        /// Scheduler
        /// </summary>
        public void ThreadProc()
        {
            InitializeGestureModule();
            wait();
        }

        /// <summary>
        /// Once the thread has initialized, it will fall into a wait 
        /// state in order to wait for instructions from the Scheduler.
        /// </summary>
        private void wait()
        {
            while(true);
        }

        #endregion

        #region Scheduler Event Methods

        /// <summary>
        /// A delegate method that allows the Scheduler to assign a 
        /// job to this gesture thread.
        /// </summary>
        /// <param name="e">A wrapper class containing the Player data to check for poses</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DoWork(GestureModuleArgs e)
        {
            // Run the gesture module on the given Players
            List<GestureEvent> events = this.gestureModule.processPlayers(e.Players);
            //List<GestureEvent> events = new List<GestureEvent>();
            //events.Add(new GestureEvent("Testing"));

            GestureCompletedArgs args = new GestureCompletedArgs(events);

            //invoke main thread to send back gesture info
            completed.BeginInvoke(args, null, null);
        }

        #endregion

        #region Getters / Setters

        /// <summary>
        /// The Worker method called by the Scheduler
        /// </summary>
        public MainCall Worker
        {
            get
            {
                return DoWork;
            }
        }

        #endregion

    }
}
