using System.Collections.Generic;
using System.Text;
using Verse;

namespace RimWorld;

public class RitualOutcomeComp_GravshipFacilities : RitualOutcomeComp_QualitySingleOffset
{
	public Dictionary<ThingDef, float> facilityQualityOffsets = new Dictionary<ThingDef, float>();

	private Dictionary<ThingDef, int> tmpFacilityCount = new Dictionary<ThingDef, int>();

	protected override string LabelForDesc => label;

	public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
	{
		return GetQualityFactor(ritual.Ritual, ritual.selectedTarget, ritual.obligation, ritual.assignments, data)?.quality ?? 0f;
	}

	public override QualityFactor GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
	{
		Building_GravEngine building_GravEngine = ritualTarget.Thing?.TryGetComp<CompPilotConsole>()?.engine;
		if (building_GravEngine == null)
		{
			return null;
		}
		float num = 0f;
		int num2 = 0;
		ThingDef thingDef = default(ThingDef);
		int num3 = default(int);
		foreach (CompGravshipFacility gravshipComponent in building_GravEngine.GravshipComponents)
		{
			CompPowerTrader compPowerTrader = gravshipComponent.parent.TryGetComp<CompPowerTrader>();
			if (compPowerTrader != null && !compPowerTrader.PowerOn)
			{
				continue;
			}
			CompGravshipThruster compGravshipThruster = gravshipComponent.parent.TryGetComp<CompGravshipThruster>();
			if (compGravshipThruster == null || compGravshipThruster.CanBeActive)
			{
				ThingDef def = gravshipComponent.parent.def;
				if (facilityQualityOffsets.TryGetValue(def, out var value))
				{
					num += value;
					num2++;
					tmpFacilityCount.TryAdd(def, 0);
					Dictionary<ThingDef, int> dictionary = tmpFacilityCount;
					thingDef = def;
					num3 = dictionary[thingDef]++;
				}
			}
		}
		if (num2 == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<ThingDef, int> item in tmpFacilityCount)
		{
			item.Deconstruct(ref thingDef, ref num3);
			ThingDef thingDef2 = thingDef;
			int num4 = num3;
			if (facilityQualityOffsets.TryGetValue(thingDef2, out var value2))
			{
				stringBuilder.AppendLine($" - {thingDef2.LabelCap} x{num4}: +{(value2 * (float)num4).ToStringPercent()}");
			}
		}
		tmpFacilityCount.Clear();
		int num5 = 0;
		float num6 = default(float);
		foreach (KeyValuePair<ThingDef, float> facilityQualityOffset in facilityQualityOffsets)
		{
			facilityQualityOffset.Deconstruct(ref thingDef, ref num6);
			ThingDef thingDef3 = thingDef;
			num5 += thingDef3.GetCompProperties<CompProperties_GravshipFacility>().maxSimultaneous;
		}
		string text = stringBuilder.ToString();
		return new QualityFactor
		{
			label = LabelForDesc.CapitalizeFirst(),
			qualityChange = "OutcomeBonusDesc_QualitySingleOffset".Translate(num.ToStringWithSign("0.#%")).Resolve(),
			count = num2 + " / " + num5,
			quality = num,
			positive = true,
			priority = 4f,
			toolTip = (text.NullOrEmpty() ? null : text)
		};
	}
}
