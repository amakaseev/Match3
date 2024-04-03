using System.Collections;
using TMPro;
using UnityEngine;

namespace Match3 {
  public class GameScoreText: MonoBehaviour {
    [SerializeField] TextMeshProUGUI scoreText;

    float currentScore;

    private void Start() {
      Debug.Log("Score Start");
      UpdateScore();
    }

    private void LateUpdate() {
      UpdateScore();
    }

    public void UpdateScore() {
      currentScore = Mathf.Lerp(currentScore, GameManager.Instance.Score, Time.deltaTime * 2f);
      scoreText.text = Mathf.FloorToInt(currentScore).ToString();
    }
  }
}
