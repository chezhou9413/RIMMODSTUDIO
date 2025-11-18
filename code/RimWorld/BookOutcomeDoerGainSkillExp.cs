using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace RimWorld;

public class BookOutcomeDoerGainSkillExp : BookOutcomeDoer
{
	private Dictionary<SkillDef, float> values = new Dictionary<SkillDef, float>();

	private const float MultipleSkillFactor = 1.25f;

	private static readonly SimpleCurve QualityMaxLevel = new SimpleCurve
	{
		new CurvePoint(0f, 2f),
		new CurvePoint(1f, 5f),
		new CurvePoint(2f, 8f),
		new CurvePoint(3f, 10f),
		new CurvePoint(4f, 12f),
		new CurvePoint(5f, 14f),
		new CurvePoint(6f, 16f)
	};

	public new BookOutcomeProperties_GainSkillExp Props => (BookOutcomeProperties_GainSkillExp)props;

	public IReadOnlyDictionary<SkillDef, float> Values => values;

	public override bool DoesProvidesOutcome(Pawn reader)
	{
		SkillDef skillDef = default(SkillDef);
		float num = default(float);
		foreach (KeyValuePair<SkillDef, float> value in values)
		{
			value.Deconstruct(ref skillDef, ref num);
			SkillDef skill = skillDef;
			if (CanProgressSkill(reader, skill, base.Quality))
			{
				return true;
			}
		}
		return false;
	}

	public override void OnBookGenerated(Pawn author = null)
	{
		List<SkillDef> availableSkills = GetAvailableSkills();
		int count = ((!Rand.Chance(0.25f)) ? 1 : 2);
		List<SkillDef> list = availableSkills.TakeRandomDistinct(count);
		float num = BookUtility.GetSkillExpForQuality(base.Quality);
		if (list.Count > 1)
		{
			num *= 1.25f;
		}
		float value = num / (float)list.Count;
		for (int i = 0; i < list.Count; i++)
		{
			values[list[i]] = value;
		}
	}

	protected virtual List<SkillDef> GetAvailableSkills(Pawn author = null)
	{
		List<SkillDef> list = new List<SkillDef>();
		if (Props.skills.Count > 0)
		{
			foreach (BookOutcomeProperties_GainSkillExp.BookStatReward skill in Props.skills)
			{
				list.Add(skill.skill);
			}
		}
		else
		{
			list = DefDatabase<SkillDef>.AllDefsListForReading;
		}
		return list;
	}

	public override void Reset()
	{
		values.Clear();
	}

	public override void OnReadingTick(Pawn reader, float factor)
	{
		SkillDef skillDef = default(SkillDef);
		float num = default(float);
		foreach (KeyValuePair<SkillDef, float> value in values)
		{
			value.Deconstruct(ref skillDef, ref num);
			SkillDef skillDef2 = skillDef;
			float num2 = num;
			if (CanProgressSkill(reader, skillDef2, base.Quality))
			{
				reader.skills.GetSkill(skillDef2).Learn(num2 * factor);
			}
		}
	}

	public override string GetBenefitsString(Pawn reader = null)
	{
		if (values.Count == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		SkillDef skillDef = default(SkillDef);
		float num = default(float);
		foreach (KeyValuePair<SkillDef, float> value in values)
		{
			value.Deconstruct(ref skillDef, ref num);
			SkillDef skillDef2 = skillDef;
			float num2 = num;
			float num3 = 1f;
			if (reader != null)
			{
				num3 = reader.skills.GetSkill(skillDef2).LearnRateFactor();
				num2 *= num3;
			}
			float f = num2 * 2500f;
			string text = string.Format("{0}: {1}", skillDef2.LabelCap, "XpPerHour".Translate(f.ToStringDecimalIfSmall()));
			int maxSkillLevel = GetMaxSkillLevel(base.Quality);
			text += string.Format(" ({0})", "BookMaxLevel".Translate(maxSkillLevel));
			if (!Mathf.Approximately(num3, 1f))
			{
				text += string.Format(" (x{0} {1})", num3.ToStringPercent("0"), "BookLearningModifier".Translate());
			}
			stringBuilder.AppendLine(" - " + text);
		}
		return stringBuilder.ToString();
	}

	public override IEnumerable<RulePack> GetTopicRulePacks()
	{
		return values.Keys.Select((SkillDef x) => x.generalRules);
	}

	public override void PostExposeData()
	{
		Scribe_Collections.Look(ref values, "values");
	}

	private static bool CanProgressSkill(Pawn pawn, SkillDef skill, QualityCategory quality)
	{
		if (pawn.skills.GetSkill(skill).TotallyDisabled)
		{
			return false;
		}
		return pawn.skills.GetSkill(skill).GetLevel(includeAptitudes: false) < GetMaxSkillLevel(quality);
	}

	private static int GetMaxSkillLevel(QualityCategory quality)
	{
		return Mathf.RoundToInt(QualityMaxLevel.Evaluate((int)quality));
	}
}
