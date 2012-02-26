using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Utils;

namespace UnityInterface
{
    /**
     * Shows how one may send strings to the unity server.
     */
    public class UnityThread
    {

        #region Vars

        /// <summary>
        /// A delegate method type used by the Scheduler to send 
        /// instructions to its child threads.
        /// </summary>
        /// <param name="e">The list of Players to process</param>
        public delegate void MainCall(UnityModuleArgs e);

        public readonly Int32 PORT = 5300;
        public readonly String IP = "localhost";
        public readonly int SEND_TIMEOUT = 1000;
        public int MAX_SEND = 1000000;
        public int tic = 0; // timestamp of events, used to queue events in unity EventManager

        internal Boolean socketReady = false;

        TcpClient clientSock;
        NetworkStream netStream;
        StreamWriter writer;
        StreamReader reader;

        #endregion

        #region Init

        private void Initialize()
        {
            try
            {
                clientSock = new TcpClient(IP, PORT);
                // get the stream for reading/writing.
                Console.WriteLine("[Client] Starting socket stream");
                netStream = clientSock.GetStream();
                writer = new StreamWriter(netStream);
                reader = new StreamReader(netStream);
                socketReady = true;
                Console.WriteLine("[Client] Stream opened");
            }
            catch (Exception e)
            {
                Console.WriteLine("[Client] Exception:\n    {0}\nTrace: {1}", e.Message, e.StackTrace);
            }
        }

        #endregion

        #region Runner

        /// <summary>
        /// The main entry point for the spawned thread. 
        /// If this method is allowed to complete, the thread will be disposed and all 
        /// memory lost.
        /// </summary>
        public void ThreadProc()
        {
            Initialize();
            Wait();
        }

        private void Wait()
        {
            while (true) ;
        }

        #endregion

        #region Socket Functions

        public void writeSocket(string output)
        {
            if (!socketReady)
            {
                return;
            }
            writer.Write(output);
            writer.Flush();
        }

        public String readSocket()
        {
            if (!socketReady)
            {
                return "";
            }
            if (netStream.DataAvailable)
            {
                return reader.ReadLine();
            }
            return "";
        }

        public void closeSocket()
        {
            if (!socketReady)
            {
                return;
            }
            writer.Close();
            reader.Close();
            netStream.Close();
            socketReady = false;
        }

        #endregion

        #region Scheduler Event Methods

        /// <summary>
        /// A delegate method that allows the Scheduler to assign a 
        /// job to this gesture thread. Formats event in JSON
        /// </summary>
        /// <param name="e">A wrapper class containing the Player data to check for poses</param>
        public void SendToUnity(UnityModuleArgs events)
        {
            string packet = FormatJSON(events);
            tic++;

            Console.WriteLine("[Client] Sending Data: " + packet);
            writeSocket(packet);
        }

        /// <summary>
        /// Takes a list of GestureEvents and concatenates them into a JSON String
        /// </summary>
        /// <param name="e">UnityModuleArgs list of GestureEvents</param>
        /// <returns>JSON string</returns>
        public string FormatJSON(UnityModuleArgs e)
        {
            int i = 0; // number of event
            string json = "{";

            foreach (GestureEvent gEvent in e.Events)
            {
                gEvent.Timestamp = tic;
                if (i > 0) // if there's more than one event in packet
                {
                    json += ",";
                }
                json += "\"e" + i + "\":";   // {"e1":
                json += gEvent.ToString();
                i++;

            }
            json += "}";

            return json;
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
                return SendToUnity;
            }
        }

        #endregion
    }
}
