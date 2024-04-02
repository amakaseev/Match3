using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SharedSources;
using UnityEngine;

namespace Match3 {

  public class Match3 : MonoBehaviour {
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;
    [SerializeField] float cellSize = 1.0f;
    [SerializeField] Vector3 originPosition = Vector3.zero;
    [SerializeField] Orientation orientation = Orientation.Vertical;

    [SerializeField] Gem gemPrefab;
    [SerializeField] GemType[] gemTypes;
    [SerializeField] float swapDuration = 0.5f;
    [SerializeField] float explodeDuration = 0.1f;
    [SerializeField] Ease easeSelect = Ease.InQuad;
    [SerializeField] Ease easeSwap = Ease.OutBack;
    [SerializeField] Ease fallEase = Ease.OutBack;

    [SerializeField] GameObject spawnVfx;
    [SerializeField] GameObject explosionVfx;

    [SerializeField] IntEventChanel scoreChanel;

    GameAudioManager audioManager;

    Grid2D<GridObject<Gem>> grid;

    InputReader inputReader;
    Vector2Int selectedGem = new Vector2Int(-1, -1);
    Tweener selectedTweener;
    bool gridIsLocked = false;

    private void Awake() {
      inputReader = GetComponent<InputReader>();
      audioManager = GetComponent<GameAudioManager>();
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

    Gem CreateGem(int x, int y) {
      Gem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
      gem.Type = gemTypes[Random.Range(0, gemTypes.Length)];
      var gridObject = new GridObject<Gem>(grid, x, y);
      gridObject.SetValue(gem);
      grid.SetValue(x, y, gridObject);

      return gem;
    }

    void OnSelectedGem() {
      if (gridIsLocked)
        return;

      var gridPos = grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));

      if (!IsValidatePosition(gridPos) || IsEmptyPosition(gridPos)) {
        return;
      }
        

      if (selectedGem == gridPos) {
        DeselectGem();
        audioManager.PlayDeselect();
      } else if (selectedGem.x == -1 && selectedGem.y == -1) {
        SelectGem(gridPos);
        audioManager.PlaySelect();
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
      if (gridObject != null) {
        gridObject.GetValue().transform.localScale = Vector3.one;
      }

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

      gridIsLocked = true;

      // Swap gems
      audioManager.PlayMatch(); // TODO: audioManager.PlayNoMatch();
      yield return StartCoroutine(SwapGems(gridPosA, gridPosB));

      // Matches?
      List<Vector2Int> matches = FindMatches();
      while (matches.Count > 0) {
        scoreChanel.Invoke(matches.Count);

        // Explode gems
        yield return StartCoroutine(ExplodeGems(matches));
        // Make gems fall
        yield return StartCoroutine(MakeGemsFall());
        // Fill empty spots
        yield return StartCoroutine(FillEmptySpots());
        matches = FindMatches();
      }

      gridIsLocked = false;

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

          if (gemA == null || gemB == null || gemC == null)
            continue;

          if (gemA.GetValue().Type == gemB.GetValue().Type && gemB.GetValue().Type == gemC.GetValue().Type) {
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

          if (gemA.GetValue().Type == gemB.GetValue().Type && gemB.GetValue().Type == gemC.GetValue().Type) {
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

      // Play SFX?

      yield return new WaitForSeconds(swapDuration);
    }

    IEnumerator ExplodeGems(List<Vector2Int> matches) {
      foreach (var match in matches) {
        var gem = grid.GetValue(match.x, match.y).GetValue();
        grid.SetValue(match.x, match.y, null);

        ExplodeVFX(match);

        gem.transform.DOPunchScale(Vector3.one * 0.1f, explodeDuration, 1, 0.5f);

        yield return new WaitForSeconds(explodeDuration);
        gem.DestroyGem();
      }
    }

    void ExplodeVFX(Vector2Int pos) {
      // TODO: Pool objects
      var vfx = Instantiate(explosionVfx, transform);
      vfx.transform.position = grid.GetWorldPositionCenter(pos.x, pos.y);
      Destroy(vfx, 2f);

      audioManager.PlayExplosion();
    }

    IEnumerator MakeGemsFall() {
      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          if (grid.GetValue(x, y) == null) {
            for (int i = y + 1; i < height; i++) {
              if (grid.GetValue(x, i) != null) {
                var gem = grid.GetValue(x, i).GetValue();
                grid.SetValue(x, y, grid.GetValue(x, i));
                grid.SetValue(x, i, null);
                gem.transform
                  .DOLocalMove(grid.GetWorldPositionCenter(x, y), 0.5f)
                  .SetEase(fallEase)
                  .OnComplete(() => { /*audioManager.PlayFell();*/ });

                audioManager.PlayFall();
                yield return new WaitForSeconds(0.1f);
                break;
              }
            }
          }
        }
      }
    }

    IEnumerator FillEmptySpots() {
      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          if (grid.GetValue(x, y) == null) {
            var gem = CreateGem(x, y);
            gem.transform.DOPunchScale(Vector3.one * 0.1f, explodeDuration, 1, 0.5f);
            SpawnVFX(new Vector2Int(x, y));
            yield return new WaitForSeconds(0.1f);
          }
        }
      }
    }

    void SpawnVFX(Vector2Int pos) {
      // TODO: Pool objects
      var vfx = Instantiate(spawnVfx, transform);
      vfx.transform.position = grid.GetWorldPositionCenter(pos.x, pos.y);
      Destroy(vfx, 2f);

      audioManager.PlaySpawn();
    }

    private void OnDrawGizmos() {
      if (grid != null) {
        grid.DebugDraw();
        for (int x = 0; x < width; x++) {
          for (int y = 0; y < height; y++) {
            var gemA = grid.GetValue(x, y);
            if (gemA == null)
              continue;
            Gizmos.color = gemA.GetValue().Type.color;
            Gizmos.DrawCube(grid.GetWorldPositionCenter(x, y), new Vector3(1, 1, 1) * cellSize);
          }
        }
      }
    }
  }

}
