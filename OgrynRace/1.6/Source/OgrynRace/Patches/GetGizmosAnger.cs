using HarmonyLib;
using OgrynRace.OgrynGizmo;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OgrynRace.Patches
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("GetGizmos")]
    public static class Patch_Pawn_GetGizmos
    {
        static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance != null && Find.Selector.NumSelected < 2)
            {
                if (__instance.def.defName == "Ogryn")
                {
                    // 先取原有 gizmos
                    var list = new List<Gizmo>(__result);

                    // 示例：只给殖民者添加
                    if (__instance.IsColonistPlayerControlled)
                    {
                        list.Add(new Gizmo_Angerbar(__instance));
                    }
                    __result = list;
                }
            }
        }
    }
}
