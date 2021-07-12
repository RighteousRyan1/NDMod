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

		[Label("Blackout")]
		[DefaultValue(true)]
		public bool disasterEnabled_Blackout;

		[Label("Hailstorm")]
		[DefaultValue(true)]
		public bool disasterEnabled_Hailstorm;

		[Label("Sinkhole")]
		[DefaultValue(true)]
		public bool disasterEnabled_Sinkhole;

		[Label("Thunderstorm")]
		[DefaultValue(true)]
		public bool disasterEnabled_Thunderstorm;

		[Label("Lava Rain")]
		[DefaultValue(true)]
		public bool disasterEnabled_LavaRain;

		[Label("Hurricane")]
		[DefaultValue(true)]
		public bool disasterEnabled_Hurricane;

		[Label("Solar Flare")]
		[DefaultValue(true)]
		public bool disasterEnabled_SolarFlare;

		[Label("Vortex")]
		[DefaultValue(true)]
		public bool disasterEnabled_Vortex;
	}
}
