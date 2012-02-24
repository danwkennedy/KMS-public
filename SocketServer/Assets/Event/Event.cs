using System;
using System.Collections;
using LitJson;
using UnityEngine;

namespace SocketServer
{
    class Event
    {
        public string type;
        public int player;
        public int timestamp;
        public int x;
        public int y;

        override public string ToString()
        {
            string returnString = "Type: " + type;
            returnString += "\r\nPlayer: " + Convert.ToInt16(player);
            returnString += "\r\nTimestamp: " + Convert.ToInt16(timestamp);
            returnString += "\r\nX: " + Convert.ToInt16(x);
            returnString += "\r\nY: " + Convert.ToInt16(y);

            return returnString;
        }
    }
}