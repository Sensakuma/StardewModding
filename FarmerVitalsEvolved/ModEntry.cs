using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace FarmerVitalsEvolved
{
	public class ModEntry : Mod
	{
		private ModConfig Config;
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

			if (this.Config.enableMod)
			{
				base.Monitor.Log("Mod is enabled", (LogLevel)1);
				helper.Events.GameLoop.DayEnding += this.DayEndGameLoop;
				helper.Events.GameLoop.DayStarted += this.DayStartGameLoop;
			}
			else
			{
				base.Monitor.Log("Mod is currently disabled.", (LogLevel)3);
			}
		}
		////////////////////////////////////////////////// MAIN MOD LOOP //////////////////////////////////////////////////
		private void DayStartGameLoop(object sender, DayStartedEventArgs e)
		{
			if (!Context.IsWorldReady)
			{
				base.Monitor.Log("Error during start of new day.", (LogLevel)4);
			}
			else
			{
				base.Monitor.Log("Starting new day.", (LogLevel)1);
				this.CalculateMaxVitals();
				this.ApplyNewMaxVitals();
				this.ApplyVitals();
			}
		}

		private void DayEndGameLoop(object sender, DayEndingEventArgs e)
		{
			if (!Context.IsWorldReady)
			{
				base.Monitor.Log("Error during end of day.", (LogLevel)4);
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
				base.Monitor.Log("Something went wrong before calculating max vitals.", (LogLevel)4);
			}
			else
			{

				if (this.Config.enableBaseVitals)
				{
					this.CalculateBaseVitals();
				}

				if (this.Config.enableEventVitals)
				{
					this.CalculateEventVitals();
				}

				if (this.Config.enableProfessionVitals && (Game1.player.professions.Contains(24)))
				{
					this.CalculateProfessionVitals();
				}

				if (this.Config.enableSkillVitals)
				{
					this.CalculateSkillVitals();
				}
				this.vitalsMaxHealth = this.newMaxHealth - this.removeVanillaHealth;
				this.vitalsMaxStamina = this.newMaxStamina - this.removeVanillaStamina;
			}
		}

		private void ApplyNewMaxVitals()
		{
			Game1.player.maxHealth += this.vitalsMaxHealth;
			Game1.player.MaxStamina += this.vitalsMaxStamina;
		}

		private void ApplyVitals()
		{
			Game1.player.health += this.vitalsMaxHealth;
			Game1.player.stamina += (float)this.vitalsMaxStamina;
		}

		private void RemoveNewMaxVitals()
		{
			Game1.player.maxHealth -= this.vitalsMaxHealth;
			Game1.player.MaxStamina -= this.vitalsMaxStamina;
		}
		////////////////////////////////////////////////// CALCULATION METHODS //////////////////////////////////////////////////
		private void CalculateBaseVitals()
		{
			this.newMaxHealth += this.Config.baseMaxHealth;
			this.newMaxStamina += this.Config.baseMaxStamina;
			this.removeVanillaHealth += vanillaMaxHealth;
			this.removeVanillaStamina += vanillaMaxStamina;
		}

		private void CalculateEventVitals()
		{
			if (this.Config.enableSnakeMilkVitals && Game1.player.mailReceived.Contains("qiCave"))
			{
				base.Monitor.Log("You drank iridium Snake Milk!", (LogLevel)3);
			}

			if (this.Config.enableStardropVitals && Game1.player.MaxStamina > vanillaMaxStamina)
			{
				int stardropCount = (Game1.player.MaxStamina - vanillaMaxStamina) / vanillaStardropStamina;
				base.Monitor.Log("If calculations are correct you have collected " + stardropCount.ToString() + " Stardrops", (LogLevel)3);
			}
		}

		private void CalculateProfessionVitals()
		{
			if (Game1.player.professions.Contains(24))
			{
				base.Monitor.Log("You have the fighter profession!", (LogLevel)3);
			}

			if (Game1.player.professions.Contains(27))
			{
				base.Monitor.Log("You have the defender profession!", (LogLevel)3);
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
		}

		private void CalculateMiningVitals()
		{
			int miningLevel = Game1.player.MiningLevel;
			int miningHealth = (int)((float)miningLevel * this.Config.miningHealthGain);
			int miningStamina = (int)((float)miningLevel * this.Config.miningStaminaGain);
			this.newMaxHealth += miningHealth;
			this.newMaxStamina += miningStamina;
		}

		private void CalculateForagingVitals()
		{
			int foragingLevel = Game1.player.ForagingLevel;
			int foragingHealth = (int)((float)foragingLevel * this.Config.foragingHealthGain);
			int foragingStamina = (int)((float)foragingLevel * this.Config.foragingStaminaGain);
			this.newMaxHealth += foragingHealth;
			this.newMaxStamina += foragingStamina;
		}

		private void CalculateFishingVitals()
		{
			int fishingLevel = Game1.player.FishingLevel;
			int fishingHealth = (int)((float)fishingLevel * this.Config.fishingHealthGain);
			int fishingStamina = (int)((float)fishingLevel * this.Config.fishingStaminaGain);
			this.newMaxHealth += fishingHealth;
			this.newMaxStamina += fishingStamina;
		}

		private void CalculateCombatVitals()
		{
			int combatLevel = Game1.player.CombatLevel;
			int combatStamina = (int)((float)combatLevel * this.Config.combatStaminaGain);
			this.newMaxStamina += combatStamina;

			if (!this.Config.overrideVanillaCombatHealth)
			{
				base.Monitor.Log("Using vanilla combat health progression, 5 health gained every level except level 5 and 10", (LogLevel)1);
			}
			else
			{
				int combatHealth = (int)((float)combatLevel * this.Config.combatHealthGain);
				this.newMaxHealth += combatHealth;
				if (combatLevel >= 10)
				{
					combatLevel -= 2;
				}
				else
				{
					if (combatLevel >= 5)
					{
						combatLevel-= 1;
					}
				}
				int vanillaCombatHealth = combatLevel * vanillaCombatHealthGain;
				this.removeVanillaHealth += vanillaCombatHealth;
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
	}
}
