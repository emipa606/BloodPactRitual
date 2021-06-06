using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual
{
    internal static class PawnRenderer_Patch
    {
        private static readonly FieldInfo _getPawn = AccessTools.Field(typeof(PawnRenderer), "pawn");

        [UsedImplicitly]
        public static void DrawEquipment_PostFix(PawnRenderer __instance)
        {
            var pawn = (Pawn) _getPawn.GetValue(__instance);
            var pactRelation = DirectPawnRelationPact.GetPactRelation(pawn);
            if (pactRelation == null)
            {
                return;
            }

            PactShieldBubble.DrawWornExtras(pawn, pactRelation);
        }
    }
}