using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman
{
    class ExtraBomb : EmptyModifier
    {
        #region Singleton
        private static ExtraBomb instance = new ExtraBomb();
        private ExtraBomb() { }
        public static ExtraBomb Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

   
        public void onBegin() {
            this.player.BombsAvailable += 1;
        }
    
        public int getRespirationTime() {
            return 0;
        }
    }
}
