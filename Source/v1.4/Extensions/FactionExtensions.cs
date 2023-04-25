using Verse;

namespace ATReforged
{
    // Mod extension for factions to control some features.
    public class ATR_FactionAndroidExtension : DefModExtension
    {
        // Bool for whether to enforce refugees and quest pawns of this faction to be androids.
        public bool membersShouldBeAndroids = false;
    }
}