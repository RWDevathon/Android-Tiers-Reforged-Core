using Verse;
using HarmonyLib;
using RimWorld;

namespace ATReforged
{
    public class ThoughtWorker_SharedBed_Patch
    {
        // People do not mind sharing a bed with surrogates controlled by their lovers or themselves.
        [HarmonyPatch(typeof(ThoughtWorker_SharedBed), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                if (!__result.Active)
                    return;

                if (SharingBedWithSurrogateRelation(p))
                {
                    __result = ThoughtState.Inactive;
                }
            }

            // Check if the shared bed is with an active surrogate of oneself or a loved one and return false if any pawn is neither.
            private static bool SharingBedWithSurrogateRelation(Pawn p)
            {
                Building_Bed ownedBed = p.ownership.OwnedBed;
                if (ownedBed == null)
                {
                    return false;
                }
                foreach (Pawn pawn in ownedBed.OwnersForReading)
                {
                    // Don't check the pawn itself in question.
                    if (pawn == p)
                    {
                        continue;
                    }

                    // If the pawn in question is not a surrogate, return false.
                    if (!Utils.IsSurrogate(pawn))
                    {
                        return false;
                    }

                    CompSkyMindLink surrogateLink = pawn.GetComp<CompSkyMindLink>();

                    // If the surrogate is uncontrolled, return false.
                    if (!surrogateLink.HasSurrogate())
                    {
                        return false;
                    }

                    // If the surrogate is neither controlled by the pawn nor a lover of the pawn, return false.
                    if (surrogateLink.GetSurrogates().FirstOrFallback() != p && !LovePartnerRelationUtility.LovePartnerRelationExists(p, surrogateLink.GetSurrogates().FirstOrFallback()))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}