namespace Match3 {

  public class GridObject<T> {
    Grid2D<GridObject<T>> grid;
    int x, y;

    public GridObject(Grid2D<GridObject<T>> grid, int x, int y) {
      this.grid = grid;
      this.x = x;
      this.y = y;
    }
  }

}
