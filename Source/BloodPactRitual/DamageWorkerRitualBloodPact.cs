using System.Linq;
using Blood_Pact_Ritual.BloodPactRitual.DefOf;
using RimWorld;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual;

public class DamageWorkerRitualBloodPact : DamageWorker
{
    private const int PactDamage = 1;

    public override DamageResult Apply(DamageInfo dinfo, Thing thing)
    {
        var res = BloodPactRitual.TryBloodPact(thing as Pawn, dinfo.Instigator as Pawn);

        // if nothing special happened, we do normal damages
        return res ? ApplyPactDamage(dinfo, thing) : ApplyNormalDamage(dinfo, thing);
    }

    private static DamageResult ApplyNormalDamage(DamageInfo dinfo, Thing thing)
    {
        var normalCut = new DamageInfo(DamageDefOf.Cut, dinfo.Amount, 0, dinfo.Angle, dinfo.Instigator,
            dinfo.HitPart, dinfo.Weapon, dinfo.Category);
        return new DamageWorker_AddInjury().Apply(normalCut, thing);
    }

    private static DamageResult ApplyPactDamage(DamageInfo dinfo, Thing thing)
    {
        // Trying to make them hurt themselves to symbolise the making of the pact
        var cutSelf = new DamageInfo(BloodPactDamageDefOf.RitualBloodPactSymbolicInjury, PactDamage, -1, 0, thing,
            null,
            dinfo.Weapon, dinfo.Category);
        var cutSelfInstigator = new DamageInfo(BloodPactDamageDefOf.RitualBloodPactSymbolicInjury, PactDamage, -1,
            0, dinfo.Instigator,
            null, dinfo.Weapon, dinfo.Category);

        // they hit a specific part of the body, if they still have it
        SetHitPart(thing as Pawn, ref cutSelf);
        SetHitPart(dinfo.Instigator as Pawn, ref cutSelfInstigator);

        dinfo.Instigator.TakeDamage(cutSelfInstigator);
        return new DamageWorker_AddInjury().Apply(cutSelf, thing);
    }

    private static void SetHitPart(Pawn pawn, ref DamageInfo dinfo)
    {
        if (pawn == null)
        {
            return;
        }

        var partRecords = pawn.def.race.body.AllParts.FindAll(x => x.def.defName.EndsWith("Hand"));

        // the first part that the pawn hasn't lost is used
        // if we don't find one, we'll let the engine hit what it wants
        foreach (var record in partRecords.InRandomOrder())
        {
            if (pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>().Any(x => x.Part == record))
            {
                continue;
            }

            dinfo.SetHitPart(record);
            dinfo.SetAllowDamagePropagation(false);
            break;
        }
    }
}