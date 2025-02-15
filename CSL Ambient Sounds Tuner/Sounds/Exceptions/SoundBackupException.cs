﻿using System;

namespace AmbientSoundsTuner2.Sounds.Exceptions
{
    public class SoundBackupException : Exception
    {
        public SoundBackupException(string soundId, string message) :
            base(string.Format("Failed to back up sound {0}: {1}", soundId, message))
        {
            this.SoundId = soundId;
        }

        public SoundBackupException(string soundId, Exception innerException)
            : base(string.Format("Failed to back up sound {0}", soundId), innerException)
        {
            this.SoundId = soundId;
        }

        public string SoundId { get; set; }
    }
}
