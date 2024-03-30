using UnityEngine;

namespace Match3 {

  public partial class Grid2D<T> {
    public abstract class CoordinateConverter {
      public abstract Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin);
      public abstract Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin);
      public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin);

      public abstract Vector3 Forward {  get; }
    }
  }

}
