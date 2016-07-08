using UnityEngine;
using System.Collections;

public class FoodSpawner : MonoBehaviour {

	public GameObject FoodPrefab;
	private int counter = 0;
	private static int maxCounter = 60;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (counter++ % maxCounter == 0) {
			this.SpawnFood();
		}

	}

	public void SpawnFood() {
		Vector3 viewPortPoint = new Vector3 (Random.value, Random.value,0.0f);
		Vector3 positionToPlace = Camera.main.ViewportToWorldPoint (viewPortPoint);
		positionToPlace.z = 0;
		Instantiate (FoodPrefab, positionToPlace, Quaternion.identity);
	}
}
