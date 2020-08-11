using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeModifiers

namespace Blood_Pact_Ritual.BloodPactRitual
{
    [StaticConstructorOnStartup]
    [UsedImplicitly]
    class Main
    {
        static Main()
        {
            var harmony = new Harmony("stoh.rimworld.bloodPactRitual");
            //Log.Message("BloodPactRitual: patching FinalizeAndAddInjury");
            harmony.Patch(
                AccessTools.Method(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury", new[]
                                                                                           {
                                                                                               typeof(Pawn), typeof(Hediff_Injury), typeof(DamageInfo), typeof(DamageWorker.DamageResult)
                                                                                           }), 
                new HarmonyMethod(typeof(DamageWorker_AddInjury_Patch).GetMethod("FinalizeAndAddInjury_Prefix")), 
                null);
            //Log.Message("BloodPactRitual: patching DrawEquipment");
            harmony.Patch(
                AccessTools.Method(typeof(PawnRenderer), "DrawEquipment"), 
                null,
                new HarmonyMethod(typeof(PawnRenderer_Patch).GetMethod("DrawEquipment_PostFix")));
            //Log.Message("BloodPactRitual: patching RelationsTrackerTick");
            harmony.Patch(
                AccessTools.Method(typeof(Pawn_RelationsTracker), "RelationsTrackerTick"), 
                null,
                new HarmonyMethod(typeof(Pawn_RelationsTracker_Patch).GetMethod("RelationsTrackerTick_PostFix")));
            //Log.Message("BloodPactRitual: patching Notify_PawnKilled");
            harmony.Patch(
                AccessTools.Method(typeof(Pawn_RelationsTracker), "Notify_PawnKilled"),
                null,
                new HarmonyMethod(typeof(Pawn_RelationsTracker_Patch).GetMethod("Notify_PawnKilled_PostFix")));
        }
    }

    // Share damages only on body parts    
    static class DamageWorker_AddInjury_Patch
    {
        [UsedImplicitly]
        public static void FinalizeAndAddInjury_Prefix(Pawn pawn, Hediff_Injury injury, ref DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            DamageShare.TryShareDamage(pawn, injury, ref dinfo);
        }
    }

    // Draw Shields on taking damage
    static class PawnRenderer_Patch
    {
        private static readonly FieldInfo _getPawn = AccessTools.Field(typeof(PawnRenderer), "pawn");

        [UsedImplicitly]
        public static void DrawEquipment_PostFix(PawnRenderer __instance)
        {
            var pawn = (Pawn)_getPawn.GetValue(__instance);
            var pactRelation = DirectPawnRelationPact.GetPactRelation(pawn);
            if (pactRelation == null) return;
            
            PactShieldBubble.DrawWornExtras(pawn, pactRelation);
        }
    }
    
    static class Pawn_RelationsTracker_Patch
    {        
        private static readonly FieldInfo _getPawn = AccessTools.Field(typeof(Pawn_RelationsTracker), "pawn");
        private const int TickRare = 60;
        
        // Checks if pacts are valids
        [UsedImplicitly]
        public static void RelationsTrackerTick_PostFix(Pawn_RelationsTracker __instance)
        {            
            var pawn = (Pawn) _getPawn.GetValue(__instance);
            if (pawn.IsHashIntervalTick(TickRare))
                DirectPawnRelationPact.CheckExistingPactValidity(pawn);
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