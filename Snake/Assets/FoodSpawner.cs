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
		Vector3 viewPortPoint = new Vector3 (Random.value, Random.value);
		Vector3 positionToPlace = Camera.main.ViewportToWorldPoint (viewPortPoint);
		Instantiate (FoodPrefab, positionToPlace, Quaternion.identity);
	}
}
