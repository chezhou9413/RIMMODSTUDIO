using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal abstract class DialogTemplate<T> : Window where T : Def
{
	private Func<string, string> FGetTagLabel = (string s) => s ?? Label.NONE;

	private SearchTool.SIndex idxList;

	internal Vector2 scrollPosParam;

	private bool bDoOnce;

	internal bool bRemoveOnClick;

	internal bool mInPlacingMode = false;

	internal int id;

	internal StatDef selected_StatFactor = null;

	internal StatDef selected_StatOffset = null;

	private string title;

	internal string customAcceptLabel;

	internal T selectedDef;

	internal T oldSelectedDef;

	internal HashSet<T> lDefs;

	internal HashSet<string> lMods;

	internal SearchTool search;

	internal int x;

	internal int y;

	internal int frameW;

	internal int frameH;

	internal int xPosOffset;

	internal int hScrollParam;

	internal Listing_X view;

	internal int iTick_AllowRemoveStat;

	internal const int WIDTH_EXTENDED = 1000;

	internal const int WIDTH_DEFAULT = 500;

	internal const float WTITLE = 500f;

	internal float ELEMENTH => SZWidgets.GetGraphicH<T>();

	internal bool IsGeneDef => selectedDef.GetType() == typeof(GeneDef);

	internal Color RemoveColor => (iTick_AllowRemoveStat > 0) ? Color.red : Color.white;

	internal int WPARAM => 1000 - (int)WindowTool.DefaultToolWindow.x;

	public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

	internal void ToggleRemove()
	{
		iTick_AllowRemoveStat = ((iTick_AllowRemoveStat <= 0) ? 2000 : 0);
	}

	internal DialogTemplate(SearchTool.SIndex listIdx, string _title, int _xPosOffset = 0)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		title = _title;
		xPosOffset = _xPosOffset;
		scrollPosParam = default(Vector2);
		view = new Listing_X();
		selected_StatFactor = null;
		selected_StatOffset = null;
		iTick_AllowRemoveStat = 0;
		bRemoveOnClick = false;
		bDoOnce = true;
		idxList = listIdx;
		search = SearchTool.Update(idxList);
		x = 0;
		y = 0;
		lMods = ListModnames();
		Preselection();
		doCloseX = true;
		absorbInputAroundWindow = true;
		closeOnClickedOutside = true;
		closeOnCancel = true;
		draggable = true;
		layer = CEditor.Layer;
	}

	internal virtual HashSet<string> ListModnames()
	{
		return DefTool.ListModnamesWithNull<T>().ToHashSet();
	}

	internal virtual void Preselection()
	{
		ASelectedModName(search.modName);
		selectedDef = lDefs.FirstOrDefault();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		SizeAndPosition();
		DrawTitle(x, y, frameW, 30);
		y += 30;
		if (!mInPlacingMode)
		{
			SZWidgets.ButtonImage(frameW - 25, 0f, 25f, 25f, "brandom", ARandomDef);
		}
		DrawDropdownModname(x, y, frameW, 30);
		y += 30;
		y += DrawCustomFilter(x, y, frameW);
		y += 2;
		Text.Font = GameFont.Small;
		SZWidgets.ListView(x, y, frameW, frameH + 28 - y, lDefs, (T def) => def.SLabel(), (T def) => def.STooltip(), DefTool.DefNameComparator, ref selectedDef, ref search.scrollPos);
		if (!DefTool.DefNameComparator(oldSelectedDef, selectedDef))
		{
			oldSelectedDef = selectedDef;
			if (Prefs.DevMode && selectedDef != null)
			{
				MessageTool.Show(selectedDef.defName);
			}
			OnSelectionChanged();
		}
		if (CEditor.IsExtendedUI)
		{
			DrawParameterBase();
		}
		DrawLowerButtons();
	}

	internal void DrawParameterBase()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (selectedDef != null)
		{
			CalcHSCROLL();
			id = 1;
			if (iTick_AllowRemoveStat > 0)
			{
				iTick_AllowRemoveStat--;
			}
			bRemoveOnClick = iTick_AllowRemoveStat > 0;
			Rect outRect = default(Rect);
			((Rect)(ref outRect))._002Ector(WindowTool.DefaultToolWindow.x - 20f, 0f, (float)(WPARAM - 12), (float)(frameH + 20));
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, (float)hScrollParam);
			Widgets.BeginScrollView(outRect, ref scrollPosParam, val);
			Rect rect = val.ContractedBy(4f);
			((Rect)(ref rect)).y = ((Rect)(ref rect)).y - 4f;
			((Rect)(ref rect)).height = hScrollParam;
			view.Begin(rect);
			view.verticalSpacing = 30f;
			DrawParameter();
			view.End();
			Widgets.EndScrollView();
		}
	}

	internal abstract void CalcHSCROLL();

	internal abstract void DrawParameter();

	internal abstract void AReset();

	internal abstract void AResetAll();

	internal abstract void ASave();

	internal abstract HashSet<T> TList();

	internal virtual void DrawLowerButtons()
	{
		WindowTool.SimpleAcceptAndExtend(this, DoAndClose, AReset, AResetAll, ASave, 1000, customAcceptLabel);
	}

	private void SizeAndPosition()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (bDoOnce)
		{
			SearchTool.SetPosition(idxList, ref windowRect, ref bDoOnce, xPosOffset);
		}
		frameW = (int)InitialSize.x - 40;
		frameH = (int)InitialSize.y - 115;
		y = 0;
		x = 0;
	}

	internal virtual void DrawTitle(int x, int y, int w, int h)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect((float)x, (float)y, (float)w, (float)h), title);
	}

	private void DrawDropdownModname(int x, int y, int w, int h)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector((float)x, (float)y, (float)w, (float)h);
		SZWidgets.FloatMenuOnButtonText(rect, FLabel.TString(search.modName), lMods, FLabel.TString, ASelectedModName);
	}

	internal virtual void ASelectedModName(string val)
	{
		search.modName = val;
		lDefs = TList();
	}

	internal abstract int DrawCustomFilter(int x, int y, int w);

	internal abstract void OnAccept();

	internal abstract void OnSelectionChanged();

	private void DoAndClose()
	{
		if (selectedDef != null)
		{
			OnAccept();
		}
		if (!mInPlacingMode)
		{
			Close();
		}
	}

	public override void OnAcceptKeyPressed()
	{
		base.OnAcceptKeyPressed();
		DoAndClose();
	}

	public override void Close(bool doCloseSound = true)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SearchTool.Save(idxList, ((Rect)(ref windowRect)).position);
		base.Close(doCloseSound);
	}

	private void ARandomDef()
	{
		DefTool.RandomSearchedDef(lDefs, ref selectedDef);
	}
}
