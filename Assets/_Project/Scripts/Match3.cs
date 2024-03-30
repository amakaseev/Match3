﻿using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3 {

  public class Match3: MonoBehaviour {
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;
    [SerializeField] float cellSize = 1.0f;
    [SerializeField] Vector3 originPosition = Vector3.zero;
    [SerializeField] Orientation orientation = Orientation.Vertical;

    [SerializeField] Gem gemPrefab;
    [SerializeField] GemType[] gemTypes;
    [SerializeField] Ease ease = Ease.InQuad;

    Grid2D<GridObject<Gem>> grid;

    InputReader inputReader;
    Vector2Int selectedGem = new Vector2Int(-1, -1);

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

    void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;
    void DeselectGem() => selectedGem = new Vector2Int(-1, -1);

    bool IsValidatePosition(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
    bool IsEmptyPosition(Vector2Int pos) => grid.GetValue(pos.x, pos.y) == null;

    IEnumerator RunGameLoop(Vector2Int gridPosA, Vector2Int gridPosB) {
      StartCoroutine(SwapGems(gridPosA, gridPosB));

      DeselectGem();
      yield return null;
    }

    IEnumerator SwapGems(Vector2Int gridPosA, Vector2Int gridPosB) {
      var gridObjectA = grid.GetValue(gridPosA.x, gridPosA.y);
      var gridObjectB = grid.GetValue(gridPosB.x, gridPosB.y);

      gridObjectA.GetValue().transform
        .DOLocalMove(grid.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f)
        .SetEase(ease);
      gridObjectB.GetValue().transform
        .DOLocalMove(grid.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f)
        .SetEase(ease);

      grid.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
      grid.SetValue(gridPosB.x, gridPosB.y, gridObjectA);

      yield return new WaitForSeconds(0.5f);
    }

    private void OnDrawGizmos() {
      grid?.DebugDraw();
    }
  }

}
