﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmbientSoundsTuner2.CommonShared;
using AmbientSoundsTuner2.CommonShared.Utils;
using AmbientSoundsTuner2.Compatibility;
using AmbientSoundsTuner2.Detour;
using AmbientSoundsTuner2.Migration;
using AmbientSoundsTuner2.SoundPack;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.Sounds;
using AmbientSoundsTuner2.Sounds.Exceptions;
using AmbientSoundsTuner2.UI;
using ColossalFramework.UI;
using ICities;

namespace AmbientSoundsTuner2
{
    public class Mod : UserModBase<Mod>
    {
        protected override ulong WorkshopId { get { return 818641631; } }

        protected override IEnumerable<ulong> IncompatibleMods
        {
            get
            {
                return new HashSet<ulong>()
                {
                    //421050717, // [ARIS] Remove Cows
                    //421052798, // [ARIS] Remove Pigs
                    //421041154, // [ARIS] Remove Seagulls
                    421527612, // SilenceObnoxiousSirens
                };
            }
        }

        internal Configuration Settings { get; private set; }

        internal string SettingsFilename { get; private set; }

        internal ModOptionsPanel OptionsPanel { get; private set; }


        #region UserModBase members

        public override string Name
        {
            get { return "Ambient Sounds Tuner"; }
        }

        public override string Description
        {
            get { return "Tune your ambient sounds volumes individually"; }
        }

        public override void OnGameLoaded(LoadMode mode)
        {
            // Before we patch, we export the current game sounds as an example file
            var exampleFile = SoundManager.instance.GetCurrentSoundSettingsAsSoundPack();
            exampleFile.SaveConfig(Path.Combine(FileUtils.GetStorageFolder(Mod.Instance), "Example." + SoundPacksManager.SOUNDPACKS_FILENAME_XML));
            exampleFile.SaveConfig(Path.Combine(FileUtils.GetStorageFolder(Mod.Instance), "Example." + SoundPacksManager.SOUNDPACKS_FILENAME_YAML));

            this.PatchSounds();
            this.Log.Debug("Mod loaded in-game");
        }

        #endregion


        #region IUserModSettingsUI

        public void OnSettingsUI(UIHelperBase helper)
        {
            this.SettingsFilename = Path.Combine(FileUtils.GetStorageFolder(this), "AmbientSoundsTuner.yml");
            this.Load();
            this.PatchUISounds();
            this.Log.Debug("Mod initialized");
            // Do regular settings UI stuff
            UIHelper uiHelper = helper as UIHelper;
            if (uiHelper != null)
            {
                this.OptionsPanel = new ModOptionsPanel(uiHelper);
                this.OptionsPanel.PerformLayout();
                this.Log.Debug("Options panel created");
            }
            else
            {
                this.Log.Warning("Could not populate the settings panel, helper is null or not a UIHelper");
            }
        }

        #endregion


        public string BuildVersion
        {
            get { return "dev version"; }
        }


        #region Loading / Unloading

        private void Load()
        {
            // We have to properly migrate the outdated XML configuration file
            try
            {
                string oldXmlSettingsFilename = Path.Combine(Path.GetDirectoryName(this.SettingsFilename), Path.GetFileNameWithoutExtension(this.SettingsFilename)) + ".xml";
                if (File.Exists(oldXmlSettingsFilename) && !File.Exists(this.SettingsFilename))
                {
                    this.Settings = Configuration.LoadConfig(oldXmlSettingsFilename, new ConfigurationMigrator());
                    this.Settings.SaveConfig(this.SettingsFilename);
                    File.Delete(oldXmlSettingsFilename);
                }
                else
                {
                    this.Settings = Configuration.LoadConfig(this.SettingsFilename, new ConfigurationMigrator());
                }
            }
            catch (Exception ex)
            {
                this.Log.Warning("An error occured while loading the settings, default values will be used instead: {0}", ex);
                this.Settings = new Configuration();
            }

            this.Log.EnableDebugLogging = this.Settings.ExtraDebugLogging;
            if (this.Settings.ExtraDebugLogging)
            {
                this.Log.Warning("Extra debug logging is enabled, please use this only to get more information while hunting for bugs; don't use this when playing normally!");
            }

            // Initialize sounds
            SoundManager.instance.InitializeSounds();

            // Load sound packs
            SoundPacksManager.instance.InitSoundPacks();

            //verify config
            ValidateSettings();


            // Detour UI click sounds
            CustomPlayClickSound.Detour();
        }

