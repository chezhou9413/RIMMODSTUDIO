using ChezhouLib.Utils;
using rimbreak.DefRef;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace rimbreak.RbVerb
{
    public class Verb_Jin_JudgmentFinalStrike : Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            Pawn pawn = CasterPawn;
            if (pawn == null || pawn.Dead || !pawn.Spawned)
                return false;

            // 强制执行，不依赖基类的TryCastShot
            bool casted = base.TryCastShot();
            // 播放特效
            UnityEffUtils.spawnEff("RbEfflord", "JudgmentFinalStrike_eff", 0.15f, pawn.DrawPos, pawn);
            // 伤害半径（5x5格）
            int radius = 2; // 5x5范围实际上是以pawn为中心，半径2格
            float damageAmount = 50f;
            Hediff hediff = HediffMaker.MakeHediff(RbHediffDef.RB_BladeBody, pawn);
            pawn.health.AddHediff(hediff);
            
            // 使用原版的范围获取方式
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(pawn.Position, radius, useCenter: true))
            {
                if (!cell.InBounds(pawn.Map))
                    continue;

                // 使用原版的方式获取该格的所有物体，但创建副本避免修改集合
                List<Thing> things = cell.GetThingList(pawn.Map);
                List<Thing> thingsCopy = new List<Thing>(things);
                foreach (Thing thing in thingsCopy)
                {
                    Pawn targetPawn = thing as Pawn;
                    if (targetPawn == null || targetPawn == pawn || targetPawn.Dead)
                        continue;

                    // 检查是否为非友方单位
                    if (IsNonFriendlyPawn(pawn, targetPawn))
                    {
                        // 造成锐器伤害
                        DamageInfo dinfo = new DamageInfo(
                            DamageDefOf.Cut,                // 锐器伤害
                            damageAmount,                   // 伤害值
                            0,                              // armorPenetration
                            -1,                             // angle
                            pawn,                           // 施加者
                            null,                           // 命中的身体部位可设null让系统自动选择
                            null,                           // 使用的武器或工具
                            DamageInfo.SourceCategory.ThingOrUnknown
                        );
                        targetPawn.TakeDamage(dinfo);
                    }
                }
            }

            // 触发冷却
            if (this.Ability != null)
            {
                int cd = this.Ability.def.cooldownTicksRange.RandomInRange;
                if (cd > 0)
                    this.Ability.StartCooldown(cd);
            }

            return true;
        }

        /// <summary>
        /// 检查目标是否为非友方单位
        /// </summary>
        private bool IsNonFriendlyPawn(Pawn caster, Pawn target)
        {
            // 自己不算
            if (caster == target)
                return false;
                
            // 死亡单位不算
            if (target.Dead)
                return false;
                
            // 没有阵营的野生生物算作非友方
            if (target.Faction == null)
                return true;
                
            // 施法者没有阵营时，所有有阵营的单位都算非友方
            if (caster.Faction == null)
                return true;
                
            // 检查是否为敌对关系
            return target.Faction.HostileTo(caster.Faction);
        }

        // 不需要目标
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true) => true;
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ) => true;
        public override bool Targetable => false;
    }
}