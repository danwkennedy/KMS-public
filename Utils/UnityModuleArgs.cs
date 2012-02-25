using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public struct UnityModuleArgs
    {
        private List<GestureEvent> events;

        public UnityModuleArgs(List<GestureEvent> _events)
        {
            events = _events;
        }

        #region Getters / Setters

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
