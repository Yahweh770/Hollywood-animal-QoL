using HollywoodAnimalQOL2.Objects;
using Managers;
using System;
using UnityEngine;
using Logger = Loggerns.Logger;

namespace HollywoodAnimalQOL2
{
    internal class CheatGUI : MonoBehaviour
    {
        private bool _showWindow = false;
        private Rect _windowRect = new Rect(10, 10, 400, 500);
        private Vector2 _scrollPosition = Vector2.zero;
        
        void Update()
        {
            // F8 для показа/скрытия GUI
            if (Input.GetKeyDown(KeyCode.F8))
            {
                _showWindow = !_showWindow;
            }
        }
        
        void OnGUI()
        {
            if (!_showWindow || !CheatManager.CheatMenuEnabled)
                return;
            
            _windowRect = GUILayout.Window(666, _windowRect, DrawWindow, "🎬 Hollywood Animal Cheat Menu");
        }
        
        void DrawWindow(int windowID)
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
            GUILayout.Label("=== HOTKEYS ===", GUILayout.ExpandWidth(true));
            GUILayout.Label("F1 - Toggle cheat menu");
            GUILayout.Label("F2 - God mode (no money spend)");
            GUILayout.Label("F3 - Add $10,000");
            GUILayout.Label("F4 - Max all character stats");
            GUILayout.Label("F5 - Research speed x10 toggle");
            GUILayout.Label("F6 - Time speed x5 toggle");
            GUILayout.Label("F7 - Reset all cheats");
            GUILayout.Label("F8 - Show/hide this window");
            
            GUILayout.Space(10);
            GUILayout.Label("=== MONEY ===", GUILayout.ExpandWidth(true));
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add $1,000"))
                CheatManager.AddMoney(1000);
            if (GUILayout.Button("Add $10,000"))
                CheatManager.AddMoney(10000);
            if (GUILayout.Button("Add $100,000"))
                CheatManager.AddMoney(100000);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set $0"))
                CheatManager.SetMoney(0);
            if (GUILayout.Button("Set $1,000,000"))
                CheatManager.SetMoney(1000000);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            CheatManager.GodMode = GUILayout.Toggle(CheatManager.GodMode, "God Mode (No Spend)");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label("=== RESEARCH SPEED ===", GUILayout.ExpandWidth(true));
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x1"))
                CheatManager.SetResearchSpeed(1f);
            if (GUILayout.Button("x2"))
                CheatManager.SetResearchSpeed(2f);
            if (GUILayout.Button("x5"))
                CheatManager.SetResearchSpeed(5f);
            if (GUILayout.Button("x10"))
                CheatManager.SetResearchSpeed(10f);
            GUILayout.EndHorizontal();
            
            GUILayout.Label($"Current: {CheatManager.ResearchSpeedMultiplier:F1}x");
            
            GUILayout.Space(10);
            GUILayout.Label("=== TIME SPEED ===", GUILayout.ExpandWidth(true));
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x0.5"))
                CheatManager.SetTimeSpeed(0.5f);
            if (GUILayout.Button("x1"))
                CheatManager.SetTimeSpeed(1f);
            if (GUILayout.Button("x2"))
                CheatManager.SetTimeSpeed(2f);
            if (GUILayout.Button("x5"))
                CheatManager.SetTimeSpeed(5f);
            GUILayout.EndHorizontal();
            
            GUILayout.Label($"Current: {CheatManager.TimeSpeedMultiplier:F1}x");
            
            GUILayout.Space(10);
            GUILayout.Label("=== CHARACTER STATS ===", GUILayout.ExpandWidth(true));
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Max All Characters"))
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
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset All Characters"))
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
                                    CheatManager.ResetCharacterStats(t);
                                    count++;
                                }
                            }
                            Logger.Log($"Reset stats for {count} characters");
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label("=== MULTIPLIERS ===", GUILayout.ExpandWidth(true));
            
            GUILayout.Label("Skill Multiplier:");
            CheatManager.CharacterSkillMultiplier = GUILayout.HorizontalSlider(CheatManager.CharacterSkillMultiplier, 0.1f, 10f);
            GUILayout.Label($"Current: {CheatManager.CharacterSkillMultiplier:F2}x");
            
            GUILayout.Space(5);
            
            GUILayout.Label("Starting Happiness:");
            CheatManager.CharacterHappiness = GUILayout.HorizontalSlider(CheatManager.CharacterHappiness, 0f, 1f);
            GUILayout.Label($"Current: {CheatManager.CharacterHappiness:F2}");
            
            GUILayout.Space(5);
            
            GUILayout.Label("Starting Loyalty:");
            CheatManager.CharacterLoyalty = GUILayout.HorizontalSlider(CheatManager.CharacterLoyalty, 0f, 1f);
            GUILayout.Label($"Current: {CheatManager.CharacterLoyalty:F2}");
            
            GUILayout.Space(10);
            GUILayout.Label("=== RESET ===", GUILayout.ExpandWidth(true));
            
            if (GUILayout.Button("Reset ALL Cheats to Default"))
            {
                CheatManager.ResetToDefaults();
                Logger.Log("All cheats reset to default values");
            }
            
            GUILayout.Space(10);
            GUILayout.Label($"Status: {(CheatManager.CheatMenuEnabled ? "ENABLED" : "DISABLED")}");
            
            GUILayout.EndScrollView();
            
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
    }
}
