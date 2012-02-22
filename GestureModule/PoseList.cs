using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModule
{
    abstract class PoseList
    {
        private GestureModule gm;

        public PoseList()
        {

        }

        public PoseList(GestureModule gm)
        {

            this.gm = gm;
        }
        public void checkPose(Player p1)
        {
            
        }

    }
}
