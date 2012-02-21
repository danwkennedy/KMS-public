using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace GestureModule
{
    class PoseHandRight : PoseList
    {
        public void checkPose(Skeleton sk)
        {

            //HandRight
            if (sk.Joints[JointType.HandRight].Position.X - sk.Joints[JointType.Spine].Position.X > .4f) 
        }
    }
}

