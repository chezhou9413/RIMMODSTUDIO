using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class Listing_X : Listing
{
	internal float DefSelectionLineHeight = 22f;

	private GameFont font;

	private Color MoodColor = new Color(0.1f, 1f, 0.1f);

	private Color MoodColorNegative = new Color(0.8f, 0.4f, 0.4f);

	private Color NoEffectColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);

	private List<Pair<Vector2, Vector2>> labelScrollbarPositions;

	private List<Vector2> labelScrollbarPositionsSetThisFrame;

	private Texture2D texRemove;

	private bool alternate = true;

	internal float CurY
	{
		get
		{
			return curY;
		}
		set
		{
			curY = value;
		}
	}

	internal float CurX
	{
		get
		{
			return curX;
		}
		set
		{
			curX = value;
		}
	}

	internal Listing_X(GameFont font)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		this.font = font;
	}

	internal Listing_X()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		font = GameFont.Small;
	}

	public override void Begin(Rect rect)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base.Begin(rect);
		Text.Font = font;
		texRemove = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
	}

	internal Listing_Standard BeginSection(float height)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(height + 8f);
		Widgets.DrawMenuSection(rect);
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.Begin(rect.ContractedBy(4f));
		return listing_Standard;
	}

	internal void FloatMenuOnButtonImage<T>(float xOff, float yOff, float w, float h, string texPath, List<T> l, Func<T, string> labelGetter, Action<T> action)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(curX + xOff, curY + yOff, w, h);
		if (Widgets.ButtonImage(butRect, ContentFinder<Texture2D>.Get(texPath)))
		{
			SZWidgets.FloatMenuOnRect(l, labelGetter, action);
		}
	}

	internal bool ButtonImage(float xOff, float yOff, float w, float h, string texPath, Action action, Color? color = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(curX + xOff, curY + yOff, w, h);
		bool flag = Widgets.ButtonImage(butRect, ContentFinder<Texture2D>.Get(texPath), (Color)(((_003F?)color) ?? Color.white));
		if (flag)
		{
			action?.Invoke();
		}
		return flag;
	}

	internal bool ButtonImage<T>(float xOff, float yOff, float w, float h, string texPath, Color color, Action<T> action, T val, string toolTip = "")
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(curX + xOff, curY + yOff, w, h);
		bool flag = Widgets.ButtonImage(val2, ContentFinder<Texture2D>.Get(texPath), color);
		if (flag)
		{
			action?.Invoke(val);
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val2, toolTip);
		}
		return flag;
	}

	internal bool ButtonImage(Texture2D tex, float xOffset, float width, float height, Action action)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		NewColumnIfNeeded(height);
		bool flag = Widgets.ButtonImage(new Rect(curX + xOffset, curY, width, height), tex);
		Gap(height + verticalSpacing);
		if (flag)
		{
			action?.Invoke();
		}
		return flag;
	}

	internal bool ButtonText(string label, float x, float y, float w, float h, Action action, string highlightTag = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, w, h);
		bool flag = Widgets.ButtonText(rect, label, drawBackground: true, doMouseoverSound: false);
		if (highlightTag != null)
		{
			UIHighlighter.HighlightOpportunity(rect, highlightTag);
		}
		if (flag)
		{
			action?.Invoke();
		}
		return flag;
	}

	internal bool ButtonText(string label, string highlightTag = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(30f);
		bool result = Widgets.ButtonText(rect, label, drawBackground: true, doMouseoverSound: false);
		if (highlightTag != null)
		{
			UIHighlighter.HighlightOpportunity(rect, highlightTag);
		}
		Gap(verticalSpacing);
		return result;
	}

	internal bool ButtonTextLabeled(string label, string buttonLabel)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(30f);
		Widgets.Label(rect.LeftHalf(), label);
		bool result = Widgets.ButtonText(rect.RightHalf(), buttonLabel, drawBackground: true, doMouseoverSound: false);
		Gap(verticalSpacing);
		return result;
	}

	internal void CheckboxLabeledWithDefault(string label, float xOff, float width, ref bool checkOn, bool defaultVal, string tooltip = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Rect val = BaseCheckboxLabeled(label, xOff, width - 24f, ref checkOn, tooltip);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width, ((Rect)(ref val)).y, 24f, 24f);
		if (Widgets.ButtonImage(val2, ContentFinder<Texture2D>.Get("bdefault")))
		{
			checkOn = defaultVal;
		}
		TooltipHandler.TipRegion(val2, CharacterEditor.Label.O_SETTODEFAULT);
		Gap(verticalSpacing);
	}

	internal void LabelEdit(int id, string text, ref string value, GameFont font)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		SZWidgets.LabelEdit(GetRect(22f), id, text, ref value, font);
		Gap(4f);
	}

	internal void CheckboxLabeledNoGap(string label, float xOff, float width, ref bool checkOn)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		BaseCheckboxLabeled(label, xOff, width, ref checkOn);
	}

	internal void CheckboxLabeled(string label, float xOff, float width, ref bool checkOn, string tooltip = null, int gap = -1)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		BaseCheckboxLabeled(label, xOff, width, ref checkOn, tooltip);
		if (gap < 0)
		{
			Gap(verticalSpacing);
		}
		else
		{
			Gap(gap);
		}
	}

	private Rect BaseCheckboxLabeled(string label, float xOff, float width, ref bool checkOn, string tooltip = null, bool nearText = false)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		float lineHeight = Text.LineHeight;
		Rect rect = GetRect(lineHeight);
		((Rect)(ref rect)).width = width;
		((Rect)(ref rect)).x = ((Rect)(ref rect)).x + xOff;
		Widgets.DrawBoxSolid(rect, ColorTool.colAsche);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, tooltip);
		}
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		Widgets.CheckboxLabeled(rect, label, ref checkOn, disabled: false, null, null, nearText);
		return rect;
	}

	internal bool CheckboxLabeledSelectable(string label, ref bool selected, ref bool checkOn)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		float lineHeight = Text.LineHeight;
		Rect rect = GetRect(lineHeight);
		bool result = Widgets.CheckboxLabeledSelectable(rect, label, ref selected, ref checkOn);
		Gap(verticalSpacing);
		return result;
	}

	public override void End()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		base.End();
		if (labelScrollbarPositions == null)
		{
			return;
		}
		for (int num = labelScrollbarPositions.Count - 1; num >= 0; num--)
		{
			if (!labelScrollbarPositionsSetThisFrame.Contains(labelScrollbarPositions[num].First))
			{
				labelScrollbarPositions.RemoveAt(num);
			}
		}
		labelScrollbarPositionsSetThisFrame.Clear();
	}

	internal void EndSection(Listing_Standard listing)
	{
		listing.End();
	}

	internal void FloatMenuButtonWithLabelDef<T>(string label, float wLabel, float wDropbox, string currentVal, ICollection<T> l, Func<T, string> labelGetter, Action<T> action, float gap = -1f) where T : Def
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY + 4f, wLabel, 30f);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(curX + wLabel, curY, wDropbox, 30f);
		Widgets.Label(rect, label);
		if (!l.EnumerableNullOrEmpty())
		{
			SZWidgets.FloatMenuOnButtonText(rect2, currentVal, l, labelGetter, action);
		}
		if (gap == -1f)
		{
			Gap(verticalSpacing);
		}
		else
		{
			Gap(gap);
		}
	}

	internal void FloatMenuButtonWithLabel<T>(string label, float wLabel, float wDropbox, string currentVal, List<T> l, Func<T, string> labelGetter, Action<T> action, float gap = -1f)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY + 4f, wLabel, 30f);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(curX + wLabel, curY, wDropbox, 30f);
		Widgets.Label(rect, label);
		if (!l.NullOrEmpty())
		{
			SZWidgets.FloatMenuOnButtonText(rect2, currentVal, l, labelGetter, action);
		}
		if (gap == -1f)
		{
			Gap(verticalSpacing);
		}
		else
		{
			Gap(gap);
		}
	}

	internal void IntAdjuster(ref int val, int countChange, int min = 0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(24f);
		((Rect)(ref rect)).width = 42f;
		if (Widgets.ButtonText(rect, "-" + countChange, drawBackground: true, doMouseoverSound: false))
		{
			val -= countChange * GenUI.CurrentAdjustmentMultiplier();
			if (val < min)
			{
				val = min;
			}
		}
		((Rect)(ref rect)).x = ((Rect)(ref rect)).x + (((Rect)(ref rect)).width + 2f);
		if (Widgets.ButtonText(rect, "+" + countChange, drawBackground: true, doMouseoverSound: false))
		{
			val += countChange * GenUI.CurrentAdjustmentMultiplier();
			if (val < min)
			{
				val = min;
			}
		}
		Gap(verticalSpacing);
	}

	internal void IntEntry(ref int val, ref string editBuffer, int multiplier = 1)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(24f);
		Widgets.IntEntry(rect, ref val, ref editBuffer, multiplier);
		Gap(verticalSpacing);
	}

	internal void IntRange(ref IntRange range, int min, int max)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(28f);
		Widgets.IntRange(rect, (int)base.CurHeight, ref range, min, max);
		Gap(verticalSpacing);
	}

	internal void IntSetter(ref int val, int target, string label)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(24f);
		if (Widgets.ButtonText(rect, label, drawBackground: true, doMouseoverSound: false))
		{
			val = target;
		}
		Gap(verticalSpacing);
	}

	internal void Label(float xOff, float yOff, float w, float h, string text, GameFont font = GameFont.Small, string tooltip = "")
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = font;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX + xOff, curY + yOff, w, h);
		Widgets.Label(rect, text);
		Text.Font = GameFont.Small;
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, tooltip);
		}
	}

	internal void LabelSimple(string label, float x, float y, float w, float h, string tooltip = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, w, h);
		Widgets.Label(rect, label);
		if (tooltip != null)
		{
			TooltipHandler.TipRegion(rect, tooltip);
		}
	}

	internal void Label(string text, float width = -1f, float yGap = -1f, float maxHeight = -1f, string tooltip = null)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		float num = Text.CalcHeight(text, base.ColumnWidth);
		bool flag = false;
		if (maxHeight >= 0f && num > maxHeight)
		{
			num = maxHeight;
			flag = true;
		}
		Rect rect = GetRect(num);
		if (width >= 0f)
		{
			((Rect)(ref rect)).width = width;
		}
		if (flag)
		{
			Vector2 scrollbarPosition = GetLabelScrollbarPosition(curX, curY);
			Widgets.LabelScrollable(rect, text, ref scrollbarPosition);
			SetLabelScrollbarPosition(curX, curY, scrollbarPosition);
		}
		else
		{
			Widgets.Label(rect, text);
		}
		if (tooltip != null)
		{
			TooltipHandler.TipRegion(rect, tooltip);
		}
		if (yGap == -1f)
		{
			Gap(verticalSpacing);
		}
		else
		{
			Gap(yGap);
		}
	}

	internal bool Listview(float width, string defName, string name, string tooltip, bool withRemove, Action<string> action)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (name == null)
		{
			return false;
		}
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, width - DefSelectionLineHeight, DefSelectionLineHeight);
		Text.WordWrap = false;
		Widgets.Label(val, name);
		Text.WordWrap = true;
		if (!string.IsNullOrEmpty(tooltip))
		{
			TooltipHandler.TipRegion(tip: new TipSignal(() => tooltip, 275), rect: val);
		}
		Text.Anchor = (TextAnchor)0;
		curY += DefSelectionLineHeight;
		if (withRemove)
		{
			Rect butRect = default(Rect);
			((Rect)(ref butRect))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width, ((Rect)(ref val)).y, DefSelectionLineHeight, DefSelectionLineHeight);
			bool flag = Widgets.ButtonImage(butRect, texRemove, Color.grey);
			if (flag)
			{
				action?.Invoke(defName);
			}
			GUI.color = Color.white;
			return flag;
		}
		return Widgets.ButtonInvisible(val);
	}

	internal bool ListviewTDC(float width, ThingDefCountClass tdc, bool selected, bool withRemove, Action<string> action, ThingDef t)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		if (tdc == null || tdc.thingDef == null)
		{
			return false;
		}
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, width - DefSelectionLineHeight, DefSelectionLineHeight);
		Text.WordWrap = false;
		Widgets.Label(val, tdc.thingDef.label + ": " + tdc.count);
		Text.WordWrap = true;
		if (selected)
		{
			curY += DefSelectionLineHeight;
			int count = tdc.count;
			count = SZWidgets.NumericIntBox(curX, curY, 70f, 30f, count, 0, 10000);
			t.UpdateCost(tdc.thingDef, count);
			curY += DefSelectionLineHeight;
		}
		if (!string.IsNullOrEmpty(tdc.thingDef.description))
		{
			TooltipHandler.TipRegion(tip: new TipSignal(() => tdc.thingDef.description, 21275), rect: val);
		}
		Text.Anchor = (TextAnchor)0;
		curY += DefSelectionLineHeight;
		if (withRemove)
		{
			Rect butRect = default(Rect);
			((Rect)(ref butRect))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width, ((Rect)(ref val)).y, DefSelectionLineHeight, DefSelectionLineHeight);
			if (Widgets.ButtonImage(butRect, texRemove, Color.grey))
			{
				action?.Invoke(tdc.thingDef.defName);
			}
			GUI.color = Color.white;
		}
		return Widgets.ButtonInvisible(val);
	}

	internal void GapLineCustom(float gapVorLinie, float gapNachLinie)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		float y = curY + gapVorLinie / 2f;
		Color color = GUI.color;
		GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
		Widgets.DrawLineHorizontal(curX, y, base.ColumnWidth);
		GUI.color = color;
		curY += gapVorLinie / 2f + gapNachLinie;
	}

	internal bool ListviewPart(float width, float itemH, ScenPart part, bool selected, bool removeActive, Action<ScenPart> action, string shiftIcon, Action<ScenPart> onShift, bool showPosition = false)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		if (!part.IsSupportedScenarioPart())
		{
			return false;
		}
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, width - itemH + 7f, itemH);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(curX, curY, itemH, itemH);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX + width - itemH - itemH, curY, itemH, itemH);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(curX + 5f + itemH, curY, width, itemH);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(curX, curY, width - itemH - itemH, itemH);
		Widgets.DrawBoxSolid(val, alternate ? new Color(0.3f, 0.3f, 0.3f, 0.5f) : new Color(0.2f, 0.2f, 0.2f, 0.5f));
		alternate = !alternate;
		Text.WordWrap = false;
		Selected selectedScenarioPart = part.GetSelectedScenarioPart();
		if (part.IsScenarioAnimal())
		{
			if (selectedScenarioPart.pkd != null)
			{
				Widgets.ThingIcon(val2, selectedScenarioPart.pkd.race);
				Widgets.Label(rect2, FLabel.PawnKindWithGenderAndAge(selectedScenarioPart));
			}
			else
			{
				Widgets.Label(val, part.Label + ": " + selectedScenarioPart.stackVal);
			}
		}
		else if (selectedScenarioPart.thingDef != null)
		{
			GUI.color = selectedScenarioPart.GetTColor();
			GUI.DrawTexture(val2, (Texture)(object)selectedScenarioPart.GetTexture2D, (ScaleMode)1, true);
			GUI.color = Color.white;
			Widgets.Label(rect2, FLabel.ThingLabel(selectedScenarioPart));
		}
		Text.WordWrap = true;
		if (selected)
		{
			if (showPosition)
			{
				try
				{
					Reflect.GetAType("Verse", "CameraJumper").CallMethod("JumpLocalInternal", new object[2]
					{
						selectedScenarioPart.location,
						CameraJumper.MovementMode.Pan
					});
				}
				catch
				{
				}
			}
			else
			{
				selectedScenarioPart.oldStackVal = selectedScenarioPart.stackVal;
				selectedScenarioPart.stackVal = SZWidgets.NumericIntBox(curX + width - 140f - (float)(removeActive ? 25 : 0), curY + 2f, 70f, 26f, selectedScenarioPart.stackVal, 1, 20000);
				if (selectedScenarioPart.stackVal != selectedScenarioPart.oldStackVal)
				{
					part.SetScenarioPartCount(selectedScenarioPart.stackVal);
				}
			}
		}
		if (Mouse.IsOver(rect3))
		{
			TooltipHandler.TipRegion(rect3, selectedScenarioPart.thingDef.STooltip());
		}
		Text.Anchor = (TextAnchor)0;
		if (removeActive)
		{
			Rect butRect = default(Rect);
			((Rect)(ref butRect))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width - itemH, ((Rect)(ref val)).y, itemH, itemH);
			if (Widgets.ButtonImage(butRect, texRemove, Color.grey))
			{
				action?.Invoke(part);
			}
			GUI.color = Color.white;
		}
		else if (!shiftIcon.NullOrEmpty())
		{
			SZWidgets.ButtonImageVar(rect, shiftIcon, onShift, part);
		}
		return Widgets.ButtonInvisible(val);
	}

	internal void FullListViewParam2<T, TDef>(List<T> l, ref T selected, Func<T, TDef> defGetter, Func<T, string> labelGetter, bool bRemoveOnClick, Action<T> removeAction) where TDef : Def
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (l.NullOrEmpty())
		{
			return;
		}
		Rect rect = default(Rect);
		for (int i = 0; i < l.Count; i++)
		{
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)3;
			((Rect)(ref rect))._002Ector(curX, curY, 400f - DefSelectionLineHeight, DefSelectionLineHeight);
			Widgets.Label(rect, labelGetter(l[i]));
			Text.Anchor = (TextAnchor)0;
			curY += DefSelectionLineHeight;
			if (bRemoveOnClick)
			{
				BlockRemove(rect, l[i], ref selected, removeAction);
			}
			else
			{
				BlockSelectClick(rect, l[i], ref selected);
			}
		}
	}

	internal void FullListViewFloat(int w, List<float> l, bool bRemoveOnClick, Action<float> removeAction)
	{
		if (!l.NullOrEmpty())
		{
			float selectedVal = 0f;
			for (int i = 0; i < l.Count; i++)
			{
				float val = l[i];
				ListViewFloat(w, val, ref selectedVal, bRemoveOnClick, removeAction);
			}
		}
	}

	internal void ListViewFloat(float w, float val, ref float selectedVal, bool bRemoveOnClick, Action<float> removeAction)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, w - DefSelectionLineHeight, DefSelectionLineHeight);
		Widgets.Label(rect, val.ToString());
		curY += DefSelectionLineHeight;
		if (bRemoveOnClick)
		{
			BlockRemove(rect, val, ref selectedVal, removeAction);
		}
		else
		{
			BlockSelectClick(rect, val, ref selectedVal);
		}
	}

	internal void FullListViewString(int w, List<string> l, bool bRemoveOnClick, Action<string> removeAction)
	{
		if (!l.NullOrEmpty())
		{
			string selectedVal = null;
			for (int i = 0; i < l.Count; i++)
			{
				string val = l[i];
				ListViewString(w, val, ref selectedVal, bRemoveOnClick, removeAction);
			}
		}
	}

	internal void ListViewString(float w, string val, ref string selectedVal, bool bRemoveOnClick, Action<string> removeAction)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, w - DefSelectionLineHeight, DefSelectionLineHeight);
		Text.WordWrap = false;
		Widgets.Label(rect, val ?? "");
		Text.WordWrap = true;
		TooltipHandler.TipRegion(tip: new TipSignal(() => val, 275), rect: rect);
		curY += DefSelectionLineHeight;
		if (bRemoveOnClick)
		{
			BlockRemove(rect, val, ref selectedVal, removeAction);
		}
		else
		{
			BlockSelectClick(rect, val, ref selectedVal);
		}
	}

	internal void FullListViewParam1<T>(List<T> l, ref T selected, bool bRemoveOnClick, Action<T> removeAction) where T : Def
	{
		if (!l.NullOrEmpty())
		{
			for (int i = 0; i < l.Count; i++)
			{
				T def = l[i];
				ListViewParam1(400f, def, ref selected, bRemoveOnClick, removeAction);
			}
		}
	}

	internal void FullListViewWorkTags(WorkTags workTags, bool bRemoveOnClick, Action<WorkTags> removeAction)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (workTags == WorkTags.None)
		{
			return;
		}
		List<WorkTags> list = workTags.GetAllSelectedItems<WorkTags>().ToList();
		Rect val = default(Rect);
		Rect butRect = default(Rect);
		for (int i = 0; i < list.Count; i++)
		{
			WorkTags workTags2 = list[i];
			if (workTags2 == WorkTags.None)
			{
				continue;
			}
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)3;
			((Rect)(ref val))._002Ector(curX, curY, 400f - DefSelectionLineHeight, DefSelectionLineHeight);
			Text.WordWrap = false;
			string label = workTags2.LabelTranslated().CapitalizeFirst();
			Widgets.Label(val, label);
			Text.WordWrap = true;
			Text.Anchor = (TextAnchor)0;
			curY += DefSelectionLineHeight;
			if (bRemoveOnClick)
			{
				((Rect)(ref butRect))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width, ((Rect)(ref val)).y, DefSelectionLineHeight, DefSelectionLineHeight);
				if (Widgets.ButtonImage(butRect, texRemove, Color.grey))
				{
					removeAction?.Invoke(workTags2);
				}
				GUI.color = Color.white;
			}
			else
			{
				Widgets.ButtonInvisible(val);
			}
		}
	}

	internal void FullListViewParam<T, T2>(List<T2> l, ref T selected, Func<T2, T> defGetter, Func<T2, float> valueGetter, Func<T2, float> secValueGetter, Func<T2, float> minGetter, Func<T2, float> maxGetter, bool isInt, bool bRemoveOnClick, Action<T2, float> valueSetter, Action<T2, float> secValueSetter, Action<T> removeAction) where T : Def
	{
		if (l.NullOrEmpty())
		{
			return;
		}
		for (int i = 0; i < l.Count; i++)
		{
			T2 val = l[i];
			float value = valueGetter(val);
			float secValue = secValueGetter?.Invoke(val) ?? 0f;
			ListViewParam(400f, defGetter(val), ref selected, ref value, ref secValue, minGetter(val), maxGetter(val), isInt, bRemoveOnClick, removeAction);
			if (selected == defGetter(val))
			{
				valueSetter(val, value);
				secValueSetter?.Invoke(val, secValue);
			}
		}
	}

	private Rect BlockLabel<T>(int offset, float width, T def, float value, float secValue) where T : Def
	{
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX + (float)offset, curY, width - DefSelectionLineHeight - (float)offset, DefSelectionLineHeight);
		Text.WordWrap = false;
		if (value != float.MinValue)
		{
			if (typeof(T) == typeof(PawnCapacityDef))
			{
				Widgets.Label(val, def.SLabel() + " offset: " + value + "      factor: " + secValue);
			}
			else
			{
				Widgets.Label(val, def.SLabel() + ": " + value);
			}
		}
		else if (typeof(T) == typeof(HeadTypeDef))
		{
			Widgets.Label(val, def.SDefname());
		}
		else
		{
			Widgets.Label(val, def.SLabel());
		}
		Text.WordWrap = true;
		return val;
	}

	private void BlockTooltip<T>(Rect rect, T def) where T : Def
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		string tooltip = def.STooltip();
		if (!string.IsNullOrEmpty(tooltip))
		{
			TipSignal tip = new TipSignal(() => tooltip, 275);
			TooltipHandler.TipRegion(rect, tip);
		}
	}

	private void BlockRemove<T>(Rect rect, T def, ref T selectedDef, Action<T> removeAction)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width, ((Rect)(ref rect)).y, DefSelectionLineHeight, DefSelectionLineHeight);
		if (Widgets.ButtonImage(butRect, texRemove, Color.grey) && removeAction != null)
		{
			removeAction(def);
			selectedDef = default(T);
		}
		GUI.color = Color.white;
	}

	private void BlockSelectClick<T>(Rect rect, T def, ref T selectedDef)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonInvisible(rect))
		{
			selectedDef = ((selectedDef != null) ? default(T) : def);
		}
	}

	private void BLockNumericValue<T>(bool isInt, ref float value, ref float secValue, float min, float max)
	{
		curY += DefSelectionLineHeight;
		if (isInt)
		{
			value = SZWidgets.NumericIntBox(curX, curY, 80f, 30f, (int)value, (int)min, (int)max);
		}
		else
		{
			value = SZWidgets.NumericFloatBox(curX, curY, 70f, 30f, value, min, max);
		}
		if (typeof(T) == typeof(PawnCapacityDef))
		{
			secValue = SZWidgets.NumericFloatBox(curX + 150f, curY, 70f, 30f, secValue, min, max);
		}
		curY += DefSelectionLineHeight;
	}

	internal void ListViewParam1<T>(float width, T def, ref T selectedDef, bool bRemoveOnClick, Action<T> removeAction) where T : Def
	{
		float value = float.MinValue;
		ListViewParam(width, def, ref selectedDef, ref value, ref value, value, value, isInt: false, bRemoveOnClick, removeAction);
	}

	internal void ListViewParam<T>(float width, T def, ref T selectedDef, ref float value, ref float secValue, float min, float max, bool isInt, bool bRemoveOnClick, Action<T> removeAction) where T : Def
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		Texture2D tIcon = def.GetTIcon();
		int offset = 0;
		if ((Object)(object)tIcon != (Object)null)
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(curX, curY, DefSelectionLineHeight, DefSelectionLineHeight);
			GUI.color = def.GetTColor();
			GUI.DrawTexture(val, (Texture)(object)def.GetTIcon());
			GUI.color = Color.white;
			offset = (int)DefSelectionLineHeight;
		}
		Rect rect = BlockLabel(offset, width, def, value, secValue);
		if (def == selectedDef && value != float.MinValue)
		{
			BLockNumericValue<T>(isInt, ref value, ref secValue, min, max);
		}
		BlockTooltip(rect, def);
		Text.Anchor = (TextAnchor)0;
		curY += DefSelectionLineHeight;
		if (bRemoveOnClick)
		{
			BlockRemove(rect, def, ref selectedDef, removeAction);
		}
		else
		{
			BlockSelectClick(rect, def, ref selectedDef);
		}
	}

	internal bool ListviewSM(float width, StatModifier sm, bool selected, bool withRemove, Action<string> action)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		if (sm == null || sm.stat == null)
		{
			return false;
		}
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, width - DefSelectionLineHeight, DefSelectionLineHeight);
		Text.WordWrap = false;
		Widgets.Label(val, sm.stat.label + ": " + sm.value);
		Text.WordWrap = true;
		if (selected)
		{
			curY += DefSelectionLineHeight;
			float min = ((sm.value < sm.stat.minValue) ? (sm.value - 10f) : sm.stat.minValue);
			sm.value = SZWidgets.NumericFloatBox(curX, curY, 70f, 30f, sm.value, min, sm.stat.maxValue);
			curY += DefSelectionLineHeight;
		}
		string tooltip = sm.stat.label + "\n" + sm.stat.category.label.Colorize(Color.yellow) + "\n\n";
		tooltip += sm.stat.description;
		TooltipHandler.TipRegion(tip: new TipSignal(() => tooltip, sm.stat.shortHash), rect: val);
		Text.Anchor = (TextAnchor)0;
		curY += DefSelectionLineHeight;
		if (withRemove)
		{
			Rect butRect = default(Rect);
			((Rect)(ref butRect))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width, ((Rect)(ref val)).y, DefSelectionLineHeight, DefSelectionLineHeight);
			if (Widgets.ButtonImage(butRect, texRemove, Color.grey))
			{
				action?.Invoke(sm.stat.defName);
			}
			GUI.color = Color.white;
		}
		return Widgets.ButtonInvisible(val);
	}

	private string GetFormattedValue(string format, float value)
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

	internal void AddMultiplierSection(string paramName, string format, ref string selectedName, float baseValue, ref float value, float min = float.MinValue, float max = float.MaxValue, bool small = false)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = (small ? GameFont.Small : GameFont.Medium);
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(curX, curY, ((Rect)(ref listingRect)).width - 130f, 30f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, ((Rect)(ref listingRect)).width, 30f);
		float num = baseValue * value;
		string s = paramName + GetFormattedValue(format, num);
		Widgets.Label(rect, s.Colorize((num > 0f) ? Color.green : ((num == 0f) ? Color.grey : Color.red)));
		if (paramName == selectedName)
		{
			Text.Font = GameFont.Small;
			value = SZWidgets.NumericFloatBox(curX + ((Rect)(ref listingRect)).width - 105f, curY + 4f, 70f, 25f, value, min, float.MaxValue);
		}
		curY += 30f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(curX, curY, ((Rect)(ref listingRect)).width, 20f);
		Widgets.DrawBoxSolid(new Rect(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y, ((Rect)(ref rect2)).width, ((Rect)(ref rect2)).height / 2f), new Color(0.195f, 0.195f, 0.193f));
		value = Widgets.HorizontalSlider(rect2, value, min, max);
		string text = value.ToString().SubstringFrom(".");
		if (text.Length > 2)
		{
			value = (float)Math.Round(value, 2);
		}
		curY += 20f;
		if (Widgets.ButtonInvisible(butRect))
		{
			selectedName = ((selectedName == paramName) ? null : paramName);
		}
	}

	internal void AddSection(string paramName, string format, ref string selectedName, ref float value, float min = float.MinValue, float max = float.MaxValue, bool small = false, string toolTip = "")
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = (small ? GameFont.Small : GameFont.Medium);
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(curX, curY, ((Rect)(ref listingRect)).width - 130f, 30f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, ((Rect)(ref listingRect)).width, 30f);
		paramName = paramName ?? "";
		Widgets.Label(rect, paramName + GetFormattedValue(format, value));
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, toolTip);
		}
		if (paramName == selectedName)
		{
			Text.Font = GameFont.Small;
			value = SZWidgets.NumericFloatBox(curX + ((Rect)(ref listingRect)).width - 105f, curY + 4f, 70f, 25f, value, min, float.MaxValue);
		}
		curY += 30f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(curX, curY, ((Rect)(ref listingRect)).width, 20f);
		value = Widgets.HorizontalSlider(rect2, value, min, max);
		string text = value.ToString().SubstringFrom(".");
		if (text.Length > 2)
		{
			value = (float)Math.Round(value, 2);
		}
		curY += 20f;
		if (Widgets.ButtonInvisible(butRect))
		{
			selectedName = ((selectedName == paramName) ? null : paramName);
		}
	}

	internal void AddIntSection(string paramName, string format, ref string selectedName, ref int value, int min = int.MinValue, int max = int.MaxValue, bool small = false, string toolTip = "", bool tiny = false)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = ((!tiny) ? (small ? GameFont.Small : GameFont.Medium) : GameFont.Tiny);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, ((Rect)(ref listingRect)).width - 130f, 30f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, ((Rect)(ref listingRect)).width, 30f);
		Widgets.Label(rect, paramName + GetFormattedValue(format, value));
		if (paramName == selectedName)
		{
			Text.Font = GameFont.Small;
			value = SZWidgets.NumericIntBox(curX + ((Rect)(ref listingRect)).width - 105f, curY + 4f, 70f, 25f, value, min, int.MaxValue);
		}
		curY += 30f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(curX, curY, ((Rect)(ref listingRect)).width, 20f);
		value = (int)Widgets.HorizontalSlider(rect2, value, min, max);
		curY += 20f;
		if (Widgets.ButtonInvisible(val))
		{
			selectedName = ((selectedName == paramName) ? null : paramName);
		}
		if (!toolTip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val, toolTip);
		}
	}

	internal bool RadioButton(string label, bool active, float tabIn = 0f, string tooltip = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		float lineHeight = Text.LineHeight;
		Rect rect = GetRect(lineHeight);
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + tabIn;
		if (!tooltip.NullOrEmpty())
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			TooltipHandler.TipRegion(rect, tooltip);
		}
		bool result = Widgets.RadioButtonLabeled(rect, label, active);
		Gap(verticalSpacing);
		return result;
	}

	internal bool SelectableFast(string text, bool selected, string tooltip)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, ((Rect)(ref listingRect)).width, DefSelectionLineHeight);
		if (selected)
		{
			GUI.DrawTexture(val, (Texture)(object)TexUI.TextBGBlack);
			GUI.color = Color.green;
			Widgets.DrawHighlight(val);
		}
		else
		{
			GUI.color = Color.white;
		}
		Widgets.Label(val, text);
		TooltipHandler.TipRegion(val, tooltip);
		curY += DefSelectionLineHeight;
		return Widgets.ButtonInvisible(val);
	}

	internal bool SelectableAbility(string name, bool selected, string tooltip, AbilityDef def, Color selColor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		float width = ((Rect)(ref listingRect)).width;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, 64f, 64f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, width, 64f);
		GUI.DrawTexture(val, (Texture)(object)def.uiIcon);
		TooltipHandler.TipRegion(val2, tooltip);
		if (selected)
		{
			GUI.DrawTexture(val2, (Texture)(object)TexUI.TextBGBlack);
			GUI.color = selColor;
			Widgets.DrawHighlight(val2);
		}
		GUI.color = Color.white;
		Widgets.Label(new Rect(((Rect)(ref val)).x + 68f, ((Rect)(ref val)).y + 23f, width - 64f, 28f), name);
		return Widgets.ButtonInvisible(val2, doMouseoverSound: false);
	}

	internal bool SelectableGene(string name, bool selected, string tooltip, GeneDef def, Color selColor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		float width = ((Rect)(ref listingRect)).width;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, 64f, 64f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, width, 64f);
		GUI.DrawTexture(val, (Texture)(object)def.Icon, (ScaleMode)0, true, 0f, def.IconColor, 0f, 0f);
		TooltipHandler.TipRegion(val2, tooltip);
		if (selected)
		{
			GUI.DrawTexture(val2, (Texture)(object)TexUI.TextBGBlack);
			GUI.color = selColor;
			Widgets.DrawHighlight(val2);
		}
		GUI.color = Color.white;
		Widgets.Label(new Rect(((Rect)(ref val)).x + 68f, ((Rect)(ref val)).y + 23f, width - 72f, 38f), name);
		return Widgets.ButtonInvisible(val2, doMouseoverSound: false);
	}

	internal bool SelectableThing(string name, bool selected, string tooltip, ThingDef def, Color selColor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		float width = ((Rect)(ref listingRect)).width;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, 64f, 64f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, width, 64f);
		Texture2D tIcon = def.GetTIcon();
		if ((Object)(object)tIcon != (Object)null)
		{
			GUI.DrawTexture(val, (Texture)(object)tIcon, (ScaleMode)0, true, 0f, def.uiIconColor, 0f, 0f);
		}
		TooltipHandler.TipRegion(val2, tooltip);
		if (selected)
		{
			GUI.DrawTexture(val2, (Texture)(object)TexUI.TextBGBlack);
			GUI.color = selColor;
			Widgets.DrawHighlight(val2);
		}
		GUI.color = Color.white;
		Widgets.Label(new Rect(((Rect)(ref val)).x + 68f, ((Rect)(ref val)).y + 23f, width - 72f, 38f), name);
		return Widgets.ButtonInvisible(val2, doMouseoverSound: false);
	}

	internal bool SelectableText<T>(string name, bool isThing, bool isAbility, bool isGene, bool isHair, bool isBeard, bool selected, string tooltip, bool withRemove, bool isWhite, T def, Color selColor, bool hasIcon = false, bool selectOnMouseOver = false)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		if (name == null)
		{
			return false;
		}
		float width = ((Rect)(ref listingRect)).width;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, width, DefSelectionLineHeight);
		try
		{
			if (isAbility)
			{
				return SelectableAbility(name, selected, tooltip, def as AbilityDef, selColor);
			}
			if (isGene)
			{
				return SelectableGene(name, selected, tooltip, def as GeneDef, selColor);
			}
			if (isThing)
			{
				return SelectableThing(name, selected, tooltip, def as ThingDef, selColor);
			}
			if (selected)
			{
				GUI.DrawTexture(val, (Texture)(object)TexUI.TextBGBlack);
				GUI.color = selColor;
				Widgets.DrawHighlight(val);
			}
			if (hasIcon)
			{
				Rect outerRect = default(Rect);
				((Rect)(ref outerRect))._002Ector(curX, curY, DefSelectionLineHeight, DefSelectionLineHeight);
				Texture2D tIcon = def.GetTIcon();
				if ((Object)(object)tIcon != (Object)null)
				{
					GUI.color = def.GetTColor();
					Widgets.DrawTextureFitted(outerRect, (Texture)(object)tIcon, 1f);
				}
			}
			Text.WordWrap = false;
			GUI.color = ((!isWhite) ? Color.gray : (selected ? Color.green : Color.white));
			if (hasIcon || isThing)
			{
				Widgets.Label(new Rect(((Rect)(ref val)).x + DefSelectionLineHeight, ((Rect)(ref val)).y, ((Rect)(ref val)).width, ((Rect)(ref val)).height), name);
			}
			else
			{
				Widgets.Label(val, name);
			}
			GUI.color = Color.white;
			if (!string.IsNullOrEmpty(tooltip))
			{
				TooltipHandler.TipRegion(val, tooltip);
			}
			Text.WordWrap = true;
			bool flag = false;
			if (isHair)
			{
				Rect rect = default(Rect);
				((Rect)(ref rect))._002Ector(curX, curY + DefSelectionLineHeight, 192f, 64f);
				Widgets.DrawBoxSolid(rect, ColorTool.colAsche);
				string texPath = (def as HairDef).texPath;
				if (!texPath.NullOrEmpty())
				{
					Graphic g = GraphicDatabase.Get<Graphic_Multi>(texPath);
					Rect val2 = default(Rect);
					((Rect)(ref val2))._002Ector(curX, curY + DefSelectionLineHeight, 64f, 64f);
					Texture2D textureFromMulti = g.GetTextureFromMulti();
					GUI.color = Color.white;
					if ((Object)(object)textureFromMulti != (Object)null)
					{
						GUI.DrawTexture(val2, (Texture)(object)textureFromMulti);
					}
					Rect val3 = default(Rect);
					((Rect)(ref val3))._002Ector(curX + 64f, curY + DefSelectionLineHeight, 64f, 64f);
					Texture2D textureFromMulti2 = g.GetTextureFromMulti("_east");
					if ((Object)(object)textureFromMulti2 == (Object)null)
					{
						textureFromMulti2 = g.GetTextureFromMulti("_west");
					}
					GUI.color = Color.white;
					if ((Object)(object)textureFromMulti2 != (Object)null)
					{
						GUI.DrawTexture(val3, (Texture)(object)textureFromMulti2);
					}
					Rect val4 = default(Rect);
					((Rect)(ref val4))._002Ector(curX + 128f, curY + DefSelectionLineHeight, 64f, 64f);
					Texture2D textureFromMulti3 = g.GetTextureFromMulti("_north");
					GUI.color = Color.white;
					if ((Object)(object)textureFromMulti3 != (Object)null)
					{
						GUI.DrawTexture(val4, (Texture)(object)textureFromMulti3);
					}
				}
				if (selectOnMouseOver && Mouse.IsOver(rect))
				{
					flag = true;
				}
			}
			if (isBeard)
			{
				Graphic g2 = GraphicDatabase.Get<Graphic_Multi>((def as BeardDef).texPath);
				Rect rect2 = default(Rect);
				((Rect)(ref rect2))._002Ector(curX, curY + DefSelectionLineHeight, 192f, 64f);
				Widgets.DrawBoxSolid(rect2, ColorTool.colAsche);
				Rect val5 = default(Rect);
				((Rect)(ref val5))._002Ector(curX, curY + DefSelectionLineHeight, 64f, 64f);
				Texture2D textureFromMulti4 = g2.GetTextureFromMulti();
				GUI.color = Color.white;
				GUI.DrawTexture(val5, (Texture)(object)textureFromMulti4);
				Rect val6 = default(Rect);
				((Rect)(ref val6))._002Ector(curX + 64f, curY + DefSelectionLineHeight, 64f, 64f);
				Texture2D textureFromMulti5 = g2.GetTextureFromMulti("_east");
				if ((Object)(object)textureFromMulti5 == (Object)null)
				{
					textureFromMulti5 = g2.GetTextureFromMulti("_west");
				}
				GUI.color = Color.white;
				GUI.DrawTexture(val6, (Texture)(object)textureFromMulti5);
				Rect val7 = default(Rect);
				((Rect)(ref val7))._002Ector(curX + 128f, curY + DefSelectionLineHeight, 64f, 64f);
				Texture2D textureFromMulti6 = g2.GetTextureFromMulti("_north");
				GUI.DrawTexture(val7, (Texture)(object)textureFromMulti6);
				if (selectOnMouseOver && Mouse.IsOver(rect2))
				{
					flag = true;
				}
			}
			if (withRemove)
			{
				Rect butRect = default(Rect);
				((Rect)(ref butRect))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width - DefSelectionLineHeight - 12f, ((Rect)(ref val)).y, DefSelectionLineHeight, DefSelectionLineHeight);
				return Widgets.ButtonImage(butRect, texRemove);
			}
			if (selectOnMouseOver && flag)
			{
				return true;
			}
			return Widgets.ButtonInvisible(val, doMouseoverSound: false);
		}
		catch
		{
			return false;
		}
	}

	internal int SelectableHorizontal(string name, bool selected, string tooltip = "", RenderTexture image = null, ThingDef thingDef = null, HairDef hairDef = null, Vector2 imageSize = default(Vector2), bool withRemove = false, float selectHeight = 22f, Color backColor = default(Color))
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		if (name == null)
		{
			return 0;
		}
		Text.Font = GameFont.Small;
		float num = ((Rect)(ref listingRect)).width - DefSelectionLineHeight;
		Text.Anchor = (TextAnchor)3;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, num + DefSelectionLineHeight, selectHeight);
		if (selected)
		{
			GUI.DrawTexture(val, (Texture)(object)TexUI.TextBGBlack);
			GUI.color = Color.green;
			Widgets.DrawHighlight(val);
		}
		Rect rect = default(Rect);
		if (thingDef != null)
		{
			((Rect)(ref rect))._002Ector(curX + 25f, curY, num, DefSelectionLineHeight);
		}
		else if (selectHeight == 22f)
		{
			((Rect)(ref rect))._002Ector(curX, curY, num, DefSelectionLineHeight);
		}
		else
		{
			((Rect)(ref rect))._002Ector(curX, curY, num + DefSelectionLineHeight, DefSelectionLineHeight);
		}
		Text.WordWrap = false;
		if (backColor != default(Color))
		{
			GUI.color = backColor;
		}
		Widgets.Label(rect, name);
		GUI.color = Color.white;
		if (!string.IsNullOrEmpty(tooltip))
		{
			TooltipHandler.TipRegion(rect, tooltip);
		}
		Text.WordWrap = true;
		if ((Object)(object)image != (Object)null)
		{
			Rect val2 = default(Rect);
			if (imageSize == default(Vector2))
			{
				((Rect)(ref val2))._002Ector(curX, curY + DefSelectionLineHeight, 64f, 90f);
			}
			else
			{
				((Rect)(ref val2))._002Ector(curX, curY + DefSelectionLineHeight, imageSize.x, imageSize.y);
			}
			GUI.color = Color.white;
			GUI.DrawTexture(val2, (Texture)(object)image);
		}
		Text.Anchor = (TextAnchor)0;
		if ((Object)(object)image != (Object)null)
		{
			if (imageSize == default(Vector2))
			{
				curX += 100f;
			}
			else
			{
				curX += imageSize.x;
			}
		}
		if (withRemove)
		{
			Rect butRect = default(Rect);
			((Rect)(ref butRect))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width - DefSelectionLineHeight, ((Rect)(ref val)).y, DefSelectionLineHeight, DefSelectionLineHeight);
			if (Widgets.ButtonImage(butRect, texRemove))
			{
				return 2;
			}
			if (Widgets.ButtonInvisible(val, doMouseoverSound: false))
			{
				return 1;
			}
			return 0;
		}
		if (Widgets.ButtonInvisible(val, doMouseoverSound: false))
		{
			return 1;
		}
		return 0;
	}

	internal int Selectable(string name, bool selected, string tooltip = "", RenderTexture image = null, ThingDef thingDef = null, HairDef hairDef = null, Vector2 imageSize = default(Vector2), bool withRemove = false, float selectHeight = 22f, Color backColor = default(Color), Color selectedColor = default(Color), bool autoincrement = true)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		if (name == null)
		{
			return 0;
		}
		Text.Font = GameFont.Small;
		float num = ((Rect)(ref listingRect)).width - DefSelectionLineHeight;
		Text.Anchor = (TextAnchor)3;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, num + DefSelectionLineHeight, selectHeight);
		if (selected)
		{
			GUI.DrawTexture(val, (Texture)(object)TexUI.TextBGBlack);
			GUI.color = selectedColor;
			Widgets.DrawHighlight(val);
		}
		GUI.color = Color.white;
		if (thingDef != null)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(curX, curY, DefSelectionLineHeight, DefSelectionLineHeight);
			Widgets.ThingIcon(rect, thingDef);
		}
		Rect rect2 = default(Rect);
		if (thingDef != null)
		{
			((Rect)(ref rect2))._002Ector(curX + 25f, curY, num, DefSelectionLineHeight);
		}
		else if (selectHeight == 22f)
		{
			((Rect)(ref rect2))._002Ector(curX, curY, num, DefSelectionLineHeight);
		}
		else
		{
			((Rect)(ref rect2))._002Ector(curX, curY, num + DefSelectionLineHeight, (name.Length > 10) ? (DefSelectionLineHeight + 12f) : DefSelectionLineHeight);
		}
		if (backColor != default(Color))
		{
			GUI.color = backColor;
		}
		Widgets.Label(rect2, name);
		GUI.color = Color.white;
		if (!string.IsNullOrEmpty(tooltip))
		{
			TooltipHandler.TipRegion(val, tooltip);
		}
		if ((Object)(object)image != (Object)null)
		{
			float num2 = (((Rect)(ref listingRect)).width - imageSize.x) / 2f;
			Rect val2 = default(Rect);
			if (imageSize == default(Vector2))
			{
				((Rect)(ref val2))._002Ector(curX, curY + DefSelectionLineHeight, 64f, 90f);
			}
			else
			{
				((Rect)(ref val2))._002Ector(curX + num2, curY + 11f, imageSize.x, imageSize.y);
			}
			GUI.color = Color.white;
			GUI.DrawTexture(val2, (Texture)(object)image);
		}
		if (hairDef != null)
		{
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(curX, curY + DefSelectionLineHeight, 64f, 64f);
			Graphic g = GraphicDatabase.Get<Graphic_Multi>(hairDef.texPath);
			Texture2D textureFromMulti = g.GetTextureFromMulti();
			GUI.color = Color.white;
			GUI.DrawTexture(val3, (Texture)(object)textureFromMulti);
			Rect val4 = default(Rect);
			((Rect)(ref val4))._002Ector(curX + 64f, curY + DefSelectionLineHeight, 64f, 64f);
			Texture2D textureFromMulti2 = g.GetTextureFromMulti("_east");
			if ((Object)(object)textureFromMulti2 == (Object)null)
			{
				textureFromMulti2 = g.GetTextureFromMulti("_west");
			}
			GUI.color = Color.white;
			GUI.DrawTexture(val4, (Texture)(object)textureFromMulti2);
			Rect val5 = default(Rect);
			((Rect)(ref val5))._002Ector(curX + 128f, curY + DefSelectionLineHeight, 64f, 64f);
			Texture2D textureFromMulti3 = g.GetTextureFromMulti("_north");
			GUI.color = Color.white;
			GUI.DrawTexture(val5, (Texture)(object)textureFromMulti3);
		}
		Text.Anchor = (TextAnchor)0;
		if (autoincrement)
		{
			curY += DefSelectionLineHeight;
			if ((Object)(object)image != (Object)null)
			{
				if (imageSize == default(Vector2))
				{
					curY += 90f;
				}
				else
				{
					curY += imageSize.y - DefSelectionLineHeight;
				}
			}
			else if (hairDef != null)
			{
				curY += 70f;
			}
		}
		if (withRemove)
		{
			Rect butRect = default(Rect);
			((Rect)(ref butRect))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width - DefSelectionLineHeight, ((Rect)(ref val)).y + (float)((name.Length > 20) ? 6 : 0), DefSelectionLineHeight, DefSelectionLineHeight);
			if (Widgets.ButtonImage(butRect, texRemove))
			{
				return 2;
			}
			if (Widgets.ButtonInvisible(val, doMouseoverSound: false))
			{
				return 1;
			}
			return 0;
		}
		if (Widgets.ButtonInvisible(val, doMouseoverSound: false))
		{
			return 1;
		}
		return 0;
	}

	internal bool TableLine(Texture2D col0Icon, string col1Val, string col2Val, string col3Val, Texture2D col4Tex, string tooltip)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		float num = ((Rect)(ref listingRect)).width - DefSelectionLineHeight;
		float num2 = (int)(num / 3f);
		float defSelectionLineHeight = DefSelectionLineHeight;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, defSelectionLineHeight, defSelectionLineHeight);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX + defSelectionLineHeight, curY, num2, defSelectionLineHeight);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(curX + num2 + defSelectionLineHeight, curY, num2, defSelectionLineHeight);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(curX + num2 * 2f + defSelectionLineHeight, curY, num2, defSelectionLineHeight);
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(curX + num2 * 3f + defSelectionLineHeight, curY, defSelectionLineHeight, defSelectionLineHeight);
		GUI.DrawTexture(val, (Texture)(object)col0Icon);
		Widgets.Label(rect, col1Val);
		Widgets.Label(rect2, col2Val);
		Widgets.Label(rect3, col3Val);
		bool result = Widgets.ButtonImage(butRect, col4Tex);
		if (!string.IsNullOrEmpty(tooltip))
		{
			TooltipHandler.TipRegion(tip: new TipSignal(() => tooltip, 2347778), rect: rect2);
		}
		curY += DefSelectionLineHeight;
		return result;
	}

	internal bool SelectableThought(string name, Texture2D icon, Color iconColor, float valopin = float.MinValue, float valmood = float.MinValue, string tooltip = "", string pName = "", bool withRemove = true)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		float num = ((Rect)(ref listingRect)).width - DefSelectionLineHeight;
		Text.Anchor = (TextAnchor)3;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, num + DefSelectionLineHeight, DefSelectionLineHeight);
		GUI.color = iconColor;
		if ((Object)(object)icon != (Object)null)
		{
			GUI.DrawTexture(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, 24f, 24f), (Texture)(object)icon);
		}
		Text.WordWrap = false;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX + 30f, curY, num, DefSelectionLineHeight);
		Widgets.DrawBoxSolid(rect, alternate ? new Color(0.15f, 0.15f, 0.15f) : new Color(0.1f, 0.1f, 0.1f));
		alternate = !alternate;
		Widgets.Label(rect, name);
		if (tooltip != null)
		{
			TooltipHandler.TipRegion(tip: new TipSignal(() => tooltip, 275), rect: rect);
		}
		if (!string.IsNullOrEmpty(pName))
		{
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 190f, ((Rect)(ref rect)).y - 1f, 120f, 24f);
			Widgets.Label(rect2, pName);
		}
		bool flag = valopin != float.MinValue;
		float num2 = (flag ? valopin : valmood);
		int num3 = ((num2 < 0f) ? 4 : 0);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 80f - (float)num3, ((Rect)(ref rect)).y, 48f, 24f);
		if (num2 == 0f)
		{
			GUI.color = (flag ? ColorTool.colBeige : NoEffectColor);
		}
		else if (num2 > 0f)
		{
			GUI.color = (flag ? ColorTool.colLightBlue : MoodColor);
		}
		else
		{
			GUI.color = (flag ? ColorTool.colPink : MoodColorNegative);
		}
		Widgets.Label(rect3, num2.ToString("##0"));
		bool result = false;
		if (withRemove)
		{
			Rect butRect = default(Rect);
			((Rect)(ref butRect))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 42f, ((Rect)(ref rect)).y, 24f, 24f);
			result = Widgets.ButtonImage(butRect, ContentFinder<Texture2D>.Get("UI/Buttons/Delete"));
		}
		Text.WordWrap = true;
		Text.Anchor = (TextAnchor)0;
		curY += DefSelectionLineHeight;
		return result;
	}

	internal float Slider(float val, float min, float max, Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = color;
		float result = Widgets.HorizontalSlider(GetRect(22f), val, min, max);
		Gap(verticalSpacing);
		return result;
	}

	internal float Slider(float val, float min, float max)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float result = Widgets.HorizontalSlider(GetRect(22f), val, min, max);
		Gap(verticalSpacing);
		return result;
	}

	internal float Slider(float val, float min, float max, float width)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(22f);
		((Rect)(ref rect)).width = width;
		return Widgets.HorizontalSlider(rect, val, min, max);
	}

	internal float SliderWithNumeric(float val, float min, float max, int decimals)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		float value = (float)Math.Round(Widgets.HorizontalSlider(GetRect(22f), val, min, max), decimals);
		Rect rect = GetRect(22f);
		((Rect)(ref rect)).width = 70f;
		value = SZWidgets.NumericFloatBox(rect, value, float.MinValue, float.MaxValue);
		Gap(4f);
		return value;
	}

	internal int SliderWithNumeric(int val, int min, int max)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		int value = (int)Widgets.HorizontalSlider(GetRect(22f), val, min, max);
		Rect rect = GetRect(22f);
		((Rect)(ref rect)).width = 70f;
		value = SZWidgets.NumericIntBox(rect, value, int.MinValue, int.MaxValue);
		Gap(4f);
		return value;
	}

	internal string TextEntry(string text, int lineCount = 1)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(Text.LineHeight * (float)lineCount);
		string result = ((lineCount != 1) ? Widgets.TextArea(rect, text) : Widgets.TextField(rect, text));
		Gap(verticalSpacing);
		return result;
	}

	internal string TextEntryLabeledWithDefaultAndCopy(string label, string text, string defaultVal)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(Text.LineHeight * 1f);
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width - 50f;
		Rect rect2 = rect.LeftHalf().Rounded();
		Rect rect3 = rect.RightHalf().Rounded();
		Widgets.Label(rect2, label);
		string text2 = Widgets.TextField(rect3, text);
		string[] array = text2.SplitNoEmpty(";");
		if (array.Length > 1)
		{
			TooltipHandler.TipRegion(rect2, label + "\n" + array.Length + " saved entities");
		}
		else
		{
			TooltipHandler.TipRegion(rect2, label + "\n" + array.Length + " saved entity");
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width, ((Rect)(ref rect)).y, 24f, 24f);
		if (Widgets.ButtonImage(val, ContentFinder<Texture2D>.Get("bdefault")))
		{
			text2 = defaultVal;
		}
		TooltipHandler.TipRegion(val, CharacterEditor.Label.O_SETTODEFAULT);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width + 25f, ((Rect)(ref rect)).y, 24f, 24f);
		if (Widgets.ButtonImage(val2, ContentFinder<Texture2D>.Get("UI/Buttons/Copy")))
		{
			Clipboard.CopyToClip(text);
		}
		TooltipHandler.TipRegion(val2, CharacterEditor.Label.O_COPYTOCLIPBOARD);
		Gap(verticalSpacing);
		return text2;
	}

	internal string TextEntryLabeled(string label, string text, int lineCount = 1)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(Text.LineHeight * (float)lineCount);
		string result = Widgets.TextEntryLabeled(rect, label, text);
		Gap(verticalSpacing);
		return result;
	}

	internal void TextFieldNumeric<T>(float xStart, float width, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(Text.LineHeight);
		((Rect)(ref rect)).x = xStart;
		((Rect)(ref rect)).width = width;
		Widgets.TextFieldNumeric(rect, ref val, ref buffer, min, max);
		Gap(verticalSpacing);
	}

	internal void TextFieldNumericLabeled<T>(string label, float xStart, float width, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(Text.LineHeight);
		((Rect)(ref rect)).x = xStart;
		((Rect)(ref rect)).width = width;
		Widgets.TextFieldNumericLabeled(rect, label, ref val, ref buffer, min, max);
		Gap(verticalSpacing);
	}

	private Vector2 GetLabelScrollbarPosition(float x, float y)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (labelScrollbarPositions == null)
		{
			return Vector2.zero;
		}
		for (int i = 0; i < labelScrollbarPositions.Count; i++)
		{
			Vector2 first = labelScrollbarPositions[i].First;
			if (first.x == x && first.y == y)
			{
				return labelScrollbarPositions[i].Second;
			}
		}
		return Vector2.zero;
	}

	private void SetLabelScrollbarPosition(float x, float y, Vector2 scrollbarPosition)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (labelScrollbarPositions == null)
		{
			labelScrollbarPositions = new List<Pair<Vector2, Vector2>>();
			labelScrollbarPositionsSetThisFrame = new List<Vector2>();
		}
		labelScrollbarPositionsSetThisFrame.Add(new Vector2(x, y));
		for (int i = 0; i < labelScrollbarPositions.Count; i++)
		{
			Vector2 first = labelScrollbarPositions[i].First;
			if (first.x == x && first.y == y)
			{
				labelScrollbarPositions[i] = new Pair<Vector2, Vector2>(new Vector2(x, y), scrollbarPosition);
				return;
			}
		}
		labelScrollbarPositions.Add(new Pair<Vector2, Vector2>(new Vector2(x, y), scrollbarPosition));
	}

	internal void NavSelectorColor(int width, string label, string tip, Color? color, Action onClicked)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(22f);
		((Rect)(ref rect)).width = width;
		Text.Font = GameFont.Small;
		SZWidgets.LabelBackground(RectSolid(rect, showEdit: false), label, ColorTool.colAsche, 0, tip);
		SZWidgets.ButtonSolid(RectRandom(rect), (Color)(((_003F?)color) ?? Color.clear), onClicked);
	}

	internal Rect GetRect2(int w, int h = 22)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(h);
		((Rect)(ref rect)).width = w;
		return rect;
	}

	private static Rect RectNext(Rect rect)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 22f, ((Rect)(ref rect)).y + 2f, 22f, 22f);
	}

	private static Rect RectOnClick(Rect rect, bool hasTexture, int offset, bool showEdit)
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

	private static Rect RectSolid(Rect rect, bool showEdit)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + (float)(showEdit ? 21 : 0), ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - (float)(showEdit ? 40 : 19), 24f);
	}

	private static Rect RectTexture(Rect rect, bool showEdit)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + (float)(showEdit ? 25 : 0), ((Rect)(ref rect)).y, 24f, 24f);
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
}
