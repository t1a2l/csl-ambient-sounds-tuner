﻿using System.Collections.Generic;
using System.Xml.Serialization;
using AmbientSoundsTuner2.CommonShared;
using AmbientSoundsTuner2.CommonShared.Configuration;

namespace AmbientSoundsTuner2.Migration
{
    [XmlRoot("Configuration")]
    public class ConfigurationV4 : VersionedConfig
    {
        public ConfigurationV4()
        {
            this.Version = 4;

            this.SoundPackPreset = "Default";
            this.ExtraDebugLogging = false;
            this.AmbientSounds = new SerializableDictionary<AudioManager.AmbientType, Sound>();
            this.AnimalSounds = new SerializableDictionary<string, Sound>();
            this.BuildingSounds = new SerializableDictionary<string, Sound>();
            this.VehicleSounds = new SerializableDictionary<string, Sound>();
            this.MiscSounds = new SerializableDictionary<string, Sound>();
        }

        public string SoundPackPreset { get; set; }

        public bool ExtraDebugLogging { get; set; }

        public SerializableDictionary<AudioManager.AmbientType, Sound> AmbientSounds { get; set; }

        public SerializableDictionary<string, Sound> AnimalSounds { get; set; }

        public SerializableDictionary<string, Sound> BuildingSounds { get; set; }

        public SerializableDictionary<string, Sound> VehicleSounds { get; set; }

        public SerializableDictionary<string, Sound> MiscSounds { get; set; }

        public IDictionary<T, Sound> GetSoundsByCategoryId<T>(string id)
        {
            switch (id)
            {
                case "Ambient": return this.AmbientSounds as IDictionary<T, Sound>;
                case "Animal": return this.AnimalSounds as IDictionary<T, Sound>;
                case "Building": return this.BuildingSounds as IDictionary<T, Sound>;
                case "Vehicle": return this.VehicleSounds as IDictionary<T, Sound>;
                case "Misc": return this.MiscSounds as IDictionary<T, Sound>;
            }
            return null;
        }

        public class Sound
        {
            public string SoundPack { get; set; }

            public float Volume { get; set; }
        }
    }
}
