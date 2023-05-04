using RimWorld;
using Verse;

namespace ATReforged
{
    public class HediffCompProperties_SeverityChangeAffectsNeed : HediffCompProperties
    {
        public NeedDef needDef;

        public float severityToChangeRatio = 1f;

        public bool changeOnIncrease = true;

        public HediffCompProperties_SeverityChangeAffectsNeed()
        {
            compClass = typeof(HediffComp_SeverityChangeAffectsNeed);
        }
    }
}
