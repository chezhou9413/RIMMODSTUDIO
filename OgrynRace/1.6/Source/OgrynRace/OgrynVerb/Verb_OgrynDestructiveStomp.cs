using OgrynRace.DefRef;
using RimWorld;
using Verse;
using Verse.AI;

namespace OgrynRace.OgrynVerb
{
    /// <summary>
    /// Ogryn破碎践踏 —— 点击即施放（不弹出目标选择器）
    /// </summary>
    public class Verb_OgrynDestructiveStomp : Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            bool casted = base.TryCastShot();
            if (!casted)
            {
                return false;
            }
            Pawn pawn = CasterPawn;
            if (pawn == null || pawn.Dead || !pawn.Spawned)
                return false;
            Hediff hediff = HediffMaker.MakeHediff(OgrynHediffDef.Ogryn_OgrynStomp, pawn);
            pawn.health.AddHediff(hediff);
            // 触发冷却
            if (this.Ability != null)
            {
                int cd = this.Ability.def.cooldownTicksRange.RandomInRange;
                if (cd > 0)
                    this.Ability.StartCooldown(cd);
            }
            return true;
        }

        // 不需要目标
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true) => true;
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ) => true;
        public override bool Targetable => false;
    }
}
