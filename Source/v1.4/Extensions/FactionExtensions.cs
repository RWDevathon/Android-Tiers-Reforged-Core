using Verse;

namespace ATReforged
{
    // Mod extension for factions to control some features.
    public class ATR_FactionExtension : DefModExtension
    {
        // Bools for whether groups of this faction will have some members be remotely controlled surrogates.
        public bool canUseMechanicalSurrogates = false;
        public bool canUseOrganicSurrogates = false;

        // Bool for whether to enforce refugees and quest pawns of this faction to be androids.
        public bool membersShouldBeAndroids = false;
    }
}