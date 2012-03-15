using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Utils
{
    public class Player : IEquatable<Player>
    {

        /// <summary>
        /// A holder class for a Player.
        /// Stores the Player's associated KinectId and Skeleton as well as the Player number.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="kinectId"></param>
        /// <param name="skeleton"></param>
        public Player(int playerId, string kinectId, Skeleton skeleton)
        {
            PlayerId = playerId;
            KinectId = kinectId;
            Skeleton = skeleton;
        }

        #region Properties

        /// <summary>
        /// The unique Identifier for the Kinect the Player is using.
        /// </summary>
        public String KinectId
        {
            get;
            set;
        }

        /// <summary>
        /// The unique Identifier for the Player
        /// </summary>
        public int PlayerId
        {
            get;
            private set;
        }

        /// <summary>
        /// The skeleton representation of the Player
        /// </summary>
        public Skeleton Skeleton
        {
            get;
            set;
        }

        #endregion

        #region IEquatable<Player> Members

        public bool Equals(Player other)
        {
            // Two players can be identified as the same if their KinectId and Skeleton TrackingId are the same
            return KinectId.Equals(other.KinectId) && Skeleton.TrackingId == other.Skeleton.TrackingId;
        }

        #endregion
    }
}
