using System;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.Audio; // Ne pas oublier pour manipuler l'Audio Mixer

[RequireComponent(typeof(AudioSource))]
public class BarrelOrganController : MonoBehaviour
{
    [Header("Configuration Série")]

    public ArduinoListener arduinoListener;
    private float finalWheelSpeed;

    [Header("Configuration Audio Mixer")]
    public AudioMixer audioMixer; // Glisse ton 'OrganMixer' ici dans l'inspecteur

    [Header("Configuration de l'Orgue")]
    public float targetSpeed = 0.5f;
    public float smoothTime = 0.5f;

    private AudioSource audioSource;
    private float currentPitchVelocity = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.Play();
        audioSource.pitch = 0f;
    }

    void Update()
    {
        //ne rien faire si la manivelle est still
        if (arduinoListener.isStill)
        {
            return;
        }

        finalWheelSpeed = arduinoListener.finalWheelSpeed;
        Debug.Log("Current organ speed = " + finalWheelSpeed);
        // 1. Lecture Arduino

        // 2. Vitesse de lecture cible (le pitch de l'AudioSource)
        float targetPlaybackSpeed = 0.5f;
        if (finalWheelSpeed > 0.001f)
        {
            targetPlaybackSpeed = finalWheelSpeed / targetSpeed;
        }

        // Limites de l'effet Pitch Shifter de Unity (il ne peut pas aller en dessous de 0.50 ou au-dessus de 2.0)
        targetPlaybackSpeed = Mathf.Clamp(targetPlaybackSpeed, 0.51f, 1.99f);

        // Si la manivelle s'arrête vraiment, on gère la pause
        if (finalWheelSpeed <= 0.001f)
        {
            if (audioSource.isPlaying) audioSource.Pause();
            return; // On stoppe le calcul ici pour éviter les divisions par zéro
        }
        else
        {
            if (!audioSource.isPlaying) audioSource.UnPause();
        }

        // 3. Application fluide de la vitesse de lecture
        audioSource.pitch = Mathf.SmoothDamp(audioSource.pitch, targetPlaybackSpeed, ref currentPitchVelocity, smoothTime);

        // 4. FONCTION DE COMPENSATION (Mathématique)
        // Pour que la tonalité reste à 1.0 (normale), le shifter doit être l'inverse de la vitesse.
        float compensationPitch = 1.0f / audioSource.pitch;

        // Envoi de la valeur de compensation à l'Audio Mixer
        audioMixer.SetFloat("OrganPitch", compensationPitch);
    }
}