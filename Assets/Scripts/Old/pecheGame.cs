using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pecheGame : MonoBehaviour
{
    public GameUI uiObject;
    public ArduinoListener arduinoListener;

    [Header("Référence Animator")]
    public Animator playerAnimator;

    [Header("Paramètres du Poisson")]
    [Tooltip("Force maximale de la résistance du poisson (à 100% de la jauge)")]
    public float maxResistance = 15f;
    [Tooltip("Force minimale de la résistance du poisson (au début, à 0%)")]
    public float minResistance = 2f;

    [Header("Comportement Dynamique (Lutte)")]
    [Tooltip("Temps minimum avant que le poisson change d'humeur")]
    public float tempsHumeurMin = 2.0f;
    [Tooltip("Temps maximum avant que le poisson change d'humeur")]
    public float tempsHumeurMax = 4.0f;
    [Tooltip("Multiplicateur de force quand le poisson s'énerve (Phase Attaque)")]
    public float multiplicateurAttaque = 2.0f;
    [Tooltip("Multiplicateur de force quand le poisson fatigue (Phase Calme)")]
    public float multiplicateurFatigue = 0.3f;

    [Header("Paramètres du Joueur")]
    public float playerForce = 20f;

    [Header("Paramètres des Animations")]
    [Tooltip("Vitesse de descente de la jauge (par seconde) au-delà de laquelle poissonGagne se déclenche")]
    public float seuilDescente = 5f;

    // Variables internes pour la gestion des phases
    private float humeurTimer;
    private float humeurDureeActuelle;
    private float multiplicateurHumeurActuel = 1.0f;

    private float progressPrecedente;
    private string animationActuelle = "";

    private bool gameHasEnded = false;

    [Header("Tranition de scene")]
    public int sceneSuivante = 2;
    public int secondesAttente = 3;
    public Animator rideauAnimator;

    void Start()
    {
        uiObject.progressValue = 0;
        progressPrecedente = 0f;

        // Initialise le premier changement d'humeur
        ChangerHumeurPoisson();
        JouerAnimation("Peche_intensité1");
    }

    void Update()
    {
        if (gameHasEnded == true) return;
        // 1. Entrée du joueur (Arduino)
        float joueurForce = arduinoListener.finalWheelSpeedFullRange * Time.deltaTime * playerForce;
        uiObject.progressValue += joueurForce;

        // 2. Gestion et calcul de la résistance du poisson
        if (uiObject.progressValue > 0 && uiObject.progressValue < 100)
        {
            GérerHumeurPoisson();

            // Résistance de base linéaire selon la progression (0% à 100%)
            float progressionNormalisee = uiObject.progressValue / 100f;
            float resistanceBase = Mathf.Lerp(minResistance, maxResistance, progressionNormalisee);

            // Application de la phase actuelle (Attaque, Normal ou Fatigue)
            float resistancePoisson = resistanceBase * multiplicateurHumeurActuel * Time.deltaTime;

            // --- SÉCURITÉ ANTI-BLOCAGE ---
            // Si le joueur mouline (force > 0) et qu'on est proche de la fin, 
            // on s'assure que la résistance ne dépasse jamais 90% de la force du joueur.
            // Cela garantit qu'on puisse toujours gagner, même en phase d'attaque à 95% de la jauge.
            if (progressionNormalisee > 0.8f && joueurForce > 0)
            {
                resistancePoisson = Mathf.Min(resistancePoisson, joueurForce * 0.9f);
            }

            uiObject.progressValue -= resistancePoisson;
        }

        // Sécurité pour que la jauge reste entre 0 et 100
        uiObject.progressValue = Mathf.Clamp(uiObject.progressValue, 0f, 100f);

        // 3. Gestion des animations
        MettreAJourAnimation();

        // 4. Vérification de la victoire
        if (uiObject.progressValue >= 100f)
        {
            GagnerLePoisson();
        }

        progressPrecedente = uiObject.progressValue;
    }

    void GérerHumeurPoisson()
    {
        humeurTimer += Time.deltaTime;
        if (humeurTimer >= humeurDureeActuelle)
        {
            ChangerHumeurPoisson();
        }
    }

    void ChangerHumeurPoisson()
    {
        humeurTimer = 0f;
        humeurDureeActuelle = Random.Range(tempsHumeurMin, tempsHumeurMax);

        // Tire au sort l'état du poisson : 35% Attaque, 45% Normal, 20% Fatigue
        float roll = Random.value;

        if (roll < 0.35f)
        {
            // Le poisson s'énerve et tire fort
            multiplicateurHumeurActuel = multiplicateurAttaque;
            // Optionnel : Tu peux ajouter un feedback visuel ou un son ici "Poisson fâché !"
        }
        else if (roll < 0.80f)
        {
            // Comportement normal
            multiplicateurHumeurActuel = 1.0f;
        }
        else
        {
            // Le poisson fatigue, le joueur reprend l'avantage
            multiplicateurHumeurActuel = multiplicateurFatigue;
        }
    }

    void MettreAJourAnimation()
    {
        float progress = uiObject.progressValue;
        float vitesseChangement = (progress - progressPrecedente) / Time.deltaTime;

        if (vitesseChangement < -seuilDescente)
        {
            JouerAnimation("Peche_poissonGagne");
            return;
        }

        if (progress < 33.33f)
        {
            JouerAnimation("Peche_intensité1");
        }
        else if (progress < 66.66f)
        {
            JouerAnimation("Peche_intensité2");
        }
        else
        {
            JouerAnimation("Peche_intensité3");
        }
    }

    void JouerAnimation(string nomAnimation)
    {
        if (animationActuelle == nomAnimation) return;

        animationActuelle = nomAnimation;
        playerAnimator.Play(nomAnimation);
    }

    void GagnerLePoisson()
    {
        gameHasEnded = true;
        Debug.Log("Poisson attrapé ! Beau combat !");
        JouerAnimation("WonGame");
        StartCoroutine(waitForRideaux());
    }

    IEnumerator waitForRideaux()
    {

        yield return new WaitForSeconds(secondesAttente);
        rideauAnimator.SetTrigger("close");
        yield return new WaitForSeconds(secondesAttente);
        SceneManager.LoadScene(sceneSuivante);
    }
}