using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 using System.Runtime.InteropServices;

using Microsoft.Kinect;
using System.IO;
using KinectManagementServer;



/*
 * 
 * 
 * make sure the main class has this:
 * public enum Pose { lefthandup, righthandup, handleft, handright, crouch }
 * 
 * 
 * 
 * 
 * 
 */

namespace GestureModule
{
    class GestureModule
    {
        public delegate void MainThreadEvent(Player p1, Player p2);
        private List<PoseList> poses;
        private PoseCrouch crouch;
        private PoseHandLeft handleft;
        private PoseHandRight handright;
        private PoseLeftHandUp lefthandup;
        private PoseRightHandUp righthandup;
        private GestureCompletedArgs gestureCompleteArgs;
        

        public GestureModule()
        {

            this.crouch = new PoseCrouch();
            this.handleft = new PoseHandLeft();
            this.handright = new PoseHandRight();
            this.lefthandup = new PoseLeftHandUp();
            this.righthandup = new PoseRightHandUp();
            this.poses.Add(crouch);
            this.poses.Add(handleft);
            this.poses.Add(handright);
            this.poses.Add(lefthandup);
            this.poses.Add(righthandup);
            this.gestureCompleteArgs = new GestureCompletedArgs();
        }

        
        

        public void DoWork(Player p1, Player p2){
            foreach (PoseList p in poses)
            {
                p.checkPose(p1);
                p.checkPose(p2);
            }

            // invoke here

        }
         

        

    }
}
