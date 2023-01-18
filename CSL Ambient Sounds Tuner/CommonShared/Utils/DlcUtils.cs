using System;

namespace AmbientSoundsTuner2.CommonShared.Utils
{
    /// <summary>
    /// Contains various utilities regarding DLCs.
    /// </summary>
    public static class DlcUtils
    {
        /// <summary>
        /// An enumerator that contains all available DLCs.
        /// </summary>
        [Flags]
        public enum Dlc
        {
            None = 0,
            Deluxe = 1,
            AfterDark = 2,
            SnowFall = 3,
            NaturalDisasters = 4,
            InMotion = 5,
            GreenCities = 6,
            Parks = 7,
            Industry = 8,
            Campus = 9,
            SunsetHarbor = 10,
            Airport = 11,
            PlazasAndPromenades = 12,
            FinancialDistricts = 13,
            Football = 14,
            Football2345 = 15,
            MusicFestival = 16,
            Christmas = 17
        }

        /// <summary>
        /// Gets the installed DLCs.
        /// </summary>
        /// <value>
        /// The installed DLCs.
        /// </value>
        public static Dlc InstalledDlcs
        {
            get
            {
                Dlc dlcs = Dlc.None;
                if (IsDeluxeInstalled) dlcs |= Dlc.Deluxe;
                if (IsAfterDarkInstalled) dlcs |= Dlc.AfterDark;
                if (IsSnowfallInstalled) dlcs |= Dlc.SnowFall;
                if (IsNaturalDisastersInstalled) dlcs |= Dlc.NaturalDisasters;
                if (IsInMotionInstalled) dlcs |= Dlc.InMotion;
                if (IsGreenCitiesInstalled) dlcs |= Dlc.GreenCities;
                if (IsParksInstalled) dlcs |= Dlc.Parks;
                if (IsIndustryInstalled) dlcs |= Dlc.Industry;
                if (IsCampusInstalled) dlcs |= Dlc.Campus;
                if (IsSunsetHarbourInstalled) dlcs |= Dlc.SunsetHarbor;
                if (IsAirportInstalled) dlcs |= Dlc.Airport;
                if (IsPlazasAndPromenadesInstalled) dlcs |= Dlc.PlazasAndPromenades;
                if (IsFinancialDistrictsInstalled) dlcs |= Dlc.FinancialDistricts;
                if (IsFootballInstalled) dlcs |= Dlc.Football;
                if (IsFootball2345Installed) dlcs |= Dlc.Football2345;
                if (IsMusicFestivalInstalled) dlcs |= Dlc.MusicFestival;
                if (IsChristmasInstalled) dlcs |= Dlc.Christmas;
                return dlcs;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a DLC is installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if a DLC is installed; otherwise, <c>false</c>.
        /// </value>
        /// 

        public static bool IsDeluxeInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.DeluxeDLC);

        public static bool IsAfterDarkInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.AfterDarkDLC);

        public static bool IsSnowfallInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.SnowFallDLC);

        public static bool IsNaturalDisastersInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.NaturalDisastersDLC);

        public static bool IsInMotionInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.InMotionDLC);

        public static bool IsGreenCitiesInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.GreenCitiesDLC);

        public static bool IsParksInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.ParksDLC);

        public static bool IsIndustryInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.IndustryDLC);

        public static bool IsCampusInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.CampusDLC);

        public static bool IsSunsetHarbourInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.UrbanDLC);

        public static bool IsAirportInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.AirportDLC);

        public static bool IsPlazasAndPromenadesInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.PlazasAndPromenadesDLC);

        public static bool IsFinancialDistrictsInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.FinancialDistrictsDLC);

        public static bool IsFootballInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.Football);

        public static bool IsFootball2345Installed => SteamHelper.IsDLCOwned(SteamHelper.DLC.Football2345);

        public static bool IsMusicFestivalInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.MusicFestival);

        public static bool IsChristmasInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.Christmas);

    }
}
