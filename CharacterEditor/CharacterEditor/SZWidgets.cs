using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CharacterEditor;

internal static class SZWidgets
{
	private static int iUID = -1;

	internal const string CO_ITEMICON = "iconTex";

	internal static bool bCountOpen = false;

	internal static bool bQualityOpen = false;

	internal static bool bStuffOpen = false;

	internal static bool bStyleOpen = false;

	internal static List<string> lSimilar = new List<string>();

	internal static string sFind = "";

	internal static string sFindOld = "";

	internal static string sFindTemp = "";

	internal static string tDefName = null;

	internal static int waitTimer = 0;

	internal static int iLabelId = -1;

	internal static bool bToggleSearch = false;

	internal static bool bFocusOnce = true;

	private static int iShowId = 0;

	private static string tempText = "";

	private static int iTempTextID = 1000;

	internal static bool bRemoveOnClick = false;

	internal static Color RemoveColor => bRemoveOnClick ? Color.red : Color.white;

	private static void DefSelectorSimpleBase<T>(Rect r, int w, HashSet<T> l, ref T def, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, bool hasLIcon = false, Action<T> onLIcon = null, bool drawLabel = true) where T : Def
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref r)).width = w;
		Text.Font = GameFont.Small;
		if (drawLabel)
		{
			Rect rect = r.RectLabel(hasLR, hasLIcon, hasRightIcon: false);
			LabelBackground(rect, labelInfo + labelGetter(def), ColorTool.colAsche);
			string toolTip = FLabel.DefDescription(def);
			ButtonInvisibleMouseOver(rect, delegate
			{
				FloatMenuOnRect(l, labelGetter, onSelect);
			}, toolTip);
		}
		if (!hasLR)
		{
			return;
		}
		if (Widgets.ButtonImage(RectPrevious(r), ContentFinder<Texture2D>.Get("bbackward")))
		{
			def = l.GetPrev(def);
			if (onSelect != null)
			{
				onSelect(def);
			}
		}
		if (Widgets.ButtonImage(RectNext(r), ContentFinder<Texture2D>.Get("bforward")))
		{
			def = l.GetNext(def);
			if (onSelect != null)
			{
				onSelect(def);
			}
		}
	}

	internal static void DefSelectorSimpleBullet<T>(Rect r, int posX, int posY, int w, HashSet<T> l, ref T def, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, Texture2D texLIcon = null, float angle = 0f, Action<T> onLIcon = null, bool drawLabel = true) where T : Def
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		DefSelectorSimpleBase(r, w, l, ref def, labelInfo, labelGetter, onSelect, hasLR, (Object)(object)texLIcon != (Object)null, onLIcon, drawLabel);
		if (!((Object)(object)texLIcon != (Object)null))
		{
			return;
		}
		if (!drawLabel)
		{
			ButtonInvisibleMouseOver(r, delegate
			{
				FloatMenuOnRect(l, labelGetter, onLIcon);
			}, FLabel.DefDescription(def));
		}
		else
		{
			ButtonInvisibleVar(r, onLIcon, def, def.STooltip());
		}
	}

	internal static void DefSelectorSimpleTex<T>(Rect r, int w, HashSet<T> l, ref T def, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, Texture2D texLIcon = null, Action<T> onLIcon = null, bool drawLabel = true, string tooltip = "") where T : Def
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		DefSelectorSimpleBase(r, w, l, ref def, labelInfo, labelGetter, onSelect, hasLR, (Object)(object)texLIcon != (Object)null, onLIcon, drawLabel);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(r, tooltip);
		}
		if (!((Object)(object)texLIcon != (Object)null))
		{
			return;
		}
		Rect rect = r.RectIconLeft(hasLR);
		if (!drawLabel)
		{
			Image(r.RectIconLeft(hasLR), texLIcon);
			ButtonInvisibleMouseOver(rect, delegate
			{
				FloatMenuOnRect(l, labelGetter, onLIcon);
			}, FLabel.DefDescription(def));
		}
		else
		{
			ButtonImageVar(rect, texLIcon, onLIcon, def, def.STooltip());
		}
	}

	internal static void DefSelectorSimple<T>(Rect r, int w, HashSet<T> l, ref T def, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, string texLIcon = null, Action<T> onLIcon = null, bool drawLabel = true, string tooltip = "") where T : Def
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		DefSelectorSimpleBase(r, w, l, ref def, labelInfo, labelGetter, onSelect, hasLR, texLIcon != null, onLIcon, drawLabel);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(r, tooltip);
		}
		if (texLIcon == null)
		{
			return;
		}
		Rect rect = r.RectIconLeft(hasLR);
		if (!drawLabel)
		{
			Image(r.RectIconLeft(hasLR), texLIcon);
			ButtonInvisibleMouseOver(rect, delegate
			{
				FloatMenuOnRect(l, labelGetter, onLIcon);
			}, FLabel.DefDescription(def));
		}
		else
		{
			ButtonImageVar(rect, texLIcon, onLIcon, def, def.STooltip());
		}
	}

	internal static void NonDefSelectorSimple<T>(Rect r, int w, HashSet<T> l, ref T val, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, string texLIcon = null, Action<T> onLIcon = null)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref r)).width = w;
		Text.Font = GameFont.Small;
		bool flag = texLIcon != null;
		Rect rect = r.RectLabel(hasLR, flag, hasRightIcon: false);
		LabelBackground(rect, labelInfo + labelGetter(val), ColorTool.colAsche);
		ButtonInvisibleMouseOver(rect, delegate
		{
			FloatMenuOnRect(l, labelGetter, onSelect);
		});
		if (hasLR)
		{
			if (Widgets.ButtonImage(RectPrevious(r), ContentFinder<Texture2D>.Get("bbackward")))
			{
				val = l.GetPrev(val);
				if (onSelect != null)
				{
					onSelect(val);
				}
			}
			if (Widgets.ButtonImage(RectNext(r), ContentFinder<Texture2D>.Get("bforward")))
			{
				val = l.GetNext(val);
				if (onSelect != null)
				{
					onSelect(val);
				}
			}
		}
		if (flag)
		{
			ButtonImageVar(r.RectIconLeft(hasLR), texLIcon, onLIcon, val);
		}
	}

	internal static Rect RectPlusY(this Rect rect, int y)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + (float)y, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
	}

	internal static Rect RectLablelI(this Rect rect, int inputW)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - (float)inputW - ((Rect)(ref rect)).height * 2f, ((Rect)(ref rect)).height);
	}

	internal static Rect RectInput(this Rect rect, int inputW)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - ((Rect)(ref rect)).height - (float)inputW, ((Rect)(ref rect)).y, (float)inputW, ((Rect)(ref rect)).height);
	}

	internal static Rect RectMinus(this Rect rect, int inputW)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - ((Rect)(ref rect)).height * 2f - (float)inputW, ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height);
	}

	internal static Rect RectPlus(this Rect rect)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - ((Rect)(ref rect)).height, ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height);
	}

	internal static Rect RectSlider(this Rect rect)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
	}

	internal static Rect RectLabel(this Rect rect, bool inEditMode, bool hasLeftIcon, bool hasRightIcon)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + (float)OffsetEditLeft(inEditMode) + OffsetIcon(hasLeftIcon, ((Rect)(ref rect)).height), ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - (float)OffsetEditBoth(inEditMode) - OffsetIcon(hasLeftIcon, ((Rect)(ref rect)).height) - OffsetIcon(hasRightIcon, ((Rect)(ref rect)).height), ((Rect)(ref rect)).height);
	}

	internal static Rect RectIconLeft(this Rect rect, bool inEditMode)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + (float)OffsetEditLeft(inEditMode), ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height);
	}

	internal static Rect RectIconRight(this Rect rect, bool inEditMode)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - (float)OffsetEditLeft(inEditMode) - ((Rect)(ref rect)).height, ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height);
	}

	private static int OffsetEditLeft(bool inEditMode)
	{
		return inEditMode ? 21 : 0;
	}

	private static int OffsetEditBoth(bool inEditMode)
	{
		return inEditMode ? 42 : 0;
	}

	private static float OffsetIcon(bool hasIcon, float h)
	{
		return hasIcon ? h : 0f;
	}

	private static void GetEditRect(Rect rLabel, out Rect rValue, out Rect rLeft)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		rLeft = new Rect(rLabel);
		((Rect)(ref rLeft)).width = ((Rect)(ref rLeft)).width - 80f;
		rValue = new Rect(rLabel);
		((Rect)(ref rValue)).x = ((Rect)(ref rValue)).x + ((Rect)(ref rLeft)).width;
		((Rect)(ref rValue)).width = 80f;
	}

	private static void GetEditRects(Rect rLabel, string label, out Rect rValue, out Rect rLeft, out Rect rRight)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Text.CalcSize(label);
		rValue = new Rect(rLabel);
		((Rect)(ref rValue)).x = ((Rect)(ref rValue)).x + (val.x + 15f);
		((Rect)(ref rValue)).width = 80f;
		rLeft = new Rect(rLabel);
		((Rect)(ref rLeft)).width = val.x + 15f;
		rRight = new Rect(rLabel);
		((Rect)(ref rRight)).x = ((Rect)(ref rValue)).x + ((Rect)(ref rValue)).width;
		((Rect)(ref rRight)).width = ((Rect)(ref rLabel)).width - ((Rect)(ref rRight)).x + (float)OffsetEditLeft(inEditMode: true);
	}

	private static void ToggleParameterSlider(int id)
	{
		iUID = ((iUID == id) ? (-1) : id);
	}

	internal static void LabelFloatZeroFieldSlider(Listing_X view, int w, int id, Func<float?, string> FLabelValue, ref float? value, float min, float max, int decimals)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		LabelFloatZeroFieldSlider(view.GetRect(22f), w, id, FLabelValue, ref value, min, max, decimals, view);
	}

	internal static void LabelFloatZeroFieldSlider(Rect r, int w, int id, Func<float?, string> FLabelValue, ref float? value, float min, float max, int decimals, Listing_X view = null)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		bool flag = iUID == id;
		int inputW = 80;
		((Rect)(ref r)).width = w;
		Rect rect = (flag ? r.RectLablelI(inputW) : r);
		Text.Font = GameFont.Small;
		LabelBackground(rect, FLabelValue(value), ColorTool.colAsche);
		ButtonInvisibleMouseOver(rect, delegate
		{
			ToggleParameterSlider(id);
		});
		if (flag)
		{
			if (Widgets.ButtonText(r.RectMinus(inputW), "-") && value.HasValue)
			{
				value -= 1f;
				value = (float)Math.Round(value.Value, decimals);
			}
			if (Widgets.ButtonText(r.RectPlus(), "+") && value.HasValue)
			{
				value += 1f;
				value = (float)Math.Round(value.Value, decimals);
			}
			Rect rect2 = view?.GetRect(((Rect)(ref r)).height) ?? r.RectSlider();
			float value2 = (value.HasValue ? value.Value : 0f);
			value2 = FloatSlider(rect2, value2, min, max, decimals);
			value2 = FloatField(r.RectInput(inputW), value2, float.MinValue, float.MaxValue);
			value = value2;
		}
		view?.Gap(2f);
	}

	internal static void LabelFloatFieldSlider(Listing_X view, int w, int id, Func<float, string> FLabelValue, ref float value, float min, float max, int decimals)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		LabelFloatFieldSlider(view.GetRect(22f), w, id, FLabelValue, ref value, min, max, decimals, view);
	}

	internal static void LabelFloatFieldSlider(Rect r, int w, int id, Func<float, string> FLabelValue, ref float value, float min, float max, int decimals, Listing_X view = null)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		bool flag = iUID == id;
		int inputW = 80;
		((Rect)(ref r)).width = w;
		Rect rect = (flag ? r.RectLablelI(inputW) : r);
		Text.Font = GameFont.Small;
		LabelBackground(rect, FLabelValue(value), ColorTool.colAsche);
		ButtonInvisibleMouseOver(rect, delegate
		{
			ToggleParameterSlider(id);
		});
		if (flag)
		{
			if (Widgets.ButtonText(r.RectMinus(inputW), "-"))
			{
				value -= 1f;
				value = (float)Math.Round(value, decimals);
			}
			if (Widgets.ButtonText(r.RectPlus(), "+"))
			{
				value += 1f;
				value = (float)Math.Round(value, decimals);
			}
			Rect rect2 = view?.GetRect(((Rect)(ref r)).height) ?? r.RectSlider();
			value = FloatSlider(rect2, value, min, max, decimals);
			value = FloatField(r.RectInput(inputW), value, float.MinValue, float.MaxValue);
		}
		view?.Gap(2f);
	}

	internal static void LabelIntFieldSlider(Listing_X view, int w, int id, Func<int, string> FLabelValue, ref int value, int min, int max)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		LabelIntFieldSlider(view.GetRect(22f), w, id, FLabelValue, ref value, min, max, view);
	}

	internal static void LabelIntFieldSlider(Rect r, int w, int id, Func<int, string> FLabelValue, ref int value, int min, int max, Listing_X view = null)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		bool flag = iUID == id;
		int inputW = 80;
		((Rect)(ref r)).width = w;
		Rect rect = (flag ? r.RectLablelI(inputW) : r);
		Text.Font = GameFont.Small;
		LabelBackground(rect, FLabelValue(value), ColorTool.colAsche);
		ButtonInvisibleMouseOver(rect, delegate
		{
			ToggleParameterSlider(id);
		});
		if (flag)
		{
			if (Widgets.ButtonText(r.RectMinus(inputW), "-"))
			{
				value--;
			}
			if (Widgets.ButtonText(r.RectPlus(), "+"))
			{
				value++;
			}
			Rect rect2 = view?.GetRect(((Rect)(ref r)).height) ?? r.RectSlider();
			value = IntSlider(rect2, value, min, max);
			value = IntField(r.RectInput(inputW), value, int.MinValue, int.MaxValue);
		}
		view?.Gap(2f);
	}

	internal static float FloatSlider(Rect rect, float value, float min, float max, int decimals)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (float)Math.Round(Widgets.HorizontalSlider(rect, value, min, max), decimals);
	}

	internal static int IntSlider(Rect rect, int value, int min, int max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (int)Widgets.HorizontalSlider(rect, value, min, max);
	}

	internal static float FloatField(Rect rect, float value, float min, float max)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		string text = value.ToString();
		if (text.EndsWith("."))
		{
			text += "0";
		}
		else if (!text.Contains("."))
		{
			text += ".0";
		}
		text = Widgets.TextField(rect, text, 32);
		if (text.EndsWith("."))
		{
			text += "0";
		}
		else if (!text.Contains("."))
		{
			text += ".0";
		}
		float result = 0f;
		float.TryParse(text, out result);
		value = ((result < min) ? min : ((!(result > max)) ? result : max));
		return value;
	}

	internal static int IntField(Rect rect, int value, int min, int max)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		string text = value.ToString();
		text = Widgets.TextField(rect, text, 32);
		int result = 0;
		int.TryParse(text, out result);
		value = ((result < min) ? min : ((result <= max) ? result : max));
		return value;
	}

	private static string GetFormattedValue(string format, float value)
	{
		switch (format)
		{
		case "%":
			return " [" + Math.Round(100f * value, 0) + " %]";
		case "s":
			return " [" + value + " s]";
		case "ticks":
			return " [" + value + " ticks]";
		case "rpm":
			return " [" + ((value == 0f) ? CharacterEditor.Label.INFINITE : Math.Round(60f / value * 60f, 0).ToString()) + " rpm]";
		case "cps":
			return " [" + ((value == 0f) ? CharacterEditor.Label.INFINITE : value.ToString()) + " cps]";
		case "cells":
			return " [" + value + " cells]";
		default:
			if (format.StartsWith("max"))
			{
				return " [" + value + "/" + format.SubstringFrom("max") + "]";
			}
			switch (format)
			{
			case "int":
				return " [" + (int)Math.Round(value) + "]";
			case "quadrum":
				return " [" + Enum.GetName(typeof(Quadrum), (int)value) + "]";
			case "addict":
				return " " + (100.0 - Math.Round(100f * value, 0)) + " %";
			case "high":
				return " " + value.ToStringPercent("F0");
			default:
				if (format.StartsWith("DEF"))
				{
					return " [" + format.SubstringFrom("DEF") + "]";
				}
				if (format.StartsWith("dauer"))
				{
					return " " + format.SubstringFrom("dauer");
				}
				if (format == "pain")
				{
					int num = (int)value;
					return (num == 0) ? CharacterEditor.Label.PAINLESS : ("PainCategory_" + HealthTool.ConvertSliderToPainCategory(num)).Translate().ToString();
				}
				if (format.StartsWith("comp"))
				{
					if (!format.Contains("%"))
					{
						return " " + value.ToStringPercent("F0") + " " + format.SubstringFrom("comp");
					}
					return " " + format.SubstringFrom("comp");
				}
				return " [" + value + "]";
			}
		}
	}

	internal static void FloatMenuOnButtonImage<T>(Rect rect, Texture2D tex, ICollection<T> l, Func<T, string> labelGetter, Action<T> action, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = ((!Mouse.IsOver(rect)) ? Color.white : GenUI.MouseoverColor);
		GUI.DrawTexture(rect, (Texture)(object)tex);
		GUI.color = Color.white;
		if (Widgets.ButtonInvisible(rect))
		{
			FloatMenuOnRect(l, labelGetter, action);
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void FloatMenuOnButtonInvisible<T>(Rect rect, ICollection<T> l, Func<T, string> labelGetter, Action<T> action, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonInvisible(rect))
		{
			FloatMenuOnRect(l, labelGetter, action);
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void FloatMenuOnButtonInvisibleColorDef(Rect rect, ICollection<ColorDef> l, Func<ColorDef, string> labelGetter, Action<ColorDef> action, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonInvisible(rect))
		{
			FloatMenuOnRectColorDef(l, labelGetter, action);
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void FloatMenuOnButtonStuffOrStyle<T>(Rect rectThing, Rect rectClickable, ICollection<T> l, Func<T, string> labelGetter, Selected s, Action<T> action) where T : Def
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rectClickable))
		{
			Widgets.DrawHighlight(rectClickable);
		}
		if (typeof(T) == typeof(ThingStyleDef))
		{
			GUI.DrawTexture(rectThing, (Texture)(object)IconForStyle(s));
			TooltipHandler.TipRegion(rectThing, s.style.STooltip());
		}
		else
		{
			GUI.DrawTexture(rectThing, (Texture)(object)IconForStuff(s));
			TooltipHandler.TipRegion(rectThing, s.stuff.STooltip());
		}
		if (Widgets.ButtonInvisible(rectClickable))
		{
			FloatMenuOnRect(l, labelGetter, action, s);
		}
	}

	internal static void FloatMenuOnButtonText<T>(Rect rect, string curVal, ICollection<T> l, Func<T, string> labelGetter, Action<T> action, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonText(rect, curVal))
		{
			FloatMenuOnRect(l, labelGetter, action);
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void FloatMenuOnLabel<T>(Rect rect, Color color, ICollection<T> l, Func<T, string> labelGetter, T selected, Action<T> action, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		LabelBackground(rect, labelGetter(selected), color);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawLightHighlight(rect);
		}
		FloatMenuOnButtonInvisible(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 20f, ((Rect)(ref rect)).height), l, labelGetter, action, toolTip);
	}

	internal static void FloatMenuOnLabelAndImage<T>(Rect rect, Color imgColor, string texPath, Pawn pawnForImage, Color lblColor, ICollection<T> l, Func<T, string> labelGetter, T selected, Action<T> action, Action imgAction, bool showFloatMenu = true) where T : Def
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawBoxSolid(rect, imgColor);
		if (pawnForImage != null)
		{
			RenderTexture val = PortraitsCache.Get(pawnForImage, new Vector2(((Rect)(ref rect)).width, ((Rect)(ref rect)).height), Rot4.South);
			GUI.DrawTexture(rect, (Texture)(object)val);
		}
		else
		{
			Image(rect, texPath);
		}
		if (showFloatMenu)
		{
			FloatMenuOnLabel(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y - 20f, ((Rect)(ref rect)).width, 20f), lblColor, l, labelGetter, selected, action);
		}
		if (Widgets.ButtonInvisible(rect))
		{
			imgAction?.Invoke();
		}
	}

	internal static List<FloatMenuOption> FloatMenuOnRectColorDef(ICollection<ColorDef> l, Func<ColorDef, string> labelGetter, Action<ColorDef> action)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		if (!l.EnumerableNullOrEmpty())
		{
			foreach (ColorDef element in l)
			{
				string label = labelGetter(element);
				FloatMenuOption floatMenuOption = new FloatMenuOption(label, delegate
				{
					if (action != null)
					{
						action(element);
					}
				});
				if (element != null)
				{
					floatMenuOption.SetFMOIcon(ContentFinder<Texture2D>.Get("bfavcolor"));
					floatMenuOption.iconColor = element.color;
					floatMenuOption.tooltip = element.STooltip();
				}
				list.Add(floatMenuOption);
			}
			WindowTool.Open(new FloatMenu(list));
		}
		return list;
	}

	internal static List<FloatMenuOption> FloatMenuOnRect<T>(ICollection<T> l, Func<T, string> labelGetter, Action<T> action, Selected s = null, bool doWindow = true)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		if (!l.EnumerableNullOrEmpty())
		{
			foreach (T element in l)
			{
				string label = labelGetter(element);
				FloatMenuOption floatMenuOption = new FloatMenuOption(label, delegate
				{
					if (action != null)
					{
						action(element);
					}
				});
				if (element != null)
				{
					floatMenuOption.SetFMOIcon(element.GetTIcon(s));
					floatMenuOption.iconColor = element.GetTColor();
					floatMenuOption.tooltip = element.STooltip();
				}
				list.Add(floatMenuOption);
			}
			if (doWindow)
			{
				WindowTool.Open(new FloatMenu(list));
			}
		}
		return list;
	}

	internal static void FloatMixedMenuOnButtonImage<T1, T2>(Rect rect, Texture2D tex, List<T1> l1, List<T2> l2, Func<T1, string> labelGetter1, Func<T2, string> labelGetter2, Action<T1> action1, Action<T2> action2, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = ((!Mouse.IsOver(rect)) ? Color.white : GenUI.MouseoverColor);
		GUI.DrawTexture(rect, (Texture)(object)tex);
		GUI.color = Color.white;
		if (Widgets.ButtonInvisible(rect))
		{
			List<FloatMenuOption> list = FloatMenuOnRect(l1, labelGetter1, action1, null, doWindow: false);
			list.AddRange(FloatMenuOnRect(l2, labelGetter2, action2, null, doWindow: false));
			WindowTool.Open(new FloatMenu(list));
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static Texture2D IconForStuff(Selected s)
	{
		return (s != null && s.stuff != null) ? Widgets.GetIconFor(s.stuff, s.stuff) : null;
	}

	internal static Texture2D IconForStyle(Selected s)
	{
		return (s != null && s.thingDef != null && s.stuff != null && s.style != null) ? Widgets.GetIconFor(s.thingDef, s.stuff, s.style) : null;
	}

	internal static Texture2D IconForStyleCustom(Selected s, ThingStyleDef style)
	{
		return (s != null && s.thingDef != null && s.stuff != null && style != null) ? Widgets.GetIconFor(s.thingDef, s.stuff, style) : null;
	}

	private static void SetFMOIcon(this FloatMenuOption fmo, Texture2D t)
	{
		if ((Object)(object)t != (Object)null)
		{
			fmo.SetMemberValue("iconTex", t);
		}
	}

	internal static void FlipTextureHorizontally(Texture2D original)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Color[] pixels = original.GetPixels();
		Color[] array = (Color[])(object)new Color[pixels.Length];
		int width = ((Texture)original).width;
		int height = ((Texture)original).height;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				array[i + j * width] = pixels[width - i - 1 + j * width];
			}
		}
		original.SetPixels(array);
		original.Apply();
	}

	internal static void FlipTextureVertically(Texture2D original)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Color[] pixels = original.GetPixels();
		Color[] array = (Color[])(object)new Color[pixels.Length];
		int width = ((Texture)original).width;
		int height = ((Texture)original).height;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				array[i + j * width] = pixels[i + (height - j - 1) * width];
			}
		}
		original.SetPixels(array);
		original.Apply();
	}

	internal static void ButtonImageTex(Rect rect, Texture2D tex, Action action)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)tex == (Object)null) && Widgets.ButtonImage(rect, tex))
		{
			action?.Invoke();
		}
	}

	internal static void ButtonImage(Rect rect, string texPath, Action action, string tooolTip = "")
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!texPath.NullOrEmpty())
		{
			if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath)))
			{
				action?.Invoke();
			}
			if (!tooolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooolTip);
			}
		}
	}

	internal static void ButtonImage(float x, float y, float w, float h, string texPath, Action action, string toolTip = "", Color col = default(Color))
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!texPath.NullOrEmpty())
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(x, y, w, h);
			if ((!(col == default(Color))) ? Widgets.ButtonImage(val, ContentFinder<Texture2D>.Get(texPath), col) : Widgets.ButtonImage(val, ContentFinder<Texture2D>.Get(texPath)))
			{
				action?.Invoke();
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(val, toolTip);
			}
		}
	}

	internal static void ButtonImageCol(Rect rect, string texPath, Action action, Color color, string toolTip = "")
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!texPath.NullOrEmpty())
		{
			if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath), color))
			{
				action?.Invoke();
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}
	}

	internal static void ButtonHighlight(Rect rect, string texPath, Action<Color> action, Color color, string toolTip = "")
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!texPath.NullOrEmpty())
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawBoxSolid(rect, new Color(color.r, color.g, color.b, 0.4f));
			}
			if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath), color, color))
			{
				action?.Invoke(color);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}
	}

	internal static void ButtonHighlight(float x, float y, float w, float h, string texPath, Action<Color> action, Color color, string toolTip = "")
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!texPath.NullOrEmpty())
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(x, y, w, h);
			if (Mouse.IsOver(val))
			{
				Widgets.DrawBoxSolid(val, new Color(color.r, color.g, color.b, 0.4f));
			}
			if (Widgets.ButtonImage(val, ContentFinder<Texture2D>.Get(texPath), color, color))
			{
				action?.Invoke(color);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(val, toolTip);
			}
		}
	}

	internal static void ButtonImageCol(float x, float y, float w, float h, string texPath, Action<Color> action, Color color, string toolTip = "")
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!texPath.NullOrEmpty())
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(x, y, w, h);
			if (Widgets.ButtonImage(val, ContentFinder<Texture2D>.Get(texPath), color))
			{
				action?.Invoke(color);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(val, toolTip);
			}
		}
	}

	internal static void ButtonImageCol2<T>(Rect rect, string texPath, Action<T> action, T value, Color color, string toolTip = "")
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!texPath.NullOrEmpty())
		{
			if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath), color))
			{
				action?.Invoke(value);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}
	}

	internal static void ButtonImageVar<T>(Rect rect, Texture2D tex, Action<T> action, T value, string toolTip = "")
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)tex == (Object)null))
		{
			if (Widgets.ButtonImage(rect, tex))
			{
				action?.Invoke(value);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}
	}

	internal static void ButtonImageVar<T>(Rect rect, string texPath, Action<T> action, T value, string toolTip = "")
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!texPath.NullOrEmpty())
		{
			if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath)))
			{
				action?.Invoke(value);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}
	}

	internal static void ButtonImageVar<T>(float x, float y, float w, float h, string texPath, Action<T> action, T value, string toolTip = "")
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!texPath.NullOrEmpty())
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(x, y, w, h);
			if (Widgets.ButtonImage(val, ContentFinder<Texture2D>.Get(texPath)))
			{
				action?.Invoke(value);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(val, toolTip);
			}
		}
	}

	internal static void ButtonInvisible(Rect rect, Action action, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonInvisible(rect))
		{
			action?.Invoke();
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void ButtonInvisibleMouseOver(Rect rect, Action action, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		if (Widgets.ButtonInvisible(rect))
		{
			action?.Invoke();
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void ButtonInvisibleMouseOverVar<T>(Rect rect, Action<T> action, T val, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		if (Widgets.ButtonInvisible(rect))
		{
			action?.Invoke(val);
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void ButtonInvisibleVar<T>(Rect rect, Action<T> action, T value, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonInvisible(rect))
		{
			action?.Invoke(value);
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void ButtonSolid(Rect rect, Color color, Action action, string tooltip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawRectFast(rect, color);
		ButtonInvisible(rect, action, tooltip);
		GUI.color = Color.white;
	}

	internal static void ButtonText(Rect rect, string label, Action action, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonText(rect, label))
		{
			action?.Invoke();
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void ButtonText(float x, float y, float w, float h, string label, Action action, string toolTip = "")
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, w, h);
		if (Widgets.ButtonText(rect, label))
		{
			action?.Invoke();
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}

	internal static void ButtonTextureTextHighlight(Rect rect, string text, Texture2D icon, Color color, Action action, string toolTip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		GUI.color = color;
		GUI.DrawTexture(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height), (Texture)(object)icon);
		GUI.color = Color.white;
		Text.Font = GameFont.Small;
		float num = ((Rect)(ref rect)).height - 10f - ((Rect)(ref rect)).height / 2f;
		Widgets.Label(new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).height + 5f, ((Rect)(ref rect)).y + num, ((Rect)(ref rect)).width - ((Rect)(ref rect)).height - 5f, ((Rect)(ref rect)).height), text);
		ButtonInvisible(rect, action, toolTip);
	}

	internal static void ButtonTextureTextHighlight2(Rect rect, string text, string texPath, Color color, Action action, string toolTip = "", bool withButton = true, float textOffset = 10f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		if (texPath != null)
		{
			GUI.color = color;
			GUI.DrawTexture(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height), (Texture)(object)ContentFinder<Texture2D>.Get(texPath));
			GUI.color = Color.white;
		}
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		float num = ((Rect)(ref rect)).height - textOffset - ((Rect)(ref rect)).height / 2f;
		if (texPath == null)
		{
			Widgets.Label(rect, text);
		}
		else
		{
			Widgets.Label(new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).height + 5f, ((Rect)(ref rect)).y + num, ((Rect)(ref rect)).width - ((Rect)(ref rect)).height - 5f, ((Rect)(ref rect)).height), text);
		}
		Text.Anchor = (TextAnchor)0;
		if (withButton)
		{
			ButtonInvisible(rect, action, toolTip);
		}
	}

	internal static void ButtonTextVar<T>(float x, float y, float w, float h, string label, Action<T> action, T value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, w, h);
		if (Widgets.ButtonText(rect, label))
		{
			action?.Invoke(value);
		}
	}

	internal static void ButtonThingVar<T>(Rect rect, T val, Action<T> action, string tooltip)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		ThingDrawer(rect, val as Thing);
		ButtonInvisibleVar(rect, action, val, tooltip);
	}

	internal static void CheckBoxOnChange(Rect rect, string label, bool checkState, Action<bool> action)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		bool flag = checkState;
		Widgets.CheckboxLabeled(rect, label, ref checkState);
		if (flag != checkState)
		{
			action?.Invoke(checkState);
		}
	}

	internal static void ColorBox(Rect rect, Color col, Action<Color> action, bool halfAlfa = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		Listing_X listing_X = new Listing_X();
		listing_X.Begin(rect);
		GUI.color = Color.red;
		Type aType = Reflect.GetAType("Verse", "Widgets");
		aType.SetMemberValue("RangeControlTextColor", Color.red);
		float num = listing_X.Slider(col.r, 0f, ColorTool.IMAX);
		GUI.color = Color.green;
		aType.SetMemberValue("RangeControlTextColor", Color.green);
		float num2 = listing_X.Slider(col.g, 0f, ColorTool.IMAX);
		GUI.color = Color.blue;
		aType.SetMemberValue("RangeControlTextColor", Color.blue);
		float num3 = listing_X.Slider(col.b, 0f, ColorTool.IMAX);
		GUI.color = Color.white;
		aType.SetMemberValue("RangeControlTextColor", Color.white);
		float num4 = listing_X.Slider(col.a, halfAlfa ? 0.49f : 0f, ColorTool.IMAX);
		bool flag = col.r != num || col.g != num2 || col.b != num3 || col.a != num4;
		listing_X.End();
		if (flag)
		{
			action?.Invoke(new Color(num, num2, num3, num4));
		}
	}

	internal static void Image(Rect rect, string texPath)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)ContentFinder<Texture2D>.Get(texPath));
	}

	internal static void Image(Rect rect, Texture2D tex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)tex);
	}

	internal static void LabelEdit(Rect rect, int id, string text, ref string value, GameFont font, bool capitalize = false)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = font;
		Widgets.DrawBoxSolid(rect, ColorTool.colAsche);
		if (iLabelId != id)
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(rect);
			((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + 3f;
			if (capitalize)
			{
				Widgets.Label(rect2, text.NullOrEmpty() ? value.CapitalizeFirst() : (text + " " + value.CapitalizeFirst()));
			}
			else
			{
				Widgets.Label(rect2, text.NullOrEmpty() ? value : (text + " " + value));
			}
			ButtonInvisible(rect, delegate
			{
				iLabelId = id;
			});
			TooltipHandler.TipRegion(rect, value);
		}
		else
		{
			Rect rect3 = default(Rect);
			((Rect)(ref rect3))._002Ector(rect);
			((Rect)(ref rect3)).width = ((Rect)(ref rect3)).height;
			((Rect)(ref rect3)).x = ((Rect)(ref rect)).width - ((Rect)(ref rect3)).height;
			ButtonImage(rect3, "UI/Buttons/DragHash", delegate
			{
				iLabelId = -1;
			});
			value = Widgets.TextField(rect, value, 256, CharacterEditor.Label.ValidNameRegex);
		}
	}

	internal static void Label(Rect rect, string text, Action action = null, string tooltip = "")
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (text != null)
		{
			Widgets.Label(rect, text);
		}
		if (action != null)
		{
			ButtonInvisible(rect, action);
		}
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, tooltip);
		}
	}

	internal static void Label(float x, float y, float w, float h, string text, Action action = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, w, h);
		Widgets.Label(rect, text);
		if (action != null)
		{
			ButtonInvisible(rect, action);
		}
	}

	internal static void LabelBackground(Rect rect, string text, Color col, int offset = 0, string tooltip = "", Color colText = default(Color))
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawBoxSolid(rect, col);
		if (text == null)
		{
			text = "";
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x + 3f + (float)offset, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 3f, ((Rect)(ref rect)).height);
		if (((Rect)(ref rect)).height > 20f && text.Length <= 22)
		{
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + (((Rect)(ref rect)).height - 20f) / 2f;
		}
		if (colText != default(Color))
		{
			Color color = GUI.color;
			GUI.color = colText;
			Widgets.Label(rect2, text);
			GUI.color = color;
		}
		else
		{
			Widgets.Label(rect2, text);
		}
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect2, tooltip);
		}
	}

	internal static void LabelCol<T>(Rect rect, string text, Color col, Action<T> action, T value, string tooltip = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Color color = GUI.color;
		GUI.color = col;
		Widgets.Label(rect, text);
		GUI.color = color;
		if (action != null)
		{
			ButtonInvisibleVar(rect, action, value, tooltip);
		}
	}

	internal static void TraitListView(float x, float y, float w, float h, List<Trait> l, ref Vector2 scrollPos, int elemH, Action<Trait> onClick, Action<Trait> onRandom, Action<Trait> onPrev, Action<Trait> onNext, Func<Trait, string> Flabel, Func<Trait, string> Ftooltip)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		if (l.NullOrEmpty())
		{
			return;
		}
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(x, y, w, h);
		float num = l.Count * elemH + 20;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, num);
		Widgets.BeginScrollView(outRect, ref scrollPos, val);
		Rect rect = val.ContractedBy(6f);
		((Rect)(ref rect)).height = num;
		Color green = Color.green;
		Listing_X listing_X = new Listing_X();
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width + 18f;
		listing_X.Begin(rect);
		listing_X.DefSelectionLineHeight = elemH;
		Rect rect2 = default(Rect);
		for (int i = 0; i < l.Count; i++)
		{
			if (listing_X.CurY + (float)elemH > scrollPos.y && listing_X.CurY - h < scrollPos.y)
			{
				Trait trait = l[i];
				Color colText = ((trait.sourceGene != null) ? ColorTool.colSkyBlue : Color.white);
				((Rect)(ref rect2))._002Ector(listing_X.CurX, listing_X.CurY, ((Rect)(ref outRect)).width - 16f, (float)elemH);
				string tooltip = "";
				if (Mouse.IsOver(rect2))
				{
					tooltip = Ftooltip(trait);
				}
				NavSelectorVar(rect2, trait, onClick, onRandom, onPrev, onNext, null, Flabel(trait), tooltip, null, colText);
			}
			listing_X.CurY += 25f;
		}
		listing_X.End();
		Widgets.EndScrollView();
	}

	internal static void AToggleSearch()
	{
		bToggleSearch = !bToggleSearch;
	}

	internal static ICollection<T> CreateSearch<T>(float x, ref float y, float w, float h, ICollection<T> l, Func<T, string> labelGetter)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		ICollection<T> collection = new List<T>();
		try
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(x, y, w, h);
			sFind = Widgets.TextField(rect, sFind, 256);
			char c = ((!sFind.NullOrEmpty()) ? sFind.First() : ' ');
			bool flag = char.IsUpper(c);
			string text = (flag ? sFind : sFind.ToLower());
			foreach (T item in l)
			{
				if (text.NullOrEmpty())
				{
					collection.Add(item);
				}
				else if (flag)
				{
					string text2 = labelGetter(item);
					if (text2.StartsWith(text))
					{
						collection.Add(item);
					}
				}
				else
				{
					string text2 = labelGetter(item).ToLower();
					if (text2.StartsWith(text) || text2.Contains(text))
					{
						collection.Add(item);
					}
				}
			}
		}
		catch
		{
		}
		y += 4f;
		return collection;
	}

	internal static float GetGraphicH<T>()
	{
		bool flag = typeof(T) == typeof(AbilityDef);
		bool flag2 = typeof(T) == typeof(HairDef);
		bool flag3 = typeof(T) == typeof(BeardDef);
		bool flag4 = typeof(T) == typeof(ThingDef);
		bool flag5 = typeof(T) == typeof(GeneDef);
		bool flag6 = typeof(T) == typeof(Pawn);
		return (flag || flag5 || flag2 || flag3 || flag4) ? 64 : (flag6 ? 90 : 0);
	}

	internal static void ListView<T>(float x, float y, float w, float h, ICollection<T> l, Func<T, string> labelGetter, Func<T, string> tooltipGetter, Func<T, T, bool> comparator, ref T selectedThing, ref Vector2 scrollPos, bool withRemove = false, Action<T> action = null, bool withSearch = true, bool drawSection = false, bool hasIcon = false, bool selectOnMouseOver = false)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		if (l == null)
		{
			return;
		}
		bool flag = typeof(T) == typeof(AbilityDef);
		bool isHair = typeof(T) == typeof(HairDef);
		bool isBeard = typeof(T) == typeof(BeardDef);
		bool flag2 = typeof(T) == typeof(ThingDef);
		bool flag3 = typeof(T) == typeof(GeneDef);
		bool flag4 = typeof(T) == typeof(Pawn);
		bool flag5 = typeof(T) == typeof(ScenPart);
		float num = ((!(flag || flag3 || flag2)) ? ((flag4 || flag5) ? 22 : 32) : 0);
		float graphicH = GetGraphicH<T>();
		float num2 = (withSearch ? 25f : 0f);
		ICollection<T> collection = (withSearch ? CreateSearch(x, ref y, w, num2, l, labelGetter) : l);
		float num3 = 10f + (float)collection.Count * (num + graphicH);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(x, y + num2, w, h - num2);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref val)).width - 16f, num3);
		if (drawSection)
		{
			Widgets.DrawMenuSection(val);
		}
		Widgets.BeginScrollView(val, ref scrollPos, val2);
		Rect rect = val2.ContractedBy(6f);
		((Rect)(ref rect)).height = num3;
		Color selColor = (drawSection ? Color.blue : Color.green);
		Listing_X listing_X = new Listing_X();
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width + 18f;
		listing_X.Begin(rect);
		listing_X.DefSelectionLineHeight = num;
		try
		{
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)3;
			if (flag4)
			{
				using IEnumerator<T> enumerator = collection.GetEnumerator();
				int num4 = 0;
				while (enumerator.MoveNext())
				{
					Pawn pawn = enumerator.Current as Pawn;
					bool selected = comparator(selectedThing, enumerator.Current);
					string tooltip = tooltipGetter(enumerator.Current);
					num4 = listing_X.Selectable(pawn.GetPawnName(needFull: true), selected, tooltip, PortraitsCache.Get(pawn, new Vector2(128f, 180f), Rot4.South), null, null, default(Vector2), withRemove: false, num + graphicH, (pawn.Faction == null) ? Color.white : pawn.Faction.Color, ColorTool.colLightGray);
					if (num4 == 1)
					{
						selectedThing = enumerator.Current;
						if (action != null)
						{
							action(selectedThing);
						}
						else
						{
							SoundDefOf.Mouseover_Category.PlayOneShotOnCamera();
						}
					}
				}
			}
			else
			{
				using IEnumerator<T> enumerator2 = collection.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (listing_X.CurY + num + graphicH > scrollPos.y && listing_X.CurY - 700f < scrollPos.y)
					{
						string tooltip = tooltipGetter(enumerator2.Current);
						string name = labelGetter(enumerator2.Current);
						bool selected = comparator(enumerator2.Current, selectedThing);
						bool isWhite = true;
						if (listing_X.SelectableText(name, flag2, flag, flag3, isHair, isBeard, selected, tooltip, withRemove, isWhite, enumerator2.Current, selColor, hasIcon, selectOnMouseOver))
						{
							selectedThing = enumerator2.Current;
							action?.Invoke(selectedThing);
						}
					}
					listing_X.CurY += num;
					listing_X.CurY += graphicH;
				}
			}
			Text.Anchor = (TextAnchor)0;
		}
		catch
		{
		}
		listing_X.End();
		Widgets.EndScrollView();
	}

	internal static void ListView<T>(Rect rect, ICollection<T> l, Func<T, string> labelGetter, Func<T, string> tooltipGetter, Func<T, T, bool> comparator, ref T selectedThing, ref Vector2 scrollPos, bool withRemove = false, Action<T> action = null, bool withSearch = true, bool drawSection = false, bool isHead = false, bool selectOnMouse = false)
	{
		ListView(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height, l, labelGetter, tooltipGetter, comparator, ref selectedThing, ref scrollPos, withRemove, action, withSearch, drawSection, isHead, selectOnMouse);
	}

	internal static void FullListviewScenPart(Rect rect, List<ScenPart> l, bool withRemove, Action<ScenPart> removeAction, string shiftIcon, Action<ScenPart> onShift, bool showPosition, bool withSearch, ref Vector2 scrollPos, ref ScenPart selectedPart)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (l == null)
		{
			return;
		}
		float x = ((Rect)(ref rect)).x;
		float y = ((Rect)(ref rect)).y;
		float width = ((Rect)(ref rect)).width;
		float height = ((Rect)(ref rect)).height;
		float num = 32f;
		float num2 = (withSearch ? 25f : 0f);
		List<ScenPart> list = (withSearch ? CreateSearch(((Rect)(ref rect)).x, ref y, width, num2, l, FLabel.ScenPartLabel).ToList() : l);
		float num3 = 10f + (float)list.Count * num;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(x, y + num2, width, height - num2);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, num3);
		Widgets.BeginScrollView(outRect, ref scrollPos, val);
		Rect rect2 = val.ContractedBy(6f);
		((Rect)(ref rect2)).height = num3;
		Color green = Color.green;
		Listing_X listing_X = new Listing_X();
		((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width + 18f;
		listing_X.Begin(rect2);
		listing_X.DefSelectionLineHeight = num;
		try
		{
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)3;
			using (List<ScenPart>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (listing_X.CurY + num > scrollPos.y && listing_X.CurY - 700f < scrollPos.y && listing_X.ListviewPart(width, num, enumerator.Current, enumerator.Current == selectedPart, withRemove, removeAction, shiftIcon, onShift, showPosition))
					{
						selectedPart = ((selectedPart != null) ? null : enumerator.Current);
					}
					listing_X.CurY += num;
				}
			}
			Text.Anchor = (TextAnchor)0;
		}
		catch
		{
		}
		listing_X.End();
		Widgets.EndScrollView();
	}

	internal static float NavSelectorCount(Rect rect, Selected s, int max)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		if (Widgets.ButtonImage(RectPrevious(rect), ContentFinder<Texture2D>.Get("bbackward")))
		{
			s.stackVal--;
			s.oldStackVal = s.stackVal;
			s.UpdateBuyPrice();
		}
		if (Widgets.ButtonImage(RectNext(rect), ContentFinder<Texture2D>.Get("bforward")))
		{
			s.stackVal++;
			s.oldStackVal = s.stackVal;
			s.UpdateBuyPrice();
		}
		LabelBackground(RectSolid(rect), CharacterEditor.Label.COUNT + s.stackVal, ColorTool.colAsche);
		if (Widgets.ButtonImage(RectToggle(rect), ContentFinder<Texture2D>.Get("UI/Buttons/DragHash")))
		{
			bCountOpen = !bCountOpen;
		}
		int num = 0;
		if (bCountOpen)
		{
			num++;
			s.stackVal = (int)Widgets.HorizontalSlider(RectSlider(rect, num), s.stackVal, 1f, max);
			if (s.stackVal != s.oldStackVal)
			{
				s.oldStackVal = s.stackVal;
				s.UpdateBuyPrice();
			}
		}
		if (s.tempThing != null)
		{
			s.tempThing.stackCount = s.stackVal;
		}
		return ((Rect)(ref rect)).y + 27f + (float)(num * 26);
	}

	internal static void NavSelectorImageBox(Rect rect, Action onClicked, Action onRandom, Action onPrev, Action onNext, Action onTextureClick, Action onToggle, string label, string tipLabel = null, string tipRandom = null, string tipTexture = null, string texturePath = null, Color colTex = default(Color), string tipToggle = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (onPrev != null)
		{
			ButtonImage(RectPrevious(rect), "bbackward", onPrev);
		}
		if (onNext != null)
		{
			ButtonImage(RectNext(rect), "bforward", onNext);
		}
		Text.Font = GameFont.Small;
		bool flag = texturePath != null;
		bool flag2 = onToggle != null;
		LabelBackground(RectSolid(rect), label, ColorTool.colAsche, flag ? 25 : 0, "", colTex);
		ButtonImage(RectTexture(rect), texturePath, onTextureClick, tipTexture);
		int num = (CEditor.IsRandom ? 25 : 0);
		num += (flag2 ? 25 : 0);
		ButtonInvisibleMouseOver(RectOnClick(rect, flag, num), onClicked, tipLabel);
		if (CEditor.IsRandom && onRandom != null)
		{
			ButtonImage(RectRandom(rect), "brandom", onRandom, tipRandom);
			ButtonImage(RectToggleLeft(rect), flag2 ? "UI/Buttons/DragHash" : null, onToggle, tipToggle);
		}
		else
		{
			ButtonImage(RectToggle(rect), flag2 ? "UI/Buttons/DragHash" : null, onToggle, tipToggle);
		}
	}

	internal static void NavSelectorVar<T>(Rect rect, T val, Action<T> onClick, Action<T> onRandom, Action<T> onPrev, Action<T> onNext, Action<T> onToggle, string label, string tooltip, string tipRandom, Color colText)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		bool isRandom = CEditor.IsRandom;
		if (isRandom && onPrev != null)
		{
			ButtonImageVar(RectPrevious(rect), "bbackward", onPrev, val);
		}
		if (isRandom && onNext != null)
		{
			ButtonImageVar(RectNext(rect), "bforward", onNext, val);
		}
		Text.Font = GameFont.Small;
		bool flag = onToggle != null;
		int offset = (CEditor.IsRandom ? 25 : 0);
		LabelBackground(RectSolid(rect, isRandom), label, ColorTool.colAsche, 0, "", colText);
		if (onClick != null)
		{
			ButtonInvisibleMouseOverVar(RectOnClick(rect, hasTexture: false, offset, isRandom), onClick, val, tooltip);
		}
		if (CEditor.IsRandom)
		{
			ButtonImageVar(RectRandom(rect), "brandom", onRandom, val, tipRandom);
			if (flag)
			{
				ButtonImageVar(RectToggleLeft(rect), flag ? "UI/Buttons/DragHash" : null, onToggle, val);
			}
		}
		else if (flag)
		{
			ButtonImageVar(RectToggle(rect), flag ? "UI/Buttons/DragHash" : null, onToggle, val);
		}
	}

	internal static void NavSelectorImageBox2<T>(Rect rect, T val, Action<T> onClicked, Action<T> onRandom, Action<T> onPrev, Action<T> onNext, Action<T> onTextureClick, Action<T> onToggle, string label, string tipLabel = null, string tipRandom = null, string tipTexture = null, string texturePath = null, Color colTex = default(Color))
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		if (onPrev != null)
		{
			ButtonImageVar(RectPrevious(rect), "bbackward", onPrev, val);
		}
		if (onNext != null)
		{
			ButtonImageVar(RectNext(rect), "bforward", onNext, val);
		}
		Text.Font = GameFont.Small;
		bool flag = val is Thing;
		bool flag2 = flag || texturePath != null;
		bool flag3 = onToggle != null;
		LabelBackground(RectSolid(rect), label, ColorTool.colAsche, flag2 ? 25 : 0);
		int num = (CEditor.IsRandom ? 25 : 0);
		num += (flag3 ? 25 : 0);
		if (flag)
		{
			ButtonThingVar(RectTexture(rect), val, onTextureClick, tipTexture);
		}
		else
		{
			ButtonImageVar(RectTexture(rect), texturePath, onTextureClick, val, tipTexture);
		}
		ButtonInvisibleMouseOverVar(RectOnClick(rect, flag2, num), onClicked, val, tipLabel);
		if (CEditor.IsRandom)
		{
			ButtonImageVar(RectRandom(rect), "brandom", onRandom, val, tipRandom);
			ButtonImageVar(RectToggleLeft(rect), flag3 ? "UI/Buttons/DragHash" : null, onToggle, val);
		}
		else
		{
			ButtonImageVar(RectToggle(rect), flag3 ? "UI/Buttons/DragHash" : null, onToggle, val);
		}
	}

	internal static float NavSelectorQuality(Rect rect, Selected s, HashSet<QualityCategory> lOfQuality)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		if (s == null || !s.HasQuality)
		{
			return ((Rect)(ref rect)).y;
		}
		if (Widgets.ButtonImage(RectPrevious(rect), ContentFinder<Texture2D>.Get("bbackward")))
		{
			s.quality = lOfQuality.NextOrPrevIndex(s.quality, next: false, random: false);
			s.UpdateBuyPrice();
		}
		if (Widgets.ButtonImage(RectNext(rect), ContentFinder<Texture2D>.Get("bforward")))
		{
			s.quality = lOfQuality.NextOrPrevIndex(s.quality, next: true, random: false);
			s.UpdateBuyPrice();
		}
		Text.Font = GameFont.Small;
		LabelBackground(RectSolid(rect), CharacterEditor.Label.QUALITY + ((QualityCategory)s.quality).GetLabel().CapitalizeFirst(), ColorTool.colAsche);
		if (Widgets.ButtonImage(RectToggle(rect), ContentFinder<Texture2D>.Get("UI/Buttons/DragHash")))
		{
			bQualityOpen = !bQualityOpen;
		}
		Rect rect2 = RectClickableT(rect);
		if (Mouse.IsOver(rect2))
		{
			Widgets.DrawHighlight(rect2);
		}
		FloatMenuOnButtonInvisible(rect2, lOfQuality, (QualityCategory q) => q.GetLabel(), delegate(QualityCategory q)
		{
			s.quality = (int)q;
			s.UpdateBuyPrice();
		});
		int num = 0;
		if (bQualityOpen)
		{
			num++;
			s.quality = (int)Widgets.HorizontalSlider(RectSlider(rect, num), s.quality, 0f, lOfQuality.Count - 1);
		}
		if (s.tempThing != null)
		{
			s.tempThing.SetQuality(s.quality);
		}
		return ((Rect)(ref rect)).y + 27f + (float)(num * 26);
	}

	internal static float NavSelectorStuff(Rect rect, Selected s)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		if (s == null || s.thingDef == null)
		{
			return ((Rect)(ref rect)).y;
		}
		bool madeFromStuff = s.thingDef.MadeFromStuff;
		if (!madeFromStuff)
		{
			return ((Rect)(ref rect)).y;
		}
		Text.Font = GameFont.Small;
		if (Widgets.ButtonImage(RectPrevious(rect), ContentFinder<Texture2D>.Get("bbackward")) && madeFromStuff)
		{
			s.SetStuff(next: false, random: false);
		}
		if (Widgets.ButtonImage(RectNext(rect), ContentFinder<Texture2D>.Get("bforward")) && madeFromStuff)
		{
			s.SetStuff(next: true, random: false);
		}
		LabelBackground(RectSolid(rect), CharacterEditor.Label.STUFF + s.StuffLabelGetter(s.stuff), ColorTool.colAsche, madeFromStuff ? 25 : 0);
		if (Widgets.ButtonImage(RectToggle(rect), ContentFinder<Texture2D>.Get("UI/Buttons/DragHash")))
		{
			bStuffOpen = !bStuffOpen;
		}
		if (madeFromStuff)
		{
			GUI.color = s.GetTColor();
			FloatMenuOnButtonStuffOrStyle(RectTexture(rect), RectClickableT(rect), s.lOfStuff, s.StuffLabelGetter, s, delegate(ThingDef stuff)
			{
				s.SetStuff(stuff);
			});
			GUI.color = Color.white;
		}
		if (s.tempThing != null)
		{
			s.tempThing.SetStuffDirect(s.stuff);
		}
		int num = 0;
		if (bStuffOpen && madeFromStuff)
		{
			num++;
			s.stuffIndex = (int)Widgets.HorizontalSlider(RectSlider(rect, num), s.stuffIndex, 0f, s.lOfStuff.Count - 1);
			s.CheckSetStuff();
		}
		return ((Rect)(ref rect)).y + 27f + (float)(num * 26);
	}

	internal static float NavSelectorStyle(Rect rect, Selected s)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		if (s == null || s.thingDef == null)
		{
			return ((Rect)(ref rect)).y;
		}
		bool flag = s.thingDef.CanBeStyled() && s.lOfStyle.Count > 1;
		if (!flag)
		{
			return ((Rect)(ref rect)).y;
		}
		Text.Font = GameFont.Small;
		if (Widgets.ButtonImage(RectPrevious(rect), ContentFinder<Texture2D>.Get("bbackward")) && flag)
		{
			s.SetStyle(next: false, random: false);
		}
		if (Widgets.ButtonImage(RectNext(rect), ContentFinder<Texture2D>.Get("bforward")) && flag)
		{
			s.SetStyle(next: true, random: false);
		}
		string text = (flag ? s.StyleLabelGetter(s.style) : "");
		LabelBackground(RectSolid(rect), CharacterEditor.Label.STYLE + text, ColorTool.colAsche, flag ? 25 : 0);
		if (Widgets.ButtonImage(RectToggle(rect), ContentFinder<Texture2D>.Get("UI/Buttons/DragHash")))
		{
			bStyleOpen = !bStyleOpen;
		}
		if (flag)
		{
			FloatMenuOnButtonStuffOrStyle(RectTexture(rect), RectClickableT(rect), s.lOfStyle, s.StyleLabelGetter, s, delegate(ThingStyleDef style)
			{
				s.SetStyle(style);
			});
		}
		int num = 0;
		if (bStyleOpen && flag)
		{
			num++;
			s.styleIndex = (int)Widgets.HorizontalSlider(RectSlider(rect, num), s.styleIndex, 0f, s.lOfStyle.Count - 1);
			s.CheckSetStyle();
		}
		if (s.tempThing != null)
		{
			s.tempThing.SetStyleDef(s.style);
		}
		return ((Rect)(ref rect)).y + 27f + (float)(num * 26);
	}

	internal static float NumericFloatBox(Rect rect, float value, float min, float max)
	{
		return NumericFloatBox(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height, value, min, max);
	}

	internal static float NumericFloatBox(float x, float y, float w, float h, float value, float min, float max)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(x, y, 25f, h);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x + 20f, y, w, h);
		Rect butRect2 = default(Rect);
		((Rect)(ref butRect2))._002Ector(x + w + 15f, y, 25f, h);
		if (Widgets.ButtonImage(butRect, ContentFinder<Texture2D>.Get("bbackward")))
		{
			value -= 1f;
			value = (float)Math.Round(value, 2);
		}
		if (Widgets.ButtonImage(butRect2, ContentFinder<Texture2D>.Get("bforward")))
		{
			value += 1f;
			value = (float)Math.Round(value, 2);
		}
		string text = value.ToString();
		if (text.EndsWith("."))
		{
			text += "0";
		}
		else if (!text.Contains("."))
		{
			text += ".0";
		}
		text = Widgets.TextField(rect, text, 32);
		if (text.EndsWith("."))
		{
			text += "0";
		}
		else if (!text.Contains("."))
		{
			text += ".0";
		}
		float result = 0f;
		if (float.TryParse(text, out result))
		{
			value = ((result < min) ? min : ((!(result > max)) ? result : max));
		}
		return value;
	}

	internal static long NumericLongBox(Rect rect, long value, long min, long max)
	{
		return NumericLongBox(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height, value, min, max);
	}

	internal static long NumericLongBox(float x, float y, float w, float h, long value, long min, long max)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(x, y, 25f, h);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x + 20f, y, w, h);
		Rect butRect2 = default(Rect);
		((Rect)(ref butRect2))._002Ector(x + w + 15f, y, 25f, h);
		if (Widgets.ButtonImage(butRect, ContentFinder<Texture2D>.Get("bbackward")))
		{
			value--;
		}
		if (Widgets.ButtonImage(butRect2, ContentFinder<Texture2D>.Get("bforward")))
		{
			value++;
		}
		string text = value.ToString();
		text = Widgets.TextField(rect, text, 32);
		long result = 0L;
		if (long.TryParse(text, out result))
		{
			value = ((result < min) ? min : ((result <= max) ? result : max));
		}
		return value;
	}

	internal static int NumericIntBox(Rect rect, int value, int min, int max)
	{
		return NumericIntBox(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height, value, min, max);
	}

	internal static int NumericIntBox(float x, float y, float w, float h, int value, int min, int max)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(x, y, 25f, h);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x + 20f, y, w, h);
		Rect butRect2 = default(Rect);
		((Rect)(ref butRect2))._002Ector(x + w + 15f, y, 25f, h);
		if (Widgets.ButtonImage(butRect, ContentFinder<Texture2D>.Get("bbackward")))
		{
			value--;
		}
		if (Widgets.ButtonImage(butRect2, ContentFinder<Texture2D>.Get("bforward")))
		{
			value++;
		}
		string text = value.ToString();
		text = Widgets.TextField(rect, text, 32);
		int result = 0;
		if (int.TryParse(text, out result))
		{
			value = ((result < min) ? min : ((result <= max) ? result : max));
		}
		return value;
	}

	internal static int NumericTextField(Rect rect, int value, int min, int max)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		string text = value.ToString();
		text = Widgets.TextField(rect, text, 32);
		int result = 0;
		if (int.TryParse(text, out result))
		{
			value = ((result < min) ? min : ((result <= max) ? result : max));
		}
		return value;
	}

	internal static int NumericTextField(float x, float y, float w, float h, int value, int min, int max)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x + 20f, y, w, h);
		string text = value.ToString();
		text = Widgets.TextField(rect, text, 32);
		int result = 0;
		if (int.TryParse(text, out result))
		{
			value = ((result < min) ? min : ((result <= max) ? result : max));
		}
		return value;
	}

	internal static void ScrollView(int x, int y, int w, int h, int objCount, int objH, ref Vector2 scrollPos, Action<Listing_X> drawFunction)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector((float)x, (float)y, (float)w, (float)h);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, (float)y, ((Rect)(ref outRect)).width - 16f, (float)(objCount * objH));
		Widgets.BeginScrollView(outRect, ref scrollPos, val);
		Rect rect = val.ContractedBy(4f);
		((Rect)(ref rect)).height = objCount * objH;
		Listing_X listing_X = new Listing_X();
		listing_X.Begin(rect);
		drawFunction(listing_X);
		listing_X.End();
		Widgets.EndScrollView();
	}

	internal static void SimpleMultiplierSlider(Rect rect, string label, string format, bool showNumeric, float baseValue, ref float currentVal, float min, float max)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Listing_X listing_X = new Listing_X();
		listing_X.Begin(rect);
		string selectedName = (showNumeric ? label : "");
		listing_X.AddMultiplierSection(label, format, ref selectedName, baseValue, ref currentVal, min, max, small: true);
		listing_X.End();
	}

	internal static void SimpleSlider(Rect rect, string label, ref float currentVal, float min, float max)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Listing_X listing_X = new Listing_X();
		listing_X.Begin(rect);
		string selectedName = label;
		listing_X.AddSection(label, "", ref selectedName, ref currentVal, min, max, small: true);
		listing_X.End();
	}

	internal static void SingleSlinder(Rect rect, float currentVal, float min, float max, Action<float> action)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Listing_X listing_X = new Listing_X();
		listing_X.Begin(rect);
		float num = listing_X.Slider(currentVal, min, max);
		bool flag = num != currentVal;
		listing_X.End();
		if (flag)
		{
			action?.Invoke(num);
		}
	}

	internal static string TextArea(Rect rect, string text, int max, Regex regex)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (text == null)
		{
			text = "";
		}
		string text2 = GUI.TextArea(rect, text, max, Text.CurTextAreaStyle);
		if (text2.Length <= max && regex != null && regex.IsMatch(text2))
		{
			return text2;
		}
		return text;
	}

	internal static void ThingDrawer(Rect rect, Thing t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Widgets.ThingIcon(rect, t);
		GUI.color = Color.white;
	}

	private static Rect RectClickableT(Rect rect)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + 21f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 64f, 24f);
	}

	private static Rect RectNext(Rect rect)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 22f, ((Rect)(ref rect)).y + 2f, 22f, 22f);
	}

	private static Rect RectOnClick(Rect rect, bool hasTexture, int offset = 0, bool showEdit = true)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + (float)(showEdit ? 21 : 0) + (float)(hasTexture ? 25 : 0), ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - (float)(showEdit ? 40 : 19) - (float)(hasTexture ? 25 : 0) - (float)offset, 24f);
	}

	private static Rect RectPrevious(Rect rect)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + 2f, 22f, 22f);
	}

	private static Rect RectRandom(Rect rect)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 42f, ((Rect)(ref rect)).y, 22f, 22f);
	}

	private static Rect RectSlider(Rect rect, int i)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + 10f + (float)i * ((Rect)(ref rect)).height, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
	}

	private static Rect RectSolid(Rect rect, bool showEdit = true)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + (float)(showEdit ? 21 : 0), ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - (float)(showEdit ? 40 : 19), 24f);
	}

	private static Rect RectTexture(Rect rect)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + 25f, ((Rect)(ref rect)).y, 24f, 24f);
	}

	private static Rect RectToggle(Rect rect)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 42f, ((Rect)(ref rect)).y, 22f, 22f);
	}

	private static Rect RectToggleLeft(Rect rect)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 67f, ((Rect)(ref rect)).y, 22f, 22f);
	}

	private static void CheckAddTempTextToList(ref List<string> l)
	{
		if (iLabelId == iTempTextID)
		{
			return;
		}
		if (!tempText.NullOrEmpty())
		{
			if (l == null)
			{
				l = new List<string>();
			}
			l.Add(tempText);
		}
		tempText = "";
		iShowId = 0;
		iLabelId = -1;
	}

	private static void CheckAddTempTextToFList(ref List<float> l)
	{
		if (iLabelId == iTempTextID)
		{
			return;
		}
		if (float.TryParse(tempText, out var result))
		{
			if (l == null)
			{
				l = new List<float>();
			}
			l.Add(result);
		}
		tempText = "";
		iShowId = 0;
		iLabelId = -1;
	}

	internal static void ActivateLabelEdit(int id)
	{
		iShowId = id;
		iLabelId = iTempTextID;
	}

	internal static void AddLabelEditToList(Listing_X view, int id, ref List<string> l, Action action)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (iShowId == id)
		{
			action?.Invoke();
			LabelEdit(view.GetRect(22f), iTempTextID, "", ref tempText, GameFont.Small);
			CheckAddTempTextToList(ref l);
		}
	}

	internal static void AddLabelEditToList(Listing_X view, int id, ref List<float> l, Action action)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (iShowId == id)
		{
			action?.Invoke();
			LabelEdit(view.GetRect(22f), iTempTextID, "", ref tempText, GameFont.Small);
			CheckAddTempTextToFList(ref l);
		}
	}

	internal static void ToggleRemove()
	{
		bRemoveOnClick = !bRemoveOnClick;
	}

	internal static void DrawTagFilter(ref TagFilter t, List<string> lSamples, int w, Listing_X view, string title, ref List<string> copyList, Action<string> remove, Action<string> add)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		view.Label(0f, 0f, w - 28, 30f, title, GameFont.Medium);
		view.FloatMenuOnButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", lSamples, (string s) => s, add);
		view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
		if (view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", null) && t != null)
		{
			t.tags.CopyList(ref copyList);
		}
		if (!copyList.NullOrEmpty() && view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", null))
		{
			if (t == null)
			{
				t = new TagFilter();
			}
			t.tags.PasteList(copyList);
		}
		view.Gap(30f);
		if (t != null)
		{
			view.FullListViewString(w - 28, t.tags, bRemoveOnClick, remove);
		}
		view.GapLine(25f);
	}

	internal static void DrawStringList(ref List<string> l, List<string> lSamples, int w, Listing_X view, string title, ref List<string> copyList, Action<string> remove, Action<string> add)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		view.Label(0f, 0f, w - 28, 30f, title, GameFont.Medium);
		view.FloatMenuOnButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", lSamples, (string s) => s, add);
		view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
		if (view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", null))
		{
			l.CopyList(ref copyList);
		}
		if (!copyList.NullOrEmpty() && view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", null))
		{
			if (l.NullOrEmpty())
			{
				l = new List<string>();
			}
			l.PasteList(copyList);
		}
		view.Gap(30f);
		view.FullListViewString(w - 28, l, bRemoveOnClick, remove);
		view.GapLine(25f);
	}

	internal static void DrawStringListCustom(ref List<string> l, int id, int w, Listing_X view, string title, ref List<string> copyList, Action<string> remove, Action actionBeforeAdding = null)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		view.Label(0f, 0f, w - 28, 30f, title, GameFont.Medium);
		view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate
		{
			ActivateLabelEdit(id);
		});
		view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
		if (view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", null))
		{
			l.CopyList(ref copyList);
		}
		if (!copyList.NullOrEmpty() && view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", null))
		{
			if (l.NullOrEmpty())
			{
				l = new List<string>();
			}
			l.PasteList(copyList);
		}
		view.Gap(30f);
		AddLabelEditToList(view, id, ref l, actionBeforeAdding);
		view.FullListViewString(w - 28, l, bRemoveOnClick, remove);
		view.GapLine(25f);
	}
}
