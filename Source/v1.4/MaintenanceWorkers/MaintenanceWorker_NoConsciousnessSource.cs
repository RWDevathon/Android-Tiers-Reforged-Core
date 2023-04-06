using MechHumanlikes;
using Verse;

namespace ATReforged
{
    // Very simple maintenance worker that blocks the maintenance effect being applied to the pawn's Consciousness source(s).
    public class MaintenanceWorker_NoConsciousnessSource : MaintenanceWorker
    {
        public override bool CanApplyOnPart(Pawn pawn, BodyPartRecord part)
        {
            return part == null || pawn.health.hediffSet.GetBrain() != part;
        }
    }
}
