using UnityEngine;

public class SFXManager : MonoBehaviour
{
    // Instance unique du gestionnaire
    public static SFXManager Instance { get; private set; }

    [Header("Source audio pour les effets sp®¶ciaux")]
    public AudioSource sfxAudioSource;

    [Header("Clips audio disponibles")]
    public AudioClip spawnClip;
    public AudioClip successClip;
    public AudioClip failureClip;
    public AudioClip highlightClip;

    void Awake()
    {
        // Singleton pour ®¶viter les doublons
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garde l'objet m®∫me entre les sc®®nes
        }

        // Si aucune source n'est assign®¶e, on en cr®¶e une
        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.spatialBlend = 0f; // 2D sound
        }
    }

    // Fonctions pr®∫tes ®§ l°Øemploi
    public void PlaySpawn() => PlayClip(spawnClip);
    public void PlaySuccess() => PlayClip(successClip);
    public void PlayFailure() => PlayClip(failureClip);
    public void PlayHighlight() => PlayClip(highlightClip);

    // Fonction g®¶n®¶rique
    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxAudioSource != null)
        {
            Debug.Log($"≤•∑≈“Ù∆µ£∫{clip.name}");
            sfxAudioSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning("Clip ªÚ AudioSource Œ™ø’£°");
        }
    }
}
