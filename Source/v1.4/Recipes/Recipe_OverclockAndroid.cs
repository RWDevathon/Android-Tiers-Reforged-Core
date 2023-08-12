using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MechHumanlikes
{
    public class Recipe_OverclockAndroid : Recipe_SurgeryMechanical
    {
        // Only available if the pawn is not already overclocked or has a bill to apply overclocking active.
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (thing is Pawn pawn)
            {
                if (pawn.health.hediffSet.HasHediff(recipe.addsHediff))
                {
                    return false;
                }
                if (pawn.BillStack.Bills.Any((Bill b) => b.recipe == recipe))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        // Overclocking is always done to the brain.
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            yield return pawn.health.hediffSet.GetBrain();
        }

        // Apply overclocking.
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            pawn.health.AddHediff(recipe.addsHediff, part);
        }
    }
}