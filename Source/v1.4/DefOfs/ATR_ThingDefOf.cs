using Verse;
using RimWorld;


namespace ATReforged
{
    [DefOf]
    public static class ATR_ThingDefOf
    {
        static ATR_ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ATR_ThingDefOf));
        }

        public static ThingDef ATR_MindOperationAttachedMote;
    }

}