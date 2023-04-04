using RimWorld;

namespace ATReforged
{
    [DefOf]
    public static class ATR_StatDefOf
    {
        static ATR_StatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ATR_StatDefOf));
        }

        public static StatDef ATR_SurrogateLimitBonus;
    }
}