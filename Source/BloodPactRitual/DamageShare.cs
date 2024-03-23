using System.Reflection;
using Blood_Pact_Ritual.BloodPactRitual.DefOf;
using BloodPactRitual;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace Blood_Pact_Ritual.BloodPactRitual;

internal class DamageShare
{
    // efficiency
    private const float MinEfficiencyTakenRatio = .1f;
    private const float MaxEfficiencyTakenRatio = .3f;
    private const float EfficiencyTakenRatioDelta = MaxEfficiencyTakenRatio - MinEfficiencyTakenRatio;
    private const float MinEfficiencyRemainingRatio = .9f;
    private const float MaxEfficiencyRemainingRatio = .3f;
    private const float EfficiencyRemainingRatioDelta = MaxEfficiencyRemainingRatio - MinEfficiencyRemainingRatio;
    private const int MinDamage = 1;

    private static readonly FieldInfo Severity = AccessTools.Field(typeof(Hediff_Injury), "severityInt");

    private static float GetTakenRatio(float efficiency)
    {
        return MinEfficiencyTakenRatio + (efficiency * EfficiencyTakenRatioDelta);
    }

    private static float GetRemainingRatio(float efficiency)
    {
        return MinEfficiencyRemainingRatio + (efficiency * EfficiencyRemainingRatioDelta);
    }

    public static void TryShareDamage(Pawn pawn, Hediff_Injury injury, ref DamageInfo dinfo)
    {
        // if the pawn's dead, we don't care
        if (pawn == null || pawn.Dead || pawn.Destroyed)
        {
            return;
        }

        // some kind of injuries we don't want to handle
        // also if damage is too low, we can't share it
        if (dinfo.InstantPermanentInjury || !dinfo.Def.harmsHealth || injury.Severity <= MinDamage)
        {
            return;
        }

        // if it's already reflected damage, our job is already done
        if (dinfo.Def == BloodPactDamageDefOf.RitualBloodPactSymbolicInjury ||
            dinfo.Def == BloodPactDamageDefOf.RitualBloodPactInjuryShared
            || dinfo.Def == BloodPactDamageDefOf.RitualBloodPactInjurySharedTendable)
        {
            return;
        }

        // if it's a mutilation ritual we do not want to share the damage (as it will kill both pawns if we do)
        if (pawn.GetLord()?.LordJob is LordJob_Ritual_Mutilation)
        {
            return;
        }

        // we check for a bond (with someone else than ourselves... even if it's not supposed to happen)
        var pactRelation = DirectPawnRelationPact.GetPactRelation(pawn);

        // we got a bond ! Just checking the other isn't already dead or anything
        var pactPawn = pactRelation?.otherPawn;
        if (pactPawn == null || pactPawn.Dead || pactPawn.Destroyed)
        {
            return;
        }

        var damageAmount = injury.Severity;
        var efficiency = pactRelation.Efficiency(pawn);

        // We give some feedback for shield effect
        pactRelation.LastShieldActive = Find.TickManager.TicksGame;

        // so we split damages
        var remainingDamage = Mathf.Max(MinDamage, Mathf.RoundToInt(GetRemainingRatio(efficiency) * damageAmount));
        var takenDamage = Mathf.RoundToInt(GetTakenRatio(efficiency) * damageAmount);

        // updating the injurie's damages, w/o firing an update
        // => we use direct access
        Severity.SetValue(injury, remainingDamage);

        // Here we're going to inflict the shared damages to the other pawn (if there's any)
        if (takenDamage <= 0)
        {
            return;
        }

        var dinfoReflected = new DamageInfo(
            // using a special damage type to avoid reflecting what's already reflected
            // + we chose if we want it te be tendable or not
            BloodPactRitualMod.instance.Settings.SharedDamageNeedsTending
                ? BloodPactDamageDefOf.RitualBloodPactInjurySharedTendable
                : BloodPactDamageDefOf.RitualBloodPactInjuryShared,
            takenDamage,
            dinfo.Angle,
            0,
            dinfo.Instigator,
            ComputeHitPart(pactPawn, injury.Part), // we try to hit the exact same part
            dinfo.Weapon,
            dinfo.Category
        );
        // no damage propagation
        // because the original target will share this with the other
        dinfoReflected.SetAllowDamagePropagation(false);
        pactPawn.TakeDamage(dinfoReflected);

        // Notifying the other pawn
        var otherPactRelation = DirectPawnRelationPact.GetPactRelation(pactPawn);
        if (otherPactRelation != null && otherPactRelation.otherPawn == pawn)
        {
            otherPactRelation.LastReceivedPactDamage = Find.TickManager.TicksGame;
        }
    }

    private static BodyPartRecord ComputeHitPart(Pawn pawn, BodyPartRecord initial)
    {
        // just in cas... but shouldn't be the case at this point of the damage process
        if (initial == null)
        {
            return null;
        }

        var bodyParts = pawn.def.race.body.AllParts;
        if (bodyParts.NullOrEmpty())
        {
            return null;
        }

        // trying to find the same body part
        // doing string equality, because both pawns might not be of the same race
        // , so they might not have the same body parts, even though they're the same kind of part 
        // (thus having the same name)
        var samePart = bodyParts.Find(x => x.def.defName.Equals(initial.def.defName));
        return samePart ?? bodyParts.RandomElement();

        // if not, we just use any body part
    }
}