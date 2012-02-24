using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModule
{
    class PoseHandLeft : PoseList
    {
        
        public GestureEvent checkPose(Player p1)
        {
            //HandLeft
            if (p1.Skeleton.Joints[JointType.HandLeft].Position.X - p1.Skeleton.Joints[JointType.Spine].Position.X < -0.4f) return new GestureEvent("Player" + p1.PlayerId + " from Kinect" + p1.KinectId + "has hand left");
            return null;
        }
    }
}

