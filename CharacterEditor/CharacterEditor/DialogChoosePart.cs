using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogChoosePart : Window
{
	private Vector2 scrollPos;

	private List<BodyPartRecord> lOfParts;

	private BodyPartRecord selectedPart;

	private IPartable callback;

	private bool addWholeBody;

	private int countParts;

	private HediffDef hediff;

	private bool doOnce;

	public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

	internal DialogChoosePart(IPartable _callback, HediffDef _hediff)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		hediff = _hediff;
		scrollPos = default(Vector2);
		selectedPart = null;
		addWholeBody = false;
		doOnce = true;
		SearchTool.Update(SearchTool.SIndex.ChoosePart);
		callback = _callback;
		lOfParts = CEditor.API.Pawn.GetListOfAllowedBodyPartRecords(hediff);
		countParts = lOfParts.CountAllowNull();
		if (countParts == 1)
		{
			selectedPart = lOfParts[0];
		}
		doCloseX = true;
		absorbInputAroundWindow = true;
		draggable = true;
		layer = CEditor.Layer;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		float frameW = InitialSize.x - 40f;
		float frameH = InitialSize.y - 115f;
		if (doOnce)
		{
			SearchTool.SetPosition(SearchTool.SIndex.ChoosePart, ref windowRect, ref doOnce, 0);
		}
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(0f, 0f, 300f, 30f), Label.SELECT_BODYPART);
		DrawPartList(frameW, frameH);
		WindowTool.SimpleAcceptButton(this, DoAndClose);
	}

	private void DrawPartList(float frameW, float frameH)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, 30f, frameW, frameH);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 30f, ((Rect)(ref outRect)).width - 16f, (float)countParts * 27f - 25f);
		Widgets.BeginScrollView(outRect, ref scrollPos, val);
		Rect rect = val.ContractedBy(4f);
		((Rect)(ref rect)).height = (float)countParts * 27f + (float)(addWholeBody ? 27 : 0);
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.Begin(rect);
		if (addWholeBody && listing_Standard.RadioButton("WholeBody".Translate(), selectedPart == null))
		{
			selectedPart = null;
		}
		listing_Standard.Gap(2f);
		foreach (BodyPartRecord lOfPart in lOfParts)
		{
			if (listing_Standard.RadioButton((lOfPart == null) ? "WholeBody".Translate().ToString() : lOfPart.Label.CapitalizeFirst(), selectedPart == lOfPart, 0f, lOfPart?.def?.description))
			{
				selectedPart = lOfPart;
			}
			listing_Standard.Gap(2f);
		}
		listing_Standard.End();
		Widgets.EndScrollView();
	}

	private void DoAndClose()
	{
		callback.SelectedPart = selectedPart;
		Close();
	}

	public override void Close(bool doCloseSound = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SearchTool.Save(SearchTool.SIndex.ChoosePart, ((Rect)(ref windowRect)).position);
		base.Close(doCloseSound);
	}
}
