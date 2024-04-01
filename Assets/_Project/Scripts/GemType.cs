using UnityEngine;

namespace Match3 {

  [CreateAssetMenu(menuName = "Math3/GemType", fileName = "GemType")]
  public class GemType: ScriptableObject {
    public Sprite sprite;
    public Color color = Color.white;
  }

}
