using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace rimbreak.Utility
{
    public static class MoteUtility
    {
        public static Mote TryAttachMote(Thing target, Map map, string moteDefName, float scale = 1f, Vector3? offset = null)
        {
            if (target == null || map == null || string.IsNullOrEmpty(moteDefName))
                return null;

            var moteDef = DefDatabase<ThingDef>.GetNamed(moteDefName, false);
            if (moteDef == null)
                return null;

            Vector3 actualOffset = offset ?? Vector3.zero;

            // 优先创建附着Mote
            var mote = MoteMaker.MakeAttachedOverlay(target, moteDef, actualOffset, scale);
            if (mote != null)
                return mote;

            // 获取精确位置：Projectile等有ExactPosition，否则用Position
            Vector3 pos = target is Projectile proj ? proj.ExactPosition : target.Position.ToVector3Shifted();

            // 创建静态Mote
            return MoteMaker.MakeStaticMote(pos, map, moteDef, scale, true);
        }
    }
}
