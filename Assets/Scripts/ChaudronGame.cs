using UnityEngine;
using UnityEngine.UIElements;
using System.Collections; // Requis pour les Coroutines

public class cuisineGame : MonoBehaviour
{
    // ─────────────────────────────────────────
    //  RÉFÉRENCES EXTERNES
    // ─────────────────────────────────────────
    [Header("Références")]
    public ArduinoListener arduinoListener;
    public UIDocument uiDocument;
    public GameUI gameUI;

    // ─────────────────────────────────────────
    //  TEXTURES D'INSTRUCTIONS
    // ─────────────────────────────────────────
    [Header("Textures d'instructions (7 slots)")]
    public Texture2D textureAvantLent;
    public Texture2D textureAvantNormal;
    public Texture2D textureAvantRapide;
    public Texture2D textureArriereLent;
    public Texture2D textureArriereNormal;
    public Texture2D textureArriereRapide;
    public Texture2D textureStop;

    public Texture2D textureJuste;
    public Texture2D textureFaux;

    [Header("UI Toolkit Animation")]
    [Tooltip("Le nom de l'élément VisualElement dans votre UXML qui affiche le sprite Juste/Faux")]
    public string nomElementValidation = "ValidationIcon";
    private VisualElement validationElement;

    // ─────────────────────────────────────────
    //  PLAGES DE VITESSE CIBLE
    //  Valeur signée : positif = avant, négatif = arrière.
    //  La manivelle envoie 0 à 1 (1 quasi impossible à atteindre),
    //  donc inutile de viser des bornes hautes irréalistes.
    // ─────────────────────────────────────────
    [Header("Plages de vitesse cible (vitesse brute 0–1, sans le signe)")]
    [Tooltip("Plage acceptée pour Avant/Arrière LENT")]
    public Vector2 rangeLent = new Vector2(0.02f, 0.2f);
    [Tooltip("Plage acceptée pour Avant/Arrière NORMAL")]
    public Vector2 rangeNormal = new Vector2(0.2f, 0.4f);
    [Tooltip("Plage acceptée pour Avant/Arrière RAPIDE")]
    public Vector2 rangeRapide = new Vector2(0.4f, 1f);
    [Tooltip("Tolérance autour de 0 pour l'instruction STOP")]
    public float toleranceStop = 0.05f;

    // ─────────────────────────────────────────
    //  TIMING
    // ─────────────────────────────────────────
    [Header("Timing")]
    [Tooltip("Durée d'affichage initiale d'une instruction (secondes)")]
    public float intervalleDepart = 3f;
    [Tooltip("Durée d'affichage minimale en fin de recette")]
    public float intervalleMin = 1.2f;
    [Tooltip("Pause entre la fin d'une instruction et la suivante")]
    public float pauseEntreInstructions = 0.5f;
    public float stopDurée = 0.5f;

    // ─────────────────────────────────────────
    //  JAUGE
    // ─────────────────────────────────────────
    [Header("Jauge")]
    public float gainSucces = 0.08f;
    public float penalteEchec = 0.05f;

    // ─────────────────────────────────────────
    //  ÉTAT INTERNE
    // ─────────────────────────────────────────
    public enum TypeInstruction
    {
        AvantLent, AvantNormal, AvantRapide,
        ArriereLent, ArriereNormal, ArriereRapide,
        Stop
    }

    private TypeInstruction instructionCourante;

    // Timer de la fenêtre d'affichage courante
    private float dureeFenetreActuelle;
    private float timerFenetre;

    // Accumulation pour la vitesse signée moyenne
    private float sommeVitesseSignee; // somme pondérée par deltaTime de (vitesse * direction)
    private float tempsEcoule;

    private bool enAttenteValidation;
    private bool enPause;
    private float timerPause;

    private VisualElement imageContainer;

    // ─────────────────────────────────────────
    //  DÉMARRAGE
    // ─────────────────────────────────────────
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        imageContainer = root.Q<VisualElement>("ImageChauderon");

        validationElement = root.Q<VisualElement>(nomElementValidation);
        if (validationElement != null)
        {
            validationElement.AddToClassList("pop-hidden");
        }

        gameUI.progressValue = 0f;
        CacherInstruction();

