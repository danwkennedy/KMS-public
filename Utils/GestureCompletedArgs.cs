using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Utils
{
    public class GestureCompletedArgs
    {
        private List<GestureEvent> events;

        public GestureCompletedArgs()
        {
            this.events = new List<GestureEvent>();
           
        }

        #region Getters/Setters

        public List<GestureEvent> Events
        {
            get
            {
                return this.events;
            }
        }

        #endregion
    }
}