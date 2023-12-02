using RimWorld;
using Verse;

namespace ATReforged
{
    public class HediffCompProperties_SeverityAffectsNeed : HediffCompProperties
    {
        public NeedDef needDef;

        public float changePerDayAtOneSeverity = 1f;

        public bool changeByPercent = false;

        public HediffCompProperties_SeverityAffectsNeed()
        {
            compClass = typeof(HediffComp_SeverityAffectsNeed);
        }
    }
}
