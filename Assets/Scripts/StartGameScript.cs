using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameScript : MonoBehaviour
{

    [SerializeField] private float startProgress;
    [SerializeField] private float launchGameTreshold = 50;
    public ArduinoListener arduinoListener;

    private bool gameHasStarted = false;

    // Update is called once per frame
    void Update()
    {
        startProgress += arduinoListener.finalWheelSpeedFullRange;
        if (startProgress >= launchGameTreshold)
        {
            startGame();
        }

    }

    void startGame()
    {
        if (gameHasStarted == false)
        {
            gameHasStarted = true;

            //Lancer la game
            Debug.Log("START GAME !! YOUHOUUU");
            SceneManager.LoadScene(1);
        }
    }
}
