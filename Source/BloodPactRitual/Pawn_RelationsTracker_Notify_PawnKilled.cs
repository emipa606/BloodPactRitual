using HarmonyLib;
using RimWorld;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual;

[HarmonyPatch(typeof(Pawn_RelationsTracker), "Notify_PawnKilled")]
internal static class Pawn_RelationsTracker_Notify_PawnKilled
{
    // Kills the other pawn in a pact on death

    public static void Postfix(DamageInfo? dinfo, Pawn ___pawn)
    {
        DirectPawnRelationPact.OnPawnKilled(___pawn, dinfo);
    }
}