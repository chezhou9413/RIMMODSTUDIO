using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CharacterEditor;

internal class DialogXenoType : GeneCreationDialogBase
{
	private List<GeneDef> selectedGenes = new List<GeneDef>();

	private bool inheritable;

	private bool? selectedCollapsed = false;

	private List<GeneCategoryDef> matchingCategories = new List<GeneCategoryDef>();

	private Dictionary<GeneCategoryDef, bool> collapsedCategories = new Dictionary<GeneCategoryDef, bool>();

	private bool hoveredAnyGene;

	private GeneDef hoveredGene;

	private static bool ignoreRestrictionsConfirmationSent;

	private const int MaxCustomXenotypes = 200;

	private static readonly Color OutlineColorSelected = new Color(1f, 1f, 0.7f, 1f);

	private Pawn pawn;

	private XenotypeDef predefinedXenoDef;

	private bool doOnce;

	public override Vector2 InitialSize => new Vector2(1036f, (float)WindowTool.MaxH);

	protected override List<GeneDef> SelectedGenes => selectedGenes;

	protected override string Header => "CreateXenotype".Translate().CapitalizeFirst();

	protected override string AcceptButtonLabel => "SaveAndApply".Translate().CapitalizeFirst();

	internal DialogXenoType(Pawn _pawn)
	{
		predefinedXenoDef = null;
		pawn = _pawn;
		if (!pawn.genes.xenotypeName.NullOrEmpty())
		{
			xenotypeName = pawn.genes.xenotypeName;
		}
		else
		{
			xenotypeName = "";
		}
		doOnce = true;
		SearchTool.Update(SearchTool.SIndex.XenoType);
		doCloseX = true;
		absorbInputAroundWindow = true;
		closeOnAccept = false;
		closeOnCancel = true;
		closeOnClickedOutside = true;
		layer = CEditor.Layer;
		draggable = true;
		alwaysUseFullBiostatsTableHeight = true;
		searchWidgetOffsetX = (float)((double)GeneCreationDialogBase.ButSize.x * 2.0 + 4.0);
		foreach (GeneCategoryDef allDef in DefDatabase<GeneCategoryDef>.AllDefs)
		{
			collapsedCategories.Add(allDef, value: false);
		}
		OnGenesChanged();
	}

	public override void PostOpen()
	{
		if (doOnce)
		{
			SearchTool.SetPosition(SearchTool.SIndex.XenoType, ref windowRect, ref doOnce, 0);
		}
		if (!ModsConfig.BiotechActive)
		{
			Close(doCloseSound: false);
		}
		else
		{
			base.PostOpen();
		}
		if ((pawn.genes != null || pawn.genes.Xenotype.IsNullOrEmpty()) && DefDatabase<XenotypeDef>.AllDefs.Contains(pawn.genes.Xenotype) && pawn.genes.Xenotype != XenotypeDefOf.Baseliner)
		{
			ALoadXenotypeDef(pawn.genes.Xenotype);
			return;
		}
		List<GeneDef> list = new List<GeneDef>();
		foreach (Gene xenogene in pawn.genes.Xenogenes)
		{
			list.Add(xenogene.def);
		}
		CustomXenotype customXenotype = new CustomXenotype();
		customXenotype.name = pawn.genes.xenotypeName?.Trim();
		if (customXenotype.name.NullOrEmpty())
		{
			customXenotype.name = "";
		}
		customXenotype.genes.AddRange(list);
		customXenotype.inheritable = pawn.genes.Xenotype.inheritable;
		customXenotype.iconDef = pawn.genes.iconDef;
		if (customXenotype.name.NullOrEmpty())
		{
			ALoadCustomXenotype(customXenotype);
		}
		else
		{
			DoFileInteraction(customXenotype.name);
		}
	}

	public override void Close(bool doCloseSound = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SearchTool.Save(SearchTool.SIndex.XenoType, ((Rect)(ref windowRect)).position);
		base.Close(doCloseSound);
	}

