﻿using System;
using AmbientSoundsTuner2.CommonShared.Utils;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.Sounds.Attributes;
using AmbientSoundsTuner2.Sounds.Exceptions;

namespace AmbientSoundsTuner2.Sounds
{
    /// <summary>
    /// Provides an abstract base class that implements <see cref="ISound`1"/>.
    /// </summary>
    [SoundVolume(1, 1)]
    public abstract class SoundBase : ISound
    {
        public virtual string Id { get { return this.GetAttribute<SoundAttribute>().Id; } }

        public virtual string Name { get { return this.GetAttribute<SoundAttribute>().Name; } }

        public virtual string CategoryId { get { return this.GetAttribute<SoundCategoryAttribute>().CategoryId; } }

        public virtual string CategoryName { get { return this.GetAttribute<SoundCategoryAttribute>().CategoryName; } }

        public virtual string SubCategoryName { get { return this.GetAttribute<SoundCategoryAttribute>().SubCategoryName; } }

        public virtual float DefaultVolume { get { return this.GetAttribute<SoundVolumeAttribute>().DefaultVolume; } }

        public virtual float MaxVolume { get { return this.GetAttribute<SoundVolumeAttribute>().MaxVolume; } }

        public virtual bool IngameOnly { get { return this.GetAttribute<SoundAttribute>().IngameOnly; } }

        public virtual DlcUtils.Dlc RequiredDlc { get { return this.GetAttribute<SoundAttribute>().RequiredDlc; } }

        private T GetAttribute<T>() where T : Attribute
        {
            return Attribute.GetCustomAttribute(this.GetType(), typeof(T), true) as T;
        }


        public SoundPacksFileV1.Audio OldSound { get; protected set; }

        public float? OldVolume { get; protected set; }


        public abstract SoundContainer GetSoundInstance();


        public virtual void BackUpSound()
        {
            if ((DlcUtils.InstalledDlcs & this.RequiredDlc) != this.RequiredDlc)
                return;

            try
            {
                var sound = this.GetSoundInstance();
                this.OldSound = SoundPatchUtils.GetAudioInfo(sound);
                if (this.OldSound == null)
                    throw new SoundBackupException($"{this.CategoryId}.{this.Id}", "AudioInfo is null");
            }
            catch (Exception ex)
            {
                if (ex is SoundBackupException) throw ex;
                else throw new SoundBackupException($"{this.CategoryId}.{this.Id}", ex);
            }
        }

        public virtual void PatchSound(SoundPacksFileV1.Audio newSound)
        {
            if ((DlcUtils.InstalledDlcs & this.RequiredDlc) != this.RequiredDlc)
                return;

            try
            {
                var sound = this.GetSoundInstance();

                if (!SoundPatchUtils.SetAudioInfo(sound, newSound))
                    throw new SoundPatchException($"{this.CategoryId}.{this.Id}", "Failed to set AudioInfo");
            }
            catch (Exception ex)
            {
                if (ex is SoundPatchException) throw ex;
                else throw new SoundPatchException($"{this.CategoryId}.{this.Id}", ex);
            }
        }

        public virtual void RevertSound()
        {
            if ((DlcUtils.InstalledDlcs & this.RequiredDlc) != this.RequiredDlc)
                return;

            if (this.OldSound != null)
                this.PatchSound(this.OldSound);
        }

        public virtual void BackUpVolume()
        {
            if ((DlcUtils.InstalledDlcs & this.RequiredDlc) != this.RequiredDlc)
                return;

            try
            {
                var sound = this.GetSoundInstance();
                float? volume = SoundPatchUtils.GetVolume(sound);
                if (!volume.HasValue)
                    throw new SoundBackupException($"{this.CategoryId}.{this.Id}", "Sound has no volume set");
                this.OldVolume = volume.Value;
            }
            catch (Exception ex)
            {
                if (ex is SoundBackupException) throw ex;
                else throw new SoundBackupException($"{this.CategoryId}.{this.Id}", ex);
            }
        }

        public virtual void PatchVolume(float volume)
        {
            if ((DlcUtils.InstalledDlcs & this.RequiredDlc) != this.RequiredDlc)
                return;

            try
            {
                var sound = this.GetSoundInstance();

                if (!SoundPatchUtils.SetVolume(sound, volume))
                    throw new SoundPatchException($"{this.CategoryId}.{this.Id}", "Failed to set volume");
            }
            catch (Exception ex)
            {
                if (ex is SoundPatchException) throw ex;
                else throw new SoundPatchException($"{this.CategoryId}.{this.Id}", ex);
            }
        }

        public virtual void RevertVolume()
        {
            if ((DlcUtils.InstalledDlcs & this.RequiredDlc) != this.RequiredDlc)
                return;

            if (this.OldVolume.HasValue)
                this.PatchVolume(this.OldVolume.Value);
        }
    }
}
