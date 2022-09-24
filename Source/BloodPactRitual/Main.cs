using System.IO;
using System.Xml.Linq;
using BloodPactRitual;
using HarmonyLib;
using RimWorld;
using Verse;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeModifiers

namespace Blood_Pact_Ritual.BloodPactRitual;

[StaticConstructorOnStartup]
class Main
{
    static Main()
    {
        var harmony = new Harmony("stoh.rimworld.bloodPactRitual");
        //Log.Message("BloodPactRitual: patching FinalizeAndAddInjury");
        harmony.Patch(
            AccessTools.Method(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury", new[]
            {
                typeof(Pawn), typeof(Hediff_Injury), typeof(DamageInfo), typeof(DamageWorker.DamageResult)
            }),
            new HarmonyMethod(typeof(DamageWorker_AddInjury_Patch).GetMethod("FinalizeAndAddInjury_Prefix")));
        //Log.Message("BloodPactRitual: patching DrawEquipment");
        harmony.Patch(
            AccessTools.Method(typeof(PawnRenderer), "DrawEquipment"),
            null,
            new HarmonyMethod(typeof(PawnRenderer_Patch).GetMethod("DrawEquipment_PostFix")));
        //Log.Message("BloodPactRitual: patching RelationsTrackerTick");
        harmony.Patch(
            AccessTools.Method(typeof(Pawn_RelationsTracker), "RelationsTrackerTick"),
            null,
            new HarmonyMethod(typeof(Pawn_RelationsTracker_Patch).GetMethod("RelationsTrackerTick_PostFix")));
        //Log.Message("BloodPactRitual: patching Notify_PawnKilled");
        harmony.Patch(
            AccessTools.Method(typeof(Pawn_RelationsTracker), "Notify_PawnKilled"),
            null,
            new HarmonyMethod(typeof(Pawn_RelationsTracker_Patch).GetMethod("Notify_PawnKilled_PostFix")));

        var hugsLibConfig = Path.Combine(GenFilePaths.SaveDataFolderPath, Path.Combine("HugsLib", "ModSettings.xml"));
        if (!new FileInfo(hugsLibConfig).Exists)
        {
            return;
        }

        var xml = XDocument.Load(hugsLibConfig);

        var modSettings = xml.Root?.Element("BloodPactRitual");
        if (modSettings == null)
        {
            return;
        }

        foreach (var modSetting in modSettings.Elements())
        {
            if (modSetting.Name == "allowRevolt")
            {
                BloodPactRitualMod.instance.Settings.AllowRevolt = bool.Parse(modSetting.Value);
            }

            if (modSetting.Name == "allowAnimal")
            {
                BloodPactRitualMod.instance.Settings.AllowAnimal = bool.Parse(modSetting.Value);
            }

            if (modSetting.Name == "allowPrisoner")
            {
                BloodPactRitualMod.instance.Settings.AllowPrisoner = bool.Parse(modSetting.Value);
            }

            if (modSetting.Name == "sharedDamageNeedsTending")
            {
                BloodPactRitualMod.instance.Settings.SharedDamageNeedsTending = bool.Parse(modSetting.Value);
            }
        }

        xml.Root.Element("BloodPactRitual")?.Remove();
        xml.Save(hugsLibConfig);

        Log.Message("[BloodPactRitual]: Imported old HugLib-settings");
    }
}

// Share damages only on body parts    

// Draw Shields on taking damage