using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource lampBlinking;
    // [SerializeField] private AudioSource dding;

    void Start()
    {
        StartCoroutine(PlaySoundWithDelay(lampBlinking, 1f));
        // StartCoroutine(PlaySoundWithDelay(dding, 4f));
        // StartCoroutine(PlaySoundWithDelay(dding, 4.5f));
        // StartCoroutine(PlaySoundWithDelay(dding, 5f));
        // StartCoroutine(PlaySoundWithDelay(dding, 5.5f));
        // StartCoroutine(PlaySoundWithDelay(dding, 7f));
    }

    IEnumerator PlaySoundWithDelay(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }
}
