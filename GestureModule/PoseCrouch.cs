using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModuleProject
{
    class PoseCrouch: PoseList
    {
        string type = "Crouch";
        static int gestureCode = 7;
        
        public GestureEvent checkPose(Player p1)
        {
            //Crouching Logic
            if (Math.Abs(p1.Skeleton.Joints[JointType.Head].Position.Y - p1.Skeleton.Joints[JointType.KneeLeft].Position.Y) < 0.5f)
            {
                return new GestureEvent(type, p1.PlayerId, gestureCode);
            }
            else
            {
                return new GestureEvent(type, p1.PlayerId, -gestureCode);
            }

            
        }
    }
}
