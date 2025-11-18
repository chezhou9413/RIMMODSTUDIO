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
    public class Verb_Jin_PaidJustice : Verb_CastAbility
    {
        private float cachedEffectiveRange = -1f;
        public override bool MultiSelect => true;

        // 计算有效射程（兼容装备 JumpRange）
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

        protected override bool TryCastShot()
        {
         
            Pawn caster = CasterPawn;
            if (caster == null || !currentTarget.IsValid)
            {
                return false;
            }

            Map map = caster.Map;
            IntVec3 startCell = caster.Position;
            float maxRange = EffectiveRange;
            Vector3 dir = (currentTarget.Cell - startCell).ToVector3();
            if (dir.sqrMagnitude < 0.001f) dir = Vector3.forward; // 防止零向量
            dir.Normalize();
            Vector3 endVec = startCell.ToVector3Shifted() + dir * maxRange;
            IntVec3 targetCell = IntVec3.FromVector3(endVec);
            JobDef dashJobDef = DefDatabase<JobDef>.GetNamed("Rb_jin_PaidJustice", false);
            if (dashJobDef == null)
            {
                Log.ErrorOnce("[Verb_Jin_PaidJustice] 找不到 JobDef 'RB_PaidJustice_Dash'", 123458);
                return false;
            }
            Job dashJob = JobMaker.MakeJob(dashJobDef, new LocalTargetInfo(targetCell));
            caster.jobs.TryTakeOrderedJob(dashJob, JobTag.Misc);
            TryPlayMuzzleEffect(map, caster.DrawPos, dir);
            if (Ability?.def?.cooldownTicksRange != null)
            {
                if (CasterPawn.health.hediffSet.HasHediff(RbHediffDef.RB_BladeBody))
                {
                   int cd = (int)(Ability.def.cooldownTicksRange.RandomInRange*0.1f);
                    if (cd > 0) Ability.StartCooldown(cd);
                }
                else
                {
                    int cd = Ability.def.cooldownTicksRange.RandomInRange;
                    if (cd > 0) Ability.StartCooldown(cd);
                }
            }
            return true;
        }

        /// <summary>
        /// 播放发射口特效
        /// </summary>
        private void TryPlayMuzzleEffect(Map map, Vector3 origin, Vector3 dir)
        {
            FleckDef fleckDef = DefDatabase<FleckDef>.GetNamed("PaidJusticeFleck", false);
            if (fleckDef == null)
            {
                return;
            }
            // 计算特效位置
            Vector3 fleckPos = origin + dir * 0f;
            float angle = dir.AngleFlat();

            FleckCreationData data = FleckMaker.GetDataStatic(fleckPos, map, fleckDef, 3.0f);
            data.rotation = angle;
            map.flecks.CreateFleck(data);
        }

        // 鼠标悬停的技能 UI 反馈
        public override void OnGUI(LocalTargetInfo target)
        {
            if (CanHitTarget(target) && ValidDashTarget(CasterPawn, target.Cell))
                base.OnGUI(target);
            else
                GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            Pawn caster = CasterPawn;
            if (caster == null || !target.IsValid)
                return false;

            // 支持任意地块
            IntVec3 cell = target.Cell;

            // 确保地块在地图范围内
            if (!cell.InBounds(caster.Map))
                return false;

            // 限制射程
            float dist = caster.Position.DistanceTo(cell);
            if (dist > EffectiveRange)
                return false;

            return true;
        }


        // 用于射线检测与射程判断
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (!targ.IsValid) return false;

            Map map = this.CasterPawn.Map;
            if (!targ.Cell.InBounds(map)) return false;

            // 仅限制射程
            float distance = root.DistanceTo(targ.Cell);
            if (distance > EffectiveRange)
                return false;

            return true;
        }
        private bool ValidDashTarget(Pawn pawn, IntVec3 cell)
        {
            if (!cell.Walkable(pawn.Map)) return false;
            if (!pawn.CanReach(cell, PathEndMode.OnCell, Danger.Deadly)) return false;
            return true;
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            Pawn pawn = CasterPawn;
            if (pawn == null) return;
            Map map = pawn.Map;
            if (!target.IsValid || !target.Cell.InBounds(map)) return;

            IntVec3 start = pawn.Position;
            float maxRange = EffectiveRange;

            // 计算方向
            Vector3 dir = (target.Cell - start).ToVector3();
            if (dir.magnitude < 0.01f) dir = Vector3.forward; // 防止零向量

            dir.Normalize();

            // 计算最大射程端点
            Vector3 endVec = start.ToVector3Shifted() + dir * maxRange;
            IntVec3 end = IntVec3.FromVector3(endVec);

            // 生成路径格子
            List<IntVec3> lineCells = GenSight.BresenhamCellsBetween(start, end);

            // 线宽
            float lineWidth = 2f;
            HashSet<IntVec3> affectedCells = new HashSet<IntVec3>();

            foreach (IntVec3 c in lineCells)
            {
                foreach (IntVec3 near in GenRadial.RadialCellsAround(c, lineWidth, true))
                {
                    if (near.InBounds(map))
                        affectedCells.Add(near);
                }
            }

            // 绘制方向线
            GenDraw.DrawFieldEdges(affectedCells.ToList(), new Color(0.7f, 1f, 1f));

            // 绘制最大射程圈
            GenDraw.DrawRadiusRing(pawn.Position, maxRange, new Color(1f, 1f, 1f, 0.3f));

            // 绘制目标点
            GenDraw.DrawTargetHighlight(end);
        }

    }
}