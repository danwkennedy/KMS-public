using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Utils
{
    public struct GestureCompletedArgs
    {
        private List<GestureEvent> events;

        public GestureCompletedArgs(List<GestureEvent> _events)
        {
            events = _events;
        }

        #region Properties

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