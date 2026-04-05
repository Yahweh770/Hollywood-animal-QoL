using System;
using System.Collections.Generic;
using System.Reflection;
using Data.GameObject.Character;
using Enums;
using Managers;
using Model;
using UnityEngine;
using Logger = Loggerns.Logger;

namespace HollywoodAnimalQOL2.Objects
{
    internal static class CheatManager
    {
        public static bool CheatMenuEnabled { get; set; } = false;
        public static bool GodMode { get; set; } = false;
        public static float MoneyMultiplier { get; set; } = 1f;
        public static float ResearchSpeedMultiplier { get; set; } = 1f;
        public static float TimeSpeedMultiplier { get; set; } = 1f;
        
        // Характеристики персонажей
        public static float CharacterSkillMultiplier { get; set; } = 1f;
        public static float CharacterHappiness { get; set; } = 1f;
        public static float CharacterLoyalty { get; set; } = 1f;
        public static float CharacterSelfEsteem { get; set; } = 1f;
        public static float CharacterAttitude { get; set; } = 1f;
        
        // Статистика персонажей
        public static int MaxStatValue { get; set; } = 100;
        public static int MinStatValue { get; set; } = 0;
        
        private static MethodInfo _setMoneyMethod;
        private static MethodInfo _addMoneyMethod;
        
        public static void Init()
        {
            Logger.Log("CheatManager initialized");
            ResetToDefaults();
        }
        
        public static void ResetToDefaults()
        {
            GodMode = false;
            MoneyMultiplier = 1f;
            ResearchSpeedMultiplier = 1f;
            TimeSpeedMultiplier = 1f;
            CharacterSkillMultiplier = 1f;
            CharacterHappiness = 1f;
            CharacterLoyalty = 1f;
            CharacterSelfEsteem = 1f;
            CharacterAttitude = 1f;
        }
        
        public static void SetMoney(int amount)
        {
            try
            {
                if (HelperObject.SaveManager != null)
                {
                    var playerData = HelperObject.SaveManager?.GetType()?.GetProperty("PlayerData")?.GetValue(HelperObject.SaveManager);
                    if (playerData != null)
                    {
                        var moneyProperty = playerData.GetType().GetProperty("Money");
                        if (moneyProperty != null && moneyProperty.CanWrite)
                        {
                            moneyProperty.SetValue(playerData, amount);
                            Logger.Log($"Money set to: {amount}");
                            return;
                        }
                        
                        // Попытка найти метод установки денег
                        var setMoneyMethod = playerData.GetType().GetMethod("SetMoney", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (setMoneyMethod != null)
                        {
                            setMoneyMethod.Invoke(playerData, new object[] { amount });
                            Logger.Log($"Money set via method to: {amount}");
                            return;
                        }
                    }
                }
                Logger.Log("Could not find money property or method");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error setting money: {ex.Message}");
            }
        }
        
        public static void AddMoney(int amount)
        {
            try
            {
                if (HelperObject.SaveManager != null)
                {
                    var playerData = HelperObject.SaveManager?.GetType()?.GetProperty("PlayerData")?.GetValue(HelperObject.SaveManager);
                    if (playerData != null)
                    {
                        var moneyProperty = playerData.GetType().GetProperty("Money");
                        if (moneyProperty != null)
                        {
                            var currentMoney = (int)moneyProperty.GetValue(playerData);
                            SetMoney(currentMoney + amount);
                            Logger.Log($"Added {amount} money. New total: {currentMoney + amount}");
                            return;
                        }
                    }
                }
                Logger.Log("Could not add money");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error adding money: {ex.Message}");
            }
        }
        
        public static void ModifyCharacterStats(TalentDataWrapper character, float skill = -1, float happiness = -1, 
            float loyalty = -1, float selfEsteem = -1, float attitude = -1)
        {
            try
            {
                if (character == null)
                {
                    Logger.Log("Character is null");
                    return;
                }
                
                var talentData = character.GetType().GetProperty("TalentData")?.GetValue(character);
                if (talentData == null)
                {
                    talentData = character;
                }
                
                var type = talentData.GetType();
                
                // Skill
                if (skill >= 0)
                {
                    var skillProperty = type.GetProperty("Skill");
                    if (skillProperty != null && skillProperty.CanWrite)
                        skillProperty.SetValue(talentData, Mathf.Clamp(skill, MinStatValue, MaxStatValue));
                }
                
                // Happiness
                if (happiness >= 0)
                {
                    var happinessProperty = type.GetProperty("Happiness");
                    if (happinessProperty != null && happinessProperty.CanWrite)
                        happinessProperty.SetValue(talentData, Mathf.Clamp(happiness, 0f, 1f));
                }
                
                // Loyalty
                if (loyalty >= 0)
                {
                    var loyaltyProperty = type.GetProperty("Loyalty");
                    if (loyaltyProperty != null && loyaltyProperty.CanWrite)
                        loyaltyProperty.SetValue(talentData, Mathf.Clamp(loyalty, 0f, 1f));
                }
                
                // Self Esteem
                if (selfEsteem >= 0)
                {
                    var selfEsteemProperty = type.GetProperty("SelfEsteem");
                    if (selfEsteemProperty != null && selfEsteemProperty.CanWrite)
                        selfEsteemProperty.SetValue(talentData, Mathf.Clamp(selfEsteem, 0f, 1f));
                }
                
                // Attitude
                if (attitude >= 0)
                {
                    var attitudeProperty = type.GetProperty("Attitude");
                    if (attitudeProperty != null && attitudeProperty.CanWrite)
                        attitudeProperty.SetValue(talentData, Mathf.Clamp(attitude, 0f, 1f));
                }
                
                Logger.Log($"Modified character stats: {character.FirstName} {character.LastName}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error modifying character stats: {ex.Message}");
            }
        }
        
        public static void MaximizeCharacterStats(TalentDataWrapper character)
        {
            ModifyCharacterStats(character, skill: MaxStatValue, happiness: 1f, loyalty: 1f, 
                selfEsteem: 1f, attitude: 1f);
        }
        
        public static void ResetCharacterStats(TalentDataWrapper character)
        {
            ModifyCharacterStats(character, skill: 50, happiness: 0.5f, loyalty: 0.5f, 
                selfEsteem: 0.5f, attitude: 0.5f);
        }
        
        public static void SetResearchSpeed(float multiplier)
        {
            ResearchSpeedMultiplier = Mathf.Max(0.1f, multiplier);
            Logger.Log($"Research speed multiplier set to: {ResearchSpeedMultiplier}");
        }
        
        public static void SetTimeSpeed(float multiplier)
        {
            TimeSpeedMultiplier = Mathf.Max(0.1f, multiplier);
            Logger.Log($"Time speed multiplier set to: {TimeSpeedMultiplier}");
        }
    }
}
