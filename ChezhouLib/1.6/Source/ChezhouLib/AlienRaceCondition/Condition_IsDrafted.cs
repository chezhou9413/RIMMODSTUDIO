using AlienRace;
using AlienRace.ExtendedGraphics;
using RimWorld;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace ChezhouLib.AlienRaceCondition
{
    public class Condition_IsDrafted : Condition
    {
        public static string XmlNameParseKey = "IsDrafted";

        // 若为 true 则反转结果（例如 invert=true 时：未被征召返回 true）
        public bool invert = false;

        // --- 缓存区：每个 Pawn 的征召状态 ---
        private static readonly Dictionary<Pawn, (bool isDrafted, int tick)> DraftedCache = new Dictionary<Pawn, (bool, int)>();

        // 缓存有效时长（单位：tick）。为了高频切换时及时响应，这里设为 1 tick。
        private const int CacheLifetimeTicks = 1;

        public override bool Satisfied(ExtendedGraphicsPawnWrapper pawnWrapper, ref ResolveData data)
        {
            if (pawnWrapper == null || pawnWrapper.WrappedPawn == null)
                return false;

            Pawn pawn = pawnWrapper.WrappedPawn;

            bool drafted = GetOrUpdateDraftedState(pawn);
            return invert ? !drafted : drafted;
        }

        private static bool GetOrUpdateDraftedState(Pawn pawn)
        {
            // 暂停时不使用缓存，直接读取最新征召状态（暂停下 tick 不推进，缓存会“卡住”）
            if (Find.TickManager.Paused)
                return pawn.Drafted;

            int currentTick = Find.TickManager.TicksGame;
            (bool isDrafted, int tick) entry;

            if (DraftedCache.TryGetValue(pawn, out entry))
            {
                if (currentTick - entry.tick < CacheLifetimeTicks)
                    return entry.isDrafted;
            }

            bool isDraftedNow = pawn.Drafted;
            DraftedCache[pawn] = (isDraftedNow, currentTick);
            return isDraftedNow;
        }

        public override void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            Utilities.SetInstanceVariablesFromChildNodesOf(xmlRoot, this, new HashSet<string>());
        }
    }
}


