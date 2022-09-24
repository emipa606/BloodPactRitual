using JetBrains.Annotations;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual;

internal static class DamageWorker_AddInjury_Patch
{
    [UsedImplicitly]
    public static void FinalizeAndAddInjury_Prefix(Pawn pawn, Hediff_Injury injury, ref DamageInfo dinfo,
        DamageWorker.DamageResult result)
    {
        DamageShare.TryShareDamage(pawn, injury, ref dinfo);
    }
}