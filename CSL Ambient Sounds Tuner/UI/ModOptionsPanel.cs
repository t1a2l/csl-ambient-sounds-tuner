﻿using System.Collections.Generic;
using System.Linq;
using AmbientSoundsTuner2.CommonShared.UI;
using AmbientSoundsTuner2.CommonShared.UI.Extensions;
using AmbientSoundsTuner2.CommonShared.Utils;
using AmbientSoundsTuner2.Defs;
using AmbientSoundsTuner2.Migration;
using AmbientSoundsTuner2.SoundPack;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.Sounds;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace AmbientSoundsTuner2.UI
{
    /// <summary>
    /// A mod options panel.
    /// </summary>
    public class ModOptionsPanel : ConfigPanelBase
    {
        public ModOptionsPanel(UIHelper helper) : base(helper) { }

        private string[] soundPacks;
        private bool isChangingSoundPackPreset = false;
        private Dictionary<string, UIDropDown> soundSelections = new Dictionary<string, UIDropDown>();

        private UIHelper modSettingsGroup;
        private UIDropDown soundPackPresetDropDown;
        private UICheckBox debugLoggingCheckBox;
        private UIHelper soundSettingsGroup;
        private UILabel versionInfoLabel;

        protected override void PopulateUI()
        {

            // Create global options
            this.modSettingsGroup = this.RootHelper.AddGroup2("Mod settings");
            this.soundPacks = new[] { "Default", "Custom" }.Union(SoundPacksManager.instance.SoundPacks.Values.OrderBy(p => p.Name).Select(p => p.Name)).ToArray();
            this.soundPackPresetDropDown = (UIDropDown)this.modSettingsGroup.AddDropdown("Sound pack preset", this.soundPacks, 0, this.SoundPackPresetDropDownSelectionChanged);
            this.soundPackPresetDropDown.selectedValue = Mod.Instance.Settings.SoundPackPreset;
            this.debugLoggingCheckBox = (UICheckBox)this.modSettingsGroup.AddCheckbox("Enable debug logging (don't use this during normal gameplay)", Mod.Instance.Settings.ExtraDebugLogging, v =>
            {
                Mod.Instance.Settings.ExtraDebugLogging = v;
                Mod.Instance.Log.EnableDebugLogging = v;
            });

            // Create tab strip
            this.soundSettingsGroup = this.RootHelper.AddGroup2("Sound settings");

            // Create tabs
            this.PopulateTabContainer();

            // Add mod information
            string versionText = Mod.Instance.BuildVersion;
            if (DlcUtils.IsAfterDarkInstalled)
                versionText += " - After Dark";
            this.versionInfoLabel = this.RootPanel.AddUIComponent<UILabel>();
            this.versionInfoLabel.isVisible = false;
            this.versionInfoLabel.autoSize = true;
            this.versionInfoLabel.textScale = 0.8f;
            this.versionInfoLabel.text = versionText;
        }

        protected void PopulateTabContainer()
        {
            // Parse all the available sounds first
            var sliders = new Dictionary<string, Dictionary<string, List<ISound>>>();
            foreach (var sound in SoundManager.instance.Sounds.Values)
            {
                if ((DlcUtils.InstalledDlcs & sound.RequiredDlc) != sound.RequiredDlc)
                    continue;

                if (!sliders.ContainsKey(sound.CategoryName))
                    sliders.Add(sound.CategoryName, new Dictionary<string, List<ISound>>());
                if (!sliders[sound.CategoryName].ContainsKey(sound.SubCategoryName))
                    sliders[sound.CategoryName].Add(sound.SubCategoryName, new List<ISound>());
                sliders[sound.CategoryName][sound.SubCategoryName].Add(sound);
            }

            // Populate the tab container
            foreach (var tabGroup in sliders)
            {
                UIHelper tabHelper = this.RootHelper;/*.AddScrollingTab(tabstrip, buttonWidth, tabGroup.Key);*/ //TODO(earalov): fix!
                foreach (var group in tabGroup.Value)
                {
                    UIHelperBase groupHelper = tabHelper.AddGroup(group.Key);
                    //TODO(earalov): fix!
                    //                    UIHelper groupHelper = tabHelper.AddGroup2(group.Key);
                    //                    ((UIComponent)groupHelper.self).parent.width -= 10; // Fix some overlap with the scrollbar
                    //                    ((UIComponent)groupHelper.self).width -= 10; // Fix some overlap with the scrollbar
                    foreach (var sound in group.Value)
                    {
                        this.CreateSoundSlider(groupHelper, sound);
                    }
                }
            }
        }

        protected void CreateSoundSlider(UIHelperBase helper, ISound sound)
        {
            // Initialize variables
            var configuration = Mod.Instance.Settings.GetSoundsByCategoryId<string>(sound.CategoryId);
            var customAudioFiles = SoundPacksManager.instance.AudioFiles.Where(kvp => kvp.Key.StartsWith(string.Format("{0}.{1}", sound.CategoryId, sound.Id))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            float volume = 0;
            if (configuration.ContainsKey(sound.Id))
                volume = configuration[sound.Id].Volume;
            else
            {
                Mod.Instance.Log.Info("No volume configuration found for {0}.{1}, using default value", sound.CategoryId, sound.Id);
                volume = sound.DefaultVolume;
            }

            // Add UI components
            UISlider uiSlider = null;
            UIPanel uiPanel = null;
            UILabel uiLabel = null;
            UIDropDown uiDropDown = null;
            var maxVolume = sound.MaxVolume;

            if (customAudioFiles.Count > 0 && configuration.ContainsKey(sound.Id) && !string.IsNullOrEmpty(configuration[sound.Id].SoundPack))
            {
                // Custom sound, determine custom max volume
                var audioFile = SoundPacksManager.instance.GetAudioFileByName(sound.CategoryId, sound.Id, configuration[sound.Id].SoundPack);
                maxVolume = Mathf.Max(audioFile.AudioInfo.MaxVolume, audioFile.AudioInfo.Volume);
            }

            uiSlider = (UISlider)helper.AddSlider(sound.Name, 0, maxVolume, 0.01f, volume, v => this.SoundVolumeChanged(sound, v));
            uiPanel = (UIPanel)uiSlider.parent;
            uiLabel = uiPanel.Find<UILabel>("Label");

            if (customAudioFiles.Count > 0)
            {
                uiDropDown = uiPanel.AttachUIComponent(GameObject.Instantiate((UITemplateManager.Peek(UITemplateDefs.ID_OPTIONS_DROPDOWN_TEMPLATE) as UIPanel).Find<UIDropDown>("Dropdown").gameObject)) as UIDropDown;
                uiDropDown.items = new[] { "Default" }.Union(customAudioFiles.Select(kvp => kvp.Value.Name)).ToArray();
                uiDropDown.height = 28;
                uiDropDown.textFieldPadding.top = 4;
                if (configuration.ContainsKey(sound.Id) && !string.IsNullOrEmpty(configuration[sound.Id].SoundPack))
                    uiDropDown.selectedValue = configuration[sound.Id].SoundPack;
                else
                    uiDropDown.selectedIndex = 0;

                uiDropDown.eventSelectedIndexChanged += (c, i) => this.SoundPackChanged(sound, i > 0 ? ((UIDropDown)c).items[i] : null, uiSlider);
                this.soundSelections[string.Format("{0}.{1}", sound.CategoryId, sound.Id)] = uiDropDown;
            }

            // Configure UI components
            uiPanel.autoLayout = false;
            uiLabel.anchor = UIAnchorStyle.Left | UIAnchorStyle.CenterVertical;
            uiLabel.width = 250;
            uiSlider.anchor = UIAnchorStyle.CenterVertical;
            uiSlider.builtinKeyNavigation = false;
            uiSlider.width = 207;
            uiSlider.relativePosition = new Vector3(uiLabel.relativePosition.x + uiLabel.width + 20, 0);
            if (customAudioFiles.Count > 0)
            {
                uiDropDown.anchor = UIAnchorStyle.CenterVertical;
                uiDropDown.width = 180;
                uiDropDown.relativePosition = new Vector3(uiSlider.relativePosition.x + uiSlider.width + 20, 0);
                uiPanel.size = new Vector2(uiDropDown.relativePosition.x + uiDropDown.width, 32);
            }
            else
            {
                uiPanel.size = new Vector2(uiSlider.relativePosition.x + uiSlider.width, 32);
            }
        }

        private void SoundVolumeChanged(ISound sound, float value)
        {
            var configuration = Mod.Instance.Settings.GetSoundsByCategoryId<string>(sound.CategoryId);

            if (!configuration.ContainsKey(sound.Id))
                configuration.Add(sound.Id, new ConfigurationV4.Sound());

            configuration[sound.Id].Volume = value;

            if (LoadingManager.instance.m_loadingComplete || !sound.IngameOnly)
                sound.PatchVolume(value);
            else
                Mod.Instance.Log.Debug("Skip patching volume of {0}.{1} as there isn't a game active", sound.CategoryId, sound.Id);
            Mod.Instance.Log.Debug("Saving config");
            Mod.Instance.Settings.SaveConfig(Mod.Instance.SettingsFilename);
        }

        private void SoundPackChanged(ISound sound, string audioFileName, UISlider uiSlider)
        {
            var configuration = Mod.Instance.Settings.GetSoundsByCategoryId<string>(sound.CategoryId);

            // Selected audio changed
            if (!configuration.ContainsKey(sound.Id))
                configuration.Add(sound.Id, new ConfigurationV4.Sound());

            // Set preset to custom
            if (!this.isChangingSoundPackPreset)
                this.soundPackPresetDropDown.selectedIndex = 1;

            if (!string.IsNullOrEmpty(audioFileName))
            {
                // Chosen audio is a custom audio
                configuration[sound.Id].SoundPack = audioFileName;
                var audioFile = SoundPacksManager.instance.GetAudioFileByName(sound.CategoryId, sound.Id, audioFileName);

                if (LoadingManager.instance.m_loadingComplete || !sound.IngameOnly)
                    sound.PatchSound(audioFile);
                else
                    Mod.Instance.Log.Debug("Skip patching sound of {0}.{1} as there isn't a game active", sound.CategoryId, sound.Id);

                uiSlider.maxValue = Mathf.Max(audioFile.AudioInfo.MaxVolume, audioFile.AudioInfo.Volume);
                uiSlider.value = audioFile.AudioInfo.Volume;
            }
            else
            {
                // Chosen audio is the default one
                configuration[sound.Id].SoundPack = "";

                if (LoadingManager.instance.m_loadingComplete || !sound.IngameOnly)
                    sound.RevertSound();
                else
                    Mod.Instance.Log.Debug("Skip reverting sound of {0}.{1} as there isn't a game active", sound.CategoryId, sound.Id);

                uiSlider.maxValue = sound.MaxVolume;
                uiSlider.value = sound.DefaultVolume;
            }
            Mod.Instance.Log.Debug("Saving config");
            Mod.Instance.Settings.SaveConfig(Mod.Instance.SettingsFilename);
        }

        private void SoundPackPresetDropDownSelectionChanged(int value)
        {
            this.isChangingSoundPackPreset = true;

            if (value == 0)
            {
                // Default
                Mod.Instance.Log.Debug("Resetting sound pack to default");
                foreach (UIDropDown dropDown in this.soundSelections.Values)
                    dropDown.selectedIndex = 0;
            }
            else if (value == 1)
            {
                // Custom, don't do anything here
            }
            else if (value >= 2)
            {
                // Sound pack
                string soundPackName = this.soundPacks[value];
                SoundPacksFileV1.SoundPack soundPack = null;
                Mod.Instance.Log.Debug("Setting sound pack to {0}", soundPackName);

                if (SoundPacksManager.instance.SoundPacks.TryGetValue(soundPackName, out soundPack))
                {
                    foreach (var dropDown in this.soundSelections)
                    {
                        var prefix = dropDown.Key.Substring(0, dropDown.Key.IndexOf('.'));
                        var id = dropDown.Key.Substring(dropDown.Key.IndexOf('.') + 1);
                        SoundPacksFileV1.Audio[] audios = null;
                        switch (prefix)
                        {
                            case "Ambient":
                                audios = soundPack.Ambients;
                                break;
                            case "AmbientNight":
                                audios = soundPack.AmbientsNight;
                                break;
                            case "Animal":
                                audios = soundPack.Animals;
                                break;
                            case "Building":
                                audios = soundPack.Buildings;
                                break;
                            case "Vehicle":
                                audios = soundPack.Vehicles;
                                break;
                            case "Misc":
                                audios = soundPack.Miscs;
                                break;
                        }
                        if (audios != null)
                        {
                            SoundPacksFileV1.Audio audio = audios.FirstOrDefault(a => a.Type == id);
                            if (audio != null)
                            {
                                Mod.Instance.Log.Debug("Setting sound {0} to {1}", audio.Type, audio.Name);
                                dropDown.Value.selectedValue = audio.Name;
                            }
                        }
                    }
                }
            }

            Mod.Instance.Settings.SoundPackPreset = this.soundPackPresetDropDown.selectedValue;
            this.isChangingSoundPackPreset = false;
        }
    }
}
