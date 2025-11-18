using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using static UnityEngine.GraphicsBuffer;

namespace OgrynRace.OgrynVerb
{
    public class Verb_OgrynDashAbility : Verb_CastAbility
    {
        private float cachedEffectiveRange = -1f;

        public override bool MultiSelect => true;

        // 计算有效射程（可兼容装备的 JumpRange）
        public override float EffectiveRange
        {
            get
            {
                if (cachedEffectiveRange < 0f)
                {
                    if (EquipmentSource != null)
                        cachedEffectiveRange = EquipmentSource.GetStatValue(StatDefOf.JumpRange);
                    else
                        cachedEffectiveRange = base.EffectiveRange;
                }
                return cachedEffectiveRange;
            }
        }

        // 主逻辑：尝试施放技能
        protected override bool TryCastShot()
        {
            bool casted = base.TryCastShot();
            if (!casted)
            {
                return false;
            }
            Pawn pawn = CasterPawn;
            if (pawn == null) return false;
            if (!ValidateTarget(currentTarget, false))
                return false;

            // 创建并下达自定义冲刺Job
            Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Ogryn_Dash_Job"), currentTarget.Thing);
            job.playerForced = true;
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            return true;
        }

        // 鼠标悬停时显示技能UI反馈
        public override void OnGUI(LocalTargetInfo target)
        {
            if (CanHitTarget(target) && ValidDashTarget(CasterPawn, target.Cell))
                base.OnGUI(target);
            else
                GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
        }

        // 强制指令时调用（Shift+右键）
        public override void OrderForceTarget(LocalTargetInfo target)
        {
            if (ValidDashTarget(CasterPawn, target.Cell))
            {
                Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Ogryn_Dash_Job"), target.Thing);
                job.playerForced = true;
                CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                // 强制指令同样进入冷却
                if (this.Ability != null && this.Ability.def != null)
                {
                    int cd = this.Ability.def.cooldownTicksRange.TrueMin == 0 && this.Ability.def.cooldownTicksRange.TrueMax == 0
                        ? 0
                        : this.Ability.def.cooldownTicksRange.RandomInRange;
                    if (cd > 0)
                    {
                        this.Ability.StartCooldown(cd);
                    }
                }
            }
        }

        // 允许敌对或中立 Pawn 作为目标（排除自己与友军）
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (!target.HasThing)
                return false;

            Pawn targetPawn = target.Thing as Pawn;
            if (targetPawn == null)
                return false;

            // 不能冲自己
            if (targetPawn == CasterPawn)
                return false;

            // 判断是否敌对、中立、友军或无派系
            bool canTarget = false;
            if (targetPawn.Faction == null)
            {
                // 无派系目标（动物、野人、机械体）
                canTarget = true;
            }
            else if (targetPawn.Faction == CasterPawn.Faction)
            {
                // 友军
                canTarget = false;
            }
            else if (targetPawn.HostileTo(CasterPawn.Faction))
            {
                // 敌对阵营
                canTarget = true;
            }
            else
            {
                // 中立阵营
                canTarget = true;
            }

            if (!canTarget)
                return false;

            // 距离与路径判断
            float dist = CasterPawn.Position.DistanceTo(targetPawn.Position);
            if (dist > EffectiveRange)
                return false;

            if (!CasterPawn.CanReach(targetPawn, PathEndMode.Touch, Danger.Deadly))
                return false;

            return true;
        }


        // 用于射线检测和射程判断（决定射线红绿显示）
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (!targ.IsValid) return false;
            if (!targ.Cell.InBounds(this.CasterPawn.Map)) return false;
            if (targ.Cell.Impassable(this.CasterPawn.Map)) return false;

            float distance = root.DistanceTo(targ.Cell);
            if (distance > EffectiveRange) return false;

            // 使用视线检测，墙体会遮挡
            if (!GenSight.LineOfSight(root, targ.Cell, this.CasterPawn.Map, true))
                return false;

            // 防止目标在无法站立的格子
            if (!ValidDashTarget(CasterPawn, targ.Cell))
                return false;

            return true;
        }

        // 自定义可冲刺检测（模仿 JumpUtility.ValidJumpTarget）
        private bool ValidDashTarget(Pawn pawn, IntVec3 cell)
        {
            if (!cell.Walkable(pawn.Map)) return false;
            if (!pawn.CanReach(cell, PathEndMode.OnCell, Danger.Deadly)) return false;
            return true;
        }

        // 动态绘制射程圈 + 遮挡检测
        public override void DrawHighlight(LocalTargetInfo target)
        {
            Pawn pawn = CasterPawn;
            if (pawn == null) return;

            if (target.IsValid && ValidDashTarget(pawn, target.Cell))
            {
                GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
            }

            // 绘制根据障碍更新的范围圈
            GenDraw.DrawRadiusRing(
                pawn.Position,
                EffectiveRange,
                Color.white,
                (IntVec3 c) =>
                    GenSight.LineOfSight(pawn.Position, c, pawn.Map, true) &&
                    ValidDashTarget(pawn, c)
            );
        }
    }
}