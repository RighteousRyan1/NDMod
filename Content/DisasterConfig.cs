using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace NDMod.Content
{
	[Label("Disasters Config")]
	public class DisasterConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		[Header("Disaster Disabling (NDMod only!)")]
		[Label("Earthquakes")]
		[DefaultValue(true)]
		public bool disasterEnabled_Earthquakes;
		
		[Label("Acid Rain")]
		[DefaultValue(true)]
		public bool disasterEnabled_AcidRain;

		[Label("Cold Front")]
		[DefaultValue(true)]
		public bool disasterEnabled_ColdFront;
	}
}
