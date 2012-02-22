using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using GestureModule;
using System.Runtime.CompilerServices;


namespace KinectManagementServer
{
    class GestureThread
    {
        private delegate void OnComplete(GestureModuleArgs e);
        private KinectManagementServer.OnCompleted mainThreadEvent;
        private GestureModule.GestureModule gestureModule;
        private List<Player> playerList;
        private bool mainThreadEventFlag = false;
        private GestureCompletedArgs gestureCompletedArgs;


        public GestureThread(KinectManagementServer.OnCompleted mainThreadEvent)
        {
            this.mainThreadEvent = mainThreadEvent;

        }


        private void Initialize()
        {
            this.playerList = new List<Player>();
            this.gestureCompletedArgs = new GestureCompletedArgs();
           

        }

        private void InitializeGestureMoudle()
        {
            this.gestureModule = new GestureModule.GestureModule();


        }


        public void ThreadProc()
        {
            this.Initialize();
            this.InitializeGestureMoudle();

            while (true)
            {
                while (!mainThreadEventFlag)
                {
                }
                //GestureModule gets used here
                    //send playerList to gestureModule
                    this.gestureModule.processPlayers(this.playerList);
                    
                    //invoke main thread to send back gesture info
                    mainThreadEvent.Invoke(gestureCompletedArgs);
                //clear playerList
                this.playerList.Clear();
                //reset flag so the thread is back to "sleep"
                mainThreadEventFlag = false;
            }



        }

        // delegate for main the invoke to pass player information
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void OnComplete(GestureModuleArgs e)
        {
            
            this.playerList.AddRange(e.Players);
            mainThreadEventFlag = true;

        }



    }
}
