using Verse;
using HarmonyLib;

namespace ATReforged
{
    public class ATRC_Pawn_HealthTracker_Patch
    {
        // Upon a pawn being downed, some pawns should die automatically.
        [HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
        public static class MakeDowned_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff, ref Pawn ___pawn)
            {
                // Pawns that should detonate on incapacitation die on being downed.
                if (___pawn.def.HasModExtension<ATR_DetonateOnIncapacitation>())
                {
                    ___pawn.Kill(dinfo, hediff);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}