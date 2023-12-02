using RimWorld;
using Verse;

namespace ATReforged
{
    // This HediffComp will modify the provided Need on a pawn based on the severity of this Hediff.
    public class HediffComp_SeverityAffectsNeed : HediffComp
    {
        private Need needCached;

        public HediffCompProperties_SeverityAffectsNeed Props => (HediffCompProperties_SeverityAffectsNeed)props;

        private Need Need
        {
            get
            {
                if (needCached == null)
                {
                    needCached = Pawn.needs.TryGetNeed(Props.needDef);
                }
                return needCached;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Need != null)
            {
                if (Props.changeByPercent)
                {
                    Need.CurLevelPercentage += Props.changePerDayAtOneSeverity * parent.Severity / GenDate.TicksPerDay;
                }
                else
                {
                    Need.CurLevel += Props.changePerDayAtOneSeverity * parent.Severity / GenDate.TicksPerDay;
                }
            }
        }
    }
}
