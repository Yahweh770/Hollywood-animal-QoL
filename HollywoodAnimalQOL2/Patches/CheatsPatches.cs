using HarmonyLib;
using HollywoodAnimalQOL2.Objects;
using Managers;
using System;
using System.Reflection;
using UnityEngine;
using Logger = Loggerns.Logger;

namespace HollywoodAnimalQOL2.Patches
{
    // Патч для изменения денег при получении/трате
    [HarmonyPatch]
    internal class MoneyPatch
    {
        static MethodBase TargetMethod()
        {
            // Ищем методы которые изменяют деньги
            var types = new[] { 
                typeof(PlayerData),
                typeof(SaveManager),
                typeof(AppController)
            };
            
            foreach (var type in types)
            {
                if (type == null) continue;
                
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    if (method.Name.Contains("Money") || method.Name.Contains("Cash"))
                    {
                        Logger.Log($"Found money-related method: {type.Name}.{method.Name}");
                        return method;
                    }
                }
            }
            return null;
        }
        
        static void Prefix(ref int amount)
        {
            if (CheatManager.GodMode || CheatManager.MoneyMultiplier != 1f)
            {
                if (amount > 0)
                {
                    amount = (int)(amount * CheatManager.MoneyMultiplier);
                }
                else if (CheatManager.GodMode && amount < 0)
                {
                    amount = 0; // Блокируем трату денег в режиме бога
                }
            }
        }
    }

    // Патч для ускорения исследований
    [HarmonyPatch(typeof(ResearchManager), "GetResearchSpeed", new Type[] { })]
    internal class ResearchSpeedPatch
    {
        static void Postfix(ref float __result)
        {
            if (CheatManager.ResearchSpeedMultiplier != 1f)
            {
                __result *= CheatManager.ResearchSpeedMultiplier;
            }
        }
    }

    // Альтернативный патч для исследования если первый не сработает
    [HarmonyPatch(typeof(TechTreeManager), "AddProgress", new Type[] { typeof(string), typeof(float) })]
    internal class ResearchProgressPatch
    {
        static void Prefix(ref float progress)
        {
            if (CheatManager.ResearchSpeedMultiplier != 1f)
            {
                progress *= CheatManager.ResearchSpeedMultiplier;
            }
        }
    }

    // Патч для изменения скорости времени
    [HarmonyPatch(typeof(TimeManager), "TimeScale", MethodType.Getter)]
    internal class TimeSpeedPatch
    {
        static void Postfix(ref float __result)
        {
            if (CheatManager.TimeSpeedMultiplier != 1f)
            {
                __result *= CheatManager.TimeSpeedMultiplier;
            }
        }
    }

    // Патч для характеристик персонажей при создании
    [HarmonyPatch(typeof(CharactersManager), "CreateTalentFromParams", MethodType.Normal)]
    internal class CharacterCreationPatch
    {
        static void Postfix(object __result)
        {
            if (__result is TalentDataWrapper talent && 
                (CheatManager.CharacterSkillMultiplier != 1f || 
                 CheatManager.CharacterHappiness != 1f ||
                 CheatManager.CharacterLoyalty != 1f))
            {
                try
                {
                    var talentData = talent.GetType().GetProperty("TalentData")?.GetValue(talent) ?? talent;
                    var type = talentData.GetType();
                    
                    // Умножаем skill
                    if (CheatManager.CharacterSkillMultiplier != 1f)
                    {
                        var skillProp = type.GetProperty("Skill");
                        if (skillProp != null)
                        {
                            var currentSkill = (float)skillProp.GetValue(talentData);
                            skillProp.SetValue(talentData, currentSkill * CheatManager.CharacterSkillMultiplier);
                        }
                    }
                    
                    // Устанавливаем happiness
                    if (CheatManager.CharacterHappiness != 1f)
                    {
                        var happinessProp = type.GetProperty("Happiness");
                        if (happinessProp != null)
                        {
                            happinessProp.SetValue(talentData, CheatManager.CharacterHappiness);
                        }
                    }
                    
                    // Устанавливаем loyalty
                    if (CheatManager.CharacterLoyalty != 1f)
                    {
                        var loyaltyProp = type.GetProperty("Loyalty");
                        if (loyaltyProp != null)
                        {
                            loyaltyProp.SetValue(talentData, CheatManager.CharacterLoyalty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error in character creation patch: {ex.Message}");
                }
            }
        }
    }

    // Патч для изменения характеристик существующих персонажей
    [HarmonyPatch(typeof(TalentDataWrapper), "Skill", MethodType.Getter)]
    internal class CharacterSkillGetterPatch
    {
        static void Postfix(ref float __result)
        {
            if (CheatManager.CharacterSkillMultiplier != 1f)
            {
                __result *= CheatManager.CharacterSkillMultiplier;
            }
        }
    }

    // Горячие клавиши для читов
    [HarmonyPatch(typeof(MonoBehaviour), "Update", MethodType.Normal)]
    internal class CheatHotkeysPatch
    {
        static void Postfix(MonoBehaviour __instance)
        {
            // F1 - включить/выключить меню читов
            if (Input.GetKeyDown(KeyCode.F1))
            {
                CheatManager.CheatMenuEnabled = !CheatManager.CheatMenuEnabled;
                Logger.Log($"Cheat menu: {(CheatManager.CheatMenuEnabled ? "ON" : "OFF")}");
            }
            
            // F2 - режим бога (бесконечные деньги)
            if (Input.GetKeyDown(KeyCode.F2))
            {
                CheatManager.GodMode = !CheatManager.GodMode;
                Logger.Log($"God mode: {(CheatManager.GodMode ? "ON" : "OFF")}");
            }
            
            // F3 - добавить 10000 денег
            if (Input.GetKeyDown(KeyCode.F3))
            {
                CheatManager.AddMoney(10000);
            }
            
            // F4 - максимизировать все характеристики всех персонажей
            if (Input.GetKeyDown(KeyCode.F4))
            {
                if (HelperObject.CharactersManager != null)
                {
                    var talentsProperty = HelperObject.CharactersManager.GetType().GetProperty("Talents");
                    if (talentsProperty != null)
                    {
                        var talents = talentsProperty.GetValue(HelperObject.CharactersManager) as System.Collections.IEnumerable;
                        if (talents != null)
                        {
                            int count = 0;
                            foreach (var talent in talents)
                            {
                                if (talent is TalentDataWrapper t)
                                {
                                    CheatManager.MaximizeCharacterStats(t);
                                    count++;
                                }
                            }
                            Logger.Log($"Maximized stats for {count} characters");
                        }
                    }
                }
            }
            
            // F5 - установить скорость исследования x10
            if (Input.GetKeyDown(KeyCode.F5))
            {
                var newMultiplier = CheatManager.ResearchSpeedMultiplier >= 10f ? 1f : 10f;
                CheatManager.SetResearchSpeed(newMultiplier);
            }
            
            // F6 - установить скорость времени x5
            if (Input.GetKeyDown(KeyCode.F6))
            {
                var newMultiplier = CheatManager.TimeSpeedMultiplier >= 5f ? 1f : 5f;
                CheatManager.SetTimeSpeed(newMultiplier);
            }
            
            // F7 - сбросить все читы к значениям по умолчанию
            if (Input.GetKeyDown(KeyCode.F7))
            {
                CheatManager.ResetToDefaults();
                Logger.Log("All cheats reset to default");
            }
        }
    }
}
