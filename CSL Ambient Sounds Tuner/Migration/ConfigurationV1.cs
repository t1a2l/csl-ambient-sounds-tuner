﻿using System.Xml.Serialization;
using AmbientSoundsTuner2.CommonShared;
using AmbientSoundsTuner2.CommonShared.Configuration;

namespace AmbientSoundsTuner2.Migration
{
    [XmlRoot("Configuration")]
    public class ConfigurationV1 : VersionedConfig
    {
        [XmlRoot("State")]
        public class StateConfig
        {
            public StateConfig() { }
        }

        public ConfigurationV1()
        {
            this.Version = 1;

            this.State = new StateConfig();
            this.ExtraDebugLogging = false;

            this.AmbientVolumes = new SerializableDictionary<AudioManager.AmbientType, float>();
            this.AnimalVolumes = new SerializableDictionary<string, float>();
            this.BuildingVolumes = new SerializableDictionary<string, float>();
            this.VehicleVolumes = new SerializableDictionary<string, float>();
            this.MiscVolumes = new SerializableDictionary<string, float>();
        }

        public StateConfig State { get; set; }

        public bool ExtraDebugLogging { get; set; }

        [XmlElement("AmbientVolumes")]
        public SerializableDictionary<AudioManager.AmbientType, float> AmbientVolumes { get; set; }

        [XmlElement("AnimalVolumes")]
        public SerializableDictionary<string, float> AnimalVolumes { get; set; }

        [XmlElement("BuildingVolumes")]
        public SerializableDictionary<string, float> BuildingVolumes { get; set; }

        [XmlElement("VehicleVolumes")]
        public SerializableDictionary<string, float> VehicleVolumes { get; set; }

        [XmlElement("MiscVolumes")]
        public SerializableDictionary<string, float> MiscVolumes { get; set; }
    }
}
