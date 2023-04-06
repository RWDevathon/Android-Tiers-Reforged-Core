using RimWorld;

namespace ATReforged
{
    [DefOf]
    public static class ATR_BackstoryDefOf
    {
        static ATR_BackstoryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ATR_BackstoryDefOf));
        }

        public static BackstoryDef ATR_MechChildhoodFreshBlank;
        public static BackstoryDef ATR_MechAdulthoodBlank;

        public static BackstoryDef ATR_NewbootChildhood;
        public static BackstoryDef ATR_NewbootAdulthood;
    }
}