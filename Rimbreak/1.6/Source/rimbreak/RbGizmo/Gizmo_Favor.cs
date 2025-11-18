using rimbreak.DefRef;
using rimbreak.RbAbilityComps;
using rimbreak.Utility;
using System;
using UnityEngine;
using Verse;

namespace rimbreak.RbGizmo
{
    public class Gizmo_Favor : Gizmo
    {
        public Pawn pawn;   // 要显示的 pawn
        public AbilityComp_BestState comp;
        public Gizmo_Favor(Pawn pawn)
        {
            this.pawn = pawn;
        }

        public override float GetWidth(float maxWidth)
        {
            return 200f; // 固定宽度
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            if (comp == null)
            {
                comp = AbilityCompGet.GetAbilityComp<AbilityComp_BestState>(pawn, RbAbility.fenny_BestState);
            }
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f); // 75 高度，适合放一条进度条
            Widgets.DrawWindowBackground(rect);
            // 内部留点边距
            float barWidth = 180f;
            float barHeight = 30f;
            float centerX = rect.x + rect.width / 2f;
            float centerY = rect.y + rect.height / 2f;
            Rect barRect = new Rect(centerX - barWidth / 2f, centerY - barHeight / 2f, barWidth, barHeight);
            float fillPct = (float)comp.curentFavor / (float)Math.Max(1, comp.maxFavor);
            Color frozenStart = new Color(1f, 1f, 0f);
            Widgets.FillableBar(barRect, fillPct,
                SolidColorMaterials.NewSolidColorTexture(frozenStart),
                BaseContent.BlackTex, false);
            string label = comp.Props.UIName + ":" + Math.Round(comp.curentFavor, 1) + "/" + comp.maxFavor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Color prevContentColor = GUI.contentColor;
            GUI.contentColor = Color.blue;
            Widgets.Label(barRect, label);
            GUI.contentColor = prevContentColor;
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect, comp.Props.UIDes);
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
