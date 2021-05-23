using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;

namespace NDMod.Common
{
    public class Disaster
    {
        /// <summary>
        /// The current duration of the disaster. This is how much longer it will last for.
        /// </summary>
        public int duration;
        public bool Active { get => duration > 0; }
        /// <summary>
        /// Choose what to do while your disaster is active!
        /// </summary>
        /// <param name="disaster">This disaster.</param>
        public virtual void UpdateActive(Disaster disaster) { }
        /// <summary>
        /// Choose what should happen when the disaster begins.
        /// </summary>
        /// <returns>Whether or not something should happen.</returns>
        public virtual bool OnBegin() => true;
        /// <summary>
        /// Choose what should happen when the disaster ends.
        /// </summary>
        /// <returns>Whether or not something should happen.</returns>
        public virtual bool OnEnd() => true;
        /// <summary>
        /// Do things while your disaster is not active. 
        /// <para></para>Do things like changing the chance of the disaster happening, or something else cool!
        /// </summary>
        /// <param name="disaster">This disaster.</param>
        public virtual void UpdateInactive(Disaster disaster) { }
        /// <summary>
        /// Change the chance for this event to occur every in-game tick. <para></para>Closer to 0 means less common, closer to 1 means more common.
        /// </summary>
        public virtual float ChanceToOccur => 0f;
        /// <summary>
        /// When this disaster begins, the duration set between MaxDuration * 0.6f and MaxDuration
        /// </summary>
        public virtual int MaxDuration => 0;
        /// <summary>
        /// The name of your Disaster! <para></para>If not set, it will default to "My programmer did not provide a name for this disaster."
        /// <para>Don't forget to set the name!</para>
        /// </summary>
        public virtual string Name => "My programmer did not provide a name for this disaster.";

        /// <summary>
        /// Determines whether or not this disaster can happen. <para></para>Set this to something like rain, a biome boolean, or anything of the sort to match your liking.
        /// </summary>
        public virtual bool CanActivate => true;
        /// <summary>
        /// Forcefully stops this disaster.
        /// </summary>
        public void ForcefullyStopDisaster()
        {
            duration = 0;
        }
        /// <summary>
        /// Forcefully begins this disaster.
        /// </summary>
        public void ForcefullyBeginDisaster()
        {
            int rand = Main.rand.Next((int)(MaxDuration * 0.6f), MaxDuration);
            duration = rand;
        }

        private bool _oldActGetBegin;
        private bool _newActGetBegin;
        private bool _oldActGetEnd;
        private bool _newActGetEnd;
        /// <summary>
        /// Completely internal. Logic for determining begin sequences.
        /// </summary>
        /// <returns>When the disaster begins.</returns>
        internal bool GetBegin()
        {
            _newActGetBegin = Active;
            bool met = _newActGetBegin && !_oldActGetBegin;
            _oldActGetBegin = _newActGetBegin;
            return met;
        }
        /// <summary>
        /// Completely internal. Logic for determining end sequences.
        /// </summary>
        /// <returns>When the disaster ends.</returns>
        internal bool GetEnd()
        {
            _newActGetEnd = Active;
            bool met = !_newActGetEnd && _oldActGetEnd;
            _oldActGetEnd = _newActGetEnd;
            return met;
        }
    }
}