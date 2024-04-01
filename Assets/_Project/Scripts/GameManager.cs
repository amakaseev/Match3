using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Match3 {

  public class GameManager: MonoBehaviour {
    [SerializeField] CanvasGroup mainMenu;
    [SerializeField] CanvasGroup gameUI;
    [SerializeField] Match3 gameBoardPrefab;

    Match3 gameBoard;

    private void Start() {
      mainMenu.gameObject.SetActive(true);
      gameUI.gameObject.SetActive(false);
    }

    public void StartGame() {
      // TODO: add cross fade
      mainMenu.gameObject.SetActive(false);
      gameUI.gameObject.SetActive(true);

      if (gameBoard != null) {
        Destroy(gameBoard);
      }
      gameBoard = Instantiate<Match3>(gameBoardPrefab);
    }
  }
}