        private bool ValidateSettings()
        {

            var nonExistingPacks = new HashSet<string>();
            ValidateSounds(Mod.Instance.Settings.AmbientNightSounds, nonExistingPacks, p => p.AmbientsNight);
            ValidateSounds(Mod.Instance.Settings.AmbientSounds, nonExistingPacks, p => p.Ambients);
            ValidateSounds(Mod.Instance.Settings.AnimalSounds, nonExistingPacks, p => p.Animals);
            ValidateSounds(Mod.Instance.Settings.BuildingSounds, nonExistingPacks, p => p.Buildings);
            ValidateSounds(Mod.Instance.Settings.MiscSounds, nonExistingPacks, p => p.Miscs);
            ValidateSounds(Mod.Instance.Settings.VehicleSounds, nonExistingPacks, p => p.Vehicles);

            if (nonExistingPacks.Count > 0)
            {
                Mod.Instance.Settings.SoundPackPreset = "Custom";
                Mod.Instance.Log.Debug("Some entries in config were reset to default values, saving config");
                Mod.Instance.Settings.SaveConfig(Mod.Instance.SettingsFilename);
                return false;
            }
            return nonExistingPacks.Count == 0;
        }

        private static void ValidateSounds(SerializableDictionary<string, ConfigurationV4.Sound> configuration, HashSet<string> nonExistingPacks, Func<SoundPacksFileV1.SoundPack, SoundPacksFileV1.Audio[]> selector)
        {
            configuration.ForEach(kvp =>
            {
                var soundName = kvp.Value.SoundPack;
                if (soundName == null)
                {
                    return;
                }
                if (SoundPacksManager.instance.SoundPacks.
                SelectMany(p => selector.Invoke(p.Value)).
                Any(a => a?.Name == soundName))
                {
                    return;
                }
                Mod.Instance.Log.Warning($"Sound {soundName} wasn't found in any of sound packs. Falling back to default value.");
                kvp.Value.SoundPack = null;
                nonExistingPacks.Add(soundName);
            });
        }

        private void Unload()
        {
            try
            {
                this.Settings.SaveConfig(this.SettingsFilename);
            }
            catch (Exception ex)
            {
                this.Log.Warning("An error occured while saving the settings, the settings are not saved: {0}", ex);
            }
            CustomPlayClickSound.UnDetour();

            // Actually, to be consistent and nice, we should also revert the other sound patching here.
            // But since that sounds are only patched in-game, and closing that game conveniently resets the other sounds, it's not really needed.
            // If it's needed at some point in the future, we can add that logic here.
        }

        #endregion


        internal void PatchSounds()
        {
            // Patch various sounds for compatibility first!
            switch (SoundDuplicator.PatchPoliceSiren())
            {
                case SoundDuplicator.PatchResult.Success:
                    this.Log.Debug("Police sirens have been patched for compatibility");
                    break;
                case SoundDuplicator.PatchResult.AlreadyPatched:
                    this.Log.Debug("Police sirens have been patched for compatibility already");
                    break;
                case SoundDuplicator.PatchResult.NotFound:
                    this.Log.Warning("Could not patch the police sirens for compatibility");
                    break;
            }
            switch (SoundDuplicator.PatchScooterSound())
            {
                case SoundDuplicator.PatchResult.Success:
                    this.Log.Debug("Scooter sounds have been patched for compatibility");
                    break;
                case SoundDuplicator.PatchResult.AlreadyPatched:
                    this.Log.Debug("Scooter sounds have been patched for compatibility already");
                    break;
                case SoundDuplicator.PatchResult.NotFound:
                    this.Log.Warning("Could not patch the scooter sounds for compatibility");
                    break;
            }
            switch (SoundDuplicator.PatchOilPowerPlant())
            {
                case SoundDuplicator.PatchResult.Success:
                    this.Log.Debug("Oil power plant sounds have been patched for compatibility");
                    break;
                case SoundDuplicator.PatchResult.AlreadyPatched:
                    this.Log.Debug("Oil power plant sounds have been patched for compatibility already");
                    break;
                case SoundDuplicator.PatchResult.NotFound:
                    this.Log.Warning("Could not patch the oil power plant sounds for compatibility");
                    break;
            }
            switch (SoundDuplicator.PatchWaterTreatmentPlant())
            {
                case SoundDuplicator.PatchResult.Success:
                    this.Log.Debug("Water treatment plant sounds have been patched for compatibility");
                    break;
                case SoundDuplicator.PatchResult.AlreadyPatched:
                    this.Log.Debug("Water treatment plant sounds have been patched for compatibility already");
                    break;
                case SoundDuplicator.PatchResult.NotFound:
                    this.Log.Warning("Could not patch the water treatment plant sounds for compatibility");
                    break;
            }

            // Try patching the sounds
            try
            {
                this.PatchGameSounds(true);
            }
            catch (Exception ex)
            {
                this.Log.Warning("Could not patch sounds: {0}", ex);
            }
        }

        internal void PatchUISounds()
        {
            // Try patching the sounds
            try
            {
                this.PatchGameSounds(false);
            }
            catch (Exception ex)
            {
                this.Log.Warning("Could not patch sounds: {0}", ex);
            }
        }