	protected override void DrawGenes(Rect rect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Invalid comparison between Unknown and I4
		hoveredAnyGene = false;
		GUI.BeginGroup(rect);
		float curY = 0f;
		DrawSection(new Rect(0f, 0f, ((Rect)(ref rect)).width, selectedHeight), selectedGenes, "SelectedGenes".Translate(), ref curY, ref selectedHeight, adding: false, rect, ref selectedCollapsed);
		if (!selectedCollapsed.Value)
		{
			curY += 10f;
		}
		float num = curY;
		Widgets.Label(0f, ref curY, ((Rect)(ref rect)).width, "Genes".Translate().CapitalizeFirst());
		float curY2 = curY + 10f;
		float num2 = (float)((double)curY2 - (double)num - 4.0);
		if (Widgets.ButtonText(new Rect((float)((double)((Rect)(ref rect)).width - 150.0 - 16.0), num, 150f, num2), "CollapseAllCategories".Translate()))
		{
			SoundDefOf.TabClose.PlayOneShotOnCamera();
			foreach (GeneCategoryDef allDef in DefDatabase<GeneCategoryDef>.AllDefs)
			{
				collapsedCategories[allDef] = true;
			}
		}
		if (Widgets.ButtonText(new Rect((float)((double)((Rect)(ref rect)).width - 300.0 - 4.0 - 16.0), num, 150f, num2), "ExpandAllCategories".Translate()))
		{
			SoundDefOf.TabOpen.PlayOneShotOnCamera();
			foreach (GeneCategoryDef allDef2 in DefDatabase<GeneCategoryDef>.AllDefs)
			{
				collapsedCategories[allDef2] = false;
			}
		}
		float num3 = curY2;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY2, ((Rect)(ref rect)).width - 16f, scrollHeight);
		Widgets.BeginScrollView(new Rect(0f, curY2, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - curY2), ref scrollPosition, val);
		Rect containingRect = val;
		((Rect)(ref containingRect)).y = curY2 + scrollPosition.y;
		((Rect)(ref containingRect)).height = ((Rect)(ref rect)).height;
		bool? collapsed = null;
		DrawSection(rect, GeneUtility.GenesInOrder, null, ref curY2, ref unselectedHeight, adding: true, containingRect, ref collapsed);
		if ((int)Event.current.type == 8)
		{
			scrollHeight = curY2 - num3;
		}
		Widgets.EndScrollView();
		GUI.EndGroup();
		if (!hoveredAnyGene)
		{
			hoveredGene = null;
		}
	}

	private void DrawSection(Rect rect, List<GeneDef> genes, string label, ref float curY, ref float sectionHeight, bool adding, Rect containingRect, ref bool? collapsed)
	{
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Invalid comparison between Unknown and I4
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		float curX = 4f;
		if (!label.NullOrEmpty())
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, curY, ((Rect)(ref rect)).width, Text.LineHeight);
			((Rect)(ref val)).xMax = ((Rect)(ref val)).xMax - (adding ? 16f : (Text.CalcSize("ClickToAddOrRemove".Translate()).x + 4f));
			if (collapsed.HasValue)
			{
				Rect val2 = default(Rect);
				((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y + (float)(((double)((Rect)(ref val)).height - 18.0) / 2.0), 18f, 18f);
				GUI.DrawTexture(val2, (Texture)(object)(collapsed.Value ? TexButton.Reveal : TexButton.Collapse));
				if (Widgets.ButtonInvisible(val))
				{
					bool? flag = !collapsed;
					collapsed = flag;
					if (collapsed.Value)
					{
						SoundDefOf.TabClose.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.TabOpen.PlayOneShotOnCamera();
					}
				}
				if (Mouse.IsOver(val))
				{
					Widgets.DrawHighlight(val);
				}
				((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + ((Rect)(ref val2)).width;
			}
			Widgets.Label(val, label);
			if (!adding)
			{
				Text.Anchor = (TextAnchor)2;
				GUI.color = ColoredText.SubtleGrayColor;
				Widgets.Label(new Rect(((Rect)(ref val)).xMax - 18f, curY, ((Rect)(ref rect)).width - ((Rect)(ref val)).width, Text.LineHeight), "ClickToAddOrRemove".Translate());
				GUI.color = Color.white;
				Text.Anchor = (TextAnchor)0;
			}
			curY += Text.LineHeight + 3f;
		}
		if (collapsed == true)
		{
			if ((int)Event.current.type == 8)
			{
				sectionHeight = 0f;
			}
			return;
		}
		float num = curY;
		bool flag2 = false;
		float num2 = (float)(34.0 + (double)GeneCreationDialogBase.GeneSize.x + 8.0);
		float num3 = ((Rect)(ref rect)).width - 16f;
		float num4 = num2 + 4f;
		float num5 = (float)(((double)num3 - (double)num4 * (double)Mathf.Floor(num3 / num4)) / 2.0);
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(0f, curY, ((Rect)(ref rect)).width, sectionHeight);
		if (!adding)
		{
			Widgets.DrawRectFast(val3, Widgets.MenuSectionBGFillColor);
		}
		curY += 4f;
		if (!genes.Any())
		{
			Text.Anchor = (TextAnchor)4;
			GUI.color = ColoredText.SubtleGrayColor;
			Widgets.Label(val3, "(" + "NoneLower".Translate() + ")");
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
		}
		else
		{
			GeneCategoryDef geneCategoryDef = null;
			int num6 = 0;
			Rect val4 = default(Rect);
			Rect val5 = default(Rect);
			for (int i = 0; i < genes.Count; i++)
			{
				GeneDef geneDef = genes[i];
				if ((adding && quickSearchWidget.filter.Active && (!matchingGenes.Contains(geneDef) || selectedGenes.Contains(geneDef)) && !matchingCategories.Contains(geneDef.displayCategory)) || (!ignoreRestrictions && geneDef.biostatArc > 0))
				{
					continue;
				}
				bool flag3 = false;
				if ((double)curX + (double)num2 > (double)num3)
				{
					curX = 4f;
					curY += (float)((double)GeneCreationDialogBase.GeneSize.y + 8.0 + 4.0);
					flag3 = true;
				}
				bool flag4 = quickSearchWidget.filter.Active && (matchingGenes.Contains(geneDef) || matchingCategories.Contains(geneDef.displayCategory));
				bool flag5 = collapsedCategories[geneDef.displayCategory] && !flag4;
				if (adding && geneCategoryDef != geneDef.displayCategory)
				{
					if (!flag3 && flag2)
					{
						curX = 4f;
						curY += (float)((double)GeneCreationDialogBase.GeneSize.y + 8.0 + 4.0);
					}
					geneCategoryDef = geneDef.displayCategory;
					((Rect)(ref val4))._002Ector(curX, curY, ((Rect)(ref rect)).width - 8f, Text.LineHeight);
					if (!flag4)
					{
						((Rect)(ref val5))._002Ector(((Rect)(ref val4)).x, ((Rect)(ref val4)).y + (float)(((double)((Rect)(ref val4)).height - 18.0) / 2.0), 18f, 18f);
						GUI.DrawTexture(val5, (Texture)(object)(flag5 ? TexButton.Reveal : TexButton.Collapse));
						if (Widgets.ButtonInvisible(val4))
						{
							collapsedCategories[geneDef.displayCategory] = !collapsedCategories[geneDef.displayCategory];
							if (collapsedCategories[geneDef.displayCategory])
							{
								SoundDefOf.TabClose.PlayOneShotOnCamera();
							}
							else
							{
								SoundDefOf.TabOpen.PlayOneShotOnCamera();
							}
						}
						if (num6 % 2 == 1)
						{
							Widgets.DrawLightHighlight(val4);
						}
						if (Mouse.IsOver(val4))
						{
							Widgets.DrawHighlight(val4);
						}
						((Rect)(ref val4)).xMin = ((Rect)(ref val4)).xMin + ((Rect)(ref val5)).width;
					}
					Widgets.Label(val4, geneCategoryDef.LabelCap);
					curY += ((Rect)(ref val4)).height;
					if (!flag5)
					{
						GUI.color = Color.grey;
						Widgets.DrawLineHorizontal(curX, curY, ((Rect)(ref rect)).width - 8f);
						GUI.color = Color.white;
						curY += 10f;
					}
					num6++;
				}
				if (adding && flag5)
				{
					flag2 = false;
					if ((int)Event.current.type == 8)
					{
						sectionHeight = curY - num;
					}
					continue;
				}
				curX = Mathf.Max(curX, num5);
				flag2 = true;
				if (DrawGene(geneDef, !adding, ref curX, curY, num2, containingRect, flag4))
				{
					if (selectedGenes.Contains(geneDef))
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
						selectedGenes.Remove(geneDef);
					}
					else
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
						selectedGenes.Add(geneDef);
					}
					if (!xenotypeNameLocked)
					{
						xenotypeName = GeneUtility.GenerateXenotypeNameFromGenes(SelectedGenes);
					}
					OnGenesChanged();
					break;
				}
			}
		}
		if (!adding || flag2)
		{
			curY += GeneCreationDialogBase.GeneSize.y + 12f;
		}
		if ((int)Event.current.type == 8)
		{
			sectionHeight = curY - num;
		}
	}

	private bool DrawGene(GeneDef geneDef, bool selectedSection, ref float curX, float curY, float packWidth, Rect containingRect, bool isMatch)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
		if (!((Rect)(ref containingRect)).Overlaps(val))
		{
			curX = ((Rect)(ref val)).xMax + 4f;
			return false;
		}
		bool selected = !selectedSection && selectedGenes.Contains(geneDef);
		bool overridden = leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef));
		Widgets.DrawOptionBackground(val, selected);
		curX += 4f;
		GeneUIUtility.DrawBiostats(geneDef.biostatCpx, geneDef.biostatMet, geneDef.biostatArc, ref curX, curY, 4f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);
		if (isMatch)
		{
			Widgets.DrawStrongHighlight(val2.ExpandedBy(6f));
		}
		GeneUIUtility.DrawGeneDef(geneDef, val2, (!inheritable) ? GeneType.Xenogene : GeneType.Endogene, () => GeneTip(geneDef, selectedSection), doBackground: false, clickable: false, overridden);
		curX += GeneCreationDialogBase.GeneSize.x + 4f;
		if (Mouse.IsOver(val))
		{
			hoveredGene = geneDef;
			hoveredAnyGene = true;
		}
		else if (hoveredGene != null && geneDef.ConflictsWith(hoveredGene))
		{
			Widgets.DrawLightHighlight(val);
		}
		if (Widgets.ButtonInvisible(val))
		{
			result = true;
		}
		curX = Mathf.Max(curX, ((Rect)(ref val)).xMax + 4f);
		return result;
	}

	private string GeneTip(GeneDef geneDef, bool selectedSection)
	{
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		string text = null;
		if (selectedSection)
		{
			if (leftChosenGroups.Any((GeneLeftChosenGroup x) => x.leftChosen == geneDef))
			{
				text = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == geneDef));
			}
			else if (cachedOverriddenGenes.Contains(geneDef))
			{
				text = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef)));
			}
			else if (randomChosenGroups.ContainsKey(geneDef))
			{
				text = ("GeneWillBeRandomChosen".Translate() + ":\n" + randomChosenGroups[geneDef].Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
			}
		}
		if (selectedGenes.Contains(geneDef) && geneDef.prerequisite != null && !selectedGenes.Contains(geneDef.prerequisite))
		{
			if (!text.NullOrEmpty())
			{
				text += "\n\n";
			}
			text += ("MessageGeneMissingPrerequisite".Translate(geneDef.label).CapitalizeFirst() + ": " + geneDef.prerequisite.LabelCap).Colorize(ColorLibrary.RedReadable);
		}
		if (!text.NullOrEmpty())
		{
			text += "\n\n";
		}
		return text + (selectedGenes.Contains(geneDef) ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
		static string GroupInfo(GeneLeftChosenGroup group)
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			return (group == null) ? null : ("GeneLeftmostActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + group.overriddenGenes.Select((GeneDef x) => (x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
		}
	}

	protected override void PostXenotypeOnGUI(float curX, float curY)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		TaggedString taggedString = "GenesAreInheritable".Translate();
		TaggedString taggedString2 = "IgnoreRestrictions".Translate();
		float num = (float)((double)Mathf.Max(Text.CalcSize(taggedString).x, Text.CalcSize(taggedString2).x) + 4.0 + 24.0);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, num, Text.LineHeight);
		Widgets.CheckboxLabeled(rect, taggedString, ref inheritable);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, "GenesAreInheritableDesc".Translate());
		}
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y + Text.LineHeight;
		int num2 = (ignoreRestrictions ? 1 : 0);
		Widgets.CheckboxLabeled(rect, taggedString2, ref ignoreRestrictions);
		int num3 = (ignoreRestrictions ? 1 : 0);
		if (num2 != num3)
		{
			if (ignoreRestrictions)
			{
				if (!ignoreRestrictionsConfirmationSent)
				{
					ignoreRestrictionsConfirmationSent = true;
					WindowTool.Open(new Dialog_MessageBox("IgnoreRestrictionsConfirmation".Translate(), "Yes".Translate(), delegate
					{
					}, "No".Translate(), delegate
					{
						ignoreRestrictions = false;
					}));
				}
			}
			else
			{
				selectedGenes.RemoveAll((GeneDef x) => x.biostatArc > 0);
				OnGenesChanged();
			}
		}
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, "IgnoreRestrictionsDesc".Translate());
		}
		postXenotypeHeight += ((Rect)(ref rect)).yMax - curY;
	}

	protected override void OnGenesChanged()
	{
		selectedGenes.SortGeneDefs();
		base.OnGenesChanged();
		if (predefinedXenoDef == null)
		{
			return;
		}
		foreach (GeneDef allGene in predefinedXenoDef.AllGenes)
		{
			if (!selectedGenes.Contains(allGene))
			{
				MessageTool.DebugPrint("predefined unloaded");
				predefinedXenoDef = null;
				break;
			}
		}
		int num = selectedGenes.CountAllowNull();
		int num2 = predefinedXenoDef.AllGenes.CountAllowNull();
		if (num != num2)
		{
			MessageTool.DebugPrint("predefined unloaded");
			predefinedXenoDef = null;
		}
	}

	private void ALoadCustomXenotype(CustomXenotype xenotype)
	{
		MessageTool.DebugPrint("loading custom xenotype " + xenotype.name);
		predefinedXenoDef = null;
		xenotypeName = xenotype.name;
		xenotypeNameLocked = false;
		selectedGenes.Clear();
		selectedGenes.AddRange(xenotype.genes);
		inheritable = xenotype.inheritable;
		iconDef = xenotype.IconDef;
		OnGenesChanged();
		ignoreRestrictions = selectedGenes.Any((GeneDef x) => x.biostatArc > 0) || !WithinAcceptableBiostatLimits(showMessage: false);
	}

	private void ALoadXenotypeDef(XenotypeDef xenotype)
	{
		MessageTool.DebugPrint("loading xenotypeDef " + xenotype.label);
		predefinedXenoDef = xenotype;
		xenotypeName = xenotype.label;
		xenotypeNameLocked = false;
		selectedGenes.Clear();
		selectedGenes.AddRange(xenotype.genes);
		inheritable = xenotype.inheritable;
		iconDef = XenotypeIconDefOf.Basic;
		OnGenesChanged();
		ignoreRestrictions = selectedGenes.Any((GeneDef g) => g.biostatArc > 0) || !WithinAcceptableBiostatLimits(showMessage: false);
	}

	protected void DoFileInteraction(string fileName)
	{
		string filePath = GenFilePaths.AbsFilePathForXenotype(fileName);
		PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Xenotype, delegate
		{
			if (GameDataSaveLoader.TryLoadXenotype(filePath, out var xenotype))
			{
				ALoadCustomXenotype(xenotype);
			}
		});
	}

	protected override void DrawSearchRect(Rect rect)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		base.DrawSearchRect(rect);
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).xMax - GeneCreationDialogBase.ButSize.x, ((Rect)(ref rect)).y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadCustom".Translate()))
		{
			WindowTool.Open(new Dialog_XenotypeList_Load(delegate(CustomXenotype xenotype2)
			{
				ALoadCustomXenotype(xenotype2);
			}));
		}
		if (!Widgets.ButtonText(new Rect((float)((double)((Rect)(ref rect)).xMax - (double)GeneCreationDialogBase.ButSize.x * 2.0 - 4.0), ((Rect)(ref rect)).y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadPremade".Translate()))
		{
			return;
		}
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		foreach (XenotypeDef item in DefDatabase<XenotypeDef>.AllDefs.OrderBy((XenotypeDef c) => 0f - c.displayPriority))
		{
			XenotypeDef xenotype = item;
			list.Add(new FloatMenuOption(xenotype.LabelCap, delegate
			{
				ALoadXenotypeDef(xenotype);
			}, xenotype.Icon, XenotypeDef.IconColor, MenuOptionPriority.Default, delegate(Rect r)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				TooltipHandler.TipRegion(r, xenotype.descriptionShort ?? xenotype.description);
			}));
		}
		Find.WindowStack.Add(new FloatMenu(list));
	}

	protected override void DoBottomButtons(Rect rect)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		SZWidgets.ButtonText(new Rect(((Rect)(ref rect)).xMax - GeneCreationDialogBase.ButSize.x - 10f, ((Rect)(ref rect)).y, GeneCreationDialogBase.ButSize.x + 10f, GeneCreationDialogBase.ButSize.y), AcceptButtonLabel, delegate
		{
			ACheckSaveAnd(apply: true);
		});
		SZWidgets.ButtonText(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "Close".Translate(), delegate
		{
			Close();
		});
		SZWidgets.ButtonText(new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 270f, ((Rect)(ref rect)).y, 110f, 38f), Label.SAVE, delegate
		{
			ACheckSaveAnd(apply: false);
		});
		if (leftChosenGroups.Any())
		{
			int num = leftChosenGroups.Sum((GeneLeftChosenGroup geneLeftChosenGroup2) => geneLeftChosenGroup2.overriddenGenes.Count);
			GeneLeftChosenGroup geneLeftChosenGroup = leftChosenGroups[0];
			string text = "GenesConflict".Translate() + ": " + "GenesConflictDesc".Translate(geneLeftChosenGroup.leftChosen.Named("FIRST"), geneLeftChosenGroup.overriddenGenes[0].Named("SECOND")).CapitalizeFirst() + ((num > 1) ? (" +" + (num - 1)) : string.Empty);
			float x = Text.CalcSize(text).x;
			GUI.color = ColorLibrary.RedReadable;
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(new Rect((float)((double)((Rect)(ref rect)).xMax - (double)GeneCreationDialogBase.ButSize.x - (double)x - 4.0), ((Rect)(ref rect)).y, x, ((Rect)(ref rect)).height), text);
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
		}
	}

	protected override bool CanAccept()
	{
		if (!base.CanAccept())
		{
			return false;
		}
		if (!selectedGenes.Any())
		{
			return true;
		}
		for (int i = 0; i < selectedGenes.Count; i++)
		{
			if (selectedGenes[i].prerequisite != null && !selectedGenes.Contains(selectedGenes[i].prerequisite))
			{
				MessageTool.Show("MessageGeneMissingPrerequisite".Translate(selectedGenes[i].label).CapitalizeFirst() + ": " + selectedGenes[i].prerequisite.LabelCap, MessageTypeDefOf.RejectInput);
				return false;
			}
		}
		if (GenFilePaths.AllCustomXenotypeFiles.EnumerableCount() >= 200)
		{
			MessageTool.Show("MessageTooManyCustomXenotypes".Translate(), MessageTypeDefOf.RejectInput);
			return false;
		}
		if (ignoreRestrictions || !leftChosenGroups.Any())
		{
			return true;
		}
		MessageTool.Show("MessageConflictingGenesPresent".Translate(), MessageTypeDefOf.RejectInput);
		return false;
	}

	protected override void Accept()
	{
		ASaveAnd(use: true);
	}

	private void ACheckSaveAnd(bool apply)
	{
		if (CanAccept())
		{
			ASaveAnd(apply);
		}
	}

	private void ASaveAnd(bool use)
	{
		IEnumerable<string> warnings = GetWarnings();
		if (warnings.Any())
		{
			WindowTool.Open(Dialog_MessageBox.CreateConfirmation("XenotypeBreaksLimits".Translate() + ":\n" + warnings.ToLineList("  - ", capitalizeItems: true) + "\n\n" + "SaveAnyway".Translate(), delegate
			{
				AcceptInner(use);
			}));
		}
		else
		{
			AcceptInner(use);
		}
	}

	private void AcceptInner(bool saveAndUse)
	{
		if (xenotypeName.NullOrEmpty())
		{
			MessageTool.DebugPrint("please choose a xenotype name!");
			return;
		}
		CustomXenotype customXenotype = new CustomXenotype();
		customXenotype.name = xenotypeName?.Trim();
		customXenotype.genes.AddRange(selectedGenes);
		customXenotype.inheritable = inheritable;
		customXenotype.iconDef = iconDef;
		string absPath = GenFilePaths.AbsFilePathForXenotype(GenFile.SanitizedFileName(customXenotype.name));
		LongEventHandler.QueueLongEvent(delegate
		{
			GameDataSaveLoader.SaveXenotype(customXenotype, absPath);
		}, "SavingLongEvent", doAsynchronously: false, null);
		if (saveAndUse)
		{
			pawn.SetPawnXenotype(customXenotype, !inheritable);
		}
		Close();
	}

	private IEnumerable<string> GetWarnings()
	{
		DialogXenoType dialogCreateXenotype = this;
		if (dialogCreateXenotype.ignoreRestrictions)
		{
			if (dialogCreateXenotype.arc > 0 && dialogCreateXenotype.inheritable)
			{
				yield return "XenotypeBreaksLimits_Archites".Translate();
			}
			if (dialogCreateXenotype.met > GeneTuning.BiostatRange.TrueMax)
			{
				yield return "XenotypeBreaksLimits_Exceeds".Translate("Metabolism".Translate().ToLower().Named("STAT"), dialogCreateXenotype.met.Named("VALUE"), GeneTuning.BiostatRange.TrueMax.Named("MAX"));
			}
			else if (dialogCreateXenotype.met < GeneTuning.BiostatRange.TrueMin)
			{
				yield return "XenotypeBreaksLimits_Below".Translate("Metabolism".Translate().ToLower().Named("STAT"), dialogCreateXenotype.met.Named("VALUE"), GeneTuning.BiostatRange.TrueMin.Named("MIN"));
			}
		}
	}

	protected override void UpdateSearchResults()
	{
		quickSearchWidget.noResultsMatched = false;
		matchingGenes.Clear();
		matchingCategories.Clear();
		if (!quickSearchWidget.filter.Active)
		{
			return;
		}
		foreach (GeneDef item in GeneUtility.GenesInOrder)
		{
			if (!selectedGenes.Contains(item))
			{
				if (quickSearchWidget.filter.Matches(item.label))
				{
					matchingGenes.Add(item);
				}
				if (quickSearchWidget.filter.Matches(item.displayCategory.label) && !matchingCategories.Contains(item.displayCategory))
				{
					matchingCategories.Add(item.displayCategory);
				}
			}
		}
		quickSearchWidget.noResultsMatched = !matchingGenes.Any() && !matchingCategories.Any();
	}
}
