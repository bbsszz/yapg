using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bomberman.Bombs
{
    [DataContract()]
    public class BombTicker : Ticker
    {
        public BombTicker(float p) : base(p) { }

      
    }
}
