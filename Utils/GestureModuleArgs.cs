using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public struct GestureModuleArgs
    {
        private List<Player> players;

        public GestureModuleArgs(List<Player> _players){
            players = _players;
        }


        #region Getters/Setters

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

        #endregion
    }
}
