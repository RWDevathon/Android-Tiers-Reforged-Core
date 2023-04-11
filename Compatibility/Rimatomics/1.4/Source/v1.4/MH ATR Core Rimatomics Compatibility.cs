using HarmonyLib;
using System.Reflection;
using Verse;

namespace ATReforged
{
    public class MH_ATR_Core_Rimatomics_Compatibility : Mod
    {
        public MH_ATR_Core_Rimatomics_Compatibility(ModContentPack content) : base(content)
        {
        }
    }

    [StaticConstructorOnStartup]
    public static class MH_ATR_Core_Rimatomics_Compatibility_PostInit
    {
        static MH_ATR_Core_Rimatomics_Compatibility_PostInit()
        {
            new Harmony("MH ATR Core Rimatomics Compatibility").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}