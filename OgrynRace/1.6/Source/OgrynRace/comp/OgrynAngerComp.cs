using HarmonyLib;
using OgrynRace.DefRef;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static UnityEngine.Networking.UnityWebRequest;
using static Verse.DamageWorker;

namespace OgrynRace.comp
{
    public class OgrynAngerCompProperties : CompProperties
    {
        public int maxAnger = 0;
        public int minAnger = 100;
        public string AngerUIName;
        public string AngerUIDes;
        public OgrynAngerCompProperties()
        {
            this.compClass = typeof(OgrynAngerComp);
        }
    }

    public class OgrynAngerComp : ThingComp
    {
        public int maxAnger = 0;
        public int CurrentAnger = 0;
        public int minAnger = 100;
        public int lastAnger = 10;
        private int tickCounter = 0;
        public override void CompTick()
        {
            base.CompTick();
            tickCounter++;
            if (tickCounter >= 60)
            {
                tickCounter = 0;
                lastAnger -= 1;
                if(lastAnger <= 0)
                {
                    if (CurrentAnger > 0)
                    {
                        CurrentAnger += -1;
                    }
                }
            }
        }
        public OgrynAngerCompProperties Props => (OgrynAngerCompProperties)this.props;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.maxAnger = Props.maxAnger;
            this.minAnger = Props.minAnger;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref maxAnger, "maxAnger", 0);
            Scribe_Values.Look(ref CurrentAnger, "CurrentAnger", 0);
            Scribe_Values.Look(ref minAnger, "minAnger", 100);
        }

        public void addOgrynAnger(int value)
        {
            this.CurrentAnger += value;
            if (CurrentAnger < minAnger)
            {
                this.CurrentAnger = minAnger;
            }
            if (CurrentAnger > maxAnger)
            {
                this.CurrentAnger = maxAnger;
            }
            lastAnger = 10;
			Pawn pawn = this.parent as Pawn;
			if (pawn != null && !pawn.Dead && CurrentAnger > 0)
			{
				if ((pawn.health == null || pawn.health.hediffSet == null || !pawn.health.hediffSet.HasHediff(OgrynHediffDef.Ogryn_Ange)))
				{
					// 只在未拥有时添加一次
					Hediff hediff = HediffMaker.MakeHediff(OgrynHediffDef.Ogryn_Ange, pawn);
					pawn.health.AddHediff(hediff);
				}
			}
        }
    }

    [HarmonyPatch(typeof(Verb_MeleeAttack), "TryCastShot")]
    public static class Patch_VerbMeleeAttack_TryCastShot
    {
        static void Postfix(bool __result, Verb_MeleeAttack __instance)
        {
            if (__result && __instance.CasterPawn != null)
            {
                Pawn attacker = __instance.CasterPawn;
                if (attacker != null)
                {
                    if (attacker != null)
                    {
                        if (attacker.def.defName == "Ogryn")
                        {
                            attacker.TryGetComp<OgrynAngerComp>().addOgrynAnger(3);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Thing), "TakeDamage")]
        public static class Patch_Thing_TakeDamage
        {
            static void Postfix(Thing __instance, DamageInfo dinfo, DamageResult __result)
            {
                Pawn pawn = __instance as Pawn;
                if (pawn != null && !pawn.Dead)
                {
                    if (pawn != null)
                    {
                        if (pawn.def.defName == "Ogryn")
                        {
                            pawn.TryGetComp<OgrynAngerComp>().addOgrynAnger(2);
                        }
                    }
                }
            }
        }
    }
}
