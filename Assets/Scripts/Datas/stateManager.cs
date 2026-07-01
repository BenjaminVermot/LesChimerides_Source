using System.Collections;
using System.Drawing.Text;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class stateManager : MonoBehaviour
{
    [Header("References")]
    public WheelObject wheelDatas;
    public VentManager ventManager;
    public SoleilManager soleilManager;

    [Header("Particles")]
    public ParticleSystem baseWindParticles;

    [Header("Animators")]
    public Animator rideauAnimator;
    public Animator decorsIntroAnimation;
    public Animator[] animauxAnimators;
    public Animator[] decorsAnimations;

    [Header("Tips")]
    public Animator tipAnimator;
    public SpriteRenderer tipSpriteRenderer;
    public float tipTimeShowing = 7f;

    [Header("GameObjects")]
    public GameObject[] decorsObjects;
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

        handleWalkingAnimations();
    }

    public void nextState()
    {
        if (wheelDatas == null || wheelDatas.statesValues == null)
        {
            return;
        }

        wheelDatas.stateIndex++;
        wheelDatas.currentState = wheelDatas.statesValues[wheelDatas.stateIndex].stateName;
        updateCurrentAnimation();

        if (wheelDatas.currentState == "vent")
        {
            startWind();
            baseWindParticles.Play();

        }
        else
        {
            baseWindParticles.Stop();
        }

        if (wheelDatas.currentState == "soleil")
        {
            startSoleil();
        }

        if (wheelDatas.statesValues[wheelDatas.stateIndex].tip == true)
        {
            StartCoroutine(launchTipAnimation());
        }
    }

    public void startWind()
    {
        if (ventManager != null)
        {
            ventManager.StartGame();
        }
    }

    void startSoleil()
    {
        if (ventManager != null)
        {
            soleilManager.StartGame();
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
                if (wheelDatas.isStill == false && wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("VentWalking");
                }
                if (wheelDatas.isStill == false && !wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("VentWalking");
                }
                if (wheelDatas.isStill == false && ventManager.isProtected == true && wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("Walking");
                }
                if (wheelDatas.isStill == true && ventManager.isProtected == true && wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("Idle");
                }
            }

            //Animations de Soleil
            if (wheelDatas.rideauEstLevé == true && wheelDatas.currentState == "soleil")
            {
                if (wheelDatas.isStill)
                {
                    animator.SetTrigger("Stops");
                }
                if (wheelDatas.isStill == false)
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
        rideauAnimator.SetTrigger("closes");

        Debug.Log("Rideau transition !");

        StartCoroutine(waitForRideaux());
    }

    IEnumerator waitForRideaux()
    {
        yield return new WaitForSeconds(2);

        animaux[wheelDatas.animationIndex].SetActive(true);
        wheelDatas.isInTransition = true;

        decorsObjects[wheelDatas.animationIndex].SetActive(true);


        wheelDatas.animationIndex++;



        //Téléporter le player vers le prochain spawn point
        character.transform.position = new Vector3(
            wheelDatas.nextSpawnPoint.x,
            wheelDatas.nextSpawnPoint.y,
            character.transform.position.z
        );

        //Lancer l'animation d'intro du nouveau spot
        decorsAnimations[wheelDatas.animationIndex].SetFloat("AnimationSpeed", 1);


        yield return new WaitForSeconds(wheelDatas.waitingTime);
        rideauAnimator.SetTrigger("opens");
        wheelDatas.isInTransition = false;
        nextState();

    }

    IEnumerator launchTipAnimation()
    {
        tipSpriteRenderer.sprite = wheelDatas.statesValues[wheelDatas.stateIndex].tipTexture;

        tipAnimator.SetTrigger("Opens");

        yield return new WaitForSeconds(tipTimeShowing);

        tipAnimator.SetTrigger("Closes");

    }
}
