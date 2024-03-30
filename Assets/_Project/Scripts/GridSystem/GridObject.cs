namespace Match3 {

  public class GridObject<T> {
    Grid2D<GridObject<T>> grid;
    int x, y;
    T gem;

    public GridObject(Grid2D<GridObject<T>> grid, int x, int y) {
      this.grid = grid;
      this.x = x;
      this.y = y;
    }

    public void SetValue(T gem) {
      this.gem = gem;
    }

    public T GetValue() => gem;
  }

}
