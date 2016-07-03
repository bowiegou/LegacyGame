using UnityEngine;
using System.Collections;

public class GameController {
	public Score GameScore;

	public GameController() {
		this.GameScore = new Score ();
	}



	void OnFoodAte() {
		this.GameScore.FoodAte++;
		//TODO: fix total score
		this.GameScore.TotalScore++;

	}

}

public struct Score {
	public int FoodAte;
	public int TotalScore;

	public Score (int foodAte = 0, int totalScore = 0)
	{
		this.FoodAte = foodAte;
		this.TotalScore = totalScore;
	}

	public int getTotalScore() {
		return this.TotalScore;
	}
	


}
