using System.Xml.Serialization;
using AmbientSoundsTuner2.CommonShared.Configuration;
using AmbientSoundsTuner2.SoundPack.Migration;


namespace AmbientSoundsTuner2.SoundPack
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
