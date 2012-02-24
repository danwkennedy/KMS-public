using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Threading;
using LitJson;

namespace SocketServer
{
    public class ServerMain
    {

        public readonly Int32 PORT = 5300;
        TcpClient serverSock;

        internal Boolean socketReady = false;
        NetworkStream netStream;
        StreamWriter outStream;
        StreamReader inStream;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //Converts a JSON string into event objects and shows text on the screen
        private Event ProcessJSON(string jsonString)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonString);

            Event e = new Event();
            e.type = jsonData["event"]["type"].ToString();
            e.player = Convert.ToInt16(jsonData["event"]["player"].ToString());
            e.timestamp = Convert.ToInt16(jsonData["event"]["timestamp"].ToString());
            e.x = Convert.ToInt16(jsonData["event"]["x"].ToString());
            e.y = Convert.ToInt16(jsonData["event"]["y"].ToString());

            return e;
        }

        public void runServerThread()
        {
            //Create a socket to list from
            //IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPAddress ipAddress = IPAddress.Any;
            TcpListener listener = new TcpListener(ipAddress, 5300);
            listener.Start();

            // Start listening...
            Console.WriteLine("[Server] Listening for Client Socket");

            //Socket clientSocket = serverSocket.Accept();
            TcpClient client = listener.AcceptTcpClient();
            // give the client 5 seconds to send an event or close the connection.
            client.ReceiveTimeout = 5000;

            Console.WriteLine("[Server] Client Socket Accepted");
            socketReady = true;
            netStream = client.GetStream();
            outStream = new StreamWriter(netStream);
            inStream = new StreamReader(netStream);

            Event e;

            while (client.Connected)
            {
                // Deserialize events.
                string input = (string)readSocket();
                if (input.Length > 0)
                    Debug.Log("Event Received: " + input + "\n");
                //e = ProcessJSON(input);
                //Debug.Log("Parsed JSON: " + e.ToString() + "\n");
                Thread.Sleep(10);
            }
            closeSocket();
        }

        public void writeSocket(string theLine)
        {
            if (!socketReady)
                return;
            String foo = theLine + "\r\n";
            outStream.Write(foo);
            outStream.Flush();
        }
        public String readSocket()
        {
            if (!socketReady)
                return "";
            if (netStream.DataAvailable)
            {
                return inStream.ReadLine();
            }
            return "";
        }
        public void closeSocket()
        {
            if (!socketReady)
                return;
            outStream.Close();
            inStream.Close();
            netStream.Close();
            socketReady = false;
        }
    }
}