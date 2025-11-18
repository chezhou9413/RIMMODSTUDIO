using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogChangeHeadAddons : Window
{
	private List<int> lMax;

	private List<string> lPaths;

	private List<int> lOldAddonVariants;

	private List<int> lAddonVariants;

	private List<Graphic> lAddonGraphics;

	private int[] aOldAddonVariants;

	private bool doOnce;

	private Pawn tempPawn;

	private object[] aAddons;

	private Vector2 scrollPosParam;

	private string paramName;

	private Listing_X view;

	public override Vector2 InitialSize => new Vector2(400f, (float)WindowTool.MaxH);

	internal DialogChangeHeadAddons()
	{
		doCloseX = true;
		absorbInputAroundWindow = false;
		draggable = true;
		layer = CEditor.Layer;
		doOnce = true;
		SearchTool.Update(SearchTool.SIndex.ChangeHeadAddons);
		Init();
	}

	internal void Init()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		scrollPosParam = default(Vector2);
		paramName = null;
		view = new Listing_X();
		tempPawn = CEditor.API.Pawn;
		lMax = new List<int>();
		lPaths = new List<string>();
		lAddonVariants = tempPawn.AlienRaceComp_GetAddonVariants();
		lAddonGraphics = tempPawn.AlienRaceComp_GetAddonGraphics();
		if (!lAddonGraphics.NullOrEmpty())
		{
			for (int i = 0; i < lAddonGraphics.Count; i++)
			{
				string text = lAddonGraphics[i].path;
				string s = text.Substring(text.Length - 1);
				int result = 0;
				if (int.TryParse(s, out result))
				{
					text = text.Substring(0, text.Length - 1);
				}
				lPaths.Add(text);
				string text2 = "";
				do
				{
					result++;
					text2 = text + result;
				}
				while (TextureTool.TestTexturePath(text2 + "_south", showError: false));
				result--;
				lMax.Add(result);
			}
		}
		aOldAddonVariants = new int[lAddonVariants.Count];
		lOldAddonVariants = new List<int>();
		aAddons = tempPawn.AlienPartGenerator_GetBodyAddonsAsArray();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (doOnce)
		{
			SearchTool.SetPosition(SearchTool.SIndex.ChangeHeadAddons, ref windowRect, ref doOnce, 370);
		}
		if (tempPawn.ThingID != CEditor.API.Pawn.ThingID)
		{
			Init();
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = InitialSize.x - 16f;
		float num4 = InitialSize.y - 16f;
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(num, num2, num3, 30f), Label.HEAD_ADDONS);
		num2 += 30f;
		Text.Font = GameFont.Small;
		Widgets.Label(new Rect(num, num2, num3 - 20f, 70f), Label.ADDONS_INFO + Label.ATTENTION.Colorize(ColorTool.colRed) + Label.ADDONS_INFO2);
		num2 += 70f;
		DrawAddons(num, num2, num3 - 20f, num4 - 180f);
		WindowTool.SimpleCloseButton(this);
	}

	private void DrawAddons(float x, float y, float w, float h)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (!lAddonVariants.NullOrEmpty())
		{
			int num = lAddonVariants.Count * 155;
			Rect outRect = default(Rect);
			((Rect)(ref outRect))._002Ector(x, y, w, h);
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, (float)num);
			Widgets.BeginScrollView(outRect, ref scrollPosParam, val);
			Rect rect = val.ContractedBy(4f);
			((Rect)(ref rect)).y = ((Rect)(ref rect)).y - 4f;
			((Rect)(ref rect)).height = num;
			view.Begin(rect);
			view.verticalSpacing = 30f;
			Text.Font = GameFont.Small;
			lAddonVariants.CopyTo(aOldAddonVariants);
			lOldAddonVariants = aOldAddonVariants.ToList();
			for (int i = 0; i < lAddonVariants.Count; i++)
			{
				DrawAddon(i, w);
			}
			view.End();
			Widgets.EndScrollView();
			if (tempPawn.ThingID == CEditor.API.Pawn.ThingID && !lOldAddonVariants.SequenceEqual(lAddonVariants))
			{
				tempPawn.AlienRaceComp_SetAddonVariants(lAddonVariants);
				CEditor.API.UpdateGraphics();
			}
		}
	}

	private void DrawCustom(float w)
	{
		if (!(tempPawn.ThingID != CEditor.API.Pawn.ThingID))
		{
		}
	}

	private void DrawAddon(int i, float w)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		if (tempPawn.ThingID != CEditor.API.Pawn.ThingID)
		{
			return;
		}
		try
		{
			string text = AlienRaceTool.BodyAddon_GetPath(aAddons[i]);
			if (text.NullOrEmpty())
			{
				text = lAddonGraphics[i].path;
			}
			text = text.Colorize(ColorTool.colBeige);
			int value = lAddonVariants[i];
			int num = AlienRaceTool.BodyAddon_GetVariantCountMax(aAddons[i]);
			if (num == 0)
			{
				num = lMax[i];
			}
			view.AddIntSection(text, "", ref paramName, ref value, 0, num, small: true);
			lAddonVariants[i] = value;
			bool flag = AlienRaceTool.BodyAddon_GetDrawForFemale(aAddons[i]);
			view.ButtonImage(0f, 0f, 20f, 20f, "bfemale", flag ? Color.white : Color.gray, ARemoveAddonFemale, i);
			bool flag2 = AlienRaceTool.BodyAddon_GetDrawForMale(aAddons[i]);
			view.ButtonImage(22f, 0f, 20f, 20f, "bmale", flag2 ? Color.white : Color.gray, ARemoveAddonMale, i);
			Vector2 val = AlienRaceTool.BodyAddon_GetDrawSize(aAddons[i]);
			float value2 = val.x;
			view.AddSection("            " + Label.DRAWSIZE, "", ref paramName, ref value2, 0f, 2f, small: true);
			if (value2 != val.x)
			{
				AOnDrawSizeChanged(i, value2);
			}
			float num2 = AlienRaceTool.BodyAddon_GetRotation(aAddons[i]);
			int value3 = (int)num2;
			view.AddIntSection(Label.ROTATION, "", ref paramName, ref value3, -360, 360, small: true);
			if (value3 != (int)num2)
			{
				AOnRotationChanged(i, value3);
			}
			view.Gap(5f);
		}
		catch (Exception ex)
		{
			Log.Error(ex.Message + "\n" + ex.StackTrace);
		}
	}

	private void AOnDrawSizeChanged(int i, float val)
	{
		AlienRaceTool.BodyAddon_SetDrawSize(aAddons[i], val);
		CEditor.API.UpdateGraphics();
	}

	private void AOnRotationChanged(int i, float val)
	{
		AlienRaceTool.BodyAddon_SetRotation(aAddons[i], val);
		CEditor.API.UpdateGraphics();
	}

	private void ARemoveAddonMale(int i)
	{
		tempPawn.AlienPartGenerator_BodyAddon_Toggle_DrawFor(i, female: false);
		CEditor.API.UpdateGraphics();
	}

	private void ARemoveAddonFemale(int i)
	{
		tempPawn.AlienPartGenerator_BodyAddon_Toggle_DrawFor(i, female: true);
		CEditor.API.UpdateGraphics();
	}

	public override void Close(bool doCloseSound = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SearchTool.Save(SearchTool.SIndex.ChangeHeadAddons, ((Rect)(ref windowRect)).position);
		base.Close(doCloseSound);
	}

	public override void OnAcceptKeyPressed()
	{
		base.OnAcceptKeyPressed();
		Close();
	}
}
