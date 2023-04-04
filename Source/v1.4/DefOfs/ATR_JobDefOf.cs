using Verse;
using RimWorld;

namespace ATReforged
{
    [DefOf]
    public static class ATR_JobDefOf
    {
        static ATR_JobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ATR_JobDefOf));
        }

        public static JobDef ATR_ResurrectMechanical;

        public static JobDef ATR_GenerateInsight;
    }
}