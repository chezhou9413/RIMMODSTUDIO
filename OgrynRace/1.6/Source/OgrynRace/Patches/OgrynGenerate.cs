using HarmonyLib;
using OgrynRace.DefRef;
using RimWorld;
using Verse;

namespace OgrynRace.Patches
{
    [HarmonyPatch(typeof(PawnGenerator), "TryGenerateNewPawnInternal")]
    public static class OgrynGenerate
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __result)
        {
			if (__result != null) { 
                if(__result.def.defName == "Ogryn")
                {
                    if(Rand.Value > 0.1)
                    {
                        Hediff newHediff = HediffMaker.MakeHediff(OgrynHediffDef.Ogryn_GuaiLi,__result);
                        __result.health.AddHediff(newHediff);
                    }
                    else
                    {
						Hediff newHediff = HediffMaker.MakeHediff(OgrynHediffDef.Ogryn_CongMingTou,__result);
						__result.health.AddHediff(newHediff);
                    }
                }
            }
        }
    }
}
