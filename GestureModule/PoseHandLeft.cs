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
        private GestureModule gm;
        public PoseHandLeft(GestureModule gm)
        {
            this.gm = gm;

        }
        public void checkPose(Player p1)
        {
            //HandLeft
            if (p1.Skeleton.Joints[JointType.HandLeft].Position.X - p1.Skeleton.Joints[JointType.Spine].Position.X < -0.4f) gm.addEvent("Player" + p1.PlayerId + " from Kinect" + p1.KinectId + "has hand left");
        }
    }
}