        private void PatchGameSounds(bool ingame)
        {
            var backedUpSounds = new List<string>();
            var backedUpVolumes = new List<string>();
            var patchedSounds = new List<string>();
            var patchedVolumes = new List<string>();

            foreach (ISound sound in SoundManager.instance.Sounds.Values)
            {
                var soundName = string.Format("{0}.{1}", sound.CategoryId, sound.Id);
                if (sound.IngameOnly != ingame)
                    continue;

                try
                {
                    IDictionary<string, ConfigurationV4.Sound> soundConfig = null;
                    switch (sound.CategoryId)
                    {
                        case "Ambient":
                            soundConfig = this.Settings.AmbientSounds;
                            break;
                        case "AmbientNight":
                            soundConfig = this.Settings.AmbientNightSounds;
                            break;
                        case "Animal":
                            soundConfig = this.Settings.AnimalSounds;
                            break;
                        case "Building":
                            soundConfig = this.Settings.BuildingSounds;
                            break;
                        case "Vehicle":
                            soundConfig = this.Settings.VehicleSounds;
                            break;
                        case "Misc":
                            soundConfig = this.Settings.MiscSounds;
                            break;
                    }

                    try
                    {
                        sound.BackUpSound();
                        backedUpSounds.Add(soundName);
                    }
                    catch (SoundBackupException ex)
                    {
                        Mod.Instance.Log.Warning("Failed to back up sound instance {0}:\r\n{1}", soundName, ex.InnerException != null ? ex.InnerException.ToString() : ex.Message);
                    }

                    try
                    {
                        sound.BackUpVolume();
                        backedUpVolumes.Add(soundName);
                    }
                    catch (SoundBackupException ex)
                    {
                        Mod.Instance.Log.Warning("Failed to back up sound volume {0}:\r\n{1}", soundName, ex.InnerException != null ? ex.InnerException.ToString() : ex.Message);
                    }

                    if (soundConfig.ContainsKey(sound.Id))
                    {
                        if (!string.IsNullOrEmpty(soundConfig[sound.Id].SoundPack) && soundConfig[sound.Id].SoundPack != "Default")
                        {
                            try
                            {
                                sound.PatchSound(SoundPacksManager.instance.GetAudioFileByName(sound.CategoryId, sound.Id, soundConfig[sound.Id].SoundPack));
                                patchedSounds.Add(soundName);
                            }
                            catch (SoundPatchException ex)
                            {
                                Mod.Instance.Log.Warning("Failed to patch sound instance {0}:\r\n{1}", soundName, ex.InnerException != null ? ex.InnerException.ToString() : ex.Message);
                            }
                        }

                        try
                        {
                            sound.PatchVolume(soundConfig[sound.Id].Volume);
                            patchedVolumes.Add(soundName);
                        }
                        catch (SoundPatchException ex)
                        {
                            Mod.Instance.Log.Warning("Failed to patch sound volume {0}:\r\n{1}", soundName, ex.InnerException != null ? ex.InnerException.ToString() : ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Mod.Instance.Log.Warning("Failed to patch sound {0}:\r\n{1}", soundName, ex);
                }
            }

            Mod.Instance.Log.Debug("Successfully backed up the following sound instances: {0}", string.Join(",", backedUpSounds.ToArray()));
            Mod.Instance.Log.Debug("Successfully backed up the following sound volumes: {0}", string.Join(",", backedUpVolumes.ToArray()));
            Mod.Instance.Log.Debug("Successfully patched the following sound instances: {0}", string.Join(",", patchedSounds.ToArray()));
            Mod.Instance.Log.Debug("Successfully patched the following sound volumes: {0}", string.Join(",", patchedVolumes.ToArray()));
        }
    }

    /** List effect script 
            foreach(EffectInfo e in UnityEngine.Resources.FindObjectsOfTypeAll<EffectInfo>())
            {
                if (e is SoundEffect)
                {
                    SoundEffect sound = (SoundEffect) e;
                    AudioInfo audio = sound.m_audioInfo;
                    UnityEngine.Debug.Log("Sound Effect: name=" + sound.name + " volume=" + audio.m_volume + " pitch=" + audio.m_pitch + " isLoop=" + audio.m_loop + " 3d=" + audio.m_is3D);
                }
                if (e is MultiEffect)
                {
                    MultiEffect multi = (MultiEffect) e;
                    if (multi.m_effects == null)
                    {
                        continue;
                    }
                    foreach (MultiEffect.SubEffect sub in multi.m_effects)
                    {
                        EffectInfo e1 = sub.m_effect;
                        if (e1 is SoundEffect)
                        {
                            SoundEffect sound = (SoundEffect)e1;
                            AudioInfo audio = sound.m_audioInfo;
                            UnityEngine.Debug.Log("Sound Effect: parent=" + multi.name + " name=" + sound.name + " volume=" + audio.m_volume + " pitch=" + audio.m_pitch + " isLoop=" + audio.m_loop + " 3d=" + audio.m_is3D);
                        }
                    }
                }
            }
     */
}
