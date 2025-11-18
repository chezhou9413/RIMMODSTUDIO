using OgrynRace.comp;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OgrynRace.OgrynGizmo
{
    public class Gizmo_Angerbar : Gizmo
    {
        public Pawn pawn;   // 要显示的 pawn
        public OgrynAngerComp comp;
        public Gizmo_Angerbar(Pawn pawn)
        {
            this.pawn = pawn;
        }

        public override float GetWidth(float maxWidth)
        {
            return 200f; // 固定宽度
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            if (comp == null) { 
                comp = pawn.GetComp<OgrynAngerComp>();
            }
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f); // 75 高度，适合放一条进度条
            Widgets.DrawWindowBackground(rect);
            // 内部留点边距
            Rect barRect = rect.ContractedBy(20f);
            float fillPct = (float)comp.CurrentAnger / (float)Math.Max(1, comp.maxAnger);
            // 绘制进度条
            Widgets.FillableBar(barRect, fillPct,
                SolidColorMaterials.NewSolidColorTexture(Color.red), // 填充颜色
                BaseContent.BlackTex, false);
            // 在进度条上画文字
            string label = comp.Props.AngerUIName + ":"+comp.CurrentAnger+"/"+comp.maxAnger;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(barRect, label);
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect,comp.Props.AngerUIDes);
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
