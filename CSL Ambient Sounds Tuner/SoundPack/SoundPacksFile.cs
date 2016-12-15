using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AmbientSoundsTuner.CommonShared.Configuration;
using AmbientSoundsTuner.SoundPack.Migration;
using UnityEngine;
using YamlDotNet.Serialization;

namespace AmbientSoundsTuner.SoundPack
{
    [XmlRoot("SoundPacksFile")]
    public class SoundPacksFile : VersionedConfig
    {
        public SoundPacksFile()
        {
            this.Version = 2;
            this.SoundPacks = new SoundPacksFileV1.SoundPack[0];
        }

        [XmlArray("SoundPacks"), XmlArrayItem("SoundPack")]
        public SoundPacksFileV1.SoundPack[] SoundPacks { get; set; }
    }
}
