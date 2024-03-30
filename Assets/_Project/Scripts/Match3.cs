using UnityEditor;
using UnityEngine;

namespace Match3 {

  public class Match3: MonoBehaviour {
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;
    [SerializeField] float cellSize = 1.0f;
    [SerializeField] Vector3 originPosition = Vector3.zero;
    [SerializeField] Orientation orientation = Orientation.Vertical;

    Grid2D<GridObject<Gem>> grid;

    void Start () {
      grid = Grid2D<GridObject<Gem>>.Grid2DHelpers.CreateGrid(width, height, cellSize, originPosition, orientation);
    }

    private void OnDrawGizmos() {
      grid?.DebugDraw();
    }
  }

}
