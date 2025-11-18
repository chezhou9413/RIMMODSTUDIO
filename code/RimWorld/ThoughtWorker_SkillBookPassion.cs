using System.Collections.Generic;
using Verse;

namespace RimWorld;

public class ThoughtWorker_SkillBookPassion : ThoughtWorker
{
	protected override ThoughtState CurrentStateInternal(Pawn p)
	{
		if (p.CurJobDef != JobDefOf.Reading)
		{
			return ThoughtState.Inactive;
		}
		if (!(p.jobs.curDriver is JobDriver_Reading { IsReading: not false }))
		{
			return ThoughtState.Inactive;
		}
		BookOutcomeDoerGainSkillExp doer = ((Book)p.CurJob.targetA.Thing).BookComp.GetDoer<BookOutcomeDoerGainSkillExp>();
		bool flag = false;
		if (doer != null)
		{
			SkillDef skillDef = default(SkillDef);
			float num = default(float);
			foreach (KeyValuePair<SkillDef, float> value in doer.Values)
			{
				value.Deconstruct(ref skillDef, ref num);
				SkillDef skillDef2 = skillDef;
				SkillRecord skill = p.skills.GetSkill(skillDef2);
				if (skill.passion == Passion.Major)
				{
					return ThoughtState.ActiveAtStage(1);
				}
				if (skill.passion == Passion.Minor)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			return ThoughtState.ActiveAtStage(0);
		}
		return ThoughtState.Inactive;
	}
}
