using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModuleProject
{
    class PoseLeanStop : PoseList
    {
        string type = "stopLean";
        static int code = 12;

        public GestureEvent checkPose(Player p1)
        {

            //LeanRight
            if (p1.Skeleton.Joints[JointType.ShoulderCenter].Position.X - p1.Skeleton.Joints[JointType.HipCenter].Position.X > -0.1f && p1.Skeleton.Joints[JointType.ShoulderCenter].Position.X - p1.Skeleton.Joints[JointType.HipCenter].Position.X < 0.1f)
            {
                return new GestureEvent(type, p1.PlayerId, code);
            }

            return null;
        }
    }
}
