using System;

namespace FarmerVitalsEvolved
{
	internal class ModConfig
	{
		public bool enableMod = true;

		public bool enableBaseVitals = true;
		public int baseMaxHealth = 50;
		public int baseMaxStamina = 200;

		public bool enableSkillVitals = true;
		public bool enableFarmingVitals = true;
		public float farmingHealthGain = 5f;
		public float farmingStaminaGain = 5f;

		public bool enableMiningVitals = true;
		public float miningHealthGain = 5f;
		public float miningStaminaGain = 5f;

		public bool enableForagingVitals = true;
		public float foragingHealthGain = 5f;
		public float foragingStaminaGain = 5f;

		public bool enableFishingVitals = true;
		public float fishingHealthGain = 5f;
		public float fishingStaminaGain = 5f;

		public bool enableCombatVitals = true;
		public bool overrideVanillaCombatHealth = true;
		public float combatHealthGain = 5f;
		public float combatStaminaGain = 5f;

		public bool enableEventVitals = true;
		public bool enableSnakeMilkVitals = true;
		public int snakeMilkHealthGain = 50;
		public int snakeMilkStaminaGain = 0;

		public bool enableStardropVitals = true;
		public int stardropHealthGain = 0;
		public int stardropStaminaGain = 30;

		public bool enableProfessionVitals = true;
		public int fighterHealthGain = 15;
		public int defenderHealthGain = 25;

		public bool enableSleepVitals = true;
		public int sleepHealthGain = 35;
		public int sleepStaminaGain = 75;
	}
}
