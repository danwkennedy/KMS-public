﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModuleProject
{
    class PoseLeftHandUp : PoseList
    {

        string type = "HandUp";
        static int gestureCode = 3;
       
        public GestureEvent checkPose(Player p1)
        {
            //LeftHandUp
            if (p1.Skeleton.Joints[JointType.HandLeft].Position.Y - p1.Skeleton.Joints[JointType.Head].Position.Y > 0.1f)
            {
                return new GestureEvent(type, p1.PlayerId, gestureCode);
            }
            else
            {
                return new GestureEvent(type, p1.PlayerId, -gestureCode);

            }
        }
    }
}

