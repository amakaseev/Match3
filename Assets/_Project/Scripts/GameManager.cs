using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Match3 {

  public class GameManager: MonoBehaviour {
    [SerializeField] CanvasGroup mainMenu;
    [SerializeField] CanvasGroup gameUI;
    [SerializeField] Match3 gameBoardPrefab;

    public static GameManager Instance { get; private set; }
    public int Score { get; private set; }

    Match3 gameBoard;


    private void Awake() {
      if (Instance == null) {
        Instance = this;
      } else {
        Destroy(gameObject);
      }
    }

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

    public void AddScore(int score) {
      Score += score;
    }
  }
}
