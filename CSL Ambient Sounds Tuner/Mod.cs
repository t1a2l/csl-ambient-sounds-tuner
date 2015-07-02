﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AmbientSoundsTuner.Compatibility;
using AmbientSoundsTuner.Detour;
using AmbientSoundsTuner.Migration;
using AmbientSoundsTuner.SoundPatchers;
using AmbientSoundsTuner.UI;
using AmbientSoundsTuner.Utils;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using CommonShared;
using CommonShared.Configuration;
using CommonShared.Utils;
using ICities;
using UnityEngine;

namespace AmbientSoundsTuner
{
    public class Mod : LoadingExtensionBase, IUserMod
    {
        internal static Configuration Settings { get; private set; }
        internal static string SettingsFilename { get; private set; }
        internal static Logger Log { get; private set; }
        internal static Mod Instance { get; private set; }

        internal ModOptionsPanel OptionsPanel { get; private set; }
        internal AmbientsPatcher AmbientsPatcher { get; private set; }
        internal AnimalsPatcher AnimalsPatcher { get; private set; }
        internal BuildingsPatcher BuildingsPatcher { get; private set; }
        internal VehiclesPatcher VehiclesPatcher { get; private set; }
        internal MiscPatcher MiscPatcher { get; private set; }

        internal static HashSet<ulong> IncompatibleMods = new HashSet<ulong>()
        {
            421050717, // [ARIS] Remove Cows
            421052798, // [ARIS] Remove Pigs
            421041154, // [ARIS] Remove Seagulls
            421527612, // SilenceObnoxiousSirens
        };

        private bool isLoaded = false;


        #region IUserMod members

        public string Name
        {
            get { return "Ambient Sounds Tuner"; }
        }

        public string Description
        {
            get { return "Tune your ambient sounds volumes individually"; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            // Since this method gets called on the main menu when this mod is enabled, we will also hook some of our loading here
            if (SimulationManager.instance.m_metaData == null || SimulationManager.instance.m_metaData.m_updateMode == SimulationManager.UpdateMode.Undefined)
            {
                // Here we ensure this gets only loaded on main menu, and not in-game
                this.Init();

                if (!this.isLoaded)
                {
                    Action<bool> pluginStateChangeCallback = null;
                    pluginStateChangeCallback = new Action<bool>(isEnabled =>
                    {
                        if (!isEnabled)
                        {
                            this.Unload();
                            PluginUtils.UnsubscribePluginStateChange(this, pluginStateChangeCallback);
                        }
                    });
                    PluginUtils.SubscribePluginStateChange(this, pluginStateChangeCallback);

                    this.Load();
                    this.isLoaded = true;
                }
            }

            // Do regular settings UI stuff
            UIHelper uiHelper = helper as UIHelper;
            if (uiHelper != null)
            {
                this.OptionsPanel = new ModOptionsPanel(uiHelper);
                this.OptionsPanel.PerformLayout();
            }
            else
            {
                Mod.Log.Warning("Could not populate the settings panel, helper is null or not a UIHelper");
            }
        }

        #endregion


        #region Loading / Unloading

        private void Init()
        {
            SettingsFilename = Path.Combine(FileUtils.GetStorageFolder(this), "AmbientSoundsTuner.xml");
            Log = new Logger(this);
            Instance = this;

            this.AmbientsPatcher = new AmbientsPatcher();
            this.AnimalsPatcher = new AnimalsPatcher();
            this.BuildingsPatcher = new BuildingsPatcher();
            this.VehiclesPatcher = new VehiclesPatcher();
            this.MiscPatcher = new MiscPatcher();
        }

        private void Load()
        {
            this.CheckIncompatibility();

            Mod.Settings = VersionedConfig.LoadConfig<Configuration>(Mod.SettingsFilename, new ConfigurationMigrator());
            Mod.Log.EnableDebugLogging = Mod.Settings.ExtraDebugLogging;

            if (Mod.Settings.ExtraDebugLogging)
            {
                Mod.Log.Warning("Extra debug logging is enabled, please use this only to get more information while hunting for bugs; don't use this when playing normally!");
            }

            CustomPlayClickSound.Detour();
            this.PatchUISounds();
        }

        private void Unload()
        {
            CustomPlayClickSound.UnDetour();
        }


        private void CheckIncompatibility()
        {
            var list = PluginUtils.GetPluginInfosOf(IncompatibleMods);
            if (list.Count > 0)
            {
                string text = string.Join(", ",
                    list.Where(kvp => kvp.Value.isEnabled)
                        .Select(kvp => string.Format("{0} ({1})", kvp.Value.GetInstances<IUserMod>()[0].Name, kvp.Value.publishedFileID.AsUInt64.ToString()))
                        .OrderBy(s => s)
                        .ToArray());

                if (text != "")
                {
                    Mod.Log.Warning("You've got some known incompatible mods enabled! It's possible that this mod doesn't work as expected.\nThe following incompatible mods are enabled: {0}.", text);
                }
            }
        }

        #endregion


        #region LoadingExtensionBase members

        /// <summary>
        /// Our entry point. Here we load the mod.
        /// </summary>
        /// <param name="mode">The game mode.</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            this.Init();
            this.Load();
            PatchSounds();
        }

