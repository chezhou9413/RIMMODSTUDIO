using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class SZContainers
{
	internal static bool DrawElementStack<T>(Rect rect, List<T> l, bool bRemoveOnClick, Action<T> removeAction, Func<T, Def> defGetter = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (l.NullOrEmpty())
		{
			return false;
		}
		try
		{
			GenUI.DrawElementStack(rect, 32f, l, delegate(Rect r, T def)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				GUI.DrawTexture(r, (Texture)(object)BaseContent.ClearTex);
				if (Mouse.IsOver(r))
				{
					Widgets.DrawHighlight(r);
					string text = def.STooltip();
					TipSignal tip = new TipSignal(text, 987654);
					TooltipHandler.TipRegion(r, tip);
				}
				Texture2D tIcon = def.GetTIcon();
				tIcon = tIcon ?? BaseContent.BadTex;
				if (Widgets.ButtonImage(r, tIcon, doMouseoverSound: false))
				{
					if (bRemoveOnClick)
					{
						removeAction(def);
						throw new Exception("removed");
					}
					if (defGetter != null)
					{
						WindowTool.Open(new Dialog_InfoCard(defGetter(def)));
					}
				}
			}, (T def) => 32f, 4f, 5f, allowOrderOptimization: false);
		}
		catch
		{
		}
		return true;
	}
}
