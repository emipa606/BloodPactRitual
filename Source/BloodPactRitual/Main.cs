using System.IO;
using System.Reflection;
using System.Xml.Linq;
using BloodPactRitual;
using HarmonyLib;
using Verse;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeModifiers

namespace Blood_Pact_Ritual.BloodPactRitual;

[StaticConstructorOnStartup]
static class Main
{
    static Main()
    {
        new Harmony("stoh.rimworld.bloodPactRitual").PatchAll(Assembly.GetExecutingAssembly());


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

        xml.Root?.Element("BloodPactRitual")?.Remove();
        xml.Save(hugsLibConfig);

        Log.Message("[BloodPactRitual]: Imported old HugLib-settings");
    }
}