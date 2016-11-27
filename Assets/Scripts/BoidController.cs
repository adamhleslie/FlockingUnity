﻿using UnityEngine;
using System.Collections.Generic;

public class BoidController : MonoBehaviour
{
	private Rigidbody body;

	[SerializeField]
	private float maxMagnitude;

	// Need to store local awareness of
	// 1. Boids
	private static Dictionary<int, Vector3> boidList;
	private int id;
	// 2. Obstacles
	// 3. Predators
	private Vector3 predLoc;

	// Initialization
	void Start ()
	{
		body = GetComponent<Rigidbody>();

		// Initialize the boidList if needed
		if (boidList == null)
			boidList = new Dictionary<int, Vector3>();

		// Generate new id for this boid
		id = (int) Random.Range(1.0f,10000.0f);
		// Add to boidList
		boidList.Add(id, body.position);
	}

	void FixedUpdate ()
	{
		// Step 1: Update spatial awareness
		boidList[id] = body.position;

		// Step 2: Generate a container of acceleration vectors
		List<Vector3> accelerations = new List<Vector3>();
		foreach (KeyValuePair<int, Vector3> entry in boidList)
		{
			if (entry.Key != id) 
			{
				float dist = Vector3.Distance(entry.Value, body.position);
				if (dist < 5) 
				{
					// If close, move away
					accelerations.Add((body.position - entry.Value) * 20);
				}
				else if (dist < 15) 
				{
					// If far, move closer
					accelerations.Add((entry.Value - body.position) * 2);
				}
			}
		}

		// Step 3: Accumulate acceleration vectors
		Vector3 output = AccumulateAccelerations(accelerations);

		// Step 4: Apply acceleration vectors to boid
		body.AddForce(output);
	}

	private Vector3 AccumulateAccelerations (ICollection<Vector3> accelerations)
	{
		float totalMagnitude = 0;
		Vector3 result = Vector3.zero;

		foreach (Vector3 acc in accelerations)
		{
			totalMagnitude += acc.magnitude;
			if (totalMagnitude >= maxMagnitude)
			{
				// Scale the acceleration down when its entire magnitude not used
				float scale = 1 - ((totalMagnitude - maxMagnitude) / acc.magnitude);
				result += scale * acc;
				break;
			}
			else
				result += acc;
		}

		return result;
	}
}