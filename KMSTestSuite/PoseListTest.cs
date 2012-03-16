using GestureModuleProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Kinect;
using System.Collections.Generic;
using TestFrameworkUtils;

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

            SkeletonCollectionHelper skeletonHelper;
            List<List<GestureEvent>> expectedPoses;
            List<List<GestureEvent>> detectedPoses;


            string[] files = Directory.GetFiles(@"C:\Users\Evan\KMS\KMSTestSuite\TestPoses");

            foreach (string filepath in files)
            {
                using (FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.None))
                {

                    BinaryFormatter binFormatter = new BinaryFormatter();
                    skeletonHelper = (SkeletonCollectionHelper)binFormatter.Deserialize(fs);

                    fs.Flush();
                    fs.Close();
                }

                // Assert.IsNotNull(skeletonHelper);
                expectedPoses = (List<List<GestureEvent>>) skeletonHelper.ExpectedPoses;
                detectedPoses = new List<List<GestureEvent>>();


                Player p1 = null;
                p1 = new Player(4, "testplayer", skeletonHelper.Skeletons[0]);
                List<Player> playerList = new List<Player>();
                playerList.Add(p1);

                foreach (Skeleton skal in skeletonHelper.Skeletons)
                {
                    p1.Skeleton = skal;
                    List<GestureEvent> results = parent.processPlayers(playerList);
                    detectedPoses.Add(results);

                }

                if (expectedPoses.Count != detectedPoses.Count) Assert.Fail("expected pose count not equal to actual pose count");

                //this 'for' block fixed pending further confirmation of relevant data structure.
                foreach (List<GestureEvent> result in detectedPoses)
                {
                    foreach (List<GestureEvent> predicted in expectedPoses)
                    {
                        if (result.Count == predicted.Count)
                        {
                            if (result[0].Type.Equals(predicted[0].Type))
                            {
                                continue;
                            }
                            Assert.Fail("result not equal to predicted");
                        }
                        Assert.Fail("Expected poselist length not equal to detected pose list length.");
                    }
                }
                Assert.AreEqual(detectedPoses, skeletonHelper.ExpectedPoses);
                detectedPoses.Clear();
            }
            //more tests
        }


    }
}
