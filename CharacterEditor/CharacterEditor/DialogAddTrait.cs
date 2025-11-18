using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogAddTrait : Window
{
	private List<KeyValuePair<TraitDef, TraitDegreeData>> lOfTraits;

	private Vector2 scrollPos;

	private KeyValuePair<TraitDef, TraitDegreeData> selectedTrait;

	private KeyValuePair<TraitDef, TraitDegreeData> oldSelectedTrait;

	private bool doOnce;

	private HashSet<StatModifier> lOfSM;

	private List<string> lOfFilters;

	private SearchTool search;

	private Trait oldTrait;

	private Func<StatModifier, string> FStatLabel = (StatModifier t) => (t == null) ? Label.ALL : t.stat.LabelCap.ToString();

	public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

	internal DialogAddTrait(Trait _trait = null)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		oldTrait = _trait;
		scrollPos = default(Vector2);
		doOnce = true;
		search = SearchTool.Update(SearchTool.SIndex.TraitDef);
		lOfSM = TraitTool.ListOfTraitStatModifier(search.modName, withNull: true);
		lOfFilters = new List<string>();
		lOfFilters.Add(null);
		lOfFilters.Add(Label.STAT);
		lOfFilters.Add(Label.MENTAL);
		lOfFilters.Add(Label.THOUGHTS);
		lOfFilters.Add(Label.INSPIRATIONS);
		lOfFilters.Add(Label.FOCUS);
		lOfFilters.Add(Label.SKILLGAINS);
		lOfFilters.Add(Label.ABILITIES);
		lOfFilters.Add(Label.NEEDS);
		lOfFilters.Add(Label.INGESTIBLEMOD);
		lOfTraits = TraitTool.ListOfTraitsKeyValuePair(search.modName, (StatModifier)search.ofilter1, search.filter1);
		TraitTool.UpdateDicTooltip(lOfTraits);
		selectedTrait = lOfTraits.FirstOrDefault();
		oldSelectedTrait = selectedTrait;
		doCloseX = true;
		absorbInputAroundWindow = true;
		closeOnCancel = true;
		closeOnClickedOutside = true;
		draggable = true;
		layer = CEditor.Layer;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (doOnce)
		{
			SearchTool.SetPosition(SearchTool.SIndex.TraitDef, ref windowRect, ref doOnce, 105);
		}
		int h = (int)InitialSize.y - 115;
		int num = (int)InitialSize.x - 40;
		int x = 0;
		int y = 0;
		SZWidgets.ButtonImage(num - 25, 0f, 25f, 25f, "brandom", ARandomTrait);
		y = DrawTitle(x, y, num, 30);
		y = DrawDropdown(x, y, num);
		y = DrawList(x, y, num, h);
		WindowTool.SimpleAcceptButton(this, DoAndClose);
	}

	private int DrawTitle(int x, int y, int w, int h)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect((float)x, (float)y, (float)w, (float)h), Label.ADD_TRAIT);
		return h;
	}

	private int DrawDropdown(int x, int y, int w)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector((float)x, (float)y, (float)w, 30f);
		SZWidgets.FloatMenuOnButtonText(val, search.SelectedModName, CEditor.API.Get<HashSet<string>>(EType.ModsTraitDef), (string s) => s ?? Label.ALL, AChangedModName);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(val);
		((Rect)(ref val2)).y = ((Rect)(ref val)).y + 30f;
		SZWidgets.FloatMenuOnButtonText(val2, FStatLabel((StatModifier)search.ofilter1), lOfSM, FStatLabel, AChangedSM);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(val2);
		((Rect)(ref rect)).y = ((Rect)(ref val2)).y + 30f;
		SZWidgets.FloatMenuOnButtonText(rect, search.SelectedFilter1, lOfFilters, (string s) => (s == null) ? Label.ALL : s, AChangedCategory);
		return y + 90;
	}

	private int DrawList(int x, int y, int w, int h)
	{
		Text.Font = GameFont.Small;
		SZWidgets.ListView(x, y, w, h - y + 30, lOfTraits, TraitTool.FTraitLabel, TraitTool.FTraitTooltip, TraitTool.FTraitComparator, ref selectedTrait, ref scrollPos);
		if (!TraitTool.FTraitComparator(oldSelectedTrait, selectedTrait))
		{
			oldSelectedTrait = selectedTrait;
			if (Prefs.DevMode)
			{
				MessageTool.Show(selectedTrait.Key.defName);
			}
		}
		return h - y;
	}

	private void AChangedModName(string val)
	{
		search.modName = val;
		lOfTraits = TraitTool.ListOfTraitsKeyValuePair(search.modName, (StatModifier)search.ofilter1, search.filter1);
		TraitTool.UpdateDicTooltip(lOfTraits);
	}

	private void AChangedSM(StatModifier val)
	{
		search.ofilter1 = val;
		lOfTraits = TraitTool.ListOfTraitsKeyValuePair(search.modName, (StatModifier)search.ofilter1, search.filter1);
		TraitTool.UpdateDicTooltip(lOfTraits);
	}

	private void AChangedCategory(string val)
	{
		search.filter1 = val;
		lOfTraits = TraitTool.ListOfTraitsKeyValuePair(search.modName, (StatModifier)search.ofilter1, search.filter1);
		TraitTool.UpdateDicTooltip(lOfTraits);
	}

	private void ARandomTrait()
	{
		selectedTrait = lOfTraits.RandomElement();
		SZWidgets.sFind = TraitTool.FTraitLabel(selectedTrait);
	}

	private void DoAndClose()
	{
		if (selectedTrait.Key != null)
		{
			CEditor.API.Pawn.AddTrait(selectedTrait.Key, selectedTrait.Value, random: false, doChangeSkillValue: true, oldTrait);
		}
		Close();
	}

	public override void Close(bool doCloseSound = true)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SearchTool.Save(SearchTool.SIndex.TraitDef, ((Rect)(ref windowRect)).position);
		base.Close(doCloseSound);
	}

	public override void OnAcceptKeyPressed()
	{
		base.OnAcceptKeyPressed();
		DoAndClose();
	}
}
