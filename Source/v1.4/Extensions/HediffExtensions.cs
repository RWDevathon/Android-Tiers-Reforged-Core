using System.Collections.Generic;
using Verse;

namespace ATReforged
{
    // Mod extension for Hediffs to affect the pawn's ability to use the SkyMind network. These attributes are only used for humanlikes.
    public class ATR_SkyMindHediffExtension : DefModExtension
    {

        // Marks the host hediff as something that allows connecting to the SkyMind network.
        public bool allowsConnection = false;

        // Marks the host hediff as something that prevents connecting to the SkyMind network.
        public bool blocksConnection = false;

        // Marks the host hediff as acting as a transceiver, meaning its host can be a surrogate controller and is not a surrogate.
        public bool isTransceiver = false;

        // Marks the host hediff as acting as a receiver, meaning its host is a surrogate. It can't be a controller.
        public bool isReceiver = false;

        // Some states are illegal and should not be allowed.
        public override IEnumerable<string> ConfigErrors()
        {
            if (allowsConnection)
            {
                if (blocksConnection)
                {
                    yield return "A HediffDef was given an ATR_SkyMindHediffExtension that both allowed and blocked SkyMind connections.";
                }
                if (!(isTransceiver || isReceiver))
                {
                    yield return "A HediffDef was given an ATR_SkyMindHediffExtension that allows SkyMind connection but is neither a transceiver nor receiver.";
                }
            }
        }
    }
}