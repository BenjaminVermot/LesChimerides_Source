using System.Collections;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class VentManager : MonoBehaviour
{
    public GameObject character;
    public WheelObject wheelDatas;

    public stateManager stateManager;
    public bool isProtected = false;


    [Header("Wind values")]
    private float windWaveElapsedTime = 0f;
    public float windForce = 20f;
    public int intervalBetweenWindWaves = 8;
    public int windDuration = 4;


    [Header("Feedbacks")]
    public ParticleSystem windParticles;

    [Header("Animations")]
    public Animator characterAnimator;


    public void StartGame()
    {
        StartCoroutine(waitForNextWave());
        Debug.Log("VENT STARTED");
    }

    // Update is called once per frame
    void Update()
    {
        if (wheelDatas.ventIsHere == false)
        {
            return;
        }

        windWaveElapsedTime += Time.deltaTime;

        float normalizedTime = Mathf.Clamp01(windWaveElapsedTime / Mathf.Max(0.01f, windDuration));
        float windEnvelope = Mathf.Sin(normalizedTime * Mathf.PI);

        if (isProtected == false)
        {
            character.transform.position += Vector3.left * (windForce * windEnvelope * Time.deltaTime);
        }
    }

    IEnumerator waitForNextWave()
    {
        yield return new WaitForSeconds(intervalBetweenWindWaves);
        if (wheelDatas.currentState == "vent")
        {
            launchWind();
        }
        else
        {
            this.enabled = false;
        }
    }

    void launchWind()
    {
        //Jouer un son 
        if (windParticles != null)
        {
            windParticles.Play();
        }

        windWaveElapsedTime = 0f;
        wheelDatas.ventIsHere = true;

        stateManager.updateCurrentAnimation();
        Debug.Log("Lancer le vent");


        StartCoroutine(waitBeforeEndWindWave());
    }

    IEnumerator waitBeforeEndWindWave()
    {

        yield return new WaitForSeconds(windDuration);

        //éteindre le son
        if (windParticles != null)
        {
            windParticles.Stop();
        }

        wheelDatas.ventIsHere = false;
        stateManager.updateCurrentAnimation();
        StartCoroutine(waitForNextWave());

    }


}
