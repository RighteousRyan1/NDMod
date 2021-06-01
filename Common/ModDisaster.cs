using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace NDMod.Common
{
    /// <summary>
    /// Make your own natural disaster!
    /// <para></para>
    /// You can change how it behaves, how long it stays active, whatever!
    /// <para></para>
    /// You can access any disaster using <code>ModContent.GetInstance</code>
    /// </summary>
    public class ModDisaster
    {
        public virtual TagCompound Save() => null;
        public virtual void Load(TagCompound tag) { }
        public Mod mod => ModLoader.Mods.First(mod => mod.Code == GetType().Assembly);
        /// <summary>
        /// The current duration of the disaster. This is how much longer it will last for.
        /// </summary>
        public int duration;
        public int cdTimer;
        /// <summary>
        /// Set this if you want NPCs to go home during this disaster. False by default.
        /// </summary>
        public virtual bool ShouldTownNPCsGoToHomes => false;
        /// <summary>
        /// Put the things in here you wish to do when the player saves and quits.
        /// </summary>
        public virtual void SaveAndQuit() { }
        /// <summary>
        /// Put things you always want to update here. This runs both when active and inactive.
        /// </summary>
        public virtual void UpdateAlways() { }
        /// <summary>
        /// Put all things you want to instantiate on Mod.Load() here.
        /// </summary>
        public virtual void Initialize() { }
        public bool Active { get => duration > 0; }
        /// <summary>
        /// Choose what to do while your disaster is active!
        /// </summary>
        /// <param name="disaster">This disaster.</param>
        public virtual void UpdateActive(ModDisaster disaster) { }
        /// <summary>
        /// Choose what should happen when the disaster begins.
        /// </summary>
        /// <returns>Whether or not something should happen.</returns>
        public virtual bool OnBegin() => true;
        /// <summary>
        /// Choose what should happen when the disaster ends.
        /// </summary>
        /// <returns>Whether or not something should happen.</returns>
        public virtual bool OnEnd()
        {
            cdTimer = Cooldown;
            return true;
        }
        /// <summary>
        /// Do things while your disaster is not active. 
        /// <para></para>Do things like changing the chance of the disaster happening, or something else cool!
        /// </summary>
        /// <param name="disaster">This disaster.</param>
        public virtual void UpdateInactive(ModDisaster disaster) { }
        /// <summary>
        /// Change the chance for this event to occur every in-game tick. <para></para>Closer to 0 means less common, closer to 1 means more common.
        /// </summary>
        public virtual float ChanceToOccur => 0f;
        /// <summary>
        /// When this disaster begins, the duration set between MaxDuration * SetDurationBounds(ref lowestPercentile) and MaxDuration
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
        public virtual bool CanActivate => cdTimer <= 0;

        /// <summary>
        /// Set this to any number. This number is how many ticks must pass before RFG can start randomly rolling this event again.
        /// </summary>
        public virtual int Cooldown => 0;
        /// <summary>
        /// The minimum duration the disaster can last.
        /// </summary>
        public virtual int MinDuration => 1;
        /// <summary>
        /// Forcefully stops this disaster.
        /// </summary>
        public void End() => duration = 0;
        /// <summary>
        /// Have a chance at naturally activating this event every X ticks. 
        /// <para>
        /// Not setting this or keeping it at 1 can make it hard to make good chances/rare chances of occurrance.
        /// </para>
        /// </summary> 
        // Should this be kept?
        // public virtual int RandomUpdateTime => 1;

        /// <summary>
        /// Tries to begin the disaster. If it cannot begin, an error message will be printed into chat.
        /// </summary>
        public bool TryBegin()
        {
            if (!CanActivate)
            {
                Main.NewTextMultiline($"Failed to start the {GetType().Name} ({Name}) disaster. It cannot be activated." 
                    + $"\nDid you mean to set CanActivate differently?", true, Color.Red);
                return false;
            }
            int rand = Main.rand.Next(MinDuration, MaxDuration + 1);
            duration = rand;
            return true;
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
        internal void Update()
        {
            if (duration > 0)
                duration--;

            if (cdTimer > 0)
                cdTimer--;

            // Every 150 ticks, attempt at starting any valid disaster.
            if (Main.GameUpdateCount % 150 /*RandomUpdateTime*/ == 0)
            {
                if (Main.rand.NextFloat() <= ChanceToOccur && !Active && CanActivate)
                {
                    TryBegin();
                }
            }
            var isDoneJoining = Main.LocalPlayer.GetModPlayer<Content.ModPlayers.DisasterPlayer>().joiningWorldTimer <= 0;
            if (GetBegin() && isDoneJoining)
                OnBegin();

            if (GetEnd() && isDoneJoining)
                OnEnd();

            UpdateAlways();

            if (!CanActivate && Active)
                End();
        }
    }
}