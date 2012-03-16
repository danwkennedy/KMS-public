using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Kinect;
using System.IO;
using Utils;


namespace GestureModuleProject
{
    public class GestureModule
    {
        
        private List<PoseList> poses;
        private PoseCrouch crouch;
        private PoseHandLeft handleft;
        private PoseHandRight handright;
        private PoseLeftHandUp lefthandup;
        private PoseRightHandUp righthandup;
        private PoseLeanLeft leanleft;
        private PoseLeanRight leanright;
        private PoseLeanStop leanstop;
        
         
         
        private Boolean[] gestureState;
        private Boolean[] previousGestureState;
        private static int gestureCodeCount = 8;

        //DONOT use 0 because the this number needs to be multiplied by (-1) at some point to indicate status
        public enum gestureCode 
        {   
            
            handleft = 1,
            handright = 2,
            lefthandup = 3,
            righthandup = 4,
            leanleft = 5,
            leanright = 6,
            crouch = 7,
        }

        
       
        public GestureModule()
        {
            poses = new List<PoseList>();
            this.crouch = new PoseCrouch();
            this.handleft = new PoseHandLeft();
            this.handright = new PoseHandRight();
            this.lefthandup = new PoseLeftHandUp();
            this.righthandup = new PoseRightHandUp();
            this.leanleft = new PoseLeanLeft();
            this.leanright = new PoseLeanRight();
            //this.leanstop = new PoseLeanStop();
            this.poses.Add(crouch);
            this.poses.Add(handleft);
            this.poses.Add(handright);
            this.poses.Add(lefthandup);
            this.poses.Add(righthandup);
            this.poses.Add(leanleft);
            this.poses.Add(leanright);
            //this.poses.Add(leanstop);
            gestureState = new Boolean[gestureCodeCount];
            previousGestureState = new Boolean[gestureCodeCount];
            for (int i = 0; i < gestureCodeCount; i++)
            {
                gestureState[i] = false;
                previousGestureState[i] = false;

            }


        }

        public List<GestureEvent> processPlayers(List<Player> playerList)
        {
            List<GestureEvent> gestureEvents = new List<GestureEvent>();
            foreach (PoseList p in poses)
            {
                foreach (Player player in playerList)
                {
                    GestureEvent pose = p.checkPose(player);
                    if (pose != null)
                    {
                        
                        if (pose.Code > 0)
                        {
                            gestureState[pose.Code] = true;
                            
                        }
                        else
                        {
                            gestureState[-pose.Code] = false;
                        }

                        //continue if nothing has changed
                        if (gestureState[Math.Abs(pose.Code)] == previousGestureState[Math.Abs(pose.Code)])
                        {
                            continue;
                        }
                        else
                        {
                            
                            if (pose.Code < 0)
                            {
                                pose.Type = "Stop" + pose.Type;
                            }
                        }

                        gestureEvents.Add(pose);
                        previousGestureState[Math.Abs(pose.Code)] = gestureState[Math.Abs(pose.Code)];
                    }

                }
                
            }

            

            return gestureEvents;

        }
    }
}
