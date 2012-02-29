using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModule
{
    class PoseRightHandUp : PoseList
    {

        string type = "handup"; // temporarily hacked to make demo work , should be RightHandUp

        public GestureEvent checkPose(Player p1)
        {
            //RightHandUp
            if (p1.Skeleton.Joints[JointType.HandRight].Position.Y - p1.Skeleton.Joints[JointType.Head].Position.Y > 0.1f)
            {
                return new GestureEvent(type, p1.PlayerId);
            }

            return null;
        }
    }
}

