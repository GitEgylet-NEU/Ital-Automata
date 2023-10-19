using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidSpawner : MonoBehaviour
{
	public GameObject fluidParticlePrefab;
	public float particlesPerSecond = 10f;
	public float maxParticles = 0f;
	public float timer = 5f;
	public float spawnRadius = .25f;

	float time;
	float countdown;
	float particles;

	// Start is called before the first frame update
	void Start()
	{
		time = 0f;
		particles = 0f;
		countdown = 0f;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (timer > 0f)
		{
			time += Time.fixedDeltaTime;
			if (time >= timer)
			{
				enabled = false;
			}
		}
		
		if (maxParticles > 0f && particles >= maxParticles)
		{
			enabled = false;
		}

		countdown += Time.fixedDeltaTime;
		if (countdown >= 1 / particlesPerSecond)
		{
			countdown = 0f;
			Spawn();
		}
	}

	void Spawn()
	{
		GameObject fluid = Instantiate(fluidParticlePrefab, transform);
		fluid.transform.localPosition = new Vector2(Random.Range(0, spawnRadius), Random.Range(0, spawnRadius));

		particles++;
	}
}