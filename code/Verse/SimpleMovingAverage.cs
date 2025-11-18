using System;

namespace Verse;

public class SimpleMovingAverage
{
	private readonly float[] values;

	private int lastValueIndex;

	public SimpleMovingAverage(int numValues)
	{
		values = new float[numValues];
	}

	public void AddValue(float value)
	{
		values[lastValueIndex] = value;
		lastValueIndex = (lastValueIndex + 1) % values.Length;
	}

	public float GetAverage()
	{
		float num = 0f;
		float[] array = values;
		int num2 = ((array[array.Length - 1] == 0f) ? lastValueIndex : values.Length);
		for (int i = 0; i < num2; i++)
		{
			num += values[i];
		}
		return num / (float)num2;
	}

	public void Reset()
	{
		lastValueIndex = 0;
		Array.Clear(values, 0, values.Length);
	}
}
