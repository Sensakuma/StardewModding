using System;

namespace FarmerVitalsEvolved
{
	internal class ModConfig
	{
		public bool enableMod = true;

		public bool enableBaseVitals = true;
		public int baseMaxHealth = 50;
		public int baseMaxStamina = 200;

		public bool enableStardropVitals = true;
		public int stardropHealthGain = 0;
		public int stardropStaminaGain = 30;

		public bool enableSnakeMilkVitals = true;
		public int snakeMilkHealthGain = 50;
		public int snakeMilkStaminaGain = 0;

		public bool enableProfessionVitals = true;
		public int fighterHealthGain = 20;
		public int defenderHealthGain = 30;

		public bool enableFarmingVitals = true;
		public float farmingHealthGain = 5.0f;
		public float farmingStaminaGain = 5.0f;

		public bool enableMiningVitals = true;
		public float miningHealthGain = 5.0f;
		public float miningStaminaGain = 5.0f;

		public bool enableForagingVitals = true;
		public float foragingHealthGain = 5.0f;
		public float foragingStaminaGain = 5.0f;

		public bool enableFishingVitals = true;
		public float fishingHealthGain = 5.0f;
		public float fishingStaminaGain = 5.0f;

		public bool enableCombatVitals = true;
		public bool overrideVanillaCombatHealth = true;
		public float combatHealthGain = 5.0f;
		public float combatStaminaGain = 5.0f;
	}
}
