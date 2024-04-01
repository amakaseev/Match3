using UnityEngine;

namespace Match3 {

  [RequireComponent(typeof(SpriteRenderer))]
  public class Gem: MonoBehaviour{
    [SerializeField] GemType type;

    public GemType Type {
      get => type;
      set {
        this.type = value;
        GetComponent<SpriteRenderer>().sprite = value.sprite;
      }
    }

    public void DestroyGem() => Destroy(gameObject);
  }

}
