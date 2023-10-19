public static class UnitConverter
{
	public const float lengthScale = .1f; // how many meters are in a Unity unit?

	/// <returns>input value in meters</returns>
	public static float UnityToMetric(float a)
	{
		return a * lengthScale;
	}
	/// <param name="a">value in meters</param>
	/// <returns>input value in Unity unit</returns>
	public static float MetricToUnity(float a)
	{
		return a / lengthScale;
	}

	/// <param name="a">area in square meters</param>
	public static float MetricToLitre(float a)
	{
		return a * 1000f; // 1 m^3 == 1000l
	}
	/// <returns>area in square meters</returns>
	public static float LitreToMetric(float a)
	{
		return a / 1000f;
	}

	public static string FormatVolume(float litres)
	{
		float millilitres = litres * 1000;
		if (millilitres - 1000f >= 0f)
		{
			return (millilitres / 1000f).ToString("0.##") + " l";
		}
		else if (millilitres - 100f >= 0f)
		{
			return (millilitres / 100f).ToString("0.##") + " dl";
		}
		return millilitres.ToString("0.##") + " ml";
	}
}