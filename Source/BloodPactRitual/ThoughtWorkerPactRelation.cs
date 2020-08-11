using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual
{
    public class ThoughtWorkerPactRelation : ThoughtWorker
    {
        private const int Like = 50;
        private const int Disike = -50;

        private enum RelationStages
        {
            MutualTrust = 0,
            Like,
            Neutral,
            LikeVsDislike,
            DislikeVsLike,
            Unsafe,
            Dislike,
            MutualHate
        }

        // dislike, neutral, like
        private static readonly RelationStages[][] Stages = {
            new[] {RelationStages.MutualHate, RelationStages.Dislike, RelationStages.DislikeVsLike},
            new[] {RelationStages.Unsafe, RelationStages.Neutral, RelationStages.Neutral},
            new[] {RelationStages.LikeVsDislike, RelationStages.Like, RelationStages.MutualTrust}
        };

        private static int GetOpinionIndex(int opinion)
        {
            return opinion >= Like ? 2 : opinion <= Disike ? 0 : 1;
        }
        
        private static RelationStages GetRelationStage(int myOpinion, int theirOpinion)
        {
            return Stages[GetOpinionIndex(myOpinion)][GetOpinionIndex(theirOpinion)];
        }
        
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            // if we don't exist, no thought !
            if (!p.Spawned && !p.IsCaravanMember())
                return ThoughtState.Inactive;

            var bonded = DirectPawnRelationPact.GetPactPawn(p);
            if (bonded == null || DirectPawnRelationPact.IsAnimalPact(p, bonded))
                return ThoughtState.Inactive;
            
            // we check if they like each other
            var opinion = p.relations.OpinionOf(bonded);
            var opinionOther = bonded.relations.OpinionOf(p);
            
            var stage = (int) GetRelationStage(opinion, opinionOther);            
            return ThoughtState.ActiveAtStage(stage);
        }
    }
}