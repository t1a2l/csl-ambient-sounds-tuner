﻿using System;
using AmbientSoundsTuner2.CommonShared.Utils;

namespace AmbientSoundsTuner2.Sounds.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SoundAttribute : Attribute
    {
        public SoundAttribute()
        {
            this.IngameOnly = true;
            this.RequiredDlc = DlcUtils.Dlc.None;
        }

        public SoundAttribute(string id, string name)
            : this()
        {
            this.Id = id;
            this.Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public bool IngameOnly { get; set; }

        public DlcUtils.Dlc RequiredDlc { get; set; }
    }
}
