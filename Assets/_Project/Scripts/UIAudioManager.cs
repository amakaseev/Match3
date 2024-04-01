using UnityEngine;

namespace Match3 {

  [RequireComponent(typeof(AudioSource))]
  public class UIAudioManager : MonoBehaviour {
    [SerializeField] AudioClip click;

    AudioSource audioSource;

    private void Start() {
      audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick() => audioSource.PlayOneShot(click);
  }

}
