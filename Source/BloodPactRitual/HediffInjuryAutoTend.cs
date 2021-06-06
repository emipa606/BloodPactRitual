using System.Linq;
using System.Text;
using Verse;

namespace Blood_Pact_Ritual.BloodPactRitual
{
    public class HediffInjuryAutoTend : Hediff_Injury
    {
        public override TextureAndColor StateIcon =>
            // avoiding Tended Texture and Color
            TextureAndColor.None;

        public override string TipStringExtra
        {
            get
            {
                // the goal here is to avoid extra lines from HediffComp_TendDuration
                // From Hediff.TipStringExtra
                var stringBuilder = new StringBuilder();
                foreach (var specialDisplayStat in HediffStatsUtility.SpecialDisplayStats(CurStage, this))
                {
                    if (specialDisplayStat.ShouldDisplay)
                    {
                        stringBuilder.AppendLine(specialDisplayStat.LabelCap + ": " + specialDisplayStat.ValueString);
                    }
                }

                // From Hediff_Injury.TipStringExtra
                if (comps == null)
                {
                    return stringBuilder.ToString();
                }

                foreach (var comp in comps.Where(comp => !(comp is HediffComp_TendDuration)))
                {
                    var compTipStringExtra = comp.CompTipStringExtra;
                    if (!compTipStringExtra.NullOrEmpty())
                    {
                        stringBuilder.AppendLine(compTipStringExtra);
                    }
                }

                return stringBuilder.ToString();
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            Tended(
                1f, // best quality 
                1 // to avoid mote text (see Tended implementation) 
            );
        }
    }
}