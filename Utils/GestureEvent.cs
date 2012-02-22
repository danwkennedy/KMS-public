﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public class GestureEvent
    {
        private string gesture;

        public GestureEvent(string _gesture)
        {
            gesture = _gesture;
        }


        #region Getters / Setters

        public string Gesture
        {
            get
            {
                return this.gesture;
            }
        }

        #endregion

    }
}
