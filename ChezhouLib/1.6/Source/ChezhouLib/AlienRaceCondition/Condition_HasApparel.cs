using AlienRace;
using AlienRace.ExtendedGraphics;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace ChezhouLib.AlienRaceCondition
{
    public class Condition_HasApparel : Condition
    {
        public static string XmlNameParseKey = "HasApparel";

        public List<string> apparelDefs = new List<string>();
        private HashSet<string> apparelDefSet;

        public bool requireAll = false;
        public bool invert = false;

        // --- 缓存区：每个 Pawn 的装备哈希表 ---
        private static readonly Dictionary<Pawn, (HashSet<string> wornNames, int tick)> WornCache
            = new Dictionary<Pawn, (HashSet<string>, int)>();

        // 缓存有效时间（帧数）；例如 60 表示每秒更新一次
        private const int CacheLifetimeTicks = 30;

        public override bool Satisfied(ExtendedGraphicsPawnWrapper pawnWrapper, ref ResolveData data)
        {
            if (pawnWrapper == null || pawnWrapper.WrappedPawn == null)
                return false;

            Pawn pawn = pawnWrapper.WrappedPawn;

            // 读取缓存或构建新缓存
            HashSet<string> wornNames = GetOrUpdateWornNames(pawn, pawnWrapper);
            // 合并：服装 + 武器 的 defName 集合
            HashSet<string> combinedNames = new HashSet<string>();
            if (wornNames != null)
            {
                foreach (var n in wornNames)
                    combinedNames.Add(n);
            }
            var eqNames = GetEquipmentDefNames(pawn);
            if (eqNames != null)
            {
                foreach (var n in eqNames)
                    combinedNames.Add(n);
            }

            if (combinedNames.Count == 0)
                return invert;

            if (apparelDefSet == null)
                apparelDefSet = new HashSet<string>(apparelDefs);

            bool result;
            if (requireAll)
                result = apparelDefSet.IsSubsetOf(combinedNames);
            else
                result = apparelDefSet.Overlaps(combinedNames);

            return invert ? !result : result;
        }

        private static HashSet<string> GetOrUpdateWornNames(Pawn pawn, ExtendedGraphicsPawnWrapper pawnWrapper)
        {
            int currentTick = Find.TickManager.TicksGame;
            (HashSet<string> wornNames, int tick) entry;

            if (WornCache.TryGetValue(pawn, out entry))
            {
                // 若缓存仍有效则直接使用
                if (currentTick - entry.tick < CacheLifetimeTicks)
                    return entry.wornNames;
            }

            var worn = pawnWrapper.GetWornApparel;
            if (worn == null)
            {
                WornCache[pawn] = (new HashSet<string>(), currentTick);
                return WornCache[pawn].wornNames;
            }

            var newWorn = new HashSet<string>(worn.Select(a => a.def.defName));
            WornCache[pawn] = (newWorn, currentTick);
            return newWorn;
        }

        // 读取当前 Pawn 所有已装备武器/装备（equipment 列表）的 defName 集合
        private static IEnumerable<string> GetEquipmentDefNames(Pawn pawn)
        {
            if (pawn == null || pawn.equipment == null)
                return null;
            var list = pawn.equipment.AllEquipmentListForReading;
            if (list == null || list.Count == 0)
                return null;
            List<string> names = new List<string>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                ThingWithComps eq = list[i];
                if (eq != null && eq.def != null && !string.IsNullOrEmpty(eq.def.defName))
                    names.Add(eq.def.defName);
            }
            return names;
        }

        public override void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            Utilities.SetInstanceVariablesFromChildNodesOf(xmlRoot, this, new HashSet<string>());
            apparelDefSet = new HashSet<string>(apparelDefs);
        }
    }
}
