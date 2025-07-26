using HarmonyLib;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual;

[HarmonyPatch(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury", typeof(Pawn), typeof(Hediff_Injury),
    typeof(DamageInfo), typeof(DamageWorker.DamageResult))]
internal static class DamageWorker_AddInjury_FinalizeAndAddInjury
{
    public static void Prefix(Pawn pawn, Hediff_Injury injury, ref DamageInfo dinfo,
        DamageWorker.DamageResult result)
    {
        DamageShare.TryShareDamage(pawn, injury, ref dinfo);
    }
}