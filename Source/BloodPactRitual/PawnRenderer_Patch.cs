using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual;

internal static class PawnRenderer_Patch
{
    public static void DrawEquipment_PostFix(Pawn pawn)
    {
        var pactRelation = DirectPawnRelationPact.GetPactRelation(pawn);
        if (pactRelation == null)
        {
            return;
        }

        PactShieldBubble.DrawWornExtras(pawn, pactRelation);
    }
}