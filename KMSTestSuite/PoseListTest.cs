using GestureModuleProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Kinect;
using System.Collections.Generic;
using TestFrameworkUtils;
using System.Threading;

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
            GestureModule parent = null;

            SkeletonCollectionHelper skeletonHelper;
            List<List<GestureEvent>> expectedPoses;
            List<List<GestureEvent>> detectedPoses;


            string[] files = Directory.GetFiles(@"C:\Users\Chase\gitrepos\KMS\KMSTestSuite\TestPoses");

            foreach (string filepath in files)
            {
                parent = new GestureModule();
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
                    //iterates through the skeleton data, passing processing to the gestureModule on every frame (skeleton)
                    p1.Skeleton = skal;
                    List<GestureEvent> results = parent.processPlayers(playerList);
                    //collection of the resultant gestureEvent lists
                    detectedPoses.Add(results);

                }

                // For my own testing, I am printing all pose sequences that are detected.
                List<string> detectedEventSequences = new List<string>();
                foreach (List<GestureEvent> cEventSeq in detectedPoses)
                {
                    string cEventSeqStr = "";
                    foreach (GestureEvent cEvent in cEventSeq)
                    {
                        cEventSeqStr += cEvent.Type + ", ";
                       
                    }
                    if (!cEventSeqStr.Equals(""))
                    {
                        detectedEventSequences.Add(cEventSeqStr);
                        
                    }
                  
                }
                Console.WriteLine("");
                // The detectedPoses list will likely be a different size when multi-pose bin files are tested later.
                //if (expectedPoses.Count != detectedPoses.Count) Assert.Fail("expected pose count not equal to actual pose count");

                //this 'for' block fixed pending further confirmation of relevant data structure.
                /** OBSELETE
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
                        //
                        Assert.Fail("Expected poselist length not equal to detected pose list length.");
                    }
                }
                Assert.AreEqual(detectedPoses, skeletonHelper.ExpectedPoses);
                */
                
                // Check that the gesture module detected the appropriate events.
                // This segment of code works by testing that each sequence of expected poses occurs, in order.  


                // (1) The first loop iterates over the list of pose sequences that are expected.  The actual list of pose sequences that are detected 
                // each frame are most likely to contain poses that are detected out of order, etc.
                // (2) The second loop iterates the list of pose sequences that are detected each frame.  As long as each pose sequence in the list 
                // consists only of items in the first expected pose sequence, then the test will continue.  If one incorrect pose is detected, failure ensues.  
                // (3) The third loop iterates over each of the detected event in a single pose sequence and uses the (4) loop to check that the pose is valid.  It is valid if it is detected an
                // and foundEvent variable is set to true.
                // If all of the detected events in the current actual pose sequence are detected (3 loop), then the 
                //
                // 1. Tests that each expected sequence was actually detected.
                // 2. 
                int detectedIndex = 0;
                foreach (List<GestureEvent> currExpectedList in expectedPoses)
                {
                    // check that the curr actual list only consists of poses that are expected.
                    // it should have at least one frame with all of the poses that are expected! (Else return failure)

                    List<GestureEvent> currActualList;
                    while (true)
                    {
                        if (detectedIndex >= detectedPoses.Count)
                        {
                            Assert.Fail("Failure: Not all of the poses were detected.  Out of data");
                            
                        }
                        currActualList = detectedPoses[detectedIndex];
                        // check that each element in the actual list matches one of those in the expected list
                        // if all are detected, the test pases and the next expected list may be tested.
                        
                        // validates that the curractual list consists only of poses that are expected.
                        foreach (GestureEvent aDetectedEvent in currActualList)
                        {
                            Boolean foundEvent = false;
                            foreach (GestureEvent validExpectedEvent in currExpectedList)
                            {
                                if (aDetectedEvent.Type.Equals(validExpectedEvent.Type))
                                {
                                    foundEvent = true;
                                    break;
                                }
                            }
                            if (foundEvent == false)
                            {
                                Assert.Fail("Failure: A pose that was not expected was detected");
                                
                            }
                        }

                         
                        if (currActualList.Count == currExpectedList.Count)
                        {
                            // Indicates that the currActualList consists of all elements in currExpectList.
                            // This means we can iterate to the next currExpectedList!
                            //Assert.IsTrue(currActualList.Count == currExpectedList.Count); // trivial, but shows progress.
                            detectedIndex++;
                            break;
                        }

                        detectedIndex++;
                        // here the currActual list consisted of valid elements, but the currExpecctedList has not been found,
                        // therefore, we should iterate to the next currActualList.
                    }
                }




                detectedPoses.Clear();
            }
            //more tests
        }


    }
}
