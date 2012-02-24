using UnityEngine;
using System;
using System.Collections;
using System.Threading;

namespace SocketServer
{
    public class NetworkGUI : MonoBehaviour
    {
        private GameObject gui;
        private string remoteIP = "127.0.0.1";
        private int remotePort = 25001;
        private int listenPort = 25000;
        private string remoteGUID = "";
        private Boolean useNat = false;

        public void OnNetworkInstantiate(NetworkMessageInfo msg) { }

        public void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (Network.peerType == NetworkPeerType.Disconnected)
            {
                useNat = GUILayout.Toggle(useNat, "Use NAT punchthrough");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);

                GUILayout.BeginVertical();
                if (GUILayout.Button("Connect"))
                {
                    if (useNat)
                    {
                        if (remoteGUID.Length != 0)
                        {
                            Debug.LogWarning("Invalid GUID given, must be a valid one as reported by Network.player.guid or returned in a HostData struture from the master server");
                        }
                        else
                        {
                            Network.Connect(remoteGUID);
                        }
                    }
                    else
                    {
                        Network.Connect(remoteIP, remotePort);
                    }
                }
                if (GUILayout.Button("Start Server"))
                {
                    //Network.InitializeServer(32, listenPort, useNat);
                    ServerMain server = new ServerMain();

                    Thread serverThread = new Thread(new ThreadStart(server.runServerThread));
                    serverThread.Priority = System.Threading.ThreadPriority.AboveNormal;
                    serverThread.Start();
                }
                GUILayout.EndVertical();
                if (useNat)
                {
                    remoteGUID = GUILayout.TextField(remoteGUID, GUILayout.MinWidth(145));
                }
                else
                {
                    remoteIP = GUILayout.TextField(remoteIP, GUILayout.MinWidth(100));
                    remotePort = Convert.ToInt16(GUILayout.TextField(remotePort.ToString()));
                }
            }
            else
            {
                if (useNat)
                {
                    GUILayout.Label("GUID: " + Network.player.guid + " - ");
                }
                GUILayout.Label("Local IP/port: " + Network.player.ipAddress + "/" + Network.player.port);
                GUILayout.Label(" - External IP/port: " + Network.player.externalIP + "/" + Network.player.externalPort);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Disconnect"))
                    Network.Disconnect(200);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}