using Verse;

namespace ATReforged
{
    // Mod extension for races to control some features. These attributes are only used for humanlikes, there is no reason to provide any to non-humanlikes.
    public class ATR_AndroidExtension : DefModExtension
    {
        // Bool for whether this race can use the SkyMind inherently without the use of an implant. This only affects sapients.
        public bool canInherentlyUseSkyMind = false;

        // Bool for whether this race needs Cores (brain part) when assigned as a sapient.
        public bool needsCoreAsAndroid = true;
    }
}