using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 using System.Runtime.InteropServices;

using Microsoft.Kinect;
using System.IO;
using Utils;

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
    public class GestureModuleClass
    {
        
        private List<PoseList> poses;
        private PoseCrouch crouch;
        private PoseHandLeft handleft;
        private PoseHandRight handright;
        private PoseLeftHandUp lefthandup;
        private PoseRightHandUp righthandup;
        private GestureCompletedArgs gestureCompleteArgs;
        

        public GestureModuleClass()
        {
            poses = new List<PoseList>();
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
            //this.gestureCompleteArgs = new GestureCompletedArgs();
        }


        public void addEvent(String s)
        {
          //  this.gestureCompleteArgs.Events.Add(new GestureEvent(s));

        }
        


        public void processPlayers(List<Player> playerList){
            foreach (PoseList p in poses)
            {
                foreach (Player player in playerList)
                {
                    p.checkPose(player);

                }
            }

        

        }


         

        

    }
}
