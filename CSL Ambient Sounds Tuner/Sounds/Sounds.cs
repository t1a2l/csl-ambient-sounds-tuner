﻿using System;
using System.Linq;
using AmbientSoundsTuner2.CommonShared.Utils;
using AmbientSoundsTuner2.Detour;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.Sounds.Attributes;

namespace AmbientSoundsTuner2.Sounds
{
    #region Ambients - Day
    [SoundCategory("Ambient", "Ambients", "Day")]
    public abstract class AmbientDaySoundBase : SoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            int index = (int)Enum.Parse(typeof(AudioManager.AmbientType), this.Id);
            if (AudioManager.instance.m_properties != null && AudioManager.instance.m_properties.m_ambients != null
                && AudioManager.instance.m_properties.m_ambients.Length > index)
            {
                return new SoundContainer(AudioManager.instance.m_properties.m_ambients[index]);
            }
            return null;
        }
    }

    [Sound("Agricultural", "Agricultural")]
    public class AmbientAgriculturalSound : AmbientDaySoundBase { }

    [Sound("City", "City")]
    public class AmbientCitySound : AmbientDaySoundBase { }

    [Sound("Forest", "Forest")]
    public class AmbientForestSound : AmbientDaySoundBase { }

    [Sound("Industrial", "Industrial")]
    public class AmbientIndustrialSound : AmbientDaySoundBase { }

    [Sound("Plaza", "Plaza")]
    public class AmbientPlazaSound : AmbientDaySoundBase { }

    [Sound("Sea", "Sea")]
    public class AmbientSeaSound : AmbientDaySoundBase { }

    [Sound("Stream", "Stream")]
    public class AmbientStreamSound : AmbientDaySoundBase { }

    [Sound("Suburban", "Suburban")]
    public class AmbientSuburbanSound : AmbientDaySoundBase { }

    [Sound("World", "World")]
    public class AmbientWorldSound : AmbientDaySoundBase { }

    #endregion

    #region Ambients - Night

    [SoundCategory("AmbientNight", "Ambients", "Night")]
    public abstract class AmbientNightSoundBase : SoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            int index = (int)Enum.Parse(typeof(AudioManager.AmbientType), this.Id);
            if (AudioManager.instance.m_properties != null && AudioManager.instance.m_properties.m_ambientsNight != null
                && AudioManager.instance.m_properties.m_ambientsNight.Length > index)
            {
                return new SoundContainer(AudioManager.instance.m_properties.m_ambientsNight[index]);
            }
            return null;
        }
    }

    [Sound("Agricultural", "Agricultural")]
    public class AmbientNightAgriculturalSound : AmbientNightSoundBase { }

    [Sound("City", "City")]
    public class AmbientNightCitySound : AmbientNightSoundBase { }

    [Sound("Forest", "Forest")]
    public class AmbientNightForestSound : AmbientNightSoundBase { }

    [Sound("Leisure", "Leisure")]
    public class AmbientNightLeisureSound : AmbientNightSoundBase { }

    [Sound("Suburban", "Suburban")]
    public class AmbientNightSuburbanSound : AmbientNightSoundBase { }

    [Sound("Tourist", "Tourist")]
    public class AmbientNightTouristSound : AmbientNightSoundBase { }

    #endregion

    #region Animals

    [SoundCategory("Animal", "Animals", "Animals")]
    public abstract class AnimalSoundBase : SoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            CitizenInfo info = PrefabCollection<CitizenInfo>.FindLoaded(this.Id);
            if (info != null)
            {
                switch (this.Id)
                {
                    case "Cow":
                    case "Pig":
                        return new SoundContainer(((LivestockAI)info.m_citizenAI).m_randomEffect as SoundEffect);

                    case "Seagull":
                        MultiEffect effect = ((BirdAI)info.m_citizenAI).m_randomEffect as MultiEffect;
                        if (effect != null)
                            return new SoundContainer(effect.m_effects.FirstOrDefault(e => e.m_effect.name == "Seagull Scream").m_effect as SoundEffect);
                        break;
                }
            }
            return null;
        }
    }

    [Sound("Cow", "Cows")]
    public class AnimalCowSound : AnimalSoundBase { }

    [Sound("Pig", "Pigs")]
    public class AnimalPigSound : AnimalSoundBase { }

    [Sound("Seagull", "Seagulls")]
    public class AnimalSeagullSound : AnimalSoundBase { }

    #endregion

    #region Buildings

    public abstract class BuildingSoundBase : SoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            return new SoundContainer(SoundManager.GetAudioInfoFromBuildingInfo(this.Id));
        }
    }


    [SoundCategory("Building", "Buildings", "Electricity and Water")]
    public abstract class BuildingElectricityAndWaterSoundBase : BuildingSoundBase { }

    [Sound("Advanced Wind Turbine", "Advanced Wind Turbine")]
    public class BuildingAdvancedWindTurbineSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Coal Power Plant", "Coal Power Plant")]
    public class BuildingCoalPowerPlantSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Fusion Power Plant", "Fusion Power Plant")]
    [SoundVolume(4, 4)]
    public class BuildingFusionPowerPlantSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Dam Power House", "Hydro Power Plant")]
    public class BuildingHydroPowerPlantSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Combustion Plant", "Incineration Plant")]
    public class BuildingIncinerationPlantSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Nuclear Power Plant", "Nuclear Power Plant")]
    public class BuildingNuclearPowerPlantSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Oil Power Plant", "Oil Power Plant")]
    public class BuildingOilPowerPlantSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Building Power Plant Small", "Small Power Plant")]
    public class BuildingSmallPowerPlantSound : BuildingElectricityAndWaterSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetAudioInfoFromArray(BuildingManager.instance.m_properties.m_serviceSounds, this.Id));
            return null;
        }
    }

    [Sound("Solar Power Plant", "Solar Power Plant")]
    public class BuildingSolarPowerPlantSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Water Outlet", "Water Drain Pipe")]
    public class BuildingWaterDrainPipeSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Water Intake", "Water Pumping Station")]
    public class BuildingWaterPumpingStationSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Water Treatment Plant", "Water Treatment Plant")]
    public class BuildingWaterTreatmentPlantSound : BuildingElectricityAndWaterSoundBase { }

    [Sound("Wind Turbine", "Wind Turbine")]
    public class BuildingWindTurbineSound : BuildingElectricityAndWaterSoundBase { }


    [SoundCategory("Building", "Buildings", "Services")]
    public abstract class BuildingServiceSoundBase : BuildingSoundBase { }

    [Sound("Cemetery", "Cemetery")]
    public class BuildingCemeterySound : BuildingServiceSoundBase { }

    [Sound("Crematory", "Crematory")]
    public class BuildingCrematorySound : BuildingServiceSoundBase { }

    [Sound("Elementary School", "Elementary School")]
    public class BuildingElementarySchoolSound : BuildingServiceSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            return new SoundContainer(SoundManager.GetFirstAudioInfoFromBuildingInfos(new[] { this.Id, "Elementary_School_EU" }));
        }
    }

    [Sound("Building Fire Station", "Fire Station")]
    public class BuildingFireStationSound : BuildingServiceSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetAudioInfoFromArray(BuildingManager.instance.m_properties.m_serviceSounds, this.Id));
            return null;
        }
    }

    [Sound("High School", "High School")]
    public class BuildingHighSchoolSound : BuildingServiceSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            return new SoundContainer(SoundManager.GetFirstAudioInfoFromBuildingInfos(new[] { this.Id, "highschool_EU" }));
        }
    }

    [Sound("Building Hospital", "Hospital")]
    public class BuildingHospitalSound : BuildingServiceSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetAudioInfoFromArray(BuildingManager.instance.m_properties.m_serviceSounds, this.Id));
            return null;
        }
    }

    [Sound("Building Police Station", "Police Station")]
    public class BuildingPoliceStationSound : BuildingServiceSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetAudioInfoFromArray(BuildingManager.instance.m_properties.m_serviceSounds, this.Id));
            return null;
        }
    }

    [Sound("University", "University")]
    public class BuildingUniversitySound : BuildingServiceSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            return new SoundContainer(SoundManager.GetFirstAudioInfoFromBuildingInfos(new[] { this.Id, "University_EU" }));
        }
    }


    [SoundCategory("Building", "Buildings", "Public Transport")]
    public abstract class BuildingPublicTransportSoundBase : BuildingSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetAudioInfoFromArray(BuildingManager.instance.m_properties.m_subServiceSounds, this.Id));
            return null;
        }
    }

    [Sound("Building Airport", "Airport")]
    public class BuildingAirportSound : BuildingPublicTransportSoundBase { }

    [Sound("Building Bus Depot", "Bus Depot")]
    public class BuildingBusDepotSound : BuildingPublicTransportSoundBase { }

    [Sound("Building Harbor", "Harbor")]
    public class BuildingHarborSound : BuildingPublicTransportSoundBase { }

    [Sound("Building Metro Station", "Metro Station")]
    public class BuildingMetroStationSound : BuildingPublicTransportSoundBase { }

    [Sound("Building Train Station", "Train Station")]
    public class BuildingTrainStationSound : BuildingPublicTransportSoundBase { }


    [SoundCategory("Building", "Buildings", "Leisure")]
    public abstract class BuildingAfterDarkSoundBase : BuildingSoundBase { }

    [Sound("Casino", "Casino (After Dark)", RequiredDlc = DlcUtils.Dlc.AfterDark)]
    public class BuildingCasinoSound : BuildingAfterDarkSoundBase { }

    [Sound("Zoo", "Zoo (After Dark)", RequiredDlc = DlcUtils.Dlc.AfterDark)]
    public class BuildingZooSound : BuildingAfterDarkSoundBase { }


    [SoundCategory("Building", "Buildings", "Other")]
    public abstract class BuildingOtherSoundBase : BuildingSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetAudioInfoFromArray(BuildingManager.instance.m_properties.m_subServiceSounds, this.Id));
            return null;
        }
    }

    [Sound("Building Commercial", "Commercial")]
    public class BuildingCommercialSound : BuildingOtherSoundBase { }

    [Sound("Building Industrial", "Industrial")]
    [SoundVolume(0.5f, 0.5f)]
    public class BuildingIndustrialSound : BuildingOtherSoundBase { }


    [SoundCategory("Building", "Buildings", "Miscellaneous")]
    public abstract class BuildingMiscellaneousSoundBase : BuildingSoundBase { }

    [Sound("On Fire", "On Fire")]
    public class BuildingOnFireSound : BuildingMiscellaneousSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
            {
                FireEffect effect = BuildingManager.instance.m_properties.m_fireEffect as FireEffect;
                if (effect != null)
                    return new SoundContainer(effect.m_soundEffect);
            }
            return null;
        }
    }

    [Sound("On Upgrade", "On Upgrade")]
    [SoundVolume(0.25f, 0.25f)]
    public class BuildingOnUpgradeSound : BuildingMiscellaneousSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetSubEffectFromMultiEffect(BuildingManager.instance.m_properties.m_levelupEffect as MultiEffect, "Levelup Sound"));
            return null;
        }
    }

    #endregion

    #region Vehicles

    public abstract class VehicleSoundBase : SoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            return new SoundContainer(EffectCollection.FindEffect(this.Id) as SoundEffect);
        }
    }


    [SoundCategory("Vehicle", "Vehicles", "Engines")]
    public abstract class VehicleEngineSoundBase : VehicleSoundBase { }

    [Sound("Aircraft Movement", "Aircraft Movement")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleAircraftMovementSound : VehicleEngineSoundBase { }

    [Sound("Aircraft Sound", "Aircraft Sound")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleAircraftSound : VehicleEngineSoundBase { }

    [Sound("Aircraft Landing", "Aircraft Landing")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleAircraftLandingSound : VehicleEngineSoundBase { }

    [Sound("Private Plane Movement", "Private Planes", RequiredDlc = DlcUtils.Dlc.SunsetHarbor)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehiclePrivatePlaneMovementSound : VehicleEngineSoundBase { }

    [Sound("Private Plane Sound", "Private Planes Sound", RequiredDlc = DlcUtils.Dlc.SunsetHarbor)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehiclePrivatePlaneSound : VehicleEngineSoundBase { }

    [Sound("Helicopter Movement Small", "Small Helicopters Movement", RequiredDlc = DlcUtils.Dlc.NaturalDisasters)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleHelicopterMovementSmallSound : VehicleEngineSoundBase { }

    [Sound("Helicopter Movement Large", "Large Helicopters Movement", RequiredDlc = DlcUtils.Dlc.NaturalDisasters)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleHelicopterMovementLargeSound : VehicleEngineSoundBase { }

    [Sound("Helicopter Sound Small", "Small Helicopters Sound", RequiredDlc = DlcUtils.Dlc.NaturalDisasters)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleHelicopterSoundSmallSound : VehicleEngineSoundBase { }

    [Sound("Helicopter Sound Large", "Large Helicopters Sound", RequiredDlc = DlcUtils.Dlc.NaturalDisasters)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleHelicopterSoundLargeSound : VehicleEngineSoundBase { }

    [Sound("Blimp Movement", "Blimps Movement", RequiredDlc = DlcUtils.Dlc.InMotion)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleBlimpMovementSoundLarge : VehicleEngineSoundBase { }

    [Sound("Blimp Sound Large", "Blimps Sound", RequiredDlc = DlcUtils.Dlc.InMotion)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleBlimpSoundLarge : VehicleEngineSoundBase { }

    [Sound("Large Car Sound", "Large Cars")]
    [SoundVolume(1.5f, 1.5f)]
    public class VehicleLargeCarSound : VehicleEngineSoundBase { }

    [Sound("Bus Sound", "Buses")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleBusSound : VehicleEngineSoundBase { }

    [Sound("Small Car Sound", "Small Cars")]
    [SoundVolume(1.5f, 1.5f)]
    public class VehicleSmallCarSound : VehicleEngineSoundBase { }

    [Sound("Scooter Sound", "Scooters")]
    [SoundVolume(1.5f, 1.5f)]
    public class VehicleScooterSound : VehicleEngineSoundBase { }

    [Sound("Metro Movement", "Metros")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleMetroSound : VehicleEngineSoundBase { }

    [Sound("Train Movement", "Trains")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleTrainSound : VehicleEngineSoundBase { }

    [Sound("Tram Sound", "Trams", RequiredDlc = DlcUtils.Dlc.SnowFall)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleTramSound : VehicleEngineSoundBase { }

    [Sound("Monorail Movement", "Monorails", RequiredDlc = DlcUtils.Dlc.InMotion)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleMonorailMovementSound : VehicleEngineSoundBase { }   

    [Sound("Cable Car Movement", "Cable Cars", RequiredDlc = DlcUtils.Dlc.InMotion)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleCableCarMovementSound : VehicleEngineSoundBase { }

    [Sound("Ship Sound", "Passenger Ships Sound")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehiclePassengerShipSound : VehicleEngineSoundBase { }

    [Sound("Cargo Ship Movement", "Cargo Ships Movement")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleCargoShipMovementSound : VehicleEngineSoundBase { }

    [Sound("Cargo Ship Sound", "Cargo Ships Sound")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleCargoShipSound : VehicleEngineSoundBase { }

    [Sound("Ferry Movement", "Ferries", RequiredDlc = DlcUtils.Dlc.InMotion)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleFerryMovementSound : VehicleEngineSoundBase { }

    [Sound("Ferry Sound", "Ferries", RequiredDlc = DlcUtils.Dlc.InMotion)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleFerrySound : VehicleEngineSoundBase { }

    [Sound("Boat Movement 01", "Fishing Boats Movement 1", RequiredDlc = DlcUtils.Dlc.SunsetHarbor)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleBoatMovementSound01 : VehicleEngineSoundBase { }

    [Sound("Boat Movement 02", "Fishing Boats Movement 2", RequiredDlc = DlcUtils.Dlc.SunsetHarbor)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleBoatMovementSound02 : VehicleEngineSoundBase { }

    [Sound("Boat Sound 01", "Fishing Boats Sound 1", RequiredDlc = DlcUtils.Dlc.SunsetHarbor)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleBoatSound01 : VehicleEngineSoundBase { }

    [Sound("Boat Sound 02", "Fishing Boats Sound 2", RequiredDlc = DlcUtils.Dlc.SunsetHarbor)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleBoatSound02 : VehicleEngineSoundBase { }

    [SoundCategory("Vehicle", "Vehicles", "Sirens")]
    public abstract class VehicleSirenSoundBase : VehicleSoundBase { }

    [Sound("Ambulance Siren", "Ambulances")]
    public class VehicleAmbulanceSirenSound : VehicleSirenSoundBase { }

    [Sound("Fire Truck Siren", "Fire Trucks")]
    [SoundVolume(3, 3)]
    public class VehicleFireTruckSirenSound : VehicleSirenSoundBase { }

    [Sound("Police Car Siren", "Police Cars")]
    public class VehiclePoliceCarSirenSound : VehicleSirenSoundBase { }

    [SoundCategory("Vehicle", "Vehicles", "Miscellaneous")]
    public abstract class VehicleMiscellaneousSoundBase : VehicleSoundBase { }

    [Sound("Bus Arrive", "Bus Arrivals")]
    public class VehicleBusArrivalsSound : VehicleMiscellaneousSoundBase { }

    [Sound("Metro Arrive", "Metro Arrivals")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleMetroArrivalsSound : VehicleEngineSoundBase { }

    [Sound("Train Arrive", "Train Arrivals")]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleTrainArrivalsSound : VehicleEngineSoundBase { }

    [Sound("Tram Arrive", "Tram Arrivals", RequiredDlc = DlcUtils.Dlc.SnowFall)]
    public class VehicleTramArrivalsSound : VehicleMiscellaneousSoundBase { }

    [Sound("Monorail Arrive", "Monorail Arrivals", RequiredDlc = DlcUtils.Dlc.InMotion)]
    [SoundVolume(DefaultVolume = 0.5f)]
    public class VehicleMonorailArrivalsSound : VehicleEngineSoundBase { }   



    #endregion

    #region Misc

    public abstract class MiscSoundBase : SoundBase { }


    [SoundCategory("Misc", "Misc", "Bulldozer")]
    public abstract class BulldozerSoundBase : MiscSoundBase { }

    [Sound("Building Bulldoze Sound", "Buildings")]
    public class BulldozerBuildingSound : BulldozerSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetSubEffectFromMultiEffect(BuildingManager.instance.m_properties.m_bulldozeEffect as MultiEffect, this.Id));
            return null;
        }
    }

    [Sound("Prop Bulldoze Sound", "Props")]
    public class BulldozerPropSound : BulldozerSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetSubEffectFromMultiEffect(PropManager.instance.m_properties.m_bulldozeEffect as MultiEffect, this.Id));
            return null;
        }
    }

    [Sound("Road Bulldoze Sound", "Roads")]
    public class BulldozerRoadSound : BulldozerSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetSubEffectFromMultiEffect(NetManager.instance.m_properties.m_bulldozeEffect as MultiEffect, this.Id));
            return null;
        }
    }


    [SoundCategory("Misc", "Misc", "Placements")]
    public abstract class PlacementSoundBase : MiscSoundBase { }

    [Sound("Building Placement Sound", "Buildings")]
    public class PlacementBuildingSound : PlacementSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetSubEffectFromMultiEffect(BuildingManager.instance.m_properties.m_placementEffect as MultiEffect, this.Id));
            return null;
        }
    }

    [Sound("Prop Placement Sound", "Props")]
    public class PlacementPropSound : PlacementSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetSubEffectFromMultiEffect(PropManager.instance.m_properties.m_placementEffect as MultiEffect, this.Id));
            return null;
        }
    }

    [Sound("Road Placement Sound", "Roads")]
    public class PlacementRoadSound : PlacementSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(SoundManager.GetSubEffectFromMultiEffect(NetManager.instance.m_properties.m_placementEffect as MultiEffect, this.Id));
            return null;
        }
    }


    [SoundCategory("Misc", "Misc", "User Interface")]
    public abstract class UserInterfaceSoundBase : MiscSoundBase { }

    [Sound("UI Clicks", "Clicks", IngameOnly = false)]
    public class UserInterfaceClickSound : UserInterfaceSoundBase
    {
        public override SoundContainer GetSoundInstance() { return null; }

        public override void BackUpSound() { }

        public override void BackUpVolume() { }

        public override void PatchSound(SoundPacksFileV1.Audio newSound) { }

        public override void PatchVolume(float volume)
        {
            if (!this.OldVolume.HasValue)
                this.OldVolume = 1;
            CustomPlayClickSound.UIClickSoundVolume = volume;
        }
    }

    [Sound("UI Clicks (Disabled)", "Clicks (disabled components)", IngameOnly = false)]
    public class UserInterfaceClickDisabledSound : UserInterfaceSoundBase
    {
        public override SoundContainer GetSoundInstance() { return null; }

        public override void BackUpSound() { }

        public override void BackUpVolume() { }

        public override void PatchSound(SoundPacksFileV1.Audio newSound) { }

        public override void PatchVolume(float volume)
        {
            if (!this.OldVolume.HasValue)
                this.OldVolume = 1;
            CustomPlayClickSound.DisabledUIClickSoundVolume = volume;
        }
    }

    [Sound("Road Draw Sound", "Road Drawer")]
    public class UserInterfaceRoadDrawerSound : UserInterfaceSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(NetManager.instance.m_properties.m_drawSound);
            return null;
        }
    }

    [Sound("Zone Fill Sound", "Zone Filler")]
    public class UserInterfaceZoneFillerSound : UserInterfaceSoundBase
    {
        public override SoundContainer GetSoundInstance()
        {
            if (BuildingManager.instance.m_properties != null)
                return new SoundContainer(ZoneManager.instance.m_properties.m_fillEffect as SoundEffect);
            return null;
        }
    }


    #endregion

}
