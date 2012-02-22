using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using Microsoft.Kinect;
using System.Runtime.Serialization.Formatters.Binary;

namespace KinectClient
{
    /// <summary>
    /// Extends the KinectClient class.
    /// Initializes a Kinect sensor and streams the skeleton data 
    /// to the server via an Annonymous Pipe.
    /// </summary>
    class KinectClientPipe : KinectClient
    {
        string serverHandle;
        AnonymousPipeClientStream clientStream;

        /// <summary>
        /// Creates an instance of KinectClientPipe.
        /// Receives the pipe handler and KinectId
        /// </summary>
        /// <param name="_handle">The pipe handler. Used to connect to the PipeServer</param>
        /// <param name="_id">The Kinect Id of the Kinect we wish to stream from</param>
        public KinectClientPipe(string _handle, string _id)
        {
            this.serverHandle = _handle;
            this.kinectId = _id;
        }

        /// <summary>
        /// Starts the Kinect and waits for an end of stream.
        /// </summary>
        public override void Start()
        {
            try
            {
#if (DEBUG)
                Console.WriteLine("[Client] Starting pipe stream");
#endif
                clientStream = new AnonymousPipeClientStream(PipeDirection.Out, serverHandle);

#if (DEBUG)
                Console.WriteLine("[Client] Stream opened");
#endif
                sensor = null;

#if (DEBUG)
                Console.WriteLine("[Client] Found {0} Kinects", KinectSensor.KinectSensors.Count);
#endif

                // Start the Kinect
                initKinect();

                // Wait until the connection is severed
                while (clientStream.IsConnected);
            }
            catch (Exception e)
            {
#if (DEBUG)
                Console.WriteLine("[Client] Exception:\n    {0}\nTrace: {1}", e.Message, e.StackTrace);
#else
                throw e;
#endif
            }
        }

        /// <summary>
        /// Receives the skeleton data from the Kinect and transmits it over the Pipe
        /// </summary>
        protected override void SendSkeletonData()
        {
            // If the previous frame is still being read, wait for it to finish
            clientStream.WaitForPipeDrain();

            // Binary serialize and write the skeleton data over the pipe
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(clientStream, skeletonData);
        }
    }
}
