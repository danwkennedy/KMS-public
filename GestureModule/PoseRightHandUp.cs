using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace GestureModule
{
    class PoseRightHandUp : PoseList
    {
        public void checkPose(Skeleton sk)
        {
            //RightHandUp
            if (sk.Joints[JointType.HandRight].Position.Y - sk.Joints[JointType.Head].Position.Y > 0.1f) 
        }
    }
}

