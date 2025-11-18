using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogFullheal : Window
{
	private Dictionary<Hediff, bool> dicToRemove;

	private List<Hediff> lOfHediff;

	private Vector2 scrollPos;

	private bool isChecked;

	private bool doOnce;

	private bool bAll = false;

	private bool bToogle = false;

	public override Vector2 InitialSize => new Vector2(400f, 500f);

	internal DialogFullheal()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		scrollPos = default(Vector2);
		doOnce = true;
		SearchTool.Update(SearchTool.SIndex.FullHeal);
		lOfHediff = CEditor.API.Pawn.health.hediffSet.hediffs;
		dicToRemove = new Dictionary<Hediff, bool>();
		foreach (Hediff item in lOfHediff)
		{
			if (item.Part != null && item.def.hediffClass == typeof(Hediff_AddedPart))
			{
				dicToRemove.Add(item, value: false);
			}
			else
			{
				dicToRemove.Add(item, value: true);
			}
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		float num = InitialSize.x - 40f;
		float num2 = InitialSize.y - 115f;
		if (doOnce)
		{
			SearchTool.SetPosition(SearchTool.SIndex.FullHeal, ref windowRect, ref doOnce, 0);
		}
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(0f, 0f, 350f, 30f), Label.HEAL);
		Text.Font = GameFont.Small;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, 30f, num, num2);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 30f, num - 16f, (float)lOfHediff.Count * 29f - 5f);
		Widgets.BeginScrollView(outRect, ref scrollPos, val);
		Rect rect = val.ContractedBy(4f);
		((Rect)(ref rect)).height = (float)lOfHediff.Count * 29f;
		try
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.Begin(rect);
			using (List<Hediff>.Enumerator enumerator = lOfHediff.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					isChecked = dicToRemove[enumerator.Current];
					if (bToogle)
					{
						isChecked = !isChecked;
					}
					else if (bAll)
					{
						isChecked = true;
					}
					listing_Standard.CheckboxLabeled(enumerator.Current.GetFullLabel(), ref isChecked, enumerator.Current.TipStringExtra);
					dicToRemove[enumerator.Current] = isChecked;
					listing_Standard.Gap(5f);
				}
			}
			if (bToogle)
			{
				bToogle = false;
			}
			else if (bAll)
			{
				bAll = false;
			}
			listing_Standard.End();
		}
		catch
		{
		}
		Widgets.EndScrollView();
		WindowTool.SimpleCustomButton(this, 0, AToggle, Label.FLIP, "");
		WindowTool.SimpleCustomButton(this, 100, AAll, Label.ALL, "");
		WindowTool.SimpleAcceptButton(this, DoAndClose);
	}

	private void AAll()
	{
		bAll = true;
	}

	private void AToggle()
	{
		bToogle = true;
	}

	private void DoAndClose()
	{
		foreach (Hediff key in dicToRemove.Keys)
		{
			try
			{
				if (dicToRemove[key])
				{
					CEditor.API.Pawn.RemoveHediff(key);
				}
			}
			catch
			{
				if (CEditor.API.Pawn.Dead)
				{
					ResurrectionUtility.TryResurrect(CEditor.API.Pawn);
				}
			}
		}
		CEditor.API.UpdateGraphics();
		Close();
	}

	public override void Close(bool doCloseSound = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SearchTool.Save(SearchTool.SIndex.FullHeal, ((Rect)(ref windowRect)).position);
		base.Close(doCloseSound);
	}
}
