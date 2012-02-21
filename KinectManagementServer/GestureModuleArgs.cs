using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectManagementServer
{
    class GestureModuleArgs
    {
        private List<Player> players;

        public GestureModuleArgs(List<Player> _players){
            players = _players;
        }

        public List<Player> Players
        {
            get
            {
                return this.players;
            }

            //Don't need this
            /* set
             * {
             *     this.players = value;
             * }*/
        }
    }
}
