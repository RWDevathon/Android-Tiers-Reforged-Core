using RimWorld;
using Verse;

namespace ATReforged
{
    // Mod extension for races to control some features. These attributes are only used for humanlikes, there is no reason to provide any to non-humanlikes.
    public class ATR_PawnExtension : DefModExtension
    {
        // Bool for whether this race can use the SkyMind inherently without the use of an implant. This only affects sapients.
        public bool canInherentlyUseSkyMind = false;

        // Bool for whether this race is considered an android. Sapient androids need cores.
        public bool isAndroid = false;

        // Defs for what hediffs are the standard to use for controller/receiver implants for SkyMind-related spawning purposes.
        public HediffDef defaultTransceiverImplant;
        public HediffDef defaultReceiverImplant;
    }

    // Mod extension for particular races to mark them as something that should explode on downed or death.
    public class ATR_DetonateOnIncapacitation : DefModExtension
    {
        public float explosionRadius = 0.5f;
        public DamageDef damageType;
        public int damageAmount;

        public ThingDef itemToSpawnOnDetonation;
        public int itemCountToSpawn;
    }
}