using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual;

public class ThoughtWorkerPactDanger : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.Spawned && !p.IsCaravanMember())
        {
            return ThoughtState.Inactive;
        }

        var bonded = DirectPawnRelationPact.GetPactPawn(p);
        if (bonded == null)
        {
            return ThoughtState.Inactive;
        }

        // if the other is prisoner 
        if (IsPrisonerOfEnemyFaction(p, bonded))
        {
            return ThoughtState.ActiveAtStage((int)DangerStages.Prisoner);
        }

        // if the other is faraway
        if (IsFaraway(p, bonded))
        {
            return ThoughtState.ActiveAtStage((int)DangerStages.Faraway);
        }

        return ThoughtState.Inactive;
    }

    private static bool IsFaraway(Pawn p, Pawn bonded)
    {
        // if one is spawned and the other isn't, they're faraway
        if (p.Spawned != bonded.Spawned)
        {
            return true;
        }

        if (p.Spawned)
        {
            return p.MapHeld != bonded.MapHeld;
        }

        // if they're not spawned, either they're in the same caravan or they're farawy
        var pawnCaravan = p.GetCaravan();
        var bondedCaravan = bonded.GetCaravan();
        return pawnCaravan != bondedCaravan;

        // if they're spawned, we check they're in the same map
    }

    private static bool IsPrisonerOfEnemyFaction(Pawn pawn, Pawn bonded)
    {
        // We gotta check if he's prisoner of any faction
        // but we check first that he's not spawned or caravan member, to avoid checking all factions for no reason
        if (bonded.Spawned || bonded.IsCaravanMember())
        {
            return false;
        }

        return Find.FactionManager.AllFactionsListForReading
            .Any(f => f.kidnapped.KidnappedPawnsListForReading.Contains(pawn));
    }

    private enum DangerStages
    {
        Faraway = 0,
        Prisoner
    }
}