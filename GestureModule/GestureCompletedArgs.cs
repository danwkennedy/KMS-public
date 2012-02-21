﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace GestureModule
{
    public class GestureCompletedArgs
    {
        private List<String> events;

        public GestureCompletedArgs()
        {
            this.events = new List<String>();
           
        }

        #region Getters/Setters

        public List<String> Events
        {
            get
            {
                return this.events;
            }
        }

        #endregion
    }
}