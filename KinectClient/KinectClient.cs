using System;
using Microsoft.Kinect;

namespace KinectClient
{
    /// <summary>
    /// An abstract class for the Kinect client. Creates a framework that all other 
    /// KinectClient types must follow. The framework is the following:
    ///     - The sub-class is responsible for managing the transmission of data
    ///     - The sub-class can do this by implementing the Start() and SendSkeletonData() methods
    ///     - Start() initializes the transmission stream and calls KinectInit()
    ///     - SendSkeletonData() receives the skeleton data, formats it (if needed) and transmits it
    ///         to the server
    /// </summary>
    abstract class KinectClient
    {

        #region Vars

        protected KinectSensor sensor;
        protected Skeleton[] skeletonData;
        protected string kinectId;

        /// <summary>
        /// 
        /// </summary>
        protected TransformSmoothParameters smoothingParameters = new TransformSmoothParameters {
            Smoothing = 0.01f,
            Correction = 0.0f,
            Prediction = 0.0f,
            JitterRadius = 1.0f,
            MaxDeviationRadius = 0.25f
        };

        #endregion Vars

        #region Init

        /// <summary>
        /// Initializes and starts the Kinect sensor
        /// </summary>
        public void initKinect()
        {
            /**
            * Setting up the Kinect
            **/
            if (KinectSensor.KinectSensors.Count > 0)
            {
                KinectSensorCollection sensors = KinectSensor.KinectSensors;
                Console.WriteLine(kinectId);
                int sensorIndex = GetSensorIndexFromID(kinectId);


                sensor = KinectSensor.KinectSensors[sensorIndex];
                sensor.SkeletonStream.Enable(smoothingParameters);
                sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(KinectSkeletonFrameReady);
                KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectStatusChange);
                sensor.Start();
            }
            else
            {
                Console.WriteLine("No Kinect sensors found");
                while (true) ;
            }

        }


        /// <summary>
        /// Checks the inputted KinectID and returns its index in the KinectSensorCollection
        /// </summary>
        /// <param name="uniqueId">The unique Kinect Id inputed by the parent process</param>
        /// <returns>The index into the KinectSensorCollection associated with the unique Id</returns>
        static int GetSensorIndexFromID(string uniqueId)
        {
            for (int i = 0; i < KinectSensor.KinectSensors.Count; i++)
            {
                // Check to see if the Kinect is connected to the system
                // Check to see if the Kinect is not already running
                // Check to see if the UniqueId matches
                // If all are true return
                if (KinectSensor.KinectSensors[i].Status == KinectStatus.Connected
                    && !KinectSensor.KinectSensors[i].IsRunning
                    && KinectSensor.KinectSensors[i].UniqueKinectId.Equals(uniqueId))
                {
                    return i;
                }
            }

            throw new Exception("Invalid unique ID");
        }

        #endregion

        #region KinectCallbacks

        /// <summary>
        /// The event handler for the SkeletonFrameReady event.
        /// Grabs the skeleton data, serializes it and sends it over the pipe to the server
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Contains the skeleton frame data</param>
        protected void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Creates an array to hold the skeleton data the first time the data is received
            if (skeletonData == null)
            {
                skeletonData = new Skeleton[e.OpenSkeletonFrame().SkeletonArrayLength];
            }

            // Open the frame, copy the data and close the frame
            SkeletonFrame frame = e.OpenSkeletonFrame();
            frame.CopySkeletonDataTo(skeletonData);
            frame.Dispose();

            // Calls this sub-class method that will change according to the streaming method
            this.SendSkeletonData();
        }

        /// <summary>
        /// A Debug function that gets called whenever the Kinect's state change. Since a state change 
        /// is usually a bad thing, this event handler will mainly print out diagnostic data.
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Contains data on the sensor</param>
        protected void KinectStatusChange(object sender, StatusChangedEventArgs e)
        {
#if (DEBUG)
            Console.WriteLine("[Client] Kinect's status changed!");
            Console.WriteLine("[Client] Kinect is running: {0}", e.Sensor.IsRunning);
            Console.WriteLine("[Client] Kinect Skeletal stream is enabled: {0}", e.Sensor.SkeletonStream.IsEnabled);
            Console.WriteLine("[Client] Kinect status: {0}", e.Sensor.Status);
            Console.WriteLine("[Client] Kinect device connection ID: {0}", e.Sensor.DeviceConnectionId);
#else
            throw new Exception("[Client] Kinect's status has changed to " + e.Sensor.Status);
#endif
        }

        #endregion

        #region Abstracts

        /// <summary>
        /// Sends the received skeleton data over a stream to the main hub.
        /// The method is abstracted to a subclass to allow for different types of streaming (sockets, pipes,...)
        /// </summary>
        protected abstract void SendSkeletonData();

        /// <summary>
        /// Initializes the streams and starts the client
        /// </summary>
        public abstract void Start();

        #endregion
    }
}
