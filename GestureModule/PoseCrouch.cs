using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModule
{
    class PoseCrouch: PoseList
    {
        private GestureModule gm;
        public PoseCrouch(GestureModule gm)
        {
            this.gm = gm;

        }
        public void checkPose(Player p1)
        {
            //Crouching Logic
            if (p1.Skeleton.Joints[JointType.Head].Position.Y < 0.5f && p1.Skeleton.Joints[JointType.Head].Position.Y > 0.2f) gm.addEvent("Player" + p1.PlayerId + " from Kinect" + p1.KinectId + "crouched");
        }
    }
}
