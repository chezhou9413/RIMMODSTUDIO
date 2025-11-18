using System;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class CustomDialog : Window
{
	internal string text;

	internal string title;

	internal Action buttonAbortAction;

	internal Action buttonAcceptAction;

	internal Action buttonNextAction;

	private Vector2 scrollPosition = Vector2.zero;

	private float creationRealTime = -1f;

	private const float TitleHeight = 42f;

	protected const float ButtonHeight = 35f;

	public override Vector2 InitialSize => new Vector2(640f, 460f);

	internal CustomDialog(string text, string title, Action onAbort, Action onConfirm, Action onNext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		this.title = title;
		this.text = text;
		buttonAbortAction = onAbort;
		buttonAcceptAction = onConfirm;
		buttonNextAction = onNext;
		layer = CEditor.Layer;
		forcePause = true;
		absorbInputAroundWindow = true;
		creationRealTime = RealTime.LastRealTime;
		onlyOneOfTypeAllowed = false;
		closeOnAccept = true;
		closeOnCancel = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Rect)(ref inRect)).y;
		if (!title.NullOrEmpty())
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0f, num, ((Rect)(ref inRect)).width, 42f), title);
			num += 42f;
		}
		Text.Font = GameFont.Small;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(((Rect)(ref inRect)).x, num, ((Rect)(ref inRect)).width, (float)((double)((Rect)(ref inRect)).height - 35.0 - 5.0) - num);
		float num2 = ((Rect)(ref outRect)).width - 16f;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, num2, Text.CalcHeight(text, num2));
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref viewRect)).width, ((Rect)(ref viewRect)).height), text);
		Widgets.EndScrollView();
		GUI.color = Color.white;
		float num3 = InitialSize.y - 70f;
		SZWidgets.CheckBoxOnChange(new Rect(((Rect)(ref inRect)).x + 420f, num3 - 30f, 180f, 30f), Label.ALWAYS_SKIP, CEditor.DontAsk, ASetAlwaysSkip);
		SZWidgets.ButtonText(new Rect(((Rect)(ref inRect)).x, num3, 180f, 30f), "Cancel".Translate(), AOnAbort);
		SZWidgets.ButtonText(new Rect(((Rect)(ref inRect)).x + (float)((buttonNextAction == null) ? 420 : 210), num3, 180f, 30f), "Confirm".Translate(), AOnAccept);
		if (buttonNextAction != null)
		{
			SZWidgets.ButtonText(new Rect(((Rect)(ref inRect)).x + 420f, num3, 180f, 30f), Label.SKIP, AOnNext);
		}
	}

	private void ASetAlwaysSkip(bool val)
	{
		CEditor.DontAsk = val;
	}

	private void AOnAbort()
	{
		if (buttonAbortAction != null)
		{
			buttonAbortAction();
		}
		Close();
	}

	private void AOnAccept()
	{
		if (buttonAcceptAction != null)
		{
			buttonAcceptAction();
		}
		Close();
	}

	private void AOnNext()
	{
		if (buttonNextAction != null)
		{
			buttonNextAction();
		}
		Close();
	}

	public override void OnCancelKeyPressed()
	{
		AOnAbort();
	}

	public override void OnAcceptKeyPressed()
	{
		AOnAccept();
	}
}
