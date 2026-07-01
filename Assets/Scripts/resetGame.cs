using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class resetGame : MonoBehaviour
{
    public WheelObject wheelDatas;

    [SerializeField] private float holdDuration = 2f;
    private float holdTimer;
    public bool resetHeldForTwoSeconds;

    public Animator fadeAnimator;

    // Update is called once per frame
    void Update()
    {
        if (wheelDatas == null)
        {
            return;
        }

        if (wheelDatas.resetButtonState == 1)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration && resetHeldForTwoSeconds == false)
            {
                resetHeldForTwoSeconds = true;
                Debug.Log("resetButtonState est resté à 1 pendant 2 secondes");
                fadeAnimator.SetTrigger("FadeIn");
                StartCoroutine(waitBeforeRestart());
            }
        }
        else
        {
            holdTimer = 0f;
            resetHeldForTwoSeconds = false;
        }
    }

    IEnumerator waitBeforeRestart()
    {
        yield return new WaitForSeconds(2);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
