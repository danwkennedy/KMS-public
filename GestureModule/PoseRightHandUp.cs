﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModuleProject
{
    class PoseRightHandUp : PoseList
    {

        string type = "handUp";

        public GestureEvent checkPose(Player p1)
        {
            //RightHandUp
            if (p1.Skeleton.Joints[JointType.HandRight].Position.Y - p1.Skeleton.Joints[JointType.Head].Position.Y > 0.1f)
            {
                return new GestureEvent(type, p1.PlayerId);
            }

            return null;
        }
    }
}

