using System.Collections;
using UnityEngine;

public class SoleilManager : MonoBehaviour
{
    public WheelObject wheelDatas;
    public stateManager stateManager;
    public Animator soleilAnimator;
    private float randomLookingTime;
    private float randomCountingTime;

    [SerializeField] private float extraTimeToStop = 2f;

    [SerializeField] bool isLooking = false;


    void StartGame()
    {
        startBossIntroAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if (wheelDatas.finalWheelSpeed >= 0.05f && isLooking)
        {
            //GETS CAUGHT !!
            gotCaught();
        }
    }


    void looks()
    {
        //lancer l'animation de regard
        soleilAnimator.SetTrigger("looks");
        StartCoroutine(extraTime());
    }

    IEnumerator extraTime()
    {
        yield return new WaitForSeconds(extraTimeToStop);
        StartCoroutine(waitForNextCounting());
        isLooking = true;
    }

    IEnumerator waitForNextCounting()
    {
        randomLookingTime = getRandomNumber(4f, 7f);
        yield return new WaitForSeconds(randomLookingTime);

        soleilAnimator.SetTrigger("counts");
        isLooking = false;
        StartCoroutine(waitForNextLooking());

    }

    IEnumerator waitForNextLooking()
    {
        randomCountingTime = getRandomNumber(2f, 5f);
        yield return new WaitForSeconds(randomCountingTime);

        looks();

    }

    float getRandomNumber(float min, float max)
    {
        return Random.Range(min, max);
    }

    void startBossIntroAnimation()
    {

    }

    void gotCaught()
    {
        //faire fondu au noir, et après 2 secondes respawn au dernier point 
        //Reset le temps d'attente du boss
        //? maybe rejouer l'animation d'intro
    }
}
