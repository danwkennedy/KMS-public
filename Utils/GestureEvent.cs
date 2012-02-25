using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public class GestureEvent
    {
        private string type;
        private int player;
        private int timestamp;

        public GestureEvent(string _type, int _player, int _timestamp)
        {
            type = _type;
            player = _player;
            timestamp = _timestamp;
        }


        #region Getters / Setters

        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                this.type = value;
            }
        }

        public int Player
        {
            get
            {
                return player;
            }
            set
            {
                this.player = value;
            }
        }

        public int Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                this.timestamp = value;
            }
        }

        #endregion
        /// <summary>
        /// Formats a GestureEvent in JSON: {"type":"handLeft","player":"1","timestamp":"123"}
        /// </summary>
        /// <returns>JSON String</returns>
        override public string ToString()
        {
            return "{\"type\":\""+type+"\",\"player\":\""+player+"\",\"timestamp\":\""+timestamp+"\"}";
        }
    }
}