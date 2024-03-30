using System;
using UnityEngine;

namespace Match3 {

  public partial class Grid2D<T> {
    readonly int width;
    readonly int height;
    readonly float cellSize;
    readonly Vector3 origin;
    readonly T[,] gridArray;

    readonly CoordinateConverter coordinateConverter;

    public event Action<int, int, T> OnValueChangeEvent;

    public static Grid2D<T> VerticalGrid(int width, int height, float cellSize, Vector3 origin) {
      return new Grid2D<T>(width, height, cellSize, origin, new VerticalConverter());
    }
    public static Grid2D<T> HorizontalGrid(int width, int height, float cellSize, Vector3 origin) {
      return new Grid2D<T>(width, height, cellSize, origin, new HorizontalConverter());
    }

    public Grid2D(int width, int height, float cellSize, Vector3 origin, CoordinateConverter coordinateConverter) {
      this.width = width;
      this.height = height;
      this.cellSize = cellSize;
      this.origin = origin;
      this.gridArray = new T[width, height];

      this.coordinateConverter = coordinateConverter ?? new VerticalConverter();
    }

    bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

    // Set value to a grid position
    public void SetValue(Vector3 worldPosition, T value) {
      var pos = GetXY(worldPosition);
      SetValue(pos.x, pos.y, value);
    }

    public void SetValue(int x, int y, T value) {
      if (IsValid(x, y)) {
        gridArray[x, y] = value;
        OnValueChangeEvent?.Invoke(x, y, value);
      }
    }

    // Get value from a grid position
    public T GetValue(Vector3 worldPosition) {
      var pos = GetXY(worldPosition);
      return GetValue(pos.x, pos.y);
    }

    public T GetValue(int x, int y) => (IsValid(x, y)) ? gridArray[x, y] : default;

    public Vector2Int GetXY(Vector3 worldPosition) => coordinateConverter.WorldToGrid(worldPosition, cellSize, origin);
    public Vector3 GetWorldPositionCenter(int x, int y) => coordinateConverter.GridToWorldCenter(x, y, cellSize, origin);


    public void DebugDraw() {
#if UNITY_EDITOR
      Vector3 GetWorldPosition(int x, int y) => coordinateConverter.GridToWorld(x, y, cellSize, origin);

      for (int x = 0; x < width; ++x) {
        for (int y = 0; y < height; ++y) {
          UnityEditor.Handles.color = Color.red;
          UnityEditor.Handles.Label(GetWorldPositionCenter(x, y), $"{x},{y}", new GUIStyle { alignment = TextAnchor.MiddleCenter, fontSize = Mathf.FloorToInt(14 * cellSize), normal = new GUIStyleState { textColor = Color.red } });
          Gizmos.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1));
          Gizmos.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y));
        }
      }

      Gizmos.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height));
      Gizmos.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height));
#endif
    }
  }

}
