using rimbreak.DefRef;
using rimbreak.RbAbilityComps;
using rimbreak.Utility;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace rimbreak.RbGizmo
{
    public class Gizmo_SnowBreath : Gizmo
    {
        public Pawn pawn;   // 要显示的 pawn
        public AbilityComp_WolvesTreadSnow comp;
        public Gizmo_SnowBreath(Pawn pawn)
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
                comp = AbilityCompGet.GetAbilityComp<AbilityComp_WolvesTreadSnow>(pawn,RbAbility.lyfe_WolvesTreadSnow);
            }
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f); // 75 高度，适合放一条进度条
            Widgets.DrawWindowBackground(rect);
            // 内部留点边距
            float barWidth = 180f;
            float barHeight = 30f;
            float centerX = rect.x + rect.width / 2f;
            float centerY = rect.y + rect.height / 2f;
            Rect barRect = new Rect(centerX - barWidth / 2f, centerY - barHeight / 2f, barWidth, barHeight);
            float fillPct = (float)comp.curentBreath / (float)Math.Max(1, comp.maxSnowBreath);
            Color frozenStart = new Color(0.8f, 1f, 1f);
            // 绘制渐变效果的进度条
            Widgets.FillableBar(barRect, fillPct,
                SolidColorMaterials.NewSolidColorTexture(frozenStart),
                BaseContent.BlackTex, false);
            string label = comp.Props.UIName + ":" + comp.curentBreath + "/" + comp.maxSnowBreath;
            Text.Anchor = TextAnchor.MiddleCenter;
            // 仅本控件内将文字改为绿色，并在绘制后恢复，避免污染全局颜色
            Color prevContentColor = GUI.contentColor;
            GUI.contentColor = Color.green;
            Widgets.Label(barRect, label);
            GUI.contentColor = prevContentColor;
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect, comp.Props.UIDes);
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
