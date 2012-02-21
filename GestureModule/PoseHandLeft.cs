using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace GestureModule
{
    class PoseHandLeft : PoseList
    {
        public void checkPose(Skeleton sk)
        {
            //HandLeft
            if (sk.Joints[JointType.HandLeft].Position.X - sk.Joints[JointType.Spine].Position.X < -0.4f) 
        }
    }
}

