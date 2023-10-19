using UnityEngine;

public static class Calculator
{
	/// <summary>
	/// radius and velocity must be in the same measurement (cm, cm/s)!
	/// Calculated from: I=v*A (t�rfogat �raml�sa = �raml�si sebess�g * mer�leges keresztmetszet)
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