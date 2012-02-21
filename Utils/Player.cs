using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Utils
{
    public class Player
    {
        private String kinectId;

        private int playerId;
        private Skeleton skeleton;

        public Player(int id, string KID, Skeleton s)
        {
            playerId = id;
            kinectId = KID;
            skeleton = s;
        }

        #region Getters/Setters

        public String KinectId
        {
            get
            {
                return this.kinectId;
            }

            set
            {
                this.kinectId = value;
            }
        }

        public int PlayerId
        {
            get
            {
                return this.playerId;
            }
        }

        public Skeleton Skeleton
        {
            get
            {
                return this.skeleton;
            }

            set
            {
                this.skeleton = value;
            }
        }

        #endregion

    }
}
