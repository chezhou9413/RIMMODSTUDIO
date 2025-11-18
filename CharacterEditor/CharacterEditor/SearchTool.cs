using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class SearchTool
{
	internal enum SIndex
	{
		AbilityDef = 1,
		Animal,
		OtherPawn,
		GeneDef,
		HediffDef,
		ThoughtDef,
		TraitDef,
		Weapon,
		BackstoryDefChild,
		BackstroryDefAdult,
		Race,
		FindPawn,
		Editor,
		Capsule,
		ChangeFaction,
		ChangeHeadAddons,
		ChoosePart,
		AddStat,
		ColorPicker,
		FullHeal,
		Psychology,
		ViewXenoType,
		XenoType,
		ChangeBirthday,
		ChangeMutant
	}

	internal string find;

	internal string findOld;

	internal string modName;

	internal object ofilter1;

	internal object ofilter2;

	internal Vector2 scrollPos;

	internal Vector2 onScreenPos;

	internal string filter1;

	internal string filter2;

	internal WeaponType weaponType;

	internal ThingCategoryDef thingCategoryDef;

	internal ThingCategory thingCategory;

	internal ApparelLayerDef apparelLayerDef;

	internal BodyPartGroupDef bodyPartGroupDef;

	internal string SelectedModName
	{
		get
		{
			return modName.NullOrEmpty() ? Label.ALL : modName;
		}
		set
		{
			modName = value;
		}
	}

	internal string SelectedFilter1
	{
		get
		{
			return filter1.NullOrEmpty() ? Label.ALL : filter1;
		}
		set
		{
			filter1 = value;
		}
	}

	internal string SelectedFilter2
	{
		get
		{
			return filter2.NullOrEmpty() ? Label.ALL : filter2;
		}
		set
		{
			filter2 = value;
		}
	}

	internal static SearchTool GetInstance(SIndex uniqueIdx)
	{
		return CEditor.API.Get<Dictionary<SIndex, SearchTool>>(EType.Search)[uniqueIdx];
	}

	internal SearchTool()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		find = "";
		findOld = "";
		modName = null;
		filter1 = null;
		filter2 = null;
		ofilter1 = null;
		ofilter2 = null;
		scrollPos = default(Vector2);
		onScreenPos = default(Vector2);
		weaponType = WeaponType.Ranged;
		thingCategoryDef = null;
		thingCategory = ThingCategory.None;
		apparelLayerDef = null;
		bodyPartGroupDef = null;
	}

	internal static void ClearSearch(SIndex uniqueIdx)
	{
		Dictionary<SIndex, SearchTool> dictionary = CEditor.API.Get<Dictionary<SIndex, SearchTool>>(EType.Search);
		if (!dictionary.ContainsKey(uniqueIdx))
		{
			dictionary.Add(uniqueIdx, new SearchTool());
		}
		SZWidgets.sFind = "";
		SZWidgets.sFindOld = "";
		dictionary[uniqueIdx].find = "";
		dictionary[uniqueIdx].findOld = "";
		dictionary[uniqueIdx].modName = "";
	}

	internal static SearchTool Update(SIndex uniqueIdx)
	{
		SZWidgets.bFocusOnce = true;
		Dictionary<SIndex, SearchTool> dictionary = CEditor.API.Get<Dictionary<SIndex, SearchTool>>(EType.Search);
		if (!dictionary.ContainsKey(uniqueIdx))
		{
			dictionary.Add(uniqueIdx, new SearchTool());
		}
		SZWidgets.sFind = dictionary[uniqueIdx].find;
		SZWidgets.sFindOld = "";
		SZWidgets.lSimilar = new List<string>();
		return dictionary[uniqueIdx];
	}

	internal static void SetPosition(SIndex uniqueIdx, ref Rect r, ref bool doOnce, int offset)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<SIndex, SearchTool> dictionary = CEditor.API.Get<Dictionary<SIndex, SearchTool>>(EType.Search);
		doOnce = false;
		Vector2 val = dictionary[uniqueIdx].onScreenPos;
		float y = dictionary[SIndex.Editor].onScreenPos.y;
		if (val != default(Vector2))
		{
			((Rect)(ref r)).position = val;
		}
		else
		{
			((Rect)(ref r)).position = new Vector2((float)(CEditor.API.EditorPosX + offset), (float)CEditor.API.EditorPosY);
		}
	}

	internal static void Save(SIndex uniqueIdx, Vector2 loc)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<SIndex, SearchTool> dictionary = CEditor.API.Get<Dictionary<SIndex, SearchTool>>(EType.Search);
		dictionary[uniqueIdx].onScreenPos = loc;
		dictionary[uniqueIdx].find = SZWidgets.sFind;
		dictionary[uniqueIdx].findOld = SZWidgets.sFindOld;
		SZWidgets.lSimilar.Clear();
		SZWidgets.bFocusOnce = true;
	}
}
