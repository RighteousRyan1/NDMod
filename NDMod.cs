using Terraria.ModLoader;
using NDMod.Common.Utilities;
using NDMod.Common;
using System.Collections.Generic;

namespace NDMod
{
	public class NDMod : Mod
	{
        public static List<Disaster> Disasters { get; private set; } = new List<Disaster>();

        private bool _newActiveDisaster;
        private bool _oldActiveDisaster;
        public override void PostUpdateEverything()
        {
            Disasters = OOPHelper.GetSubclasses<Disaster>();

            foreach (Disaster disaster in Disasters)
            {
                _newActiveDisaster = disaster.Active;

                if (_newActiveDisaster && !_oldActiveDisaster)
                {
                    disaster.OnBegin();
                }
                else if (!_newActiveDisaster && _oldActiveDisaster)
                {
                    disaster.OnEnd();
                }
                if (disaster.Active)
                {
                    disaster.UpdateActive(disaster);
                }

                _oldActiveDisaster = _newActiveDisaster;
            }
        }
    }
}