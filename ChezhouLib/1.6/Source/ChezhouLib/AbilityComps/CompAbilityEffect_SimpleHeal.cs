using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ChezhouLib.AbilityComps
{
    // 能力组件的属性定义：在 XML 中配置该组件的参数
    public class CompProperties_AbilityEffect_SimpleHeal : CompProperties_AbilityEffect
    {
        // 每次应用时对每个非永久性伤口恢复的数值
        public float healPerInjury = 5f;

        // 是否作用于施法者自身
        public bool applyToSelf = false;

        // 是否作用于目标
        public bool applyToTarget = true;

        public CompProperties_AbilityEffect_SimpleHeal()
        {
            // 指定实际的组件类
            compClass = typeof(CompAbilityEffect_SimpleHeal);
        }
    }

    // 能力组件逻辑：执行实际效果
    public class CompAbilityEffect_SimpleHeal : CompAbilityEffect
    {
        public new CompProperties_AbilityEffect_SimpleHeal Props
        {
            get { return (CompProperties_AbilityEffect_SimpleHeal)props; }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            // 目标必须是一个 Pawn；保留父类的其他检查
            if (target.Pawn == null)
            {
                return false;
            }
            return base.CanApplyOn(target, dest);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            // 根据配置，对施法者与目标分别尝试应用
            if (Props.applyToTarget && target.Pawn != null)
            {
                ApplyHealToPawn(target.Pawn);
            }
            if (Props.applyToSelf && parent?.pawn != null)
            {
                ApplyHealToPawn(parent.pawn);
            }
        }

        private void ApplyHealToPawn(Pawn pawn)
        {
            if (pawn == null || pawn.health == null)
            {
                return;
            }

            // 遍历所有 Hediff，选择非永久性伤口并进行数值恢复
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            if (hediffs == null || hediffs.Count == 0)
            {
                return;
            }

            float amount = Props.healPerInjury;
            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff_Injury injury = hediffs[i] as Hediff_Injury;
                if (injury != null && !injury.IsPermanent())
                {
                    // 使用 Hediff_Injury 自带的 Heal 方法，按数值恢复该伤口
                    injury.Heal(amount);
                }
            }

            // 反馈提示信息（可选）
            MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "Healed".Translate());
        }
    }
}


