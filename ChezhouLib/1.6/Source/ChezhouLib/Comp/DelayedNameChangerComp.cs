using ChezhouLib.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static UnityEngine.Networking.UnityWebRequest;

namespace ChezhouLib.Comp
{
    public class DelayedNameChangerComp : ThingComp
    {
        private int ticksRemaining = -1; // 剩余的tick数

        
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            ticksRemaining = 3;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", -1);
        }

        public override void CompTick()
        {
            // 如果已经初始化并且计时器正在运行
            if (ticksRemaining > 0)
            {
                ticksRemaining--;

                // 计时器结束
                if (ticksRemaining == 0)
                {
                    ChangeNameAndRemove();
                }
            }
        }

        private void ChangeNameAndRemove()
        {
            Pawn pawn = this.parent as Pawn;
            if (pawn == null || pawn.kindDef == null)
                return;

            kindRule ext = pawn.kindDef.GetModExtension<kindRule>();
            if (ext == null)
                return;
            if(ext.kindName != null)
            {
                pawn.Name = new NameTriple(ext.kindName,ext.kindName,ext.kindName);
            }
            // 任务完成，自我销毁，避免不必要的性能开销
            pawn?.AllComps.Remove(this);
        }
    }
}
