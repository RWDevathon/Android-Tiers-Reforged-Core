using MechHumanlikes;
using RimWorld;
using Verse;

namespace ATReforged
{
    // Simple maintenance worker that removes all xp that was learned since the last midnight.
    public class MaintenanceWorker_RandomMemoryLoss : MaintenanceWorker
    {
        public override void OnApplied(Pawn pawn, BodyPartRecord part)
        {
            if (pawn.skills is Pawn_SkillTracker skillTracker)
            {
                foreach (SkillRecord record in skillTracker.skills)
                {
                    if (record.TotallyDisabled)
                    {
                        continue;
                    }

                    record.Learn(-record.xpSinceMidnight, true);
                }
            }
        }
    }
}
