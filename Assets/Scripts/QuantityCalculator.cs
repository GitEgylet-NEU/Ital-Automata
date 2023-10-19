using UnityEngine;

public static class QuantityCalculator
{
	/// <summary>
	/// radius and velocity must be in the same measurement! (cm, cm/s)
	/// I=v*A (térfogat áramlása = áramlási sebesség * merőleges keresztmetszet)
	/// </summary>
	public static float CalculateFlow(float radius, float velocity)
	{
		float area = Mathf.PI * radius * radius; //cm^2
		return area * velocity; //(cm^3)/s
	}
	public static float CalculateQuantity(float radius, float velocity, float time)
	{
		float flow = CalculateFlow(radius, velocity);
		return flow * time; //ml=cm^3
	}
}