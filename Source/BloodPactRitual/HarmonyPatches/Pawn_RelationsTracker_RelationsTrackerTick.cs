using HarmonyLib;
using RimWorld;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual;

[HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.RelationsTrackerTickInterval))]
internal static class Pawn_RelationsTracker_RelationsTrackerTick
{
    private const int TickRare = 60;

    // Checks if pacts are valids

    public static void Postfix(Pawn ___pawn)
    {
        if (___pawn.IsHashIntervalTick(TickRare))
        {
            DirectPawnRelationPact.CheckExistingPactValidity(___pawn);
        }
    }
}