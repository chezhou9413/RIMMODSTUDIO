using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OgrynRace.OgrynVerb
{
   public class Verb_OgrynDisableAttack : Verb_CastAbility
    {

        public override bool MultiSelect => true;

        // 计算有效射程（可兼容装备的 JumpRange）

        // 主逻辑：尝试施放技能
        protected override bool TryCastShot()
        {

            // 调用基类逻辑 → 触发 ability.Activate() → 自动执行消耗、冷却等
            bool result = base.TryCastShot();

            // 只有技能实际释放成功后再执行自定义逻辑
            if (result && CasterPawn != null && currentTarget.Thing is Pawn targetPawn)
            {
                // 自定义效果：例如添加Job
                Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Ogryn_OgrynDisableAttack_Job"), targetPawn);
                job.playerForced = true;
                CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }

            return result; // 保留系统的返回值
        }

        // 鼠标悬停时显示技能UI反馈
        public override void OnGUI(LocalTargetInfo target)
        {
            base.OnGUI(target);
        }

        // 强制指令时调用（Shift+右键）
        public override void OrderForceTarget(LocalTargetInfo target)
        {
           base.OrderForceTarget(target);
        }

        // 允许敌对或中立 Pawn 作为目标（排除自己与友军）
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            base.ValidateTarget(target, showMessages);
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

        // 动态绘制射程圈 + 遮挡检测
        public override void DrawHighlight(LocalTargetInfo target)
        {
            base.DrawHighlight(target);
        }
    }

}
