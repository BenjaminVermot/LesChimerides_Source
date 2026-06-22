using UnityEngine;

public class TouilleAnimationsScript : MonoBehaviour
{
    [Header("Animations")]
    public Animator playerAnimator;
    public ArduinoListener arduinoListener;

    // Optionnel : pour lisser les changements de vitesse si l'Arduino est trop brusque
    [Header("Réglages")]
    public float smoothTime = 0.1f;
    private float currentSpeedVelo;

    void Update()
    {
        // Sécurité si un composant est manquant
        if (arduinoListener == null || playerAnimator == null) return;

        // 1. Gestion de l'état Immobile (Idle)
        bool immobile = arduinoListener.isStill;
        playerAnimator.SetBool("EstImmobile", immobile);

        if (immobile)
        {
            // Si immobile, on force la vitesse de touille à 0 (au cas où)
            playerAnimator.SetFloat("VitesseTouille", 0f);
        }
        else
        {
            // 2. Gestion de la vitesse de la touille
            float targetSpeed = arduinoListener.finalWheelSpeedFullRange;

            // On applique la vitesse de l'Arduino (0 à 1) au multiplicateur de l'animation
            // Mathf.SmoothDamp évite les saccades si les données Arduino sautent
            currentSpeedVelo = Mathf.MoveTowards(currentSpeedVelo, targetSpeed, Time.deltaTime / smoothTime);

            playerAnimator.SetFloat("VitesseTouille", currentSpeedVelo);
        }
    }
}