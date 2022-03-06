
namespace FarmerVitalsEvolved
{
	internal class ModConfig
	{
		public bool enableMod = true;
		public bool enableDebug = true;

		public bool enableBaseVitals = true;
		public int baseMaxHealth = 50; // Total Max Health 300
		public int baseMaxStamina = 200; // Total Max Stamina 600

		public bool enableStardropVitals = true;
		public int stardropHealthGain = 10; // 70
		public int stardropStaminaGain = 30; // 210

		public bool enableSnakeMilkVitals = true;
		public int snakeMilkHealthGain = 50;
		public int snakeMilkStaminaGain = 50;

		public bool enableCombatProfessionVitals = true;
		public int fighterHealthGain = 20;
		public int defenderHealthGain = 30;

		public bool enableFarmingVitals = true;
		public float farmingHealthGain = 1.0f;
		public float farmingStaminaGain = 4.0f;

		public bool enableMiningVitals = true;
		public float miningHealthGain = 2.0f;
		public float miningStaminaGain = 3.0f;

		public bool enableForagingVitals = true;
		public float foragingHealthGain = 1.0f;
		public float foragingStaminaGain = 4.0f;

		public bool enableFishingVitals = true;
		public float fishingHealthGain = 1.0f;
		public float fishingStaminaGain = 3.0f;

		public bool enableCombatVitals = true;
		public bool overrideVanillaCombatHealth = true;
		public float combatHealthGain = 3.0f;
		public float combatStaminaGain = 0.0f;

		public bool enableSleepVitals = true;
		public int sleepHealthGain = 5;
		public int sleepStaminaGain = 10;
		public int exhaustedLoss = 50;
		public bool enableExhaustedHealth = false;
	}
}
