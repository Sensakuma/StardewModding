using System;
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
			this.Config = base.Helper.ReadConfig<ModConfig>();
			helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
			helper.Events.GameLoop.DayEnding += this.DayEndGameLoop;
			helper.Events.GameLoop.DayStarted += this.DayStartGameLoop;
		}
		////////////////////////////////////////////////// MAIN MOD LOOP //////////////////////////////////////////////////
		private void DayStartGameLoop(object sender, DayStartedEventArgs e)
		{
			this.DebugToggle();

			if (this.Config.enableMod == false)
            {
				base.Monitor.Log("Mod is currently disabled.", (LogLevel)3);
				return;
            }
			
			if (!Context.IsWorldReady)
			{
				base.Monitor.Log("World loaded before calculations could be made.", (LogLevel)4);
				return;
			}
			else
			{
				base.Monitor.Log("New Day, Calculating Vitals...", (LogLevel)(debugVal++));
				this.CalculateMaxVitals();
				this.ApplyNewMaxVitals();
				this.ApplyVitals();
			}
		}

		private void DayEndGameLoop(object sender, DayEndingEventArgs e)
		{
			if (!Context.IsWorldReady || this.Config.enableMod == false)
			{
				return;
			}
			else
			{
				this.RemoveNewMaxVitals();
			}
		}
		////////////////////////////////////////////////// MAIN METHODS //////////////////////////////////////////////////
		private void CalculateMaxVitals()
		{
			ResetVariables();

			if (!Context.IsWorldReady)
			{
				base.Monitor.Log("World loaded before Max Vitals could be calculated.", (LogLevel)4);
				return;
			}
			else
			{
				this.CalculateEventVitals();
				this.CalculateSkillVitals();

				if (this.Config.enableBaseVitals)
				{
					this.CalculateBaseVitals();
				}

				if (this.Config.enableProfessionVitals && (Game1.player.professions.Contains(24)))
				{
					this.CalculateProfessionVitals();
				}
				base.Monitor.Log((Game1.player.maxHealth) + " MaxHealth and, " + (Game1.player.MaxStamina) + " MaxStamina before calculations.", (LogLevel)(debugVal));
				base.Monitor.Log((removeVanillaHealth) + " Vanilla MaxHealth removed, " + (removeVanillaStamina) + " Vanilla MaxStamina removed.", (LogLevel)(debugVal));
				base.Monitor.Log((newMaxHealth) + " MaxHealth added, " + (newMaxStamina) + " MaxStamina added.", (LogLevel)(debugVal));
				this.vitalsMaxHealth = this.newMaxHealth - this.removeVanillaHealth;
				this.vitalsMaxStamina = this.newMaxStamina - this.removeVanillaStamina;
				base.Monitor.Log((vitalsMaxHealth) + " MaxHealth difference, " + (vitalsMaxStamina) + "  MaxStamina difference.", (LogLevel)(debugVal));
			}
		}

		private void ApplyNewMaxVitals()
		{
			Game1.player.maxHealth += this.vitalsMaxHealth;
			Game1.player.MaxStamina += this.vitalsMaxStamina;
			base.Monitor.Log("Player now has " + (Game1.player.maxHealth) + " MaxHealth and, " + (Game1.player.MaxStamina) + " MaxStamina." , (LogLevel)(debugVal));
		}

		private void ApplyVitals()
		{
			Game1.player.health += this.vitalsMaxHealth;
			Game1.player.stamina += (float)this.vitalsMaxStamina;
		}

		private void RemoveNewMaxVitals()
		{
			base.Monitor.Log("Removing Vitals before saving...", (LogLevel)(debugVal));
			Game1.player.maxHealth -= this.vitalsMaxHealth;
			Game1.player.MaxStamina -= this.vitalsMaxStamina;
			base.Monitor.Log("Player now has " + (Game1.player.maxHealth) + " MaxHealth and, " + (Game1.player.MaxStamina) + " MaxStamina.", (LogLevel)(debugVal));
		}
		////////////////////////////////////////////////// CALCULATION METHODS //////////////////////////////////////////////////
		private void CalculateBaseVitals()
		{
			int baseMaxHealth = this.Config.baseMaxHealth;
			int baseMaxStamina = this.Config.baseMaxStamina;
			this.newMaxHealth += baseMaxHealth;
			this.newMaxStamina += baseMaxStamina;
			this.removeVanillaHealth += vanillaMaxHealth;
			this.removeVanillaStamina += vanillaMaxStamina;
			base.Monitor.Log("New base Vitals are " + (baseMaxHealth) + " Health and, " + (baseMaxStamina) + " Stamina.", (LogLevel)(debugVal));
		}

		private void CalculateEventVitals()
		{
			if (this.Config.enableSnakeMilkVitals && Game1.player.mailReceived.Contains("qiCave"))
			{
				int snakeMilkHealthGain = this.Config.snakeMilkHealthGain;
				int snakeMilkStaminaGain = this.Config.snakeMilkStaminaGain;
				this.newMaxHealth += snakeMilkHealthGain;
				this.newMaxStamina += snakeMilkStaminaGain;
				this.removeVanillaHealth += vanillaSnakeMilkHealth;
				base.Monitor.Log("Iridium Snake Milk gave you " + (snakeMilkHealthGain) + " MaxHealth and, " + (snakeMilkStaminaGain) + " MaxStamina instead of " + (vanillaSnakeMilkHealth) + " MaxHealth.", (LogLevel)(debugVal));
			}

			if (this.Config.enableStardropVitals && Game1.player.MaxStamina > vanillaMaxStamina)
			{
				int extraStamina = Game1.player.MaxStamina - vanillaMaxStamina;
				int stardropCount = extraStamina / vanillaStardropStamina;
				int vanillaStardropStaminaTotal = stardropCount * vanillaStardropStamina;
				int stardropHealth = stardropCount * this.Config.stardropHealthGain;
				int stardropStamina = stardropCount * this.Config.stardropStaminaGain;
				this.newMaxHealth += stardropHealth;
				this.newMaxStamina += stardropStamina;
				this.removeVanillaStamina += vanillaStardropStaminaTotal;
				base.Monitor.Log("If calculations are correct you have collected " + (stardropCount.ToString()) + " Stardrop(s)", (LogLevel)(debugVal));
				base.Monitor.Log("Stardrops are giving you " + (stardropHealth) + " MaxHealth and, " + (stardropStamina) + " MaxHealth instead of " + (vanillaStardropStaminaTotal), (LogLevel)(debugVal));
				if (extraStamina != vanillaStardropStaminaTotal)
                {
					int staminaRemainder = extraStamina - vanillaStardropStaminaTotal;
					base.Monitor.Log(staminaRemainder.ToString() + " Stamina remaining after Stardrop calculations, are you getting stamina from other sources?", (LogLevel)3);
				}
			}
		}

		private void CalculateProfessionVitals()
		{
			if (Game1.player.professions.Contains(24))
			{
				int fighterHealth = this.Config.fighterHealthGain;
				this.newMaxHealth += fighterHealth;
				this.removeVanillaHealth += vanillaFighterHealth;
				base.Monitor.Log("The Fighter profession is giving you " + (fighterHealth) + " MaxHealth instead of " + (vanillaFighterHealth) + ".", (LogLevel)(debugVal));
			}

			if (Game1.player.professions.Contains(27))
			{
				int defenderHealth = this.Config.defenderHealthGain;
				this.newMaxHealth += defenderHealth;
				this.removeVanillaHealth += vanillaDefenderHealth;
				base.Monitor.Log("The Defender profession is giving you " + (defenderHealth) + " MaxHealth instead of " + (vanillaDefenderHealth) + ".", (LogLevel)(debugVal));
			}
		}

		private void CalculateSkillVitals()
		{
			if (this.Config.enableFarmingVitals)
			{
				this.CalculateFarmingVitals();
			}

			if (this.Config.enableMiningVitals)
			{
				this.CalculateMiningVitals();
			}

			if (this.Config.enableForagingVitals)
			{
				this.CalculateForagingVitals();
			}

			if (this.Config.enableFishingVitals)
			{
				this.CalculateFishingVitals();
			}

			if (this.Config.enableCombatVitals)
			{
				this.CalculateCombatVitals();
			}
		}

		private void CalculateFarmingVitals()
		{
			int farmingLevel = Game1.player.FarmingLevel;
			int farmingHealth = (int)((float)farmingLevel * this.Config.farmingHealthGain);
			int farmingStamina = (int)((float)farmingLevel * this.Config.farmingStaminaGain);
			this.newMaxHealth += farmingHealth;
			this.newMaxStamina += farmingStamina;
			base.Monitor.Log("Farming Level " + (farmingLevel) + " is giving you " + (farmingHealth) + " MaxHealth and, " + (farmingStamina) + " MaxStamina.", (LogLevel)(debugVal));
		}

		private void CalculateMiningVitals()
		{
			int miningLevel = Game1.player.MiningLevel;
			int miningHealth = (int)((float)miningLevel * this.Config.miningHealthGain);
			int miningStamina = (int)((float)miningLevel * this.Config.miningStaminaGain);
			this.newMaxHealth += miningHealth;
			this.newMaxStamina += miningStamina;
			base.Monitor.Log("Mining Level " + (miningLevel) + " is giving you " + (miningHealth) + " MaxHealth and, " + (miningStamina) + " MaxStamina.", (LogLevel)(debugVal));
		}

		private void CalculateForagingVitals()
		{
			int foragingLevel = Game1.player.ForagingLevel;
			int foragingHealth = (int)((float)foragingLevel * this.Config.foragingHealthGain);
			int foragingStamina = (int)((float)foragingLevel * this.Config.foragingStaminaGain);
			this.newMaxHealth += foragingHealth;
			this.newMaxStamina += foragingStamina;
			base.Monitor.Log("Farming Level " + (foragingLevel) + " is giving you " + (foragingHealth) + " MaxHealth and, " + (foragingStamina) + " MaxStamina.", (LogLevel)(debugVal));
		}

		private void CalculateFishingVitals()
		{
			int fishingLevel = Game1.player.FishingLevel;
			int fishingHealth = (int)((float)fishingLevel * this.Config.fishingHealthGain);
			int fishingStamina = (int)((float)fishingLevel * this.Config.fishingStaminaGain);
			this.newMaxHealth += fishingHealth;
			this.newMaxStamina += fishingStamina;
			base.Monitor.Log("Farming Level " + (fishingLevel) + " is giving you " + (fishingHealth) + " MaxHealth and, " + (fishingStamina) + " MaxStamina.", (LogLevel)(debugVal));
		}

		private void CalculateCombatVitals()
		{
			int combatLevel = Game1.player.CombatLevel;
			int combatStamina = (int)((float)combatLevel * this.Config.combatStaminaGain);
			this.newMaxStamina += combatStamina;

			if (this.Config.overrideVanillaCombatHealth == false)
			{
				base.Monitor.Log("Using vanilla combat health progression, 5 health gained every level except level 5 and 10", (LogLevel)1);
				base.Monitor.Log("Combat Level " + (combatLevel) + " is giving you " + (combatStamina) + " MaxStamina.", (LogLevel)(debugVal));
			}
			else
			{
				int combatHealth = (int)((float)combatLevel * this.Config.combatHealthGain);
				int tempCombatLevel = combatLevel;
				this.newMaxHealth += combatHealth;
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
				this.removeVanillaHealth += vanillaCombatHealth;
				base.Monitor.Log("Combat Level " + (combatLevel) + " is giving you " + (combatHealth) + " MaxHealth and, " + (combatStamina) + " MaxStamina.", (LogLevel)(debugVal));
				base.Monitor.Log(vanillaCombatHealth.ToString() + " Health removed from Vanilla Combat progression", (LogLevel)(debugVal));
			}

		}
		////////////////////////////////////////////////// MISC METHODS //////////////////////////////////////////////////
		private void ResetVariables()
		{
			this.newMaxHealth = 0;
			this.newMaxStamina = 0;
			this.removeVanillaHealth = 0;
			this.removeVanillaStamina = 0;
		}

		private void DebugToggle()
        {
			if (this.Config.enableDebug)
			{
				debugVal = 1;
			}
			else
			{
				debugVal = 0;
			}
		}

		public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// get Generic Mod Config Menu's API (if it's installed)
			var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (configMenu is null)
				return;

			// register mod
			configMenu.Register(
				mod: this.ModManifest,
				reset: () => this.Config = new ModConfig(),
				save: () => this.Helper.WriteConfig(this.Config)
			);
			// MOD TOGGLE
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Mod Enabled",
				tooltip: () => null,
				getValue: () => this.Config.enableMod,
				setValue: value => this.Config.enableMod = value
			);
			// DEBUG TOGGLE
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Debug Log",
				tooltip: () => "Shows mod calculations in the SMAPI log.",
				getValue: () => this.Config.enableDebug,
				setValue: value => this.Config.enableDebug = value
			);
			// TITLE BASE VITALS
			configMenu.AddSectionTitle(
				mod: this.ModManifest,
				text: () => "Base Vitals",
				tooltip: () => "Change starting Health and Stamina."
			);
			// ENABLE BASE VITALS
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Enabled",
				tooltip: () => null,
				getValue: () => this.Config.enableBaseVitals,
				setValue: value => this.Config.enableBaseVitals = value
			);
			// CHOOSE BASE HEALTH
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Base Max Health",
				tooltip: () => "Health at the start of the game. (Vanilla is 100)",
				getValue: () => this.Config.baseMaxHealth,
				setValue: value => this.Config.baseMaxHealth = value
			);
			// CHOOSE BASE STAMINA
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Base Max Stamina",
				tooltip: () => "Stamina at the start of the game. (Vanilla is 270)",
				getValue: () => this.Config.baseMaxStamina,
				setValue: value => this.Config.baseMaxStamina = value
			);
			// TITLE STARDROP VITALS
			configMenu.AddSectionTitle(
				mod: this.ModManifest,
				text: () => "Stardrop Vitals",
				tooltip: () => "Change Health and Stamina gained from Stardrops."
			);
			// ENABLE STARDROP VITALS
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Enabled",
				tooltip: () => null,
				getValue: () => this.Config.enableStardropVitals,
				setValue: value => this.Config.enableStardropVitals = value
			);
			// STARDROP HEALTH VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Stardrop Health",
				tooltip: () => "Health gained from consuming a Stardrop. (Vanilla is 0)",
				getValue: () => this.Config.stardropHealthGain,
				setValue: value => this.Config.stardropHealthGain = value
			);
			// STARDROP STAMINA VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Stardrop Stamina",
				tooltip: () => "Stamina gained from consuming a Stardrop. (Vanilla is 34)",
				getValue: () => this.Config.stardropStaminaGain,
				setValue: value => this.Config.stardropStaminaGain = value
			);
			// TITLE IRIDIUM SNAKE MILK VITALS
			configMenu.AddSectionTitle(
				mod: this.ModManifest,
				text: () => "Iridium Snake Milk Vitals",
				tooltip: () => "Change Health and Stamina gained from Iridium Snake Milk."
			);
			// ENABLE IRIDIUM SNAKE MILK VITALS
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => this.Config.enableSnakeMilkVitals,
				setValue: value => this.Config.enableSnakeMilkVitals = value
			);
			// IRIDIUM SNAKE MILK HEALTH VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Snake Milk Health",
				tooltip: () => "Health gained from consuming a Iridium Snake Milk. (Vanilla is 25)",
				getValue: () => this.Config.snakeMilkHealthGain,
				setValue: value => this.Config.snakeMilkHealthGain = value
			);
			// IRIDIUM SNAKE MILK STAMINA VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Snake Milk Stamina",
				tooltip: () => "Stamina gained from consuming a Iridium Snake Milk. (Vanilla is 0)",
				getValue: () => this.Config.snakeMilkStaminaGain,
				setValue: value => this.Config.snakeMilkStaminaGain = value
			);
			// TITLE PROFESSION VITALS
			configMenu.AddSectionTitle(
				mod: this.ModManifest,
				text: () => "Profession Vitals",
				tooltip: () => "Change Health gained from Fighter and Defender Professions."
			);
			// ENABLE PROFESSION VITALS
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Enabled",
				tooltip: () => null,
				getValue: () => this.Config.enableProfessionVitals,
				setValue: value => this.Config.enableProfessionVitals = value
			);
			// FIGHTER HEALTH VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Fighter Health",
				tooltip: () => "Health you gain from the Fighter Profession. (Vanilla is 15)",
				getValue: () => this.Config.fighterHealthGain,
				setValue: value => this.Config.fighterHealthGain = value
			);
			// DEFENDER HEALTH VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Defender Health",
				tooltip: () => "Health you gain from the Defender Profession. (Vanilla is 25)",
				getValue: () => this.Config.defenderHealthGain,
				setValue: value => this.Config.defenderHealthGain = value
			);
			// TITLE FARMING VITALS
			configMenu.AddSectionTitle(
				mod: this.ModManifest,
				text: () => "Farming Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Farming Levels."
			);
			// ENABLE FARMING VITALS
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => this.Config.enableFarmingVitals,
				setValue: value => this.Config.enableFarmingVitals = value
			);
			// FARMING HEALTH GAIN
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Health Per Level",
				tooltip: () => null,
				getValue: () => this.Config.farmingHealthGain,
				setValue: value => this.Config.farmingHealthGain = value
			);
			// FARMING STAMINA VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => this.Config.farmingStaminaGain,
				setValue: value => this.Config.farmingStaminaGain = value
			);
			// TITLE MINING VITALS
			configMenu.AddSectionTitle(
				mod: this.ModManifest,
				text: () => "Mining Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Mining Levels."
			);
			// ENABLE MINING VITALS
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => this.Config.enableMiningVitals,
				setValue: value => this.Config.enableMiningVitals = value
			);
			// MINING HEALTH GAIN
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Health Per Level",
				tooltip: () => null,
				getValue: () => this.Config.miningHealthGain,
				setValue: value => this.Config.miningHealthGain = value
			);
			// MINING STAMINA VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => this.Config.miningStaminaGain,
				setValue: value => this.Config.miningStaminaGain = value
			);
			// TITLE FORAGING VITALS
			configMenu.AddSectionTitle(
				mod: this.ModManifest,
				text: () => "Foraging Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Foraging Levels."
			);
			// ENABLE FORAGING VITALS
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => this.Config.enableForagingVitals,
				setValue: value => this.Config.enableForagingVitals = value
			);
			// FORAGING HEALTH GAIN
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Health Per Level",
				tooltip: () => null,
				getValue: () => this.Config.foragingHealthGain,
				setValue: value => this.Config.foragingHealthGain = value
			);
			// FORAGING STAMINA VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => this.Config.foragingStaminaGain,
				setValue: value => this.Config.foragingStaminaGain = value
			);
			// TITLE FISHING VITALS
			configMenu.AddSectionTitle(
				mod: this.ModManifest,
				text: () => "Fishing Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Fishing Levels."
			);
			// ENABLE FISHING VITALS
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => this.Config.enableFishingVitals,
				setValue: value => this.Config.enableFishingVitals = value
			);
			// FISHING HEALTH GAIN
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Health Per Level",
				tooltip: () => null,
				getValue: () => this.Config.fishingHealthGain,
				setValue: value => this.Config.fishingHealthGain = value
			);
			// FISHING STAMINA VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => this.Config.fishingStaminaGain,
				setValue: value => this.Config.fishingStaminaGain = value
			);
			// TITLE COMBAT VITALS
			configMenu.AddSectionTitle(
				mod: this.ModManifest,
				text: () => "Combat Skill Vitals",
				tooltip: () => "Gain Health and Stamina from Combat Levels."
			);
			// ENABLE COMBAT VITALS
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Enable",
				tooltip: () => null,
				getValue: () => this.Config.enableCombatVitals,
				setValue: value => this.Config.enableCombatVitals = value
			);
			// OVERRIDE VANILLA COMBAT HEALTH
			configMenu.AddBoolOption(
				mod: this.ModManifest,
				name: () => "Override Vanilla Health",
				tooltip: () => "Vanilla behavior gives (5) health every level except for levels (5 & 10).",
				getValue: () => this.Config.overrideVanillaCombatHealth,
				setValue: value => this.Config.overrideVanillaCombatHealth = value
			);
			// COMBAT HEALTH GAIN
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Health Per Level",
				tooltip: () => "Only in effect if Override Vanilla Health is True",
				getValue: () => this.Config.combatHealthGain,
				setValue: value => this.Config.combatHealthGain = value
			);
			// COMBAT STAMINA VALUE
			configMenu.AddNumberOption(
				mod: this.ModManifest,
				name: () => "Stamina Per Level",
				tooltip: () => null,
				getValue: () => this.Config.combatStaminaGain,
				setValue: value => this.Config.combatStaminaGain = value
			);
		}
	}
}