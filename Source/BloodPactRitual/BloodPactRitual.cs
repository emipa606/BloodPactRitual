using System.Collections.Generic;
using Blood_Pact_Ritual.BloodPactRitual.DefOf;
using RimWorld;
using UnityEngine;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual
{
    public class BloodPactRitual
    {
        private const float MinAcceptChance = 0.005f;
        private const float MaxAcceptChance = 1f;

        private const float NoBondAnimalAcceptChance = .8f;
        private const float BondedAnimalAcceptChance = 1f;
        private const float BondedAnimalWithOtherAcceptChance = .5f;
        private const float BaseInitiatorAcceptChance = 1f;
        private const float BaseInitiatorAcceptWhenPrisonerInvolvedChance = .8f;
        private const float BaseColonistRecipientAcceptChance = 1f;

        private const float
            PrisonerRecruitChanceFactor = 10f; // much more efficient than normal recruit ways.. but more risky too


        // ======== from InteractionWorker_RecruitAttempt
        private static readonly SimpleCurve AcceptChanceFactorCurveOpinion = new SimpleCurve
        {
            new CurvePoint(-50f, 0.0f),
            new CurvePoint(50f, 1f),
            new CurvePoint(100f, 2f)
        };

        private static readonly SimpleCurve AcceptChanceFactorCurveMood = new SimpleCurve
        {
            new CurvePoint(0.0f, 0.25f),
            new CurvePoint(0.1f, 0.25f),
            new CurvePoint(0.25f, 1f),
            new CurvePoint(0.5f, 1f),
            new CurvePoint(1f, 1.5f)
        };

        /// <summary>
        ///     Try to create a blood pact between recipient and initiator
        /// </summary>
        /// <param name="recipient">the pawn on the receiving end</param>
        /// <param name="initiator">the pawn who has been acting to create a blood pact</param>
        /// <returns>true if a blood pact was indeed created</returns>
        public static bool TryBloodPact(Pawn recipient, Pawn initiator)
        {
            // check if this is really a blood pact attempts
            if (!IsValidForInitiatingBloodPact(initiator) || !IsValidForReceivingBloodPact(recipient))
            {
                return false;
            }

            // we check they dont have any existing bond already
            if (DirectPawnRelationPact.GetPactRelation(recipient) != null ||
                DirectPawnRelationPact.GetPactRelation(initiator) != null)
            {
                return false;
            }

            // we get pawns reactions
            var initiatorReaction = GetPawnReaction(initiator, recipient, true);
            var recipientReaction = GetPawnReaction(recipient, initiator, false);

            // both have a chance to go mental
            var pactOk = !TryMentalBreak(initiator, initiatorReaction);
            pactOk = !TryMentalBreak(recipient, recipientReaction) && pactOk;

            // We try to create a blood pact
            pactOk = pactOk && CreatePact(recipient, initiator);

            // Some after effects
            PostPactCreationAttempt(recipient, initiator, recipientReaction, initiatorReaction, pactOk);

            return pactOk;
        }

        /// <returns>True if pawn can take an active part in a ritual, that is not downed or crazy</returns>
        private static bool CanPerformRitual(Pawn pawn)
        {
            // no message, it just didn't work (and is weird)
            if (pawn.Spawned)
            {
                return !pawn.Downed && !pawn.InMentalState;
            }

            Log.Message("Blood pact tentative involving not spawned pawn (" + pawn + ")");
            return false;

            // if we're down, or crazy, we can't do a ritual
        }
        ////////////////////////////////////////////////

        /// <summary>
        ///     Recruit chances based on InteractionWorker_RecruitAttempt#Interacted
        /// </summary>
        /// <returns></returns>
        private static float GetBasePrisonerRecruitChance(Pawn recipient, Pawn initiator, float factor = 1f)
        {
            // Ne concerne que les prisoniers humanoides
            if (!recipient.RaceProps.Humanlike)
            {
                return 0f;
            }

            return Mathf.Clamp(factor * initiator.GetStatValue(StatDefOf.NegotiationAbility)
                                      * (1f - recipient.RecruitDifficulty(initiator.Faction)), 0f, 1f);
        }

        private static float GetBaseAcceptChance(Pawn accepting, Pawn otherPawn, bool isInitiator)
        {
            // for animal, we only check if they got a bond
            if (!accepting.RaceProps.Humanlike)
            {
                // if they got a bond with initiator, it's all good                
                if (accepting.relations.DirectRelationExists(PawnRelationDefOf.Bond, otherPawn))
                {
                    return BondedAnimalAcceptChance;
                }

                // if they don't, but do have a bond with someone else => worst case scenario
                return accepting.relations.DirectRelations.Exists(x => x.def == PawnRelationDefOf.Bond)
                    ? BondedAnimalWithOtherAcceptChance
                    : NoBondAnimalAcceptChance;
            }

            if (isInitiator)
            {
                return IsRecruitablePrisoner(otherPawn)
                    ? BaseInitiatorAcceptWhenPrisonerInvolvedChance
                    : BaseInitiatorAcceptChance;
            }

            return IsRecruitablePrisoner(accepting)
                ? GetBasePrisonerRecruitChance(accepting, otherPawn, PrisonerRecruitChanceFactor)
                : BaseColonistRecipientAcceptChance;
        }

        private static float ApplyOpinionAndMoodFactor(Pawn accepting, Pawn otherPawn, float baseChance)
        {
            var value = baseChance;

            if (!accepting.RaceProps.Humanlike || !otherPawn.RaceProps.Humanlike)
            {
                return Mathf.Clamp(value, MinAcceptChance, MaxAcceptChance);
            }

            float opinion = accepting.relations.OpinionOf(otherPawn);
            value *= AcceptChanceFactorCurveOpinion.Evaluate(opinion);

            if (accepting.needs.mood == null)
            {
                return Mathf.Clamp(value, MinAcceptChance, MaxAcceptChance);
            }

            var curLevel = accepting.needs.mood.CurLevel;
            value *= AcceptChanceFactorCurveMood.Evaluate(curLevel);

            return Mathf.Clamp(value, MinAcceptChance, MaxAcceptChance);
        }

        private static PawnReaction GetPawnReaction(Pawn accepting, Pawn otherPawn, bool isInitiator)
        {
            var baseAcceptChance = GetBaseAcceptChance(accepting, otherPawn, isInitiator);
            var acceptChance = ApplyOpinionAndMoodFactor(accepting, otherPawn, baseAcceptChance);

            var prisoner = !isInitiator && IsRecruitablePrisoner(accepting);

            if (Rand.Value <= acceptChance)
            {
                return new PawnReaction(PawnReactionMood.Accept, acceptChance, prisoner);
            }

            if (!Settings.AllowRevolt)
            {
                return new PawnReaction(PawnReactionMood.Unhappy, acceptChance, prisoner);
            }

            // here the pawn refuses
            // but can they ?
            return accepting.Downed
                ? new PawnReaction(PawnReactionMood.MentalBreakButCant, acceptChance, prisoner)
                : new PawnReaction(PawnReactionMood.MentalBreak, acceptChance, prisoner);
        }


        /// <summary>
        ///     Recruit a prisoner, based on InteractionWorker_RecruitAttempt#Interacted
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="initiator"></param>
        private static void Recruit(Pawn recipient, Pawn initiator)
        {
            // we will use a custom message for recruit chance-> we give a dummy value, and dont send message
            InteractionWorker_RecruitAttempt.DoRecruit(initiator, recipient, 1, false);
            Find.PlayLog.Add(new PlayLogEntry_Interaction(InteractionDefOf.RecruitAttempt, initiator, recipient,
                new List<RulePackDef>
                {
                    RulePackDefOf.Sentence_RecruitAttemptAccepted
                }));
        }

        private static bool TryMentalBreak(Pawn pawn, PawnReaction reaction)
        {
            if (reaction.Mood < PawnReactionMood.MentalBreak)
            {
                return false;
            }

            // we try to get mad
            var res = pawn.mindState.mentalStateHandler.TryStartMentalState(
                MentalStateDefOf.Berserk,
                "BloodPact_Attempt_MindBreak".Translate(),
                true);

            if (!res)
            {
                reaction.Mood = PawnReactionMood.MentalBreakButCant;
            }
            else
            {
                MakeMoteBubble(pawn, BloodPactThoughtDefOf.BloodPactWentBerserk.Icon, false);
            }

            return res;
        }

        private static void MakeMoteBubble(Pawn pawn, Texture2D icon, bool good)
        {
            // The regular way doesn't seem to work so...
            // we force the bubble to appear
            var moteBubble =
                (MoteBubble) ThingMaker.MakeThing(good ? ThingDefOf.Mote_ThoughtGood : ThingDefOf.Mote_ThoughtBad);
            moteBubble.SetupMoteBubble(icon, null);
            moteBubble.Attach(pawn);
            GenSpawn.Spawn(moteBubble, pawn.Position, pawn.Map);
        }

        private static void MoodDebuff(Pawn pawn, PawnReactionMood reaction, Pawn otherPawn, bool pactWorked)
        {
            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }

            if (!pactWorked)
            {
                // It didn't happen, and pawn isn't the one who got mad about this
                if (reaction < PawnReactionMood.MentalBreakButCant)
                {
                    // still a bit mad about being rejected
                    pawn.needs.mood.thoughts.memories.TryGainMemory(BloodPactThoughtDefOf.BloodPactReject,
                        otherPawn);
                    return;
                }

                // Went berserk, which prevented blood pact from happening                
                pawn.needs.mood.thoughts.memories.TryGainMemory(BloodPactThoughtDefOf.BloodPactWentBerserk, otherPawn);
                return;
            }

            if (reaction < PawnReactionMood.Unhappy)
            {
                return;
            }

            // They get pretty sad about this
            // this overrides any existing "went berserk" bad though
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BloodPactThoughtDefOf.BloodPactWentBerserk);
            pawn.needs.mood.thoughts.memories.TryGainMemory(BloodPactThoughtDefOf.BloodPactForcedPact, otherPawn);
        }

        /// <summary>
        ///     Handle some consequences of a (sensible) pact creation attempt
        /// </summary>
        /// <param name="recipient">recipient of the blood pact</param>
        /// <param name="initiator">initiator of the blood pact</param>
        /// <param name="recipientReaction">recipient (pawn) reaction to the blood pact creation</param>
        /// <param name="initiatorReaction">initiator (pawn) reaction to the blood pact creation</param>
        /// <param name="success">true if the blood pact succeded. False most likely involves some kind of revolt</param>
        private static void PostPactCreationAttempt(Pawn recipient, Pawn initiator, PawnReaction recipientReaction,
            PawnReaction initiatorReaction, bool success)
        {
            var bestChances = Mathf.Clamp(recipientReaction.AcceptChance * initiatorReaction.AcceptChance, 0f, 1f);

            // time to create some mood debuff
            MoodDebuff(initiator, initiatorReaction.Mood, recipient, success);
            MoodDebuff(recipient, recipientReaction.Mood, initiator, success);

            if (success)
            {
                // in case of success we also...
                // ... destroy the dagger used
                var weapon = initiator.equipment.Primary;
                if (weapon != null && weapon.def == BloodPactWeaponDefOf.MeleeWeapon_BloodRitualKnife)
                {
                    initiator.equipment.DestroyEquipment(weapon);
                }

                // and give some feedback !
                MoteMaker.ThrowText((recipient.DrawPos + initiator.DrawPos) / 2f, recipient.Map,
                    "BloodPact_TextMote_NewPact".Translate(),
                    Color.red, 3.65f);

                // if pawns can revolt, we tell the player what the chances of success were
                var key = recipientReaction.Prisoner ? "BloodPact_RecruitByPact" : "BloodPact_NewPact";
                if (Settings.AllowRevolt)
                {
                    key += "WithChance";
                    Find.LetterStack.ReceiveLetter(
                        (key + "_Title").Translate(),
                        (key + "_Content").Translate(initiator.NameShortColored, recipient.NameShortColored,
                            bestChances.ToStringPercent()),
                        LetterDefOf.PositiveEvent,
                        initiator);
                }
                else
                {
                    Find.LetterStack.ReceiveLetter(
                        (key + "_Title").Translate(),
                        (key + "_Content").Translate(initiator.NameShortColored, recipient.NameShortColored),
                        LetterDefOf.PositiveEvent,
                        initiator);
                }

                // we make the pawn stop hitting each-other
                StopAttacking(recipient, initiator);
                StopAttacking(initiator, recipient);
            }
            else
            {
                // gives a feedback with success chances
                MoteMaker.ThrowText((recipient.DrawPos + initiator.DrawPos) / 2f, recipient.Map,
                    "BloodPact_TextMote_NewPactFailed".Translate(bestChances.ToStringPercent()),
                    Color.red, 3.65f);
            }
        }

        private static void StopAttacking(Pawn attacker, Pawn target)
        {
            var curJob = attacker.jobs?.curJob;
            if (curJob == null)
            {
                return;
            }

            if (curJob.def == JobDefOf.AttackMelee &&
                attacker.jobs.IsCurrentJobPlayerInterruptible() &&
                curJob.targetA == target)
            {
                attacker.jobs.StopAll();
            }
        }

        private static bool CreatePact(Pawn recipient, Pawn initiator)
        {
//            var bondedHediff = BloodPactHediffDefOf.RitualBloodPact;
//            var pawnHediff = HediffMaker.MakeHediff(bondedHediff, recipient) as   HediffRitualBloodPact;
//            var instigatorHediff = HediffMaker.MakeHediff(bondedHediff, initiator) as HediffRitualBloodPact;
//
//            if (pawnHediff == null || instigatorHediff == null)
//            {
//                Log.Error("Cant create hediff, blood pact will not be created (" + recipient + "," + initiator + ")");
//                return false;
//            }

            // if one of them is a prisoner, we need to recruit them now
            if (IsRecruitablePrisoner(recipient))
            {
                Recruit(recipient, initiator);
            }

            //            pawnHediff.Bonded = initiator;
//            instigatorHediff.Bonded = recipient;
//
//            // add hediff
//            recipient.health.AddHediff(pawnHediff);
//            initiator.health.AddHediff(instigatorHediff);

            // add relation
            recipient.relations.AddDirectRelation(BloodPactPawnRelationDefOf.PawnRelationBloodPact, initiator);
            return true;
        }

        /// <returns>true if pawn is still valid (generaly speaking) as a blood pact holder</returns>
        private static bool IsValidForKeepingBloodPact(Pawn pawn)
        {
            // only from player faction
            return pawn?.relations != null && pawn.Faction != null && pawn.Faction.IsPlayer;
        }

        /// <returns>true if pawn can be the initiator in a blood pact creation.</returns>
        private static bool IsValidForInitiatingBloodPact(Pawn pawn)
        {
            return IsValidForKeepingBloodPact(pawn) && CanPerformRitual(pawn) && pawn.RaceProps.Humanlike;
        }

        private static bool IsRecruitablePrisoner(Pawn prisoner)
        {
            return prisoner.RaceProps.Humanlike && prisoner.relations != null && prisoner.IsPrisonerOfColony
                   && prisoner.Faction != null && !prisoner.Faction.IsPlayer;
        }

        /// <returns>true if pawn can be on the receiving part of a blood pact creation</returns>
        private static bool IsValidForReceivingBloodPact(Pawn pawn)
        {
            // Special case of prisoners : they can get blood pact even though they're not part of the colony
            // because they'll recruit them
            if (!IsValidForKeepingBloodPact(pawn))
            {
                return Settings.AllowPrisoner && IsRecruitablePrisoner(pawn);
            }

            return !pawn.InMentalState
                   && (pawn.RaceProps.Humanlike || Settings.AllowAnimal);
        }

        private enum PawnReactionMood
        {
            Accept,
            Unhappy,
            MentalBreakButCant,
            MentalBreak
        }

        private struct PawnReaction
        {
            internal PawnReactionMood Mood;
            internal readonly float AcceptChance;
            internal readonly bool Prisoner;

            public PawnReaction(PawnReactionMood mood, float acceptChance, bool prisoner)
            {
                Mood = mood;
                AcceptChance = acceptChance;
                Prisoner = prisoner;
            }
        }
    }
}