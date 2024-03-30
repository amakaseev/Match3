using UnityEngine;

namespace Match3 {

  public partial class Grid2D<T> {
    public class HorizontalConverter : CoordinateConverter {
      public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin) {
        return new Vector3(x, 0, y) * cellSize + origin;
      }

      public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin) {
        return GridToWorld(x, y, cellSize, origin) + new Vector3(cellSize * 0.5f, 0, cellSize * 0.5f);
      }

      public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin) {
        var gridPosition = (worldPosition - origin) / cellSize;
        return new Vector2Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.z));
      }

      public override Vector3 Forward => Vector3.forward;
    }
  }

}