        enAttenteValidation = false;
        enPause = true;
        timerPause = pauseEntreInstructions;
    }

    // ─────────────────────────────────────────
    //  BOUCLE PRINCIPALE
    // ─────────────────────────────────────────
    void Update()
    {
        if (enPause)
        {
            timerPause -= Time.deltaTime;
            if (timerPause <= 0f)
            {
                enPause = false;
                LancerNouvelleInstruction();
            }
            return;
        }

        if (enAttenteValidation)
        {
            AccumulerVitesse();

            timerFenetre -= Time.deltaTime;
            if (timerFenetre <= 0f)
            {
                JugerInstruction();
            }
        }

        if (gameUI.progressValue >= 1f)
            RecetteTerminee();
    }

    // ─────────────────────────────────────────
    //  NOUVELLE INSTRUCTION
    // ─────────────────────────────────────────
    void LancerNouvelleInstruction()
    {
        instructionCourante = (TypeInstruction)Random.Range(0, 7);
        if (instructionCourante == TypeInstruction.Stop)
        {
            dureeFenetreActuelle = stopDurée;
        }
        else
        {
            dureeFenetreActuelle = Mathf.Lerp(intervalleDepart, intervalleMin, gameUI.progressValue);
        }

        AfficherInstruction(instructionCourante);

        timerFenetre = dureeFenetreActuelle;

        sommeVitesseSignee = 0f;
        tempsEcoule = 0f;

        enAttenteValidation = true;

        Debug.Log($"[Cuisine] Instruction : {instructionCourante} | Durée : {dureeFenetreActuelle:F1}s");
    }

    // ─────────────────────────────────────────
    //  ACCUMULATION DE LA VITESSE SIGNÉE (chaque frame)
    //  On accumule la valeur brute, pas un booléen :
    //  une frame mal placée pèse peu sur la moyenne globale.
    // ─────────────────────────────────────────
    void AccumulerVitesse()
    {
        float dt = Time.deltaTime;
        tempsEcoule += dt;

        float vitesseSignee = arduinoListener.finalWheelSpeed * arduinoListener.direction;
        sommeVitesseSignee += vitesseSignee * dt;
    }

    // ─────────────────────────────────────────
    //  JUGEMENT À LA FIN DE LA FENÊTRE
    // ─────────────────────────────────────────
    void JugerInstruction()
    {
        enAttenteValidation = false;
        CacherInstruction();

        float moyenneVitesseSignee = tempsEcoule > 0f ? sommeVitesseSignee / tempsEcoule : 0f;
        bool reussi = EstDansLaPlageCible(instructionCourante, moyenneVitesseSignee);

        if (reussi)
        {
            gameUI.progressValue = Mathf.Clamp01(gameUI.progressValue + gainSucces);
            gameUI.chauderonValidationSprites = textureJuste;
            Debug.Log($"[Cuisine] ✓ Réussi ! Vitesse moyenne signée : {moyenneVitesseSignee:F3}");
        }
        else
        {
            gameUI.progressValue = Mathf.Clamp01(gameUI.progressValue - penalteEchec);
            gameUI.chauderonValidationSprites = textureFaux;
            Debug.Log($"[Cuisine] ✗ Raté ! Vitesse moyenne signée : {moyenneVitesseSignee:F3}");
        }

        if (validationElement != null)
        {
            StartCoroutine(AnimerFeedbackValidation());
        }
        else
        {
            EnclencherPauseSuivante();
        }
    }

    private IEnumerator AnimerFeedbackValidation()
    {
        validationElement.RemoveFromClassList("pop-hidden");
        validationElement.AddToClassList("pop-visible");

        yield return new WaitForSeconds(1.0f);

        validationElement.RemoveFromClassList("pop-visible");
        validationElement.AddToClassList("pop-hidden");

        yield return new WaitForSeconds(0.25f);

        EnclencherPauseSuivante();
    }

    private void EnclencherPauseSuivante()
    {
        enPause = true;
        timerPause = pauseEntreInstructions;
    }

    // ─────────────────────────────────────────
    //  VÉRIFICATION : la moyenne tombe-t-elle dans la bonne plage ?
    // ─────────────────────────────────────────
    bool EstDansLaPlageCible(TypeInstruction instruction, float moyenneVitesseSignee)
    {
        switch (instruction)
        {
            case TypeInstruction.Stop:
                return Mathf.Abs(moyenneVitesseSignee) <= toleranceStop;

            case TypeInstruction.AvantLent:
                return moyenneVitesseSignee >= rangeLent.x && moyenneVitesseSignee <= rangeLent.y;
            case TypeInstruction.AvantNormal:
                return moyenneVitesseSignee >= rangeNormal.x && moyenneVitesseSignee <= rangeNormal.y;
            case TypeInstruction.AvantRapide:
                return moyenneVitesseSignee >= rangeRapide.x && moyenneVitesseSignee <= rangeRapide.y;

            case TypeInstruction.ArriereLent:
                return moyenneVitesseSignee <= -rangeLent.x && moyenneVitesseSignee >= -rangeLent.y;
            case TypeInstruction.ArriereNormal:
                return moyenneVitesseSignee <= -rangeNormal.x && moyenneVitesseSignee >= -rangeNormal.y;
            case TypeInstruction.ArriereRapide:
                return moyenneVitesseSignee <= -rangeRapide.x && moyenneVitesseSignee >= -rangeRapide.y;

            default:
                return false;
        }
    }

    // ─────────────────────────────────────────
    //  AFFICHAGE UI TOOLKIT
    // ─────────────────────────────────────────
    void AfficherInstruction(TypeInstruction instruction)
    {
        Texture2D texture = instruction switch
        {
            TypeInstruction.AvantLent => textureAvantLent,
            TypeInstruction.AvantNormal => textureAvantNormal,
            TypeInstruction.AvantRapide => textureAvantRapide,
            TypeInstruction.ArriereLent => textureArriereLent,
            TypeInstruction.ArriereNormal => textureArriereNormal,
            TypeInstruction.ArriereRapide => textureArriereRapide,
            TypeInstruction.Stop => textureStop,
            _ => null
        };

        if (texture != null)
        {
            gameUI.chauderonQuestImg = texture;
            imageContainer.style.display = DisplayStyle.Flex;
        }
    }

    void CacherInstruction()
    {
        imageContainer.style.display = DisplayStyle.None;
    }

    // ─────────────────────────────────────────
    //  VICTOIRE
    // ─────────────────────────────────────────
    void RecetteTerminee()
    {
        Debug.Log("[Cuisine] Recette terminée !");
        CacherInstruction();
        this.enabled = false;
    }
}