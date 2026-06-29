using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LineRenderer))]
public class OrganicFrequencyLine : MonoBehaviour
{
    public ArduinoListener arduinoListener;
    public bool accorded = false;
    public bool isAccording = true;

    [Header("Contrôle de la Timeline")]
    public PlayableDirector playableDirector;

    public StillLineScript targetLine;
    private LineRenderer lineRenderer;
    public float lineRenderMultiplier = 2f;

    [Header("Configuration de la ligne")]
    [SerializeField] private int pointsCount = 100;
    [SerializeField] private float length = 10f;

    [Header("Paramètres de la Fréquence Base")]
    public float frequency = 2f;
    public float amplitude = 1f;
    public float speed = 5f;
    private float wavePhase = 0f;

    [Header("Paramètres du Noise (Organique)")]
    [Range(0f, 1f)]
    public float noiseIntensity = 0.3f;
    public float noiseRoughness = 5f;
    public float noiseSpeed = 2f;

    public float lerpedWheelValue;

    [Header("Lerp de la ligne")]
    public float lerpSpeed = 3f;

    [Header("Contrôle de la Séquence & Easing")]
    [SerializeField] private float maxSequenceSpeed = 1f;
    [SerializeField] private float easingSpeed = 2f;
    private float currentSequenceSpeed = 0f;
    private double currentTimePosition = 0f;

    [Header("Nouveau : Système d'Accordage (3s)")]
    [SerializeField] private float timeRequiredToAccord = 3f;
    private float accordageTimer = 0f;
    private bool isLockingInputs = false;
    private float lockTimer = 0f;
    [SerializeField] private float lockDuration = 3f;

    public AudioSource accordageTargetSound;
    public AudioSource accordageInstrument;

    [Header("FX & Feedback")]
    public ParticleSystem accordageParticles;
    public AudioSource accordageAudioSource;

    [Header("Transition de scène")]
    public int sceneSuivante = 3;
    public int secondesAttente = 3;
    public Animator rideauAnimator;

    private bool sequenceFinishedTriggered = false; // Sécurité pour n'appeler la fin qu'une seule fois

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointsCount;
        lineRenderer.numCornerVertices = 2;
        lineRenderer.numCapVertices = 2;

        if (playableDirector != null)
        {
            playableDirector.timeUpdateMode = DirectorUpdateMode.Manual;
            currentTimePosition = playableDirector.time;
        }
    }

    private void Update()
    {
        // GESTION DU LOCK : Si les inputs sont bloqués suite à la réussite
        if (isLockingInputs)
        {
            lockTimer -= Time.deltaTime;
            if (lockTimer <= 0f)
            {
                isLockingInputs = false; // Fin du blocage après 3 secondes
                accorded = true;
            }
        }

        // 1. On accumule la phase de la vague
        wavePhase += Time.deltaTime * speed;

        DrawOrganicLine();

        // 2. Calcul des correspondances et de la logique de verrouillage
        checkLineCorrespondance();

        // 3. Avancement de la timeline (ne se lancera que si accorded == true)
        AdvanceSequence();
    }

    private void DrawOrganicLine()
    {
        for (int i = 0; i < pointsCount; i++)
        {
            float progress = (float)i / (pointsCount - 1);
            float x = progress * length;

            float baseSineY = Mathf.Sin(progress * frequency * Mathf.PI * 2 + wavePhase) * amplitude;

            float noiseSampleX = progress * noiseRoughness;
            float noiseSampleY = Time.time * noiseSpeed;
            float noiseOffset = Mathf.PerlinNoise(noiseSampleX, noiseSampleY) - 0.5f;
            float finalNoiseY = noiseOffset * noiseIntensity;
            float finalY = baseSineY + finalNoiseY;

            lineRenderer.SetPosition(i, new Vector3(x, finalY, 0));
        }
    }

    void checkLineCorrespondance()
    {
        float targetSpeed;
        float targetSequenceSpeed = 0f;

        // --- 1. SI LES INPUTS SONT BLOQUÉS (Post-Accordage immédiat) ---
        if (isLockingInputs)
        {
            targetSpeed = targetLine.targetValue;
            lerpedWheelValue = Mathf.Lerp(lerpedWheelValue, targetSpeed, Time.deltaTime * lerpSpeed);
            frequency = lerpedWheelValue * lineRenderMultiplier;
            speed = targetLine.speed;

            if (isAccording) targetSequenceSpeed = maxSequenceSpeed;
            currentSequenceSpeed = Mathf.Lerp(currentSequenceSpeed, targetSequenceSpeed, Time.deltaTime * easingSpeed);
            return;
        }

        // --- 2. LECTURE NORMALE DES INPUTS ---
        float rawMappedSpeed = arduinoListener.finalWheelSpeed.MapRange(0f, 1f, 0.1f, 1f);
        targetSpeed = rawMappedSpeed;

        // Condition de base pour vouloir faire avancer la séquence
        if (isAccording && rawMappedSpeed <= targetLine.targetSpeedRange.y)
        {
            targetSequenceSpeed = maxSequenceSpeed;
        }

        // --- 3. VÉRIFICATION DE LA ZONE D'ACCORDAGE ---
        if (isAccording && rawMappedSpeed >= targetLine.targetSpeedRange.x && rawMappedSpeed <= targetLine.targetSpeedRange.y)
        {
            targetSpeed = targetLine.targetValue; // Effet aimant graphique

            // Gestion du chrono des 3 secondes si pas encore accordé
            if (!accorded)
            {
                accordageTimer += Time.deltaTime;
                Debug.Log($"Synchronisation en cours... {accordageTimer:F1}s / {timeRequiredToAccord}s");

                if (accordageTimer >= timeRequiredToAccord)
                {
                    AccordageTermine();
                }
            }
        }
        else
        {
            // Si on sort de la range avant d'être accordé, on reset le chrono
            if (!accorded)
            {
                accordageTimer = 0f;
            }

            // Si on n'est pas dans la bonne range, la timeline ne doit pas avancer
            targetSequenceSpeed = 0f;
        }

        // --- 4. APPLICATION DU LISSAGE GLOBALE (Évite la téléportation) ---
        currentSequenceSpeed = Mathf.Lerp(currentSequenceSpeed, targetSequenceSpeed, Time.deltaTime * easingSpeed);
        lerpedWheelValue = Mathf.Lerp(lerpedWheelValue, targetSpeed, Time.deltaTime * lerpSpeed);

        frequency = lerpedWheelValue * lineRenderMultiplier;
        speed = targetLine.speed;
    }

    private void AccordageTermine()
    {
        isLockingInputs = true;
        lockTimer = lockDuration;
        accordageTimer = 0f;

        Debug.Log("★ ACCORDAGE TERMINÉ ! Entrées bloquées, FX activés ★");

        if (accordageParticles != null)
        {
            accordageParticles.Play();
        }

        if (accordageAudioSource != null)
        {
            accordageAudioSource.Play();
        }
    }

    private void AdvanceSequence()
    {
        if (playableDirector == null) return;
        if (accorded == false && isLockingInputs == false) return;

        if (currentSequenceSpeed > 0.001f)
        {
            currentTimePosition += currentSequenceSpeed * Time.deltaTime;

            // Détection de la fin de la Timeline (remplace le Signal d'Unity instable en Manual Mode)
            if (playableDirector.extrapolationMode == DirectorWrapMode.Loop && currentTimePosition > playableDirector.duration)
            {
                currentTimePosition = currentTimePosition % playableDirector.duration;
            }
            else if (currentTimePosition >= playableDirector.duration)
            {
                currentTimePosition = playableDirector.duration;

                if (!sequenceFinishedTriggered)
                {
                    sequenceFinishedTriggered = true;
                    FinDeSequence();
                }
            }

            playableDirector.time = currentTimePosition;
            playableDirector.Evaluate();
        }
    }

    public void FinDeSequence()
    {
        Debug.Log("Fin de la séquence, on ferme les rideaux !");
        rideauAnimator.SetTrigger("close");

        StartCoroutine(waitForRideaux());
    }

    IEnumerator waitForRideaux()
    {
        yield return new WaitForSeconds(secondesAttente);
        SceneManager.LoadScene(sceneSuivante);
    }
}