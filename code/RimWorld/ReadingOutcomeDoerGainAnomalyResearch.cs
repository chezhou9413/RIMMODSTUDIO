using System.Collections.Generic;
using Verse;

namespace RimWorld;

public class ReadingOutcomeDoerGainAnomalyResearch : ReadingOutcomeDoerGainResearch
{
	public new BookOutcomeProperties_GainAnomalyResearch Props => (BookOutcomeProperties_GainAnomalyResearch)props;

	public override int RoundTo { get; }

	protected override float GetBaseValue()
	{
		return BookUtility.GetAnomalyExpForQuality(base.Quality);
	}

	public override bool DoesProvidesOutcome(Pawn reader)
	{
		ResearchProjectDef researchProjectDef = default(ResearchProjectDef);
		float num = default(float);
		foreach (KeyValuePair<ResearchProjectDef, float> value in values)
		{
			value.Deconstruct(ref researchProjectDef, ref num);
			if (researchProjectDef.CanStartNow)
			{
				return true;
			}
		}
		return false;
	}

	public override void OnReadingTick(Pawn reader, float factor)
	{
		ResearchProjectDef researchProjectDef = default(ResearchProjectDef);
		float remainder = default(float);
		foreach (KeyValuePair<ResearchProjectDef, float> value in values)
		{
			value.Deconstruct(ref researchProjectDef, ref remainder);
			ResearchProjectDef researchProjectDef2 = researchProjectDef;
			float num = remainder;
			if (researchProjectDef2.CanStartNow)
			{
				Find.ResearchManager.ApplyKnowledge(researchProjectDef2, num * factor, out remainder);
			}
		}
	}
}
