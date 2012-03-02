using GestureModule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utils;
using TestFrameworkUtils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Kinect;
using System.Collections.Generic;

namespace KMSTestSuite
{
    
    
    /// <summary>
    ///This is a test class for PoseListTest and is intended
    ///to contain all PoseListTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PoseListTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        internal virtual PoseList CreatePoseHandLeft(GestureModuleClass par)
        {
            // TODO: Instantiate an appropriate concrete class.
            PoseList target = new PoseHandLeft(par) ;
            return target;
        }

        internal virtual PoseList CreatePoseHandRight(GestureModuleClass par)
        {
            // TODO: Instantiate an appropriate concrete class.
            PoseList target = new PoseHandLeft(par);
            return target;
        }

        internal virtual PoseList CreatePoseLeftHandUp(GestureModuleClass par)
        {
            // TODO: Instantiate an appropriate concrete class.
            PoseList target = new PoseHandLeft(par);
            return target;
        }

        internal virtual PoseList CreatePoseRightHandUp(GestureModuleClass par)
        {
            // TODO: Instantiate an appropriate concrete class.
            PoseList target = new PoseHandLeft(par);
            return target;
        }

        internal virtual PoseList CreatePoseCrouch(GestureModuleClass par)
        {
            // TODO: Instantiate an appropriate concrete class.
            PoseList target = new PoseHandLeft(par);
            return target;
        }

        internal virtual GestureModuleClass CreateGestureModule()
        {
            GestureModuleClass target = new GestureModuleClass();
            return target;
        }

        /// <summary>
        ///A test for checkPose
        ///</summary>
        [TestMethod()]
        public void checkPoseTest()
        {
            //import serialized skeleton data here //
            GestureModuleClass parent = CreateGestureModule();

            NewSkeletonCollectionHelper skeletonHelper;
            LinkedList<string> detectedPoses;

            //these are all the various poses.
            PoseList handLeft = CreatePoseHandLeft(parent); // TODO: Initialize to an appropriate value
            PoseList handRight = CreatePoseHandRight(parent);
            PoseList leftHandUp = CreatePoseLeftHandUp(parent);
            PoseList rightHandUp = CreatePoseRightHandUp(parent);
            PoseList crouch = CreatePoseCrouch(parent);
            //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

            string[] files = Directory.GetFiles(@"C:\Users\Evan\KMS\KMSTestSuite\TestPoses");

            foreach (string filepath in files)
            {
                using (FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.None))
                {

                    BinaryFormatter binFormatter = new BinaryFormatter();
                    skeletonHelper = (NewSkeletonCollectionHelper)binFormatter.Deserialize(fs);

                    fs.Flush();
                    fs.Close();
                }

                // Assert.IsNotNull(skeletonHelper);
                detectedPoses = skeletonHelper.ExpectedPoses;


                Player p1 = null; // TODO: Initialize to an appropriate value
                string lastPose = null; // TODO: Initialize to an appropriate value

                p1 = new Player(4, "testplayer", skeletonHelper.Skeletons[0]);

                foreach (Skeleton skal in skeletonHelper.Skeletons)
                {
                    p1.Skeleton = skal;
                    //each pose is its own class, so unfortunately, we have to do each in turn and can't do
                    //anything nifty with generics or a foreach loop
                    //crouch
                    if (crouch.checkPose(p1))
                    {
                        if (lastPose!= null && lastPose.Equals("PoseCrouch"))
                        {
                            continue;
                        }
                        detectedPoses.AddLast("PoseCrouch");
                        lastPose = "PoseCrouch";
                        continue;
                    }

                    //rightHandUp
                    if (rightHandUp.checkPose(p1))
                    {
                        if (lastPose != null && lastPose.Equals("PoseHandRightUp"))
                        {
                            continue;
                        }
                        detectedPoses.AddLast("PoseHandRightUp");
                        lastPose = "PoseHandRightUp";
                        continue;
                    }


                    //leftHandUp
                    if (leftHandUp.checkPose(p1))
                    {
                        if (lastPose != null && lastPose.Equals("PoseHandLeftUp"))
                        {
                            continue;
                        }
                        detectedPoses.AddLast("PoseHandLeftUp");
                        lastPose = "PoseHandLeftUp";
                        continue;
                    }

                    //leftHandOut
                    if (handLeft.checkPose(p1))
                    {
                        if (lastPose != null && lastPose.Equals("PoseHandLeftOut"))
                        {
                            continue;
                        }
                        detectedPoses.AddLast("PoseHandLeftOut");
                        lastPose = "PoseHandLeftOut";
                        continue;
                    }

                    //rightHandOut
                    if (handRight.checkPose(p1))
                    {
                        if (lastPose != null && lastPose.Equals("PoseHandRightOut"))
                        {
                            continue;
                        }
                        detectedPoses.AddLast("PoseHandRightOut");
                        lastPose = "PoseHandRightOut";
                        continue;
                    }

                    lastPose = null;

                }

                if (skeletonHelper.ExpectedPoses.Count != detectedPoses.Count) Assert.Fail("expected pose count not equal to actual pose count");

                string[] skelStrings = new string[detectedPoses.Count];
                string[] actStrings = new string[detectedPoses.Count];

                detectedPoses.CopyTo(actStrings, 0);
                skeletonHelper.ExpectedPoses.CopyTo(skelStrings, 0);

                for (int i = 0; i < detectedPoses.Count; i++)
                {
                    if (!skelStrings[i].Equals(actStrings[i])) Assert.Fail("actual pose different from expected pose");
                }
                Assert.AreEqual(detectedPoses, skeletonHelper.ExpectedPoses);
                detectedPoses.Clear();
            }
            //more tests
        }


    }
}
