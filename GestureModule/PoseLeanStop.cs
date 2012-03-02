using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModule
{
    class PoseLeanStop : PoseList
    {
        string type = "stopLean";

        public GestureEvent checkPose(Player p1)
        {

            //LeanRight
            if (p1.Skeleton.Joints[JointType.ShoulderCenter].Position.X - p1.Skeleton.Joints[JointType.HipCenter].Position.X > -0.1f && p1.Skeleton.Joints[JointType.ShoulderCenter].Position.X - p1.Skeleton.Joints[JointType.HipCenter].Position.X < 0.1f)
            {
                return new GestureEvent(type, p1.PlayerId);
            }

            return null;
        }
    }
}
