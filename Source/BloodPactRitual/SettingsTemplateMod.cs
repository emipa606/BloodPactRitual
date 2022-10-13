using Mlie;
using UnityEngine;
using Verse;

namespace BloodPactRitual;

[StaticConstructorOnStartup]
internal class BloodPactRitualMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static BloodPactRitualMod instance;

    private static string currentVersion;

    /// <summary>
    ///     The private settings
    /// </summary>
    private BloodPactRitualSettings settings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public BloodPactRitualMod(ModContentPack content) : base(content)
    {
        instance = this;
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(ModLister.GetActiveModWithIdentifier("Mlie.BloodPactRitual"));
    }

    /// <summary>
    ///     The instance-settings for the mod
    /// </summary>
    internal BloodPactRitualSettings Settings
    {
        get
        {
            if (settings == null)
            {
                settings = GetSettings<BloodPactRitualSettings>();
            }

            return settings;
        }
        set => settings = value;
    }

    /// <summary>
    ///     The title for the mod-settings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "BloodPactRitual";
    }

    /// <summary>
    ///     The settings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect);
        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("BloodPact_Setting_Revolt_Title".Translate(), ref Settings.AllowRevolt,
            "BloodPact_Setting_Revolt_Desc".Translate());
        listing_Standard.CheckboxLabeled("BloodPact_Setting_Animal_Title".Translate(), ref Settings.AllowAnimal,
            "BloodPact_Setting_Animal_Desc".Translate());
        listing_Standard.CheckboxLabeled("BloodPact_Setting_Prisoner_Title".Translate(), ref Settings.AllowPrisoner,
            "BloodPact_Setting_Prisoner_Desc".Translate());
        listing_Standard.CheckboxLabeled("BloodPact_Setting_Tendable_Title".Translate(),
            ref Settings.SharedDamageNeedsTending, "BloodPact_Setting_Tendable_Desc".Translate());
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("BloodPact_Setting_CurrentModVersion_Label".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
        Settings.Write();
    }
}