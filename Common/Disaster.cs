using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;

namespace NDMod.Common
{
    public class Disaster
    {
        public int duration;
        public int severity;

        private bool active;
        public bool Active { get { return active; } }
        /// <summary>
        /// Choose what to do while your disaster is active!
        /// </summary>
        /// <param name="disaster"></param>
        public virtual void UpdateActive(Disaster disaster) { }

        public virtual bool OnBegin() => true;

        public virtual bool OnEnd() => true;
    }
}