﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModule
{
    class PoseHandRight : PoseList
    {
        string type = "HandRight";

        public GestureEvent checkPose(Player p1)
        {

            //HandRight
            if (p1.Skeleton.Joints[JointType.HandRight].Position.X - p1.Skeleton.Joints[JointType.Spine].Position.X > .4f)
            {
                return new GestureEvent(type, p1.PlayerId);
            }

            return null;
        }
    }
}

