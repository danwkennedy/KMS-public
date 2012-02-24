using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 using System.Runtime.InteropServices;

using Microsoft.Kinect;
using System.IO;
using Utils;


namespace GestureModule
{
    public class GestureModule
    {
        
        private List<PoseList> poses;
        private PoseCrouch crouch;
        private PoseHandLeft handleft;
        private PoseHandRight handright;
        private PoseLeftHandUp lefthandup;
        private PoseRightHandUp righthandup;
       
        

        public GestureModule()
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
        }


        
        

        public List<GestureEvent> processPlayers(List<Player> playerList){
            List<GestureEvent> gestureEvents = new List<GestureEvent>();
            foreach (PoseList p in poses)
            {
                foreach (Player player in playerList)
                {
                    GestureEvent pose = p.checkPose(player);
                    if (pose != null)
                    {
                        gestureEvents.Add(pose);
                    }

                }
            }

            return gestureEvents;

        }


         

        

    }
}
