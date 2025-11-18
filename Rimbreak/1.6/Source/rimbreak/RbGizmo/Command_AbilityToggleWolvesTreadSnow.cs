using RimWorld;
using UnityEngine;
using Verse;

namespace rimbreak.RbGizmo
{
    // 自定义能力按钮：根据 AbilityComp_WolvesTreadSnow 的开关状态动态显示“开启/关闭”
    public class Command_AbilityToggleWolvesTreadSnow : Command_Ability
    {
        public Command_AbilityToggleWolvesTreadSnow(Ability ability, Pawn pawn) : base(ability, pawn)
        {
        }

        protected override GizmoResult GizmoOnGUIInt(Rect butRect, GizmoRenderParms parms)
        {
            var comp = ability.CompOfType<rimbreak.RbAbilityComps.AbilityComp_WolvesTreadSnow>();
            if (comp != null)
            {
                if (comp.isOpen)
                {
                    defaultLabel = "群狼踏雪(关闭)";
                    defaultDesc = "关闭技能，开始回复雪息";
                }
                else
                {
                    defaultLabel = "群狼踏雪(开启)";
                    defaultDesc = "开启技能，开始消耗雪息进行攻击";
                }
            }
            return base.GizmoOnGUIInt(butRect, parms);
        }
    }
}


