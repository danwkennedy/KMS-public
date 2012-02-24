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
        
        public GestureEvent checkPose(Player p1)
        {
            //Crouching Logic
            if (p1.Skeleton.Joints[JointType.Head].Position.Y < 0.5f && p1.Skeleton.Joints[JointType.Head].Position.Y > 0.2f) return new GestureEvent("Player" + p1.PlayerId + " from Kinect" + p1.KinectId + "crouched");
            return null;
        }
    }
}
