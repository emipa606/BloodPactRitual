using HarmonyLib;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual;

[HarmonyPatch(typeof(PawnRenderUtility), nameof(PawnRenderUtility.DrawEquipmentAndApparelExtras))]
internal static class PawnRenderUtility_DrawEquipmentAndApparelExtras
{
    public static void Postfix(Pawn pawn)
    {
        var pactRelation = DirectPawnRelationPact.GetPactRelation(pawn);
        if (pactRelation == null)
        {
            return;
        }

        PactShieldBubble.DrawWornExtras(pawn, pactRelation);
    }
}