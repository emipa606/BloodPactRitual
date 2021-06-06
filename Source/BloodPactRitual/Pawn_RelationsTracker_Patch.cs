using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual
{
    internal static class Pawn_RelationsTracker_Patch
    {
        private const int TickRare = 60;
        private static readonly FieldInfo _getPawn = AccessTools.Field(typeof(Pawn_RelationsTracker), "pawn");

        // Checks if pacts are valids
        [UsedImplicitly]
        public static void RelationsTrackerTick_PostFix(Pawn_RelationsTracker __instance)
        {
            var pawn = (Pawn) _getPawn.GetValue(__instance);
            if (pawn.IsHashIntervalTick(TickRare))
            {
                DirectPawnRelationPact.CheckExistingPactValidity(pawn);
            }
        }

        // Kills the other pawn in a pact on death
        [UsedImplicitly]
        public static void Notify_PawnKilled_PostFix(Pawn_RelationsTracker __instance, DamageInfo? dinfo)
        {
            var pawn = (Pawn) _getPawn.GetValue(__instance);
            DirectPawnRelationPact.OnPawnKilled(pawn, dinfo);
        }
    }
}