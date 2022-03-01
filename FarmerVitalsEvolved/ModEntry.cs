using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace FarmerVitalsEvolved
{
	public class ModEntry : Mod
	{
		private ModConfig Config;
		private int debugVal;

		private int newMaxHealth;
		private int newMaxStamina;
		private int removeVanillaHealth;
		private int removeVanillaStamina;
		private int vitalsMaxHealth;
		private int vitalsMaxStamina;
		private int savedHealth;
		private int savedStamina;

		private const int vanillaCombatHealthGain = 5;
		private const int vanillaMaxHealth = 100;
		private const int vanillaMaxStamina = 270;
		private const int vanillaFighterHealth = 15;
		private const int vanillaDefenderHealth = 25;
		private const int vanillaSnakeMilkHealth = 25;
		private const int vanillaStardropStamina = 34;
		////////////////////////////////////////////////// MOD INITIALIZING //////////////////////////////////////////////////
		public override void Entry(IModHelper helper)
		{
			Config = Helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.DayEnding += OnDayEnding;
			helper.Events.GameLoop.Saving += OnSaving;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
		}

		public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
			GenerateConfigMenu();
			DebugToggle();
		}
		////////////////////////////////////////////////// MAIN MOD LOOP //////////////////////////////////////////////////
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			WorldReadyCheck();
            Monitor.Log("New Day, Calculating Vitals...", (LogLevel)debugVal);
			CalculateMaxVitals();
			ApplyNewMaxVitals();
			ApplyVitals();
		}

		private void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			WorldReadyCheck();
			SaveCurrentVitals();
			RevertVitals();
		}

		private void OnSaving(object sender, SavingEventArgs e) // Alter Current health here
        {
			if (Config.enableSleepVitals)
            {
				if (savedHealth != 0)
                {
					Game1.player.health = savedHealth;
                }
				if (savedStamina != 0)
                {
					Game1.player.stamina = savedStamina;
                }
            }
		}

		////////////////////////////////////////////////// MAIN METHODS //////////////////////////////////////////////////
		private void CalculateMaxVitals()
		{
			ResetVariables();

			CalculateEventVitals();
			CalculateSkillVitals();

			if (Config.enableBaseVitals)
			{
				CalculateBaseVitals();
			}

			if (Config.enableCombatProfessionVitals && Game1.player.professions.Contains(24))
			{
				CalculateProfessionVitals();
			}

			vitalsMaxHealth = newMaxHealth - removeVanillaHealth;
			vitalsMaxStamina = newMaxStamina - removeVanillaStamina;
			VitalsSummary();
		}

		private void ApplyNewMaxVitals()
		{
			Game1.player.maxHealth += vitalsMaxHealth;
			Game1.player.MaxStamina += vitalsMaxStamina;
            Monitor.Log("Player now has " + Game1.player.maxHealth + " MaxHealth and, " + Game1.player.MaxStamina + " MaxStamina." , (LogLevel)debugVal);
		}

		private void ApplyVitals()
		{
			Game1.player.health += Game1.player.maxHealth;
			Game1.player.stamina += Game1.player.MaxStamina;
		}

		private void RevertVitals()
		{
            Monitor.Log("Removing Vitals before saving...", (LogLevel)debugVal);
			Game1.player.maxHealth -= vitalsMaxHealth;
			Game1.player.MaxStamina -= vitalsMaxStamina;
            Monitor.Log("Player now has " + Game1.player.maxHealth + " MaxHealth and, " + Game1.player.MaxStamina + " MaxStamina.", (LogLevel)debugVal);
		}
		////////////////////////////////////////////////// CALCULATION METHODS //////////////////////////////////////////////////
		private void CalculateBaseVitals()
		{
			int baseMaxHealth = Config.baseMaxHealth;
			int baseMaxStamina = Config.baseMaxStamina;
			newMaxHealth += baseMaxHealth;
			newMaxStamina += baseMaxStamina;
			removeVanillaHealth += vanillaMaxHealth;
			removeVanillaStamina += vanillaMaxStamina;
            Monitor.Log("New base Vitals are " + baseMaxHealth + " Health and, " + baseMaxStamina + " Stamina.", (LogLevel)debugVal);
		}

		private void CalculateEventVitals()
		{
			if (Config.enableSnakeMilkVitals && Game1.player.mailReceived.Contains("qiCave"))
			{
				int snakeMilkHealthGain = Config.snakeMilkHealthGain;
				int snakeMilkStaminaGain = Config.snakeMilkStaminaGain;
				newMaxHealth += snakeMilkHealthGain;
				newMaxStamina += snakeMilkStaminaGain;
				removeVanillaHealth += vanillaSnakeMilkHealth;
                Monitor.Log("Iridium Snake Milk gave you " + snakeMilkHealthGain + " MaxHealth and, " + snakeMilkStaminaGain + " MaxStamina instead of " + vanillaSnakeMilkHealth + " MaxHealth.", (LogLevel)debugVal);
			}

			if (Config.enableStardropVitals && Game1.player.MaxStamina > vanillaMaxStamina)
			{
				int extraStamina = Game1.player.MaxStamina - vanillaMaxStamina;
				int stardropCount = extraStamina / vanillaStardropStamina;
				int vanillaStardropStaminaTotal = stardropCount * vanillaStardropStamina;
				int stardropHealth = stardropCount * Config.stardropHealthGain;
				int stardropStamina = stardropCount * Config.stardropStaminaGain;
				newMaxHealth += stardropHealth;
				newMaxStamina += stardropStamina;
				removeVanillaStamina += vanillaStardropStaminaTotal;
                Monitor.Log("If calculations are correct you have collected " + stardropCount.ToString() + " Stardrop(s)", (LogLevel)debugVal);
                Monitor.Log("Stardrops are giving you " + stardropHealth + " MaxHealth and, " + stardropStamina + " MaxHealth instead of " + vanillaStardropStaminaTotal, (LogLevel)debugVal);
				if (extraStamina != vanillaStardropStaminaTotal)
                {
					int staminaRemainder = extraStamina - vanillaStardropStaminaTotal;
                    Monitor.Log(staminaRemainder.ToString() + " Stamina remaining after Stardrop calculations, are you getting stamina from other sources?", (LogLevel)3);
				}
			}
		}

		private void CalculateProfessionVitals()
		{
			if (Game1.player.professions.Contains(24))
			{
				int fighterHealth = Config.fighterHealthGain;
				newMaxHealth += fighterHealth;
				removeVanillaHealth += vanillaFighterHealth;
                Monitor.Log("The Fighter profession is giving you " + fighterHealth + " MaxHealth instead of " + vanillaFighterHealth + ".", (LogLevel)debugVal);
			}

			if (Game1.player.professions.Contains(27))
			{
				int defenderHealth = Config.defenderHealthGain;
				newMaxHealth += defenderHealth;
				removeVanillaHealth += vanillaDefenderHealth;
                Monitor.Log("The Defender profession is giving you " + defenderHealth + " MaxHealth instead of " + vanillaDefenderHealth + ".", (LogLevel)debugVal);
			}
		}

		private void CalculateSkillVitals()
		{
			if (Config.enableFarmingVitals)
			{
				CalculateFarmingVitals();
			}

			if (Config.enableMiningVitals)
			{
				CalculateMiningVitals();
			}

			if (Config.enableForagingVitals)
			{
				CalculateForagingVitals();
			}

			if (Config.enableFishingVitals)
			{
				CalculateFishingVitals();
			}

			if (Config.enableCombatVitals)
			{
				CalculateCombatVitals();
			}
		}

		private void CalculateFarmingVitals()
		{
			int farmingLevel = Game1.player.FarmingLevel;
			int farmingHealth = (int)((float)farmingLevel * Config.farmingHealthGain);
			int farmingStamina = (int)((float)farmingLevel * Config.farmingStaminaGain);
			newMaxHealth += farmingHealth;
			newMaxStamina += farmingStamina;
            Monitor.Log("Farming Level " + farmingLevel + " is giving you " + farmingHealth + " MaxHealth and, " + farmingStamina + " MaxStamina.", (LogLevel)debugVal);
		}

		private void CalculateMiningVitals()
		{
			int miningLevel = Game1.player.MiningLevel;
			int miningHealth = (int)((float)miningLevel * Config.miningHealthGain);
			int miningStamina = (int)((float)miningLevel * Config.miningStaminaGain);
			newMaxHealth += miningHealth;
			newMaxStamina += miningStamina;
            Monitor.Log("Mining Level " + miningLevel + " is giving you " + miningHealth + " MaxHealth and, " + miningStamina + " MaxStamina.", (LogLevel)debugVal);
		}

		private void CalculateForagingVitals()
		{
			int foragingLevel = Game1.player.ForagingLevel;
			int foragingHealth = (int)((float)foragingLevel * Config.foragingHealthGain);
			int foragingStamina = (int)((float)foragingLevel * Config.foragingStaminaGain);
			newMaxHealth += foragingHealth;
			newMaxStamina += foragingStamina;
            Monitor.Log("Farming Level " + foragingLevel + " is giving you " + foragingHealth + " MaxHealth and, " + foragingStamina + " MaxStamina.", (LogLevel)debugVal);
		}

		private void CalculateFishingVitals()
		{
			int fishingLevel = Game1.player.FishingLevel;
			int fishingHealth = (int)((float)fishingLevel * Config.fishingHealthGain);
			int fishingStamina = (int)((float)fishingLevel * Config.fishingStaminaGain);
			newMaxHealth += fishingHealth;
			newMaxStamina += fishingStamina;
            Monitor.Log("Farming Level " + fishingLevel + " is giving you " + fishingHealth + " MaxHealth and, " + fishingStamina + " MaxStamina.", (LogLevel)debugVal);
		}

		private void CalculateCombatVitals()
		{
			int combatLevel = Game1.player.CombatLevel;
			int combatStamina = (int)((float)combatLevel * Config.combatStaminaGain);
			newMaxStamina += combatStamina;

			if (Config.overrideVanillaCombatHealth == false)
			{
                Monitor.Log("Using vanilla combat health progression, 5 health gained every level except level 5 and 10", (LogLevel)1);
                Monitor.Log("Combat Level " + combatLevel + " is giving you " + combatStamina + " MaxStamina.", (LogLevel)debugVal);
			}
			else
			{
				int combatHealth = (int)((float)combatLevel * Config.combatHealthGain);
				int tempCombatLevel = combatLevel;
				newMaxHealth += combatHealth;
				if (tempCombatLevel >= 10)
				{
					tempCombatLevel -= 2;
				}
				else
				{
					if (tempCombatLevel >= 5)
					{
						tempCombatLevel -= 1;
					}
				}
				int vanillaCombatHealth = tempCombatLevel * vanillaCombatHealthGain;
				removeVanillaHealth += vanillaCombatHealth;
                Monitor.Log("Combat Level " + combatLevel + " is giving you " + combatHealth + " MaxHealth and, " + combatStamina + " MaxStamina.", (LogLevel)debugVal);
                Monitor.Log(vanillaCombatHealth.ToString() + " Health removed from Vanilla Combat progression", (LogLevel)debugVal);
			}

		}
		////////////////////////////////////////////////// MISC METHODS //////////////////////////////////////////////////
		private void SaveCurrentVitals()
        {
			if (Config.enableSleepVitals)
            {
				savedHealth = Game1.player.health;
				savedStamina = (int)Game1.player.stamina;
				Monitor.Log("Ending night with " + savedHealth + " Health and " + savedStamina + " Stamina.", (LogLevel)debugVal);
			}
        }

		private void ResetVariables()
		{
			newMaxHealth = 0;
			newMaxStamina = 0;
			removeVanillaHealth = 0;
			removeVanillaStamina = 0;
		}

		private void WorldReadyCheck()
        {
			if (!Context.IsWorldReady || !Config.enableMod)
			{
				return;
			}
		}

		private void DebugToggle()
        {
			if (Config.enableDebug)
			{
				debugVal = 1;
			}
			else
			{
				debugVal = 0;
			}
		}

		private void VitalsSummary()
        {
			Monitor.Log(Game1.player.maxHealth + " MaxHealth and, " + Game1.player.MaxStamina + " MaxStamina before calculations.", (LogLevel)debugVal);
			Monitor.Log(removeVanillaHealth + " Vanilla MaxHealth removed, " + removeVanillaStamina + " Vanilla MaxStamina removed.", (LogLevel)debugVal);
			Monitor.Log(newMaxHealth + " MaxHealth added, " + newMaxStamina + " MaxStamina added.", (LogLevel)debugVal);
			Monitor.Log(vitalsMaxHealth + " MaxHealth difference, " + vitalsMaxStamina + "  MaxStamina difference.", (LogLevel)debugVal);
		}
		////////////////////////////////////////////////// CONFIG MENU //////////////////////////////////////////////////
		public void GenerateConfigMenu()
		{
			// get Generic Mod Config Menu's API (if it's installed)
			var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (configMenu is null)
				return;

			// REGISTER MOD
			configMenu.Register(
				mod: ModManifest,
				reset: () => Config = new ModConfig(),
				save: () => Helper.WriteConfig(Config)
			);
			// MOD TOGGLE
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Mod Enabled",
				tooltip: () => null,
				getValue: () => Config.enableMod,
				setValue: value => Config.enableMod = value
			);
			// DEBUG TOGGLE
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Debug Log",
				tooltip: () => "Shows mod calculations in the SMAPI log.",
				getValue: () => Config.enableDebug,
				setValue: value => Config.enableDebug = value
			);
			// BASE VITALS PAGE
			configMenu.AddPageLink(
				mod: ModManifest,
				pageId: "sensakuma.base",
				text: () => "Base Vitals Settings",
				tooltip: () => null
			);
			// SKILL PAGE
			configMenu.AddPageLink(
				mod: ModManifest,
				pageId: "sensakuma.skill",
				text: () => "Skill Vitals Settings",
				tooltip: () => null
			);
			// PROFESSIONS PAGE
			configMenu.AddPageLink(
				mod: ModManifest,
				pageId: "sensakuma.profession",
				text: () => "Profession Vitals Settings",
				tooltip: () => null
			);
			// PROFESSIONS PAGE
			configMenu.AddPageLink(
				mod: ModManifest,
				pageId: "sensakuma.healing",
				text: () => "Healing Vitals Settings",
				tooltip: () => null
			);
			////////////////////////////////////////////////// MAIN VITALS //////////////////////////////////////////////////
			configMenu.AddPage(
				mod: ModManifest,
				pageId: "sensakuma.base",
				pageTitle: () => "Base"
			);
			// TITLE BASE VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Base Vitals",
				tooltip: () => "Change starting Health and Stamina."
			);
			// ENABLE BASE VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enabled",
				tooltip: () => null,
				getValue: () => Config.enableBaseVitals,
				setValue: value => Config.enableBaseVitals = value
			);
			// CHOOSE BASE HEALTH
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Base Max Health",
				tooltip: () => "Health at the start of the game. (Vanilla is 100)",
				getValue: () => Config.baseMaxHealth,
				setValue: value => Config.baseMaxHealth = value
			);
			// CHOOSE BASE STAMINA
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Base Max Stamina",
				tooltip: () => "Stamina at the start of the game. (Vanilla is 270)",
				getValue: () => Config.baseMaxStamina,
				setValue: value => Config.baseMaxStamina = value
			);
			// TITLE STARDROP VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Stardrop Vitals",
				tooltip: () => "Change Health and Stamina gained from Stardrops."
			);
			// ENABLE STARDROP VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enabled",
				tooltip: () => null,
				getValue: () => Config.enableStardropVitals,
				setValue: value => Config.enableStardropVitals = value
			);
			// STARDROP HEALTH VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Stardrop Health",
				tooltip: () => "Health gained from consuming a Stardrop. (Vanilla is 0)",
				getValue: () => Config.stardropHealthGain,
				setValue: value => Config.stardropHealthGain = value
			);
			// STARDROP STAMINA VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Stardrop Stamina",
				tooltip: () => "Stamina gained from consuming a Stardrop. (Vanilla is 34)",
				getValue: () => Config.stardropStaminaGain,
				setValue: value => Config.stardropStaminaGain = value
			);
			// TITLE IRIDIUM SNAKE MILK VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Iridium Snake Milk Vitals",
				tooltip: () => "Change Health and Stamina gained from Iridium Snake Milk."
			);
			// ENABLE IRIDIUM SNAKE MILK VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => Config.enableSnakeMilkVitals,
				setValue: value => Config.enableSnakeMilkVitals = value
			);
			// IRIDIUM SNAKE MILK HEALTH VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Snake Milk Health",
				tooltip: () => "Health gained from consuming a Iridium Snake Milk. (Vanilla is 25)",
				getValue: () => Config.snakeMilkHealthGain,
				setValue: value => Config.snakeMilkHealthGain = value
			);
			// IRIDIUM SNAKE MILK STAMINA VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Snake Milk Stamina",
				tooltip: () => "Stamina gained from consuming a Iridium Snake Milk. (Vanilla is 0)",
				getValue: () => Config.snakeMilkStaminaGain,
				setValue: value => Config.snakeMilkStaminaGain = value
			);
			////////////////////////////////////////////////// PROFESSION VITALS //////////////////////////////////////////////////
			configMenu.AddPage(
				mod: ModManifest,
				pageId: "sensakuma.profession",
				pageTitle: () => "Professions"
			);
			// TITLE PROFESSION VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Combat Profession Vitals",
				tooltip: () => "Change Health gained from Fighter and Defender Professions."
			);
			// ENABLE PROFESSION VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enabled",
				tooltip: () => null,
				getValue: () => Config.enableCombatProfessionVitals,
				setValue: value => Config.enableCombatProfessionVitals = value
			);
			// FIGHTER HEALTH VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Fighter Health",
				tooltip: () => "Health you gain from the Fighter Profession. (Vanilla is 15)",
				getValue: () => Config.fighterHealthGain,
				setValue: value => Config.fighterHealthGain = value
			);
			// DEFENDER HEALTH VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Defender Health",
				tooltip: () => "Health you gain from the Defender Profession. (Vanilla is 25)",
				getValue: () => Config.defenderHealthGain,
				setValue: value => Config.defenderHealthGain = value
			);
			////////////////////////////////////////////////// SKILL VITALS //////////////////////////////////////////////////
			configMenu.AddPage(
				mod: ModManifest,
				pageId: "sensakuma.skill",
				pageTitle: () => "Skills"
			);
			// TITLE FARMING VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Farming Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Farming Levels."
			);
			// ENABLE FARMING VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => Config.enableFarmingVitals,
				setValue: value => Config.enableFarmingVitals = value
			);
			// FARMING HEALTH GAIN
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Health Per Level",
				tooltip: () => null,
				getValue: () => Config.farmingHealthGain,
				setValue: value => Config.farmingHealthGain = value
			);
			// FARMING STAMINA VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => Config.farmingStaminaGain,
				setValue: value => Config.farmingStaminaGain = value
			);
			// TITLE MINING VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Mining Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Mining Levels."
			);
			// ENABLE MINING VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => Config.enableMiningVitals,
				setValue: value => Config.enableMiningVitals = value
			);
			// MINING HEALTH GAIN
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Health Per Level",
				tooltip: () => null,
				getValue: () => Config.miningHealthGain,
				setValue: value => Config.miningHealthGain = value
			);
			// MINING STAMINA VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => Config.miningStaminaGain,
				setValue: value => Config.miningStaminaGain = value
			);
			// TITLE FORAGING VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Foraging Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Foraging Levels."
			);
			// ENABLE FORAGING VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => Config.enableForagingVitals,
				setValue: value => Config.enableForagingVitals = value
			);
			// FORAGING HEALTH GAIN
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Health Per Level",
				tooltip: () => null,
				getValue: () => Config.foragingHealthGain,
				setValue: value => Config.foragingHealthGain = value
			);
			// FORAGING STAMINA VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => Config.foragingStaminaGain,
				setValue: value => Config.foragingStaminaGain = value
			);
			// TITLE FISHING VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Fishing Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Fishing Levels."
			);
			// ENABLE FISHING VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => Config.enableFishingVitals,
				setValue: value => Config.enableFishingVitals = value
			);
			// FISHING HEALTH GAIN
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Health Per Level",
				tooltip: () => null,
				getValue: () => Config.fishingHealthGain,
				setValue: value => Config.fishingHealthGain = value
			);
			// FISHING STAMINA VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => Config.fishingStaminaGain,
				setValue: value => Config.fishingStaminaGain = value
			);
			// TITLE COMBAT VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Combat Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Combat Levels."
			);
			// ENABLE COMBAT VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => Config.enableCombatVitals,
				setValue: value => Config.enableCombatVitals = value
			);
			// OVERRIDE VANILLA COMBAT HEALTH
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Override Vanilla Health",
				tooltip: () => "Vanilla behavior gives (5) health every level except for levels (5 & 10).",
				getValue: () => Config.overrideVanillaCombatHealth,
				setValue: value => Config.overrideVanillaCombatHealth = value
			);
			// COMBAT HEALTH GAIN
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Health Per Level",
				tooltip: () => "Only in effect if Override Vanilla Health is True",
				getValue: () => Config.combatHealthGain,
				setValue: value => Config.combatHealthGain = value
			);
			// COMBAT STAMINA VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => Config.combatStaminaGain,
				setValue: value => Config.combatStaminaGain = value
			);
			////////////////////////////////////////////////// HEALING VITALS //////////////////////////////////////////////////
			configMenu.AddPage(
				mod: ModManifest,
				pageId: "sensakuma.healing",
				pageTitle: () => "Healing"
			);
			// TITLE SLEEP VITALS
			configMenu.AddSectionTitle(
				mod: ModManifest,
				text: () => "Sleep Vitals",
				tooltip: () => "Alter Health and Stamina gained from Sleep."
			);
			// ENABLE SLEEP VITALS
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => Config.enableSleepVitals,
				setValue: value => Config.enableSleepVitals = value
			);
			// FISHING HEALTH GAIN
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Health Restored",
				tooltip: () => null,
				getValue: () => Config.sleepHealthGain,
				setValue: value => Config.sleepHealthGain = value
			);
			// FISHING STAMINA VALUE
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Stamina Restored",
				tooltip: () => null,
				getValue: () => Config.sleepStaminaGain,
				setValue: value => Config.sleepStaminaGain = value
			);
		}
	}
}