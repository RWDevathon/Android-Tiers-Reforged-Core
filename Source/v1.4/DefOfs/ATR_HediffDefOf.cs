using Verse;
using RimWorld;

namespace ATReforged
{
    [DefOf]
    public static class ATR_HediffDefOf
    {
        static ATR_HediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ATR_HediffDefOf));
        }

        public static HediffDef ATR_StasisPill;

        public static HediffDef ATR_RemainingCharge;

        public static HediffDef ATR_AutomodulatedVoiceSynthesizer;

        public static HediffDef ATR_CoerciveVoiceSynthesizer;

        public static HediffDef ATR_AutonomousCore;

        public static HediffDef ATR_IsolatedCore;

        // Maintenance Part failures

        public static HediffDef ATR_RustedPart;

        // Maintenance effects

        public static HediffDef ATR_MaintenanceCritical;

        public static HediffDef ATR_MaintenancePoor;

        public static HediffDef ATR_MaintenanceSatisfactory;

    }
}
