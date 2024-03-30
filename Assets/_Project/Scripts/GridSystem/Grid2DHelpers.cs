using UnityEngine;

namespace Match3 {

  enum Orientation {
    Vertical, Horizontal
  }

  public partial class Grid2D<T> {
    internal static class Grid2DHelpers {
      public static Grid2D<T> CreateGrid(int width, int height, float cellSize, Vector3 origin, Orientation orientation) {
        return new Grid2D<T>(width, height, cellSize, origin, orientation switch {
          Orientation.Vertical => new VerticalConverter(),
          Orientation.Horizontal => new HorizontalConverter(),
          _ => new VerticalConverter()
        });
      }
    }
  }

}
