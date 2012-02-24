using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Utils;

namespace GestureModule
{
    public interface PoseList
    {



        GestureEvent checkPose(Player p1);

    }
}
