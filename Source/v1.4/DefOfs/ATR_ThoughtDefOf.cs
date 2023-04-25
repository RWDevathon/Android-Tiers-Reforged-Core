using RimWorld;

namespace ATReforged
{
    [DefOf]
    public static class ATR_ThoughtDefOf
    {
        static ATR_ThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ATR_ThoughtDefOf));
        }

        public static ThoughtDef ATR_PersonalityShiftAllowed;

        public static ThoughtDef ATR_PersonalityShiftDenied;
    }
}
