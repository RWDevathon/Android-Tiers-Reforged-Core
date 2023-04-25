using Verse;

namespace ATReforged
{
    public class ATR_GameComponent : GameComponent
    {
        public ATR_GameComponent(Game game)
        {
            ATRCore_Utils.gameComp = this;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref hasBuiltAndroid, "ATR_hasBuiltAndroid", false);
            Scribe_Values.Look(ref hasBuiltDrone, "ATR_hasBuiltDrone", false);
        }

        // Simple booleans for whether players have encountered some mechanics yet to display educational letters. Some letters are handled by researches.
        public bool hasBuiltDrone = false;
        public bool hasBuiltAndroid = false;
    }
}