using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModule
{
    class PoseHandRight : PoseList
    {
        private GestureModule gestureModule;

        public PoseHandRight(GestureModule gestureModule)
        {
            // TODO: Complete member initialization
            this.gestureModule = gestureModule;
        }
        public void checkPose(Player p1)
        {

            //HandRight
            if (p1.Skeleton.Joints[JointType.HandRight].Position.X - p1.Skeleton.Joints[JointType.Spine].Position.X > .4f) gestureModule.addEvent("Player" + p1.PlayerId + " from Kinect" + p1.KinectId + "has hand right");
 
        }
    }
}

