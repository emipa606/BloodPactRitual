using HugsLib;
using HugsLib.Settings;
using Verse;

// ReSharper disable NotAccessedField.Local

namespace Blood_Pact_Ritual.BloodPactRitual
{
    public class Settings : ModBase
    {
        private static SettingHandle<bool> _allowAnimal;
        private static SettingHandle<bool> _allowRevolt;
        private static SettingHandle<bool> _allowPrisoner;
        private static SettingHandle<bool> _sharedDamageNeedsTending;

        public override string ModIdentifier => "BloodPactRitual";


        /// <summary>
        ///     True if animals can be involved in blood pacts
        /// </summary>
        public static bool AllowAnimal => _allowAnimal != null && _allowAnimal;

        /// <summary>
        ///     True if pawns can revolt against a blood pact creation
        /// </summary>
        public static bool AllowRevolt => _allowRevolt != null && _allowRevolt;

        /// <summary>
        ///     True if prisoners can be forced into your colony using a blood pact
        /// </summary>
        public static bool AllowPrisoner => _allowPrisoner != null && _allowPrisoner;

        /// <summary>
        ///     True if shared damage from blood pact needs tending
        /// </summary>
        public static bool SharedDamageNeedsTending => _sharedDamageNeedsTending != null && _sharedDamageNeedsTending;

        public override void DefsLoaded()
        {
            _allowRevolt = Settings.GetHandle("allowRevolt", "BloodPact_Setting_Revolt_Title".Translate(),
                "BloodPact_Setting_Revolt_Desc".Translate(), true);
            _allowAnimal = Settings.GetHandle("allowAnimal", "BloodPact_Setting_Animal_Title".Translate(),
                "BloodPact_Setting_Animal_Desc".Translate(), true);
            _allowPrisoner = Settings.GetHandle("allowPrisoner", "BloodPact_Setting_Prisoner_Title".Translate(),
                "BloodPact_Setting_Prisoner_Desc".Translate(), true);
            _sharedDamageNeedsTending = Settings.GetHandle("sharedDamageNeedsTending",
                "BloodPact_Setting_Tendable_Title".Translate(),
                "BloodPact_Setting_Tendable_Desc".Translate(), false);
        }
    }
}