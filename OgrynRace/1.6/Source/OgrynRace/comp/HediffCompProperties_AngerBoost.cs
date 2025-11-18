using HarmonyLib;
using RimWorld;
using System;
using System.Text;
using Verse;

namespace OgrynRace.comp
{
    public class HediffCompProperties_AngerBoost : HediffCompProperties
    {
        public HediffCompProperties_AngerBoost()
        {
            this.compClass = typeof(HediffComp_AngerBoost);
        }
    }

    public class HediffComp_AngerBoost : HediffComp
    {
        public OgrynAngerComp AngerComp
        {
            get
            {
                if (Pawn == null) return null;
                return Pawn.TryGetComp<OgrynAngerComp>();
            }
        }

		// 记录内部tick计数，用于定期检查移除
		private int tickCounter = 0;

		// 在血量标签后括号中显示的额外信息（短）
		public override string CompLabelInBracketsExtra
		{
			get
			{
				// 无怒气或无组件则不显示
				if (AngerComp == null || AngerComp.CurrentAnger <= 0)
				{
					return null;
				}
				// 约定：1点怒气 = 近战伤害+2%，移速+1%
				float dmgPct = AngerComp.CurrentAnger * 2f;
				float movePct = AngerComp.CurrentAnger * 1f;
				return "Ogryn_Anger".Translate() + ":" + AngerComp.CurrentAnger;
			}
		}

		// 鼠标悬浮时的额外提示信息（长）
		public override string CompTipStringExtra
		{
			get
			{
				if (AngerComp == null || AngerComp.CurrentAnger <= 0)
				{
					return null;
				}
				// 构造多行提示，清晰说明怒气带来的各项加成
				int anger = AngerComp.CurrentAnger;
				float dmgPct = anger * 2f;
				float movePct = anger * 1f;
				float painThreshPct = anger * 1f;
				return "Ogryn_AngerTip".Translate(anger, dmgPct.ToString("0"), movePct.ToString("0"), painThreshPct.ToString("0"), painThreshPct.ToString("0"));
			}
		}

		// 每tick调用：定期在怒气为0时移除自身
		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			// 仅每60tick检查一次，避免频繁开销
			tickCounter++;
			if (tickCounter >= 60)
			{
				tickCounter = 0;
				if (AngerComp == null || AngerComp.CurrentAnger <= 0)
				{
					// 怒气为0则移除该hediff
					Pawn?.health?.RemoveHediff(this.parent);
				}
			}
		}
    }

    [HarmonyPatch(typeof(HediffSet), "PainTotal", MethodType.Getter)]
    public static class Patch_PainTotal
    {
        public static void Postfix(HediffSet __instance, ref float __result)
        {
            Pawn pawn = __instance.pawn;
            var comp = pawn.TryGetComp<OgrynAngerComp>();
            if (comp != null)
            {
                // 每 1 点怒气减少 0.01 疼痛
                __result *= Math.Max(0f, 1f - comp.CurrentAnger * 0.01f);
            }
        }
    }
    public class StatPart_Anger : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            if (req.Thing is Pawn pawn)
            {
                var comp = pawn.TryGetComp<OgrynAngerComp>();
                if (comp != null && comp.CurrentAnger > 0)
                {
                    StringBuilder sb = new StringBuilder();

					if (this.parentStat == StatDefOf.MeleeDamageFactor)
					{
						sb.AppendLine("Ogryn_AngerMeleeBonusLine".Translate((comp.CurrentAnger * 2f).ToString("0")));
					}
                    if (this.parentStat == StatDefOf.MoveSpeed)
                    {
						sb.AppendLine("Ogryn_AngerMoveBonusLine".Translate((comp.CurrentAnger * 1f).ToString("0")));
                    }
                    if (this.parentStat == StatDefOf.PainShockThreshold)
                    {
						sb.AppendLine("Ogryn_AngerPainBonusLine".Translate((comp.CurrentAnger * 1f).ToString("0")));
                    }

                    return sb.ToString().TrimEnd();
                }
            }
            return null; // 没有说明时返回 null
        }


        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.Thing is Pawn pawn)
            {
                if (pawn != null)
                {
                    OgrynAngerComp comp = pawn.TryGetComp<OgrynAngerComp>();
                    if (comp != null)
                    {
                        // 例如：1怒气 = +2% 伤害
                        if (this.parentStat == StatDefOf.MeleeDamageFactor)
                        {
                            val *= 1f + comp.CurrentAnger * 0.02f;
                        }

                        if (this.parentStat == StatDefOf.MoveSpeed)
                        {
                            val *= 1f + comp.CurrentAnger * 0.01f;
                        }

                        if (this.parentStat == StatDefOf.PainShockThreshold)
                        {
                            val += 1f + comp.CurrentAnger * 0.01f;
                        }
                    }
                }
            }
        }
    }
}
