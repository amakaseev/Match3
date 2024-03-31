using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Match3 {

  public class Match3: MonoBehaviour {
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;
    [SerializeField] float cellSize = 1.0f;
    [SerializeField] Vector3 originPosition = Vector3.zero;
    [SerializeField] Orientation orientation = Orientation.Vertical;

    [SerializeField] Gem gemPrefab;
    [SerializeField] GemType[] gemTypes;
    [SerializeField] float swapDuration = 0.5f;
    [SerializeField] Ease easeSelect = Ease.InQuad;
    [SerializeField] Ease easeSwap = Ease.OutBack;

    Grid2D<GridObject<Gem>> grid;

    InputReader inputReader;
    Vector2Int selectedGem = new Vector2Int(-1, -1);
    Tweener selectedTweener;

    private void Awake() {
      inputReader = GetComponent<InputReader>();
    }

    private void Start() {
      InitializeGrid();
      inputReader.Fire += OnSelectedGem;
    }

    private void OnDestroy() {
      inputReader.Fire -= OnSelectedGem;
    }

    void InitializeGrid() {
      grid = Grid2D<GridObject<Gem>>.Grid2DHelpers.CreateGrid(width, height, cellSize, originPosition, orientation);

      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          CreateGem(x, y);
        }
      }
    }

    void CreateGem(int x, int y) {
      Gem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
      gem.Type = gemTypes[Random.Range(0, gemTypes.Length)];
      var gridObject = new GridObject<Gem>(grid, x, y);
      gridObject.SetValue(gem);
      grid.SetValue(x, y, gridObject);
    }

    void OnSelectedGem() {
      var gridPos = grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));

      if (!IsValidatePosition(gridPos) || IsEmptyPosition(gridPos))
        return;

      if (selectedGem == gridPos) {
        DeselectGem();
      } else if (selectedGem.x == -1 && selectedGem.y == -1) {
        SelectGem(gridPos);
      } else {
        StartCoroutine(RunGameLoop(selectedGem, gridPos));
      }
    }

    void SelectGem(Vector2Int gridPos) {
      selectedGem = gridPos;

      var gridObject = grid.GetValue(gridPos.x, gridPos.y);

      if (selectedTweener != null) {
        selectedTweener.Kill();
      }

      selectedTweener = gridObject.GetValue().transform
        .DOScale(1.2f, 0.25f)
        .SetEase(easeSelect)
        .SetLoops(-1, LoopType.Yoyo);
    }

    void DeselectGem() {
      var gridObject = grid.GetValue(selectedGem.x, selectedGem.y);
      gridObject.GetValue().transform.localScale = Vector3.one;

      if (selectedTweener != null) {
        selectedTweener.Kill();
        selectedTweener = null;
      }

      selectedGem = new Vector2Int(-1, -1);
    }

    bool IsValidatePosition(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
    bool IsEmptyPosition(Vector2Int pos) => grid.GetValue(pos.x, pos.y) == null;

    IEnumerator RunGameLoop(Vector2Int gridPosA, Vector2Int gridPosB) {
      DeselectGem();

      // Swap gems
      StartCoroutine(SwapGems(gridPosA, gridPosB));

      // Matches?
      List<Vector2Int> matches = FindMatches();

      // Explode gems
      StartCoroutine(ExplodeGems(matches));

      yield return null;
    }

    List<Vector2Int> FindMatches() {
      HashSet<Vector2Int> matches = new();

      // Horizontal
      for (int y = 0; y < height; y++) {
        for (int x = 0; x < width; x++) {
          var gemA = grid.GetValue(x, y);
          var gemB = grid.GetValue(x + 1, y);
          var gemC = grid.GetValue(x + 2, y);

          if (gemA == null || gemB == null || gemC == null) continue;

          if (gemA.GetValue().Type == gemB.GetValue().Type && gemC.GetValue().Type == gemC.GetValue().Type) {
            matches.Add(new Vector2Int(x, y));
            matches.Add(new Vector2Int(x + 1, y));
            matches.Add(new Vector2Int(x + 2, y));
          }
        }
      }

      // Vertical
      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          var gemA = grid.GetValue(x, y);
          var gemB = grid.GetValue(x, y + 1);
          var gemC = grid.GetValue(x, y + 2);

          if (gemA == null || gemB == null || gemC == null)
            continue;

          if (gemA.GetValue().Type == gemB.GetValue().Type && gemC.GetValue().Type == gemC.GetValue().Type) {
            matches.Add(new Vector2Int(x, y));
            matches.Add(new Vector2Int(x, y + 1));
            matches.Add(new Vector2Int(x, y + 2));
          }
        }
      }



      return new List<Vector2Int>(matches);
    }

    IEnumerator SwapGems(Vector2Int gridPosA, Vector2Int gridPosB) {
      var gridObjectA = grid.GetValue(gridPosA.x, gridPosA.y);
      var gridObjectB = grid.GetValue(gridPosB.x, gridPosB.y);

      gridObjectA.GetValue().transform
        .DOLocalMove(grid.GetWorldPositionCenter(gridPosB.x, gridPosB.y), swapDuration)
        .SetEase(easeSwap);
      gridObjectB.GetValue().transform
        .DOLocalMove(grid.GetWorldPositionCenter(gridPosA.x, gridPosA.y), swapDuration)
        .SetEase(easeSwap);

      grid.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
      grid.SetValue(gridPosB.x, gridPosB.y, gridObjectA);

      yield return new WaitForSeconds(swapDuration);
    }

    IEnumerator ExplodeGems(List<Vector2Int> matches) {
      float explodeDuration = 0.1f;

      foreach (var match in matches) {
        var gem = grid.GetValue(match.x, match.y).GetValue();
        grid.SetValue(match.x, match.y, null);

        // Explode VFX

        gem.transform.DOPunchScale(Vector3.one * 0.1f, explodeDuration, 1, 0.5f);

        yield return new WaitForSeconds(explodeDuration);
        gem.DestroyGem();
      }
    }

    private void OnDrawGizmos() {
      grid?.DebugDraw();
    }
  }

}
