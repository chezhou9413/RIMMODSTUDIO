using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;


namespace OgrynRace.comp
{
    public class OgrynPlantCompProperties : CompProperties
    {
        public string SproutPath;
        public List<Mature> matureMature = new List<Mature>();
        public OgrynPlantCompProperties()
        {
            this.compClass = typeof(OgrynPlantComp);
        }

    }
    public class OgrynPlantComp : ThingComp
    {
        public OgrynPlantCompProperties Props => (OgrynPlantCompProperties)this.props;
        public Mature mature;
        public bool hasChosen = false;
        public override void CompTick()
        {
            if (hasChosen == false)
            {
                mature = ChooseByWeight(Props.matureMature);
                hasChosen = true;
            }
            base.CompTick();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            // 保存/读取布尔值
            Scribe_Values.Look(ref hasChosen, "hasChosen", false);
            Scribe_Deep.Look(ref mature, "mature");
        }
        public static Mature ChooseByWeight(List<Mature> list)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            float totalWeight = 0f;
            foreach (Mature m in list)
            {
                if (m.Probability > 0)
                    totalWeight += m.Probability;
            }
            if (totalWeight <= 0)
            {
                return null;
            }
            float rand = Rand.Value * totalWeight;
            float cumulative = 0f;
            foreach (var m in list)
            {
                if (m.Probability <= 0)
                    continue;
                cumulative += m.Probability;
                if (rand <= cumulative)
                {
                    return m;
                }
            }
            return list[list.Count - 1];
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.Graphic), MethodType.Getter)]
    public static class Patch_Plant_Graphic
    {
        public static void Postfix(Plant __instance, ref Graphic __result)
        {
            var comp = __instance.TryGetComp<OgrynPlantComp>();
            if (comp == null) return;
            string path = comp.Props.SproutPath;
            if (__instance.Growth > 0.3f && !string.IsNullOrEmpty(path) && !__instance.HarvestableNow)
            {
                __result = GraphicDatabase.Get<Graphic_Single>(
                    path,
                    ShaderDatabase.CutoutPlant, // ✅ 植物用 CutoutPlant
                    Vector2.one,
                    Color.white);
            }
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.TickLong))]
    public static class Patch_Plant_TickLong
    {
        public static void Postfix(Plant __instance)
        {
            var comp = __instance.TryGetComp<OgrynPlantComp>();
            if (comp == null || comp.Props.matureMature == null || comp.Props.matureMature.Count == 0)
                return;
            if (__instance.HarvestableNow)
            {
                var newGraphic = GraphicDatabase.Get<Graphic_Single>(
                 comp.mature.path,
                 ShaderDatabase.CutoutPlant,
                 Vector2.one,
                 Color.white);
                // 替换贴图
                typeof(Thing)
                    .GetField("graphicInt", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.SetValue(__instance, newGraphic);
                __instance.Map.mapDrawer.MapMeshDirty(
                   __instance.Position,
                   MapMeshFlagDefOf.Things);
            }
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.PlantCollected))]
    public static class PlantCollectedPrefixPatch
    {
        public static bool Prefix(Plant __instance, Pawn by, PlantDestructionMode plantDestructionMode)
        {
            if (__instance.HarvestableNow)
            {
                var comp = __instance.TryGetComp<OgrynPlantComp>();
                if (comp == null || comp.mature == null) return true;
                // 生成自定义物品
                var thing = ThingMaker.MakeThing(ThingDef.Named(comp.mature.ReceiveGoodsDef));
                thing.stackCount = comp.mature.count;
                GenPlace.TryPlaceThing(thing, __instance.Position, __instance.Map, ThingPlaceMode.Near);
                return true;
            }
            else
            {
                return true; // 继续执行原方法
            }

        }
    }


    public class Mature : IExposable
    {
        public string path;
        public float Probability;
        public string ReceiveGoodsDef;
        public int count;
        public void ExposeData()
        {
            // 保存/读取每个字段
            Scribe_Values.Look(ref path, "path");
            Scribe_Values.Look(ref Probability, "Probability");
            Scribe_Values.Look(ref ReceiveGoodsDef, "ReceiveGoodsDef");
            Scribe_Values.Look(ref count, "count", 0);
        }
    }
}
