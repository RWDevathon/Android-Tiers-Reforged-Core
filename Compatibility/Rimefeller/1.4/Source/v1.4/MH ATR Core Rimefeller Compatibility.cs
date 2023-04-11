using HarmonyLib;
using System.Reflection;
using Verse;

namespace ATReforged
{
    public class MH_ATR_Core_Rimefeller_Compatibility : Mod
    {
        public MH_ATR_Core_Rimefeller_Compatibility(ModContentPack content) : base(content)
        {
            new Harmony("MH ATR Core Rimefeller Compatibility").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}