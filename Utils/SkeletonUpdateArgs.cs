using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Utils
{
    public struct SkeletonUpdateArgs
    {
        private string kinectId;
        private List<Skeleton> skeletons;

        public SkeletonUpdateArgs(string _kid, List<Skeleton> _collection)
        {
            skeletons = _collection;
            kinectId = _kid;
        }

        #region Getters/Setters

        public string KinectId
        {
            get
            {
                return this.kinectId;
            }
        }

        public List<Skeleton> Skeletons
        {
            get
            {
                return this.skeletons;
            }
        }

        #endregion
    }
}
