using GestureModuleProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utils;
using TestFrameworkUtils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Kinect;
using System.Collections.Generic;
using GestureModuleProject;

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

        internal virtual GestureModule CreateGestureModule()
        {
            GestureModule target = new GestureModule();
            return target;
        }

        /// <summary>
        ///A test for checkPose
        ///</summary>
        [TestMethod()]
        public void checkPoseTest()
        {
            //import serialized skeleton data here //
            GestureModule parent = CreateGestureModule();

            NewSkeletonCollectionHelper skeletonHelper;
            LinkedList<List<GestureEvent>> expectedPoses;
            LinkedList<List<GestureEvent>> detectedPoses;


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
                expectedPoses = skeletonHelper.ExpectedPoses;
                detectedPoses = new LinkedList<List<GestureEvent>>();


                Player p1 = null; // TODO: Initialize to an appropriate value
                string lastPose = null; // TODO: Initialize to an appropriate value

                p1 = new Player(4, "testplayer", skeletonHelper.Skeletons[0]);
                List<Player> playerList = new List<Player>();
                playerList.Add(p1);

                foreach (Skeleton skal in skeletonHelper.Skeletons)
                {
                    p1.Skeleton = skal;
                    List<GestureEvent> results = parent.processPlayers(playerList);
                    detectedPoses.AddLast(results);

                }

                if (skeletonHelper.ExpectedPoses.Count != detectedPoses.Count) Assert.Fail("expected pose count not equal to actual pose count");

               
                //this 'for' block fixed pending further confirmation of relevant data structure.
                for (int i = 0; i < detectedPoses.Count; i++)
                {
                    if (expectedPoses.[i].Equals(actStrings[i])) Assert.Fail("actual pose different from expected pose");
                }
                Assert.AreEqual(detectedPoses, skeletonHelper.ExpectedPoses);
                detectedPoses.Clear();
            }
            //more tests
        }


    }
}
