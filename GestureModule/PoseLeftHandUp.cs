using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModule
{
    class PoseLeftHandUp : PoseList
    {
        private GestureModule gestureModule;

        public PoseLeftHandUp(GestureModule gestureModule)
        {
            // TODO: Complete member initialization
            this.gestureModule = gestureModule;
        }
        public void checkPose(Player p1)
        {
            //LeftHandUp
            if (p1.Skeleton.Joints[JointType.HandLeft].Position.Y - p1.Skeleton.Joints[JointType.Head].Position.Y > 0.1f) gestureModule.addEvent("Player" + p1.PlayerId + " from Kinect" + p1.KinectId + "has left hand up");
        }
    }
}

