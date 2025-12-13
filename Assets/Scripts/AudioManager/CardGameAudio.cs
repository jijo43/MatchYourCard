using UnityEngine;

public class CardGameAudio : MonoBehaviour
{
    public AudioSource source;
    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip mismatchClip;
    public AudioClip gameOverClip;

    public void PlayFlip() => source.PlayOneShot(flipClip);
    public void PlayMatch() => source.PlayOneShot(matchClip);
    public void PlayMismatch() => source.PlayOneShot(mismatchClip);
    public void PlayGameOver() => source.PlayOneShot(gameOverClip);
}
