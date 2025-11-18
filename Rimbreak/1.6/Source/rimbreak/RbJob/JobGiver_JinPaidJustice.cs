using ChezhouLib.Utils;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace rimbreak.RbJob
{
    // 执行“有偿正义”发射流程的 JobDriver
    public class JobGiver_JinPaidJustice : JobDriver
    {
        private const int WaitTicks_Warmup = 30;

        // 发射后短暂停顿（ticks）
        private const int WaitTicks_Cooldown = 90;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
			// 为目标位置做目的地预约；如果目标是物体，尝试进行占用预约
			LocalTargetInfo target = job.GetTarget(TargetIndex.A);
			if (target.IsValid)
			{
                pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, target.Cell);
				if (target.Thing != null)
				{
					return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
				}
			}
			return true;
        }

        // Job 的执行序列
        protected override IEnumerable<Toil> MakeNewToils()
        {
            // 由 Verb 传入的最大射程点
            LocalTargetInfo target = this.job.targetA;

            // 朝向目标并短暂等待（无需接触目标）
			yield return Toils_General.Wait(WaitTicks_Warmup, TargetIndex.A);

            // --- 步骤 2: "发射" 逻辑 ---
			Toil fireToil = new Toil();
            fireToil.defaultCompleteMode = ToilCompleteMode.Instant;
            fireToil.initAction = () =>
            {
                Pawn caster = this.pawn;
                Map map = caster.Map;
                IntVec3 startCell = caster.Position;
                Vector3 origin = caster.DrawPos;

                // 目标格
                IntVec3 targetCell = target.Cell;

                // 获取投射体定义
                ThingDef projDef = ThingDef.Named("PaidJusticeProjectile");
                if (projDef == null) return;

                // 生成投射体
                Projectile proj = (Projectile)GenSpawn.Spawn(projDef, startCell, map);
                if (proj == null) return;

                // 发射（优先带 Mote）
				if (proj is rimbreak.RbProjectile.Projectile_PaidJustice jinProj)
                {
                    jinProj.LaunchWithMote(
                        caster,
                        origin,
                        new LocalTargetInfo(targetCell),
                        null
                    );
                }
                else
                {
                    // 退化为标准发射
                    proj.Launch(
                        caster,
                        origin,
                        new LocalTargetInfo(targetCell),
                        new LocalTargetInfo(targetCell),
                        ProjectileHitFlags.All,
                        false,
                        null
                    );
                }
            };

            // 直接执行，不要求接触目标
			yield return fireToil;

            // 发射后短暂停顿
            yield return Toils_General.Wait(WaitTicks_Cooldown);
        }
    }
}


