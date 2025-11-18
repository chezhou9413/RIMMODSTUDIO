using OgrynRace.AI;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace OgrynRace.OgrynJobDrive
{
    public class JobDefExtension_OgrynDash : DefModExtension
    {
        public float dashSpeed = 0.4f;
        public int knockbackDistance = 10;
        public string hediffDef;
    }
    public class JobDrive_OgrynDash : JobDriver
    {
        private Vector3 startPosition;
        private Vector3 destination;
        private int totalTicks;
        private int currentTick;
        private bool isDashing = false;
        public float dashSpeed = 0.4f;// 每 tick 前进距离（约等于18格/秒）
        public int knockbackDistance = 10;// 1. 定义击退距离
        public string HediffDef;
        // 缓存目标以避免 job.targetA 在目标死亡/消失时变为 null 导致空引用
        private Pawn cachedTargetPawn;
        public Pawn TargetPawn => job.targetA.Thing as Pawn;

        public override void Notify_Starting()
        {
            base.Notify_Starting();

            var ext = job.def.GetModExtension<JobDefExtension_OgrynDash>();
            if (ext != null)
            {
                dashSpeed = ext.dashSpeed;
                knockbackDistance = ext.knockbackDistance;
                HediffDef = ext.hediffDef;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref startPosition, "startPosition");
            Scribe_Values.Look(ref destination, "destination");
            Scribe_Values.Look(ref totalTicks, "totalTicks");
            Scribe_Values.Look(ref currentTick, "currentTick");
            Scribe_Values.Look(ref isDashing, "isDashing");
            Scribe_References.Look(ref cachedTargetPawn, "cachedTargetPawn");
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // ❗确保目标存在且为Pawn，否则直接失败
            this.FailOn(() => TargetPawn == null || !TargetPawn.Spawned || TargetPawn.Dead || pawn == null || pawn.Map == null);

            // --- 瞄准准备 ---
            Toil aimToil = new Toil
            {
                initAction = () =>
                {
                    if (pawn.pather != null) pawn.pather.StopDead();
                    // 初始化时缓存目标，后续统一使用缓存引用，减少空引用风险
                    cachedTargetPawn = TargetPawn;
                    if (cachedTargetPawn == null || !cachedTargetPawn.Spawned || cachedTargetPawn.Dead)
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    if (pawn.rotationTracker != null) pawn.rotationTracker.FaceCell(cachedTargetPawn.Position);
					SoundDef.Named("Longjump_Jump").PlayOneShot(SoundInfo.InMap(pawn));
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 60 // 1 秒
            };
            yield return aimToil;
            Toil dashToil = new Toil
            {
                handlingFacing = true,
                initAction = () =>
                {
                    startPosition = pawn.Position.ToVector3Shifted();
                    // 若目标无效则直接结束
                    if (cachedTargetPawn == null || !cachedTargetPawn.Spawned || cachedTargetPawn.Dead)
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    destination = cachedTargetPawn.Position.ToVector3Shifted();

                    float distance = Vector3.Distance(startPosition, destination);
                    totalTicks = Mathf.Max(Mathf.RoundToInt(distance / dashSpeed), 10);
                    currentTick = 0;
                    isDashing = true;
                    if (pawn.pather != null) pawn.pather.StopDead();
                    if (pawn.rotationTracker != null) pawn.rotationTracker.FaceCell(cachedTargetPawn.Position);
                },
                tickAction = () =>
                {
                    if (!isDashing) return;

                    // 目标在冲刺过程中死亡/消失时的处理：
                    // 结束冲刺到当前 destination，不再访问无效目标引用
                    if (cachedTargetPawn == null || cachedTargetPawn.Dead || !cachedTargetPawn.Spawned)
                    {
                        // 保持 destination 为最近一次有效位置，不再更新
                        // 当达到总时长后正常收尾
                    }
                    else
                    {
                        // 动态更新目标位置（如果目标在移动）
                        destination = cachedTargetPawn.Position.ToVector3Shifted();
                    }

                    currentTick++;
                    float progress = Mathf.Clamp01((float)currentTick / totalTicks);

                    // 计算当前位置
                    Vector3 newPos = Vector3.Lerp(startPosition, destination, progress);
                    IntVec3 newCell = newPos.ToIntVec3();

                    // 如果地形不可通行，则提前结束冲刺
                    if (pawn.Map == null || !newCell.InBounds(pawn.Map) || !newCell.Walkable(pawn.Map))
                    {
                        isDashing = false;
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    // 移动 + 转向
                    if (newCell != pawn.Position)
                    {
                        pawn.Position = newCell;
                        if (pawn.Spawned)
                        {
                            pawn.Notify_Teleported(false, false);
                        }
                    }

                    if (cachedTargetPawn != null && cachedTargetPawn.Spawned && pawn.rotationTracker != null)
                    {
                        pawn.rotationTracker.FaceCell(cachedTargetPawn.Position);
                    }

                    // 冲刺完成
                    if (currentTick >= totalTicks)
                    {
                        isDashing = false;
                        pawn.Position = destination.ToIntVec3();
                        if (pawn.Spawned)
                        {
                            pawn.Notify_Teleported(false, false);
                        }
						SoundDef.Named("Longjump_Land").PlayOneShot(SoundInfo.InMap(pawn));

                        // 撞击伤害逻辑
                        if (cachedTargetPawn != null && cachedTargetPawn.Spawned && !cachedTargetPawn.Dead)
                        {
                            DamageInfo dinfo = new DamageInfo(
                                DamageDefOf.Blunt,
                                Rand.Range(15, 25),
                                0,
                                -1,
                                pawn
                            );
                            cachedTargetPawn.TakeDamage(dinfo);
                            if (HediffDef != null)
                            {
                                HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed(HediffDef);
                                Hediff hediff = HediffMaker.MakeHediff(hediffDef, cachedTargetPawn);
                                cachedTargetPawn.health.AddHediff(hediff);
                            }
                            // 冲刺命中中立派系目标时，扣除该派系对玩家的友好度（-50）
                            Faction targetFaction = cachedTargetPawn.Faction;
                            if (targetFaction != null && targetFaction != Faction.OfPlayer && !targetFaction.HostileTo(Faction.OfPlayer))
                            {
                                // 让底层处理可扣除范围与信件/消息
                                targetFaction.TryAffectGoodwillWith(
                                    Faction.OfPlayer,
                                    -50,
                                    canSendMessage: true,
                                    canSendHostilityLetter: true,
                                    HistoryEventDefOf.UsedHarmfulAbility
                                );
                            }
                            // ==========================================================
                            // ===== 击退逻辑 - 在撞击伤害后立即执行 =====
                            // ==========================================================
                            if (cachedTargetPawn != null && cachedTargetPawn.Spawned && !cachedTargetPawn.Dead)
                            {
                                // 2. 计算击退方向 (使用冲刺的实际方向)
                                // 使用冲刺开始位置到目标位置的方向，确保击退方向与冲刺方向完全一致
                                Vector3 startPos = startPosition;
                                Vector3 targetPos = cachedTargetPawn.Position.ToVector3Shifted();
                                Vector3 dashDirection = (targetPos - startPos).normalized;

                                // 3. 寻找最终落点
                                IntVec3 finalKnockbackCell = cachedTargetPawn.Position;
                                for (int i = 0; i < knockbackDistance; i++)
                                {
                                    // 计算路径上的下一个格子 - 使用正确的向量转换
                                    IntVec3 directionInt = new IntVec3(
                                        Mathf.RoundToInt(dashDirection.x),
                                        0,
                                        Mathf.RoundToInt(dashDirection.z)
                                    );
                                    IntVec3 nextCell = finalKnockbackCell + directionInt;

                                    // 仅使用地形阻挡：边界与地形不可通过时停止
                                    if (pawn.Map == null || !nextCell.InBounds(pawn.Map))
                                    {
                                        break;
                                    }
                                    var terrain = nextCell.GetTerrain(pawn.Map);
                                    if (terrain != null && terrain.passability == Traversability.Impassable)
                                    {
                                        break;
                                    }

                                    // 山体等为建筑(Edifice)而非地形，这里额外阻挡不可通过的建筑
                                    var edifice = nextCell.GetEdifice(pawn.Map);
                                    if (edifice != null && edifice.def != null && edifice.def.passability == Traversability.Impassable)
                                    {
                                        break;
                                    }

                                    // 地形可通过则继续推进最终落点（建筑、单位不阻挡）
                                    finalKnockbackCell = nextCell;
                                }

                                // 4. 执行击退 (强制执行，无论落点是否不同)
                                // 记录原始位置用于调试
                                IntVec3 originalPosition = cachedTargetPawn.Position;
                                // 移动目标Pawn
                                cachedTargetPawn.Position = finalKnockbackCell;
                                // 必须调用这个来通知游戏该Pawn被传送了
                                if (cachedTargetPawn.Spawned)
                                {
                                    cachedTargetPawn.Notify_Teleported(false, false);
                                }
                                // 中断目标当前路径，防止其立即走回原位
                                if (cachedTargetPawn.pather != null)
                                {
                                    cachedTargetPawn.pather.StopDead();
                                }

                                // 添加击退视觉效果
                                if (cachedTargetPawn.Spawned)
                                {
                                    MoteMaker.MakeStaticMote(cachedTargetPawn.DrawPos, cachedTargetPawn.Map, ThingDefOf.Mote_Stun, 1.0f);
                                    MoteMaker.ThrowText(cachedTargetPawn.DrawPos, cachedTargetPawn.Map, "被击退!", new Color(1f, 0.5f, 0f)); // 橙色
                                }
                            }
                        }
                        EndJobWith(JobCondition.Succeeded);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never
            };
            yield return dashToil;
            // --- 清理阶段 ---
            Toil cleanupToil = new Toil
            {
                initAction = () =>
                {
                    isDashing = false;
                    JobGiver_AIOgrynDash.RaiseOgrynDashCompleted(this.pawn);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return cleanupToil;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;
    }
}
