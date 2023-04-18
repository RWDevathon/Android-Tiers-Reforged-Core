using MechHumanlikes;
using AlienRace;
using Verse;

namespace ATReforged
{
    // Simple maintenance worker that prevents this worker from firing on non-sapient pawns like drones.
    public class MaintenanceWorker_SapientsOnly : MaintenanceWorker
    {
        public override bool CanEverApplyTo(RaceProperties race)
        {
            // Non-humanlike intelligences and non-mechanical-sapient intelligences are illegal.
            if (race.intelligence != Intelligence.Humanlike || !MHC_Utils.IsConsideredMechanicalSapient(CachedData.GetRaceFromRaceProps(race)))
            {
                return false;
            }
            return true;
        }
    }
}
