using RimWorld;
using Verse;

namespace ATReforged
{
    // This HediffComp will modify the provided Need on a pawn based on the severity changes of this Hediff.
    public class HediffComp_SeverityChangeAffectsNeed : HediffComp
    {
        private Need needCached;

        private float lastSeverityTick;

        public HediffCompProperties_SeverityChangeAffectsNeed Props => (HediffCompProperties_SeverityChangeAffectsNeed)props;

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
                if (Props.changeOnIncrease && parent.Severity > lastSeverityTick)
                {
                    Need.CurLevelPercentage += Props.severityToChangeRatio * (parent.Severity - lastSeverityTick);
                }
                else if (!Props.changeOnIncrease && parent.Severity < lastSeverityTick)
                {
                    Need.CurLevelPercentage += Props.severityToChangeRatio * (lastSeverityTick - parent.Severity);
                }

            }
        }
    }
}
