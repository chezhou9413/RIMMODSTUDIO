using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogChangeMutant : Window
{
	private Pawn pawn;

	private Vector2 scrollPos;

	private Faction selectedFaction;

	private int iChangeTick;

	private bool doOnce;

	private MutantDef selectedMutantDef;

	private RotStage selectedRotStage;

	internal Listing_X view;

	internal int x;

	internal int y;

	internal int frameW;

	internal int frameH;

	internal int xPosOffset;

	internal int hScrollParam;

	internal Vector2 scrollPosParam;

	private int mHscroll;

	private HashSet<RotStage> rotStages = new HashSet<RotStage>();

	public override Vector2 InitialSize => new Vector2(500f, (float)WindowTool.MaxH);

	internal int WPARAM => 410;

	internal DialogChangeMutant()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		xPosOffset = 0;
		view = new Listing_X();
		scrollPosParam = default(Vector2);
		x = 0;
		y = 0;
		mHscroll = 0;
		pawn = CEditor.API.Pawn;
		scrollPos = default(Vector2);
		iChangeTick = 0;
		doOnce = true;
		SearchTool.Update(SearchTool.SIndex.ChangeMutant);
		selectedMutantDef = (pawn.HasMutantTracker() ? pawn.mutant.Def : null);
		foreach (RotStage value in Enum.GetValues(typeof(RotStage)))
		{
			rotStages.Add(value);
		}
		doCloseX = true;
		absorbInputAroundWindow = true;
		closeOnCancel = true;
		closeOnClickedOutside = true;
		draggable = true;
		layer = CEditor.Layer;
	}

	public override void DoWindowContents(Rect inRect)
	{
		SizeAndPosition();
		DrawEditLabel(0f, 0f, 340f, 30f);
		DrawViewList();
		DrawLowerButtons();
	}

	private void DrawLowerButtons()
	{
		WindowTool.SimpleCustomButton(this, 0, ATurnHuman, "Turn to human", "", 120);
		WindowTool.SimpleCustomButton(this, 120, ATurnMutant, "Turn to mutant", "", 120);
		WindowTool.SimpleAcceptButton(this, DoAndClose);
	}

	private void SizeAndPosition()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (doOnce)
		{
			SearchTool.SetPosition(SearchTool.SIndex.ChangeMutant, ref windowRect, ref doOnce, 370);
		}
		frameW = (int)InitialSize.x - 40;
		frameH = (int)InitialSize.y - 115;
		y = 0;
		x = 0;
	}

	internal void CalcHSCROLL()
	{
		hScrollParam = 4000;
		if (mHscroll > 800)
		{
			hScrollParam = mHscroll;
		}
	}

	private void DrawViewList()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		CalcHSCROLL();
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, 40f, (float)WPARAM, (float)(frameH - 40));
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, (float)(hScrollParam - 30));
		Widgets.BeginScrollView(outRect, ref scrollPosParam, val);
		Rect rect = val.ContractedBy(4f);
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y - 4f;
		((Rect)(ref rect)).height = hScrollParam;
		view.Begin(rect);
		view.verticalSpacing = 30f;
		DrawParameters(400);
		view.End();
		Widgets.EndScrollView();
	}

	private void DrawParameters(int w)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		view.CurY += 5f;
		SZWidgets.DefSelectorSimple(view.GetRect(22f), w, PawnxTool.AllMutantDefs, ref selectedMutantDef, "MutantDef: ", FLabel.DefLabel, delegate(MutantDef d)
		{
			OnChangeDef(d);
		});
		view.Gap(2f);
		SZWidgets.NonDefSelectorSimple(view.GetRect(22f), w, rotStages, ref selectedRotStage, "RotStage: ", (RotStage r) => Enum.GetName(typeof(RotStage), r), delegate(RotStage r)
		{
			OnChangeRotStage(r);
		});
		view.Gap(2f);
		mHscroll = (int)view.CurY + 50;
	}

	private void OnChangeRotStage(RotStage r)
	{
		if (pawn.HasMutantTracker())
		{
			pawn.mutant.rotStage = r;
		}
		selectedRotStage = r;
	}

	private void OnChangeDef(MutantDef d)
	{
		if (pawn.HasMutantTracker() && pawn.mutant.HasTurned)
		{
			pawn.mutant.Revert();
		}
		selectedMutantDef = d;
		if (d != null)
		{
			pawn.mutant = new Pawn_MutantTracker(pawn, selectedMutantDef, RotStage.Fresh);
			pawn.mutant.Turn();
		}
		CEditor.API.UpdateGraphics();
	}

	private void ATurnHuman()
	{
		if (pawn.HasMutantTracker())
		{
			pawn.mutant.Revert();
		}
		CEditor.API.UpdateGraphics();
	}

	private void ATurnMutant()
	{
		OnChangeDef(selectedMutantDef);
	}

	private void DoAndClose()
	{
		Close();
	}

	public override void Close(bool doCloseSound = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SearchTool.Save(SearchTool.SIndex.ChangeMutant, ((Rect)(ref windowRect)).position);
		base.Close(doCloseSound);
	}

	internal string Mutantlabel()
	{
		return (pawn.mutant == null || pawn.mutant.Def == null) ? ("Mutations: " + Label.NONE) : ("Mutations: " + pawn.mutant.Def.LabelCap.RawText);
	}

	private void DrawEditLabel(float x, float y, float w, float h)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		GUI.color = Color.red;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, w, h);
		SZWidgets.Label(rect, Mutantlabel(), delegate
		{
			iChangeTick = 400;
		});
		GUI.color = Color.white;
	}
}
