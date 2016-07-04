using UnityEngine;
using System.Collections;

public class GameController {
	public Score GameScore;
	public int ScoreMutiplier = 10;

	public GameController() {
		this.GameScore = new Score (10);
	}



	void OnFoodAte() {
		GameScore.updateScore ();

	}

}

public struct Score {
	public int FoodAte;
	public int TotalScore;
	int scoreMutiplier;


	public Score (int scoreMutiplier ,int foodAte = 0, int totalScore = 0)
	{
		this.FoodAte = foodAte;
		this.TotalScore = totalScore;
		this.scoreMutiplier = scoreMutiplier;
	}

	public int getTotalScore() {
		return this.TotalScore;
	}

	public int updateScore() {
		updateScore (++this.FoodAte);
	}

	public int updateScore(int FoodAte) {
		this.FoodAte = FoodAte;
		this.TotalScore = FoodAte * scoreMutiplier;
	}


}
