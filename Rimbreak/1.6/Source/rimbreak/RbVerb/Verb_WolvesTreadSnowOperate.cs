using ChezhouLib.Utils;
using rimbreak.DefRef;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace rimbreak.RbVerb
{
    public class Verb_WolvesTreadSnowOperate : Verb_CastAbility
    {
        // 点击施放时调用，改为打开配置菜单并立即完成施法
        protected override bool TryCastShot()
        {
            Ability ability = verbTracker?.directOwner as Ability;
            if (ability == null)
            {
                return false;
            }
            AbilityComp targetComp = null;
            for (int i = 0; i < ability.comps.Count; i++)
            {
                if (ability.comps[i] is rimbreak.RbAbilityComps.AbilityComp_WolvesTreadSnow)
                {
                    targetComp = ability.comps[i];
                    break;
                }
            }
            var comp = targetComp as rimbreak.RbAbilityComps.AbilityComp_WolvesTreadSnow;
            if (comp == null)
            {
                return false;
            }
            Pawn pawn = CasterPawn;
            if (pawn == null || pawn.Dead || !pawn.Spawned)
                return false;
            // 切换开启状态
            comp.isOpen = !comp.isOpen;
            // 可选：切换开启时立即进入冷却（遵循XML冷却设置）
            if (this.Ability != null)
            {
                int cd = this.Ability.def.cooldownTicksRange.RandomInRange;
                if (cd > 0)
                {
                    this.Ability.StartCooldown(cd);
                }
            }
            return true; // 视为成功施放（用于切换状态）
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true) => true;
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ) => true;
        public override bool Targetable => false;
    }
}
