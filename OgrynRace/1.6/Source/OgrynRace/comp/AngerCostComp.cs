using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;
using Verse.Noise;
using static RimWorld.PsychicRitualRoleDef;
using static UnityEngine.GraphicsBuffer;

namespace OgrynRace.comp
{
    public class AngerCostCompProperties:CompProperties_AbilityEffect
    {    
        public int angerAmount = 10;
        public AngerCostCompProperties()
        {
            this.compClass = typeof(AngerCostComp);
        }
    }

    public class AngerCostComp : CompAbilityEffect
    {
        public new AngerCostCompProperties Props => (AngerCostCompProperties)props;
        private OgrynAngerComp OgrynAngerComp => parent.pawn.TryGetComp<OgrynAngerComp>();
        public override bool GizmoDisabled(out string reason)
        {
            Pawn pawn = parent.pawn;
            if (pawn.def.defName != "Ogryn")
            {
                reason = "Ogryn_SkillNotAvailable".Translate();
                return true;
            }
            // 示例：当 Pawn 拥有某个 Hediff（比如 Debuff）时禁用
            if (OgrynAngerComp.CurrentAnger < Props.angerAmount)
            {
                int needed = Props.angerAmount - OgrynAngerComp.CurrentAnger;
                reason = "Ogryn_AngerNotEnough".Translate(needed);
                return true;
            }
            reason = null;
            return false;
        }
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (OgrynAngerComp.CurrentAnger < Props.angerAmount)
            {
                return false;
            }
            return base.Valid(target, throwMessages);
        }
        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            OgrynAngerComp.addOgrynAnger(-Props.angerAmount);
            base.PostApplied(targets, map);
        }
    }
}