        /// <summary>
        /// Our exit point. Here we unload everything we have loaded.
        /// </summary>
        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            Settings.SaveConfig(Mod.SettingsFilename);

            this.Unload();

            // Set isLoaded and isActivated to false again so the mod will load again at the main menu
            this.isLoaded = false;
        }

        #endregion


        private int PatchSounds<T>(SoundsInstancePatcher<T> patcher, IDictionary<T, float> newVolumes)
        {
            patcher.BackupVolumes();
            return patcher.PatchVolumes(newVolumes);
        }

        internal void PatchSounds()
        {
            // Patch the sirens for compatibility first!
            var patchResult = SirensPatcher.PatchPoliceSiren();
            switch (patchResult)
            {
                case SirensPatcher.PatchResult.Success:
                    Mod.Log.Info("Police sirens have been patched");
                    break;
                case SirensPatcher.PatchResult.AlreadyPatched:
                    Mod.Log.Info("Police sirens have been patched already");
                    break;
                case SirensPatcher.PatchResult.NotFound:
                    Mod.Log.Warning("Could not patch the police sirens to be different from the ambulance sirens");
                    break;
            }

            // Try patching the sound volumes
            try
            {
                int patched = this.PatchSounds(this.AmbientsPatcher, Settings.AmbientVolumes);
                patched += this.PatchSounds(this.AnimalsPatcher, Settings.AnimalVolumes);
                patched += this.PatchSounds(this.BuildingsPatcher, Settings.BuildingVolumes);
                patched += this.PatchSounds(this.VehiclesPatcher, Settings.VehicleVolumes);
                patched += this.PatchSounds(this.MiscPatcher, Settings.MiscVolumes);
                Mod.Log.Info("{0} sound volumes have been patched", patched);
            }
            catch (Exception ex)
            {
                Mod.Log.Warning("Could not patch sound volumes: {0}", ex);
            }
        }

        internal void PatchUISounds()
        {
            float clickVolume = 1;
            float disabledClickVolume = 1;
            Mod.Settings.MiscVolumes.TryGetValue(MiscPatcher.ID_CLICK_SOUND, out clickVolume);
            Mod.Settings.MiscVolumes.TryGetValue(MiscPatcher.ID_DISABLED_CLICK_SOUND, out disabledClickVolume);

            this.MiscPatcher.PatchVolume(MiscPatcher.ID_CLICK_SOUND, clickVolume);
            this.MiscPatcher.PatchVolume(MiscPatcher.ID_DISABLED_CLICK_SOUND, disabledClickVolume);
        }
    }
}
