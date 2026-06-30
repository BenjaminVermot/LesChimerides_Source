using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public ArduinoListener arduinoListener;

    [Header("Contrôle de la Timeline")]
    public PlayableDirector sequence;
    [SerializeField] private float maxSequenceSpeed = 1f;
    [SerializeField] private float easingSpeed = 2f;
    private float currentSequenceSpeed = 0f;
    private double currentTimePosition = 0f;

    [Header("Rideaux")]
    public Transform rideaux;

    public float lerpingSpeed = 3;
    public float attenuation = 3;
    public float rideauxMaxPosition = 10f;

    [Header("UI")]
    public UIDocument uiDocument;
    private VisualElement root;
    private VisualElement mainTitleImg;

    float newPosition;
    private float lerpedYPosition;

    private bool gameHasStarted = false;
    private bool sequenceHasEnded = false;

    private float originalRideauPos;

    void Start()
    {
        root = uiDocument.rootVisualElement;
        mainTitleImg = root.Q<VisualElement>("MainTitle");

        originalRideauPos = rideaux.position.y;

        if (sequence != null)
        {
            sequence.timeUpdateMode = DirectorUpdateMode.Manual;
            sequence.Stop();
            currentTimePosition = 0.0;
            sequence.time = 0.0;
            sequence.Evaluate();
        }
    }

    void Update()
    {
        // --- GESTION DES RIDEAUX ---
        if (arduinoListener.finalWheelSpeedFullRange >= 0.05f || arduinoListener.finalWheelSpeedFullRange <= -0.05f)
        {
            newPosition = arduinoListener.finalWheelSpeedFullRange / attenuation;
        }
        else
        {
            newPosition = 0;
        }

        if (sequenceHasEnded == true && gameHasStarted == false)
        {
            lerpedYPosition = Mathf.Lerp(lerpedYPosition, originalRideauPos, Time.deltaTime * lerpingSpeed);
        }
        else
        {
            lerpedYPosition = Mathf.Lerp(lerpedYPosition, rideaux.position.y + newPosition, Time.deltaTime * lerpingSpeed);
        }

        if (rideaux.position.y >= rideauxMaxPosition)
        {
            lancerJeu();
        }

        // --- GESTION DE LA VITESSE DE LA TIMELINE ---
        if (gameHasStarted)
        {
            lerpedYPosition = Mathf.Lerp(lerpedYPosition, rideauxMaxPosition, Time.deltaTime * lerpingSpeed);
            currentSequenceSpeed = Mathf.Lerp(currentSequenceSpeed, arduinoListener.finalWheelSpeed, Time.deltaTime * easingSpeed);
        }
        else
        {
            currentSequenceSpeed = Mathf.Lerp(currentSequenceSpeed, 0, Time.deltaTime * easingSpeed);
        }

        // On n'avance la séquence que si elle n'est pas déjà terminée
        if (!sequenceHasEnded)
        {
            AdvanceSequence();
        }

        rideaux.position = new Vector2(rideaux.position.x, lerpedYPosition);
    }

    void lancerJeu()
    {
        if (gameHasStarted) return;

        Debug.Log("Game Started !!");
        mainTitleImg.AddToClassList("animated-image--fadeOut");
        gameHasStarted = true;
    }

    private void AdvanceSequence()
    {
        if (sequence == null || !gameHasStarted) return;

        if (currentSequenceSpeed > 0.001f)
        {
            // Limiter la vitesse max si nécessaire avec maxSequenceSpeed
            float speed = Mathf.Min(currentSequenceSpeed, maxSequenceSpeed);
            currentTimePosition += speed * Time.deltaTime;

            // --- DÉTECTION DE LA FIN DE LA SÉQUENCE ---
            if (currentTimePosition >= sequence.duration)
            {
                currentTimePosition = sequence.duration; // On cale à la fin exacte
                sequence.time = currentTimePosition;
                sequence.Evaluate();

                // On déclenche la fin immédiatement ici
                sequenceHasEnded = true;
                endSequence();
                return;
            }

            sequence.time = currentTimePosition;
            sequence.Evaluate();
        }
    }

    void endSequence()
    {
        Debug.Log("End sequence, start coroutine");
        StartCoroutine(waitForRideaux());
    }

    IEnumerator waitForRideaux()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(1);
    }
}