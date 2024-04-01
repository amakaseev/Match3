using UnityEngine;

namespace Match3 {

  [RequireComponent(typeof(AudioSource))]
  public class AudioManager : MonoBehaviour {
    [SerializeField] AudioClip select;
    [SerializeField] AudioClip deselect;
    [SerializeField] AudioClip match;
    [SerializeField] AudioClip noMatch;
    [SerializeField] AudioClip explosion;
    [SerializeField] AudioClip fall;
    [SerializeField] AudioClip fell;
    [SerializeField] AudioClip spawn;

    AudioSource audioSource;

    private void OnValidate() {
      if (audioSource == null) {
        audioSource = GetComponent<AudioSource>();
      }
    }

    public void PlaySelect() => audioSource.PlayOneShot(select);
    public void PlayDeselect() => audioSource.PlayOneShot(deselect);
    public void PlayMatch() => audioSource.PlayOneShot(match);
    public void PlayNoMatch() => audioSource.PlayOneShot(noMatch);
    public void PlayExplosion() => PlayRandomPitch(explosion);
    public void PlayFall() => PlayRandomPitch(fall);
    public void PlayFell() => PlayRandomPitch(fell);
    public void PlaySpawn() => PlayRandomPitch(spawn);

    void PlayRandomPitch(AudioClip clip) {
      audioSource.pitch = Random.Range(0.9f, 1.1f);
      audioSource.PlayOneShot(clip);
      audioSource.pitch = 1f;
    }
  }

}
