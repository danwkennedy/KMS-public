using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Utils;
using System.Runtime.CompilerServices;

namespace UnityInterface
{
    /// <summary>
    /// Provides a Socket interface to the Unity game.
    /// The thread associated with an instance of this class 
    /// receives a list of GestureEvents from the main thread 
    /// and transmits the event to the running Unity game for 
    /// processing.
    /// </summary>
    public class UnityThread
    {

        #region Vars

        /// <summary>
        /// A delegate method type used by the Scheduler to send 
        /// instructions to its child threads.
        /// </summary>
        /// <param name="e">The list of Players to process</param>
        public delegate void MainCall(UnityModuleArgs e);

        ///// <summary>
        ///// The port to transmit to
        ///// </summary>
        //private readonly Int32 PORT = 5300;

        ///// <summary>
        ///// The IP address to transmit to
        ///// </summary>
        //private readonly String IP = "192.168.2.3";

        /// <summary>
        /// Keeps track of the number of event frames
        /// </summary>
        private int tic = 0;

        /// <summary>
        /// The reference to the TCPClient socket
        /// </summary>
        TcpClient clientSock;

        /// <summary>
        /// Writes to the network stream
        /// </summary>
        StreamWriter writer;

        /// <summary>
        /// Reads from the network stream
        /// </summary>
        StreamReader reader;

        #endregion

        #region Init

        /// <summary>
        /// Initializes the client TCP socket as well as 
        /// the StreamReader and StreamWriter.
        /// </summary>
        private void Initialize()
        {
            try
            {
                Console.WriteLine("[TCP] Attempting to connect to: {0}:{1}", Properties.Settings.Default.UnityIP, Properties.Settings.Default.UnityPORT);
                clientSock = new TcpClient(Properties.Settings.Default.UnityIP, Properties.Settings.Default.UnityPORT);
                Console.WriteLine("[TCP] Starting socket stream");
                NetworkStream netStream = clientSock.GetStream();
                writer = new StreamWriter(netStream);
                reader = new StreamReader(netStream);
                Console.WriteLine("[TCP] Stream opened");
            }
            catch (Exception e)
            {
                Console.WriteLine("[TCP] Exception:\n    {0}\nTrace: {1}", e.Message, e.StackTrace);
                while (true) ;
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

        /// <summary>
        /// Keeps the thread alive by locking it in a while
        /// </summary>
        private void Wait()
        {
            while (clientSock.Connected) ;
        }

        #endregion

        #region Socket Functions

        /// <summary>
        /// Writes a string to the socket
        /// </summary>
        /// <param name="output">The string to output to the socket</param>
        public void writeSocket(string output)
        {
            if (clientSock.Connected)
            {
                writer.WriteLine(output);
                writer.Flush();
            }
        }

        /// <summary>
        /// Reads an input string from the socket
        /// </summary>
        /// <returns></returns>
        public String readSocket()
        {
            if (clientSock.Connected && !reader.EndOfStream)
            {
                return reader.ReadLine();
            }
            else
            {
                return "";
            }
            
        }

        /// <summary>
        /// Closes the socket
        /// </summary>
        public void closeSocket()
        {
            if (clientSock.Connected)
            {
                writer.Close();
                reader.Close();
            }
        }

        #endregion

        #region Scheduler Event Methods

        /// <summary>
        /// A delegate method that allows the Scheduler to assign a 
        /// job to this gesture thread. Formats event in JSON
        /// </summary>
        /// <param name="e">A wrapper class containing the Player data to check for poses</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendToUnity(UnityModuleArgs events)
        {
            string packet = FormatJSON(events);
            tic++;

            //Console.WriteLine("[Client] Sending Data: " + packet);
            writeSocket(packet);
            //Console.WriteLine("[Client] Written to socket");
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
