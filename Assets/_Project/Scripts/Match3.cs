using UnityEditor;
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

    Grid2D<GridObject<Gem>> grid;

    void Start () {
      InitializeGrid();
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

    private void OnDrawGizmos() {
      grid?.DebugDraw();
    }
  }

}
