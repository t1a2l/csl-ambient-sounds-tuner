﻿using System.Xml.Serialization;
using AmbientSoundsTuner2.CommonShared.Configuration;
using UnityEngine;
using YamlDotNet.Serialization;

namespace AmbientSoundsTuner2.SoundPack.Migration
{
    [XmlRoot("SoundPacksFile")]
    public class SoundPacksFileV1 : VersionedConfig
    {
        public SoundPacksFileV1()
        {
            this.Version = 1;
            this.SoundPacks = new SoundPack[0];
        }

        [XmlArray("SoundPacks"), XmlArrayItem("SoundPack")]
        public SoundPack[] SoundPacks { get; set; }


        public class SoundPack
        {
            public SoundPack()
            {
                this.Ambients = new Audio[0];
                this.AmbientsNight = new Audio[0];
                this.Animals = new Audio[0];
                this.Buildings = new Audio[0];
                this.Vehicles = new Audio[0];
                this.Miscs = new Audio[0];
            }

            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlArray("Ambients"), XmlArrayItem("Ambient")]
            public Audio[] Ambients { get; set; }

            [XmlArray("AmbientsNight"), XmlArrayItem("AmbientNight")]
            public Audio[] AmbientsNight { get; set; }

            [XmlArray("Animals"), XmlArrayItem("Animal")]
            public Audio[] Animals { get; set; }

            [XmlArray("Buildings"), XmlArrayItem("Building")]
            public Audio[] Buildings { get; set; }

            [XmlArray("Vehicles"), XmlArrayItem("Vehicle")]
            public Audio[] Vehicles { get; set; }

            [XmlArray("Miscs"), XmlArrayItem("Misc")]
            public Audio[] Miscs { get; set; }
        }

        public class Audio
        {
            public Audio()
            {
                this.AudioInfo = new AudioInfo();
            }

            [XmlAttribute("type")]
            public string Type { get; set; }

            [XmlAttribute("name")]
            public string Name { get; set; }

            public AudioInfo AudioInfo { get; set; }
        }

        public class AudioInfo
        {
            public AudioInfo()
            {
                this.Variations = new Variation[0];

                // Set some default values that make sense
                this.Volume = 1;
                this.MaxVolume = 1;
                this.Pitch = 1;
            }

            public string Clip { get; set; }

            [XmlIgnore]
            [YamlIgnore]
            public AudioClip AudioClip { get; set; }

            public float Volume { get; set; }

            public float MaxVolume { get; set; }

            public float Pitch { get; set; }

            public float FadeLength { get; set; }

            public bool IsLoop { get; set; }

            public bool Is3D { get; set; }

            public bool IsRandomTime { get; set; }

            [XmlArray("Variations"), XmlArrayItem("Variation")]
            public Variation[] Variations { get; set; }
        }

        public class Variation
        {
            public int Probability { get; set; }

            public AudioInfo AudioInfo { get; set; }
        }
    }
}
