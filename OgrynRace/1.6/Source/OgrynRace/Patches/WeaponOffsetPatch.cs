using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace OgrynRace.Patches
{
    [HarmonyPatch(typeof(PawnRenderUtility))]
    [HarmonyPatch(nameof(PawnRenderUtility.DrawEquipmentAndApparelExtras))]
    public static class WeaponOffsetPatch_Simple
    {
        static void Prefix(ref Vector3 drawPos, Pawn pawn)
        {
            if (pawn?.def?.defName == "Ogryn")
            {
                // 向前偏移（Z轴），值可按视觉调
                drawPos += new Vector3(0f, 0f, 0.2f);
            }
        }
    }
}


