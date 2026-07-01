using System;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.Audio;
using UnityEngine.Rendering; // Ne pas oublier pour manipuler l'Audio Mixer

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
    public float musicVolume = 0.5f;

    public AudioSource[] musicLayers;
    private float currentPitchVelocity = 0f;

    void Start()
    {
        foreach (AudioSource layer in musicLayers)
        {
            layer.loop = true;
            layer.Play();
            layer.pitch = 0f;
            layer.volume = musicVolume;
        }

        // musicLayers[0].volume = musicVolume;
    }

    public void addNextMusicLayer(int index)
    {
        musicLayers[index].volume = musicVolume;
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


        // 2. Vitesse de lecture cible (le pitch de l'AudioSource)
        float targetPlaybackSpeed = 0.5f;
        if (finalWheelSpeed > 0.001f)
        {
            targetPlaybackSpeed = finalWheelSpeed / targetSpeed;
        }

        // Limites de l'effet Pitch Shifter de Unity (il ne peut pas aller en dessous de 0.50 ou au-dessus de 2.0)
        targetPlaybackSpeed = Mathf.Clamp(targetPlaybackSpeed, 0.51f, 1.99f);


        foreach (AudioSource layer in musicLayers)
        {
            // Si la manivelle s'arrête vraiment, on gère la pause
            if (finalWheelSpeed <= 0.001f)
            {
                if (layer.isPlaying) layer.Pause();
                return; // On stoppe le calcul ici pour éviter les divisions par zéro
            }
            else
            {
                if (!layer.isPlaying) layer.UnPause();
            }

            // 3. Application fluide de la vitesse de lecture
            layer.pitch = Mathf.SmoothDamp(layer.pitch, targetPlaybackSpeed, ref currentPitchVelocity, smoothTime);

            // 4. FONCTION DE COMPENSATION (Mathématique)
            // Pour que la tonalité reste à 1.0 (normale), le shifter doit être l'inverse de la vitesse.
            float compensationPitch = 1.0f / layer.pitch;

            audioMixer.SetFloat("OrganPitch", compensationPitch);

        }
    }
}