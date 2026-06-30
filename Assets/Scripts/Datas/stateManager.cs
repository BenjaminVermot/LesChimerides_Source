using System.Collections;
using System.Drawing.Text;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class stateManager : MonoBehaviour
{
    public WheelObject wheelDatas;
    public VentManager ventManager;
    public Animator[] animauxAnimators;

    public Animator rideauAnimator;

    public Animator decorsIntroAnimation;
    public Animator[] decorsAnimations;

    public GameObject[] animaux;

    public GameObject character;

    [Header("StateStartBools")]
    [SerializeField] private bool windHasStarted = false;

    void Update()
    {
        if (wheelDatas == null)
        {
            return;
        }

        if (wheelDatas.currentState == "vent" && windHasStarted == false)
        {
            startWind();
            windHasStarted = true;
        }

        handleWalkingAnimations();
    }

    public void nextState()
    {
        if (wheelDatas == null || wheelDatas.states == null || wheelDatas.states.Length == 0)
        {
            return;
        }

        if (wheelDatas.stateIndex < wheelDatas.states.Length - 1)
        {
            wheelDatas.stateIndex++;
            wheelDatas.currentState = wheelDatas.states[wheelDatas.stateIndex];
            updateCurrentAnimation();
        }


    }

    public void startWind()
    {
        if (ventManager != null)
        {
            ventManager.StartGame();
        }
    }

    private bool lastIsStill = true;

    void handleWalkingAnimations()
    {
        foreach (Animator animator in animauxAnimators)
        {
            animator.SetFloat("WalkingSpeed", wheelDatas.finalWheelSpeed);
        }

        if (animauxAnimators == null)
        {
            return;
        }

        if (lastIsStill == wheelDatas.isStill)
        {
            return;
        }
        else
        {
            lastIsStill = wheelDatas.isStill;
            updateCurrentAnimation();
        }
    }

    public void updateCurrentAnimation()
    {
        foreach (Animator animator in animauxAnimators)
        {


            if (animator == null)
            {
                continue;
            }


            //ANIMATIONS DE BASE
            if (wheelDatas.rideauEstLevé == true && wheelDatas.currentState != "vent" && wheelDatas.currentState != "soleil")
            {
                resetAnimationsTriggers();

                if (wheelDatas.isStill)
                {
                    animator.SetTrigger("Idle");
                }

                if (wheelDatas.isStill == false)
                {
                    animator.SetTrigger("Walking");
                }
            }

            //ANIMATIONS DE VENT
            if (wheelDatas.rideauEstLevé == true && wheelDatas.currentState == "vent")
            {
                resetAnimationsTriggers();

                if (wheelDatas.isStill && wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("VentIdle");
                }

                if (wheelDatas.isStill && !wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("Idle");
                }

                if (wheelDatas.isStill == false && wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("VentWalking");
                }
                if (wheelDatas.isStill == false && wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("Walking");
                }
            }
        }
    }

    void resetAnimationsTriggers()
    {
        foreach (Animator animator in animauxAnimators)
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Walking");
            animator.ResetTrigger("VentIdle");
            animator.ResetTrigger("VentWalking");
        }
    }

    public void launchRideauTransition()
    {
        rideauAnimator.SetTrigger("on");

        Debug.Log("Rideau transition !");

        StartCoroutine(waitForRideaux());
    }

    IEnumerator waitForRideaux()
    {
        yield return new WaitForSeconds(2);

        animaux[wheelDatas.animationIndex].SetActive(true);
        wheelDatas.isInTransition = true;

        //Téléporter le player vers le prochain spawn point
        character.transform.position = new Vector3(
            wheelDatas.nextSpawnPoint.x,
            wheelDatas.nextSpawnPoint.y,
            character.transform.position.z
        );

        //Lancer l'animation d'intro du nouveau spot
        decorsAnimations[wheelDatas.animationIndex].SetFloat("AnimationSpeed", 1);


        yield return new WaitForSeconds(wheelDatas.waitingTime);
        rideauAnimator.SetTrigger("off");
        wheelDatas.isInTransition = false;
        nextState();

    }
}
