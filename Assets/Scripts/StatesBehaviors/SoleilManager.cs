using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SoleilManager : MonoBehaviour
{
    public GameObject boss;
    public WheelObject wheelDatas;
    public stateManager stateManager;
    public Animator soleilAnimator;
    private float randomLookingTime;
    private float randomCountingTime;

    public GameObject character;

    public Animator fadeAnimator;
    public float timeBeforeReset;

    [SerializeField] private float extraTimeToStop = 2f;

    [SerializeField] bool isLooking = false;


    // void Start()
    // {
    //     StartCoroutine(waitForNextCounting());
    // }

    public void StartGame()
    {

        StartCoroutine(waitForNextCounting());
        boss.SetActive(true);

        Debug.Log("Start 1,2,3 soleil !!");
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
        randomLookingTime = getRandomNumber(2f, 3.5f);
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


        //Fade au noir
        StopAllCoroutines();
        isLooking = false;
        fadeAnimator.SetTrigger("FadeIn");
        StartCoroutine(waitForGameReset());


    }

    IEnumerator waitForGameReset()
    {
        yield return new WaitForSeconds(timeBeforeReset);
        soleilAnimator.SetTrigger("reset");
        isLooking = false;

        character.transform.position = new Vector3(
         wheelDatas.nextSpawnPoint.x,
         wheelDatas.nextSpawnPoint.y,
         character.transform.position.z
     );





        yield return new WaitForSeconds(2);



        fadeAnimator.SetTrigger("FadeOut");
        StartCoroutine(waitForNextCounting());


    }
}
