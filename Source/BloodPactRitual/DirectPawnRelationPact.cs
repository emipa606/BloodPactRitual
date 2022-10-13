using Blood_Pact_Ritual.BloodPactRitual.DefOf;
using RimWorld;
using UnityEngine;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual;

internal class DirectPawnRelationPact : DirectPawnRelation
{
    // for animals
    private const float EfficiencyAnimal = 0f;
    private const float EfficiencyAnimalWithBond = .3f;

    private const int EfficiencyUpdateTick = 60;
    private float _lastEfficiency;
    private int _lastTickEfficiencyUpdate = -9999;

    private bool _notifiedDeath;
    public int LastReceivedPactDamage = -9999;

    // can't expose anything since we can't modify parent class. Theses are cached values 
    public int LastShieldActive = -9999;

    public DirectPawnRelationPact()
    {
    }

    private DirectPawnRelationPact(PawnRelationDef def, Pawn otherPawn, int startTicks) : base(def, otherPawn,
        startTicks)
    {
    }

    private DirectPawnRelationPact(DirectPawnRelation existing) : this(existing.def, existing.otherPawn,
        existing.startTicks)
    {
    }

    public float Efficiency(Pawn pawn)
    {
        // Update cached value if needed
        if (Find.TickManager.TicksGame <= _lastTickEfficiencyUpdate + EfficiencyUpdateTick)
        {
            return _lastEfficiency;
        }

        _lastEfficiency = ComputeEfficiency(pawn);
        _lastTickEfficiencyUpdate = Find.TickManager.TicksGame;

        return _lastEfficiency;
    }

    internal static bool IsAnimalPact(Pawn pawn, Pawn other)
    {
        return pawn != null && other != null && (!pawn.RaceProps.Humanlike || !other.RaceProps.Humanlike);
    }

    private float ComputeEfficiency(Pawn pawn)
    {
        if (IsAnimalPact(pawn, otherPawn))
        {
            return otherPawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, pawn)
                ? EfficiencyAnimalWithBond
                : EfficiencyAnimal;
        }

        // for humans, it depends on their opinions of each other
        var opinion = pawn.relations.OpinionOf(otherPawn);
        var opinionOther = otherPawn.relations.OpinionOf(pawn);
        return Mathf.Clamp01((200 + opinion + opinionOther) / 400f);
    }

    public static bool IsInvalid(Pawn pawn, DirectPawnRelation relation)
    {
        return relation.def == BloodPactPawnRelationDefOf.PawnRelationBloodPact &&
               (pawn == null || relation.otherPawn == null || pawn.Faction == null || !pawn.Faction.IsPlayer
                || relation.otherPawn.Faction == null || !relation.otherPawn.Faction.IsPlayer);
    }

    public static Pawn GetPactPawn(Pawn pawn)
    {
        var pactRelation = GetPactRelation(pawn);
        return pactRelation?.otherPawn;
    }

    public static DirectPawnRelationPact GetPactRelation(Pawn pawn)
    {
        var relation =
            pawn?.relations?.DirectRelations.Find(x => x.def == BloodPactPawnRelationDefOf.PawnRelationBloodPact);
        if (relation == null)
        {
            return null;
        }

        if (relation is DirectPawnRelationPact res)
        {
            return res;
        }

        // we have to create the specialized version
        res = new DirectPawnRelationPact(relation);

        pawn.relations.DirectRelations.Remove(relation);
        pawn.relations.DirectRelations.Add(res);

        return res;
    }

    public static void CheckExistingPactValidity(Pawn pawn)
    {
        // only one valid pact per pawn
        var first = true;
        foreach (var relation in pawn.relations.DirectRelations.FindAll(x =>
                     x.def == BloodPactPawnRelationDefOf.PawnRelationBloodPact))
        {
            if (!first || !IsValid(relation, pawn))
            {
                RemovePact(pawn, relation);
            }

            first = false;
        }
    }

    private static void RemovePact(Pawn pawn, DirectPawnRelation relation)
    {
        Log.Message("Invalid bond removed (" + pawn + ")");
        pawn.relations.RemoveDirectRelation(relation);

        if (pawn.Faction is { IsPlayer: true })
        {
            Find.LetterStack.ReceiveLetter(
                "BloodPact_Obsolete_Title".Translate(),
                "BloodPact_Obsolete_Content".Translate(pawn.NameShortColored),
                LetterDefOf.PositiveEvent,
                pawn);
        }
    }

    private static bool IsValid(DirectPawnRelation relation, Pawn pawn)
    {
        var otherPawn = relation.otherPawn;
        if (otherPawn?.Faction == null || !otherPawn.Faction.IsPlayer || otherPawn.relations == null)
        {
            return false;
        }

        // if the other's dead, the pact is valid -> we gonna get killed
        if (otherPawn.Dead)
        {
            return true;
        }

        // we check if the pact's mutual
        if (otherPawn.relations.GetDirectRelation(BloodPactPawnRelationDefOf.PawnRelationBloodPact, pawn) == null)
        {
            return false;
        }

        // if the pawn's destroyed, but not dead... we got a problem
        return !otherPawn.Destroyed;
    }

    public static void OnPawnKilled(Pawn killed, DamageInfo? dinfo)
    {
        var pactRelation = GetPactRelation(killed);
        if (pactRelation == null || pactRelation._notifiedDeath)
        {
            return;
        }

        // no loop plz
        pactRelation._notifiedDeath = true;

        // we gotta kill 
        if (IsValid(pactRelation, killed) && !pactRelation.otherPawn.Dead)
        {
            PactDeath(pactRelation.otherPawn, killed, dinfo);
        }
    }

    private static void PactDeath(Pawn toKill, Pawn killed, DamageInfo? dinfo)
    {
        // if toKill is already dead, we don't care
        if (toKill == null || toKill.Dead || toKill.Destroyed)
        {
            return;
        }

        // we check if toKill wasn't already trying to kill us
        // or didn't really have a pact with us
        var pactRelation = GetPactRelation(toKill);
        if (pactRelation == null || pactRelation._notifiedDeath)
        {
            return;
        }

        toKill.Kill(dinfo);

        Find.LetterStack.ReceiveLetter(
            "BloodPact_Death_Title".Translate(),
            "BloodPact_Death_Content".Translate(toKill.NameShortColored, killed.NameShortColored),
            LetterDefOf.NegativeEvent,
            toKill.Corpse);
    }
}