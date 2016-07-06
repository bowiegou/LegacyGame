using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public Score GameScore;
	public FoodSpawner FoodSpawner;
	public int ScoreMutiplier = 10;

	void start() {
		this.GameScore = new Score (ScoreMutiplier);
	}



	public void OnFoodAte() {
		GameScore.UpdateScore ();
		FoodSpawner.SpawnFood ();
	}

}

public struct Score {
	public int FoodAte;
	public int TotalScore;
    private readonly int _scoreMutiplier;


	public Score (int scoreMutiplier ,int foodAte = 0, int totalScore = 0)
	{
		this.FoodAte = foodAte;
		this.TotalScore = totalScore;
		this._scoreMutiplier = scoreMutiplier;
	}

	public int GetTotalScore() {
		return this.TotalScore;
	}

	public int UpdateScore() {
		return UpdateScore (++this.FoodAte);
	}

	public int UpdateScore(int foodAte) {
		this.FoodAte = foodAte;
		this.TotalScore = foodAte * _scoreMutiplier;
	    return this.TotalScore;
	}


}
