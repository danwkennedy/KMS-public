﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    [Serializable()] 
    public class GestureEvent
    {

        /// <summary>
        /// Empty constructor
        /// </summary>
        /// <param name="type">The type of event fired</param>
        /// <param name="player">The number of the player who fired the event</param>
        public GestureEvent(string type, int player)
        {
            Type = type;
            Player = player;
          
        }

        /// <summary>
        /// Second constructor that takes code as one of the params
        /// </summary>
        /// <param name="type">The type of event fired</param>
        /// <param name="player">The number of the player</param>
        /// <param name="code">The code for the event</param>
        public GestureEvent(string type, int player, int code)
        {
            Type = type;
            Player = player;
            Code = code;
        }
        #region Properties

        /// <summary>
        /// The event name
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// The number of the player who fired the event
        /// </summary>
        public int Player
        {
            get;
            set;
        }

        /// <summary>
        /// The frame number when the event was fired
        /// </summary>
        public int Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// The code that represent the gesture
        /// </summary>
        public int Code
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Formats a GestureEvent into Json
        /// Format example: {"type":"[type:string]","player":"[player number: int]","timestamp":"[]"}
        /// </summary>
        /// <returns>JSON String</returns>
        override public string ToString()
        {
            return "{\"type\":\"" + Type + "\",\"player\":\"" + Player + "\",\"timestamp\":\"" + Timestamp + "\"}";
        }
    }
}