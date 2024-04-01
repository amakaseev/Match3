using UnityEngine;

namespace Match3 {

  public class GameManager: MonoBehaviour {
    [SerializeField] GameObject activeBackground;
    [SerializeField] GameObject deactiveBackground;
    [SerializeField] Match3 gameBoardPrefab;

    Match3 gameBoard;

    private void Start() {
      activeBackground.SetActive(false);
      deactiveBackground.SetActive(true);
    }

    public void StartGame() {
      // TODO: add cross fade
      activeBackground.SetActive(true);
      deactiveBackground.SetActive(false);

      if (gameBoard != null) {
        Destroy(gameBoard);
      }
      gameBoard = Instantiate<Match3>(gameBoardPrefab);
    }
  }
}
