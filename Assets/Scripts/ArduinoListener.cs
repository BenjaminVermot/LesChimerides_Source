using System;
using UnityEngine;
using System.IO.Ports;

public class ArduinoListener : MonoBehaviour
{
    public bool isStill = true;
    private float recievedWheelSpeed = 0;
    public int direction = 0;
    public int buttonState = 0;

    [Header("Lissage de la vitesse")]
    public float finalWheelSpeed = 0;
    public float finalWheelSpeedFullRange = 0;
    public float lerpSpeed = 50f; // Baissé un peu pour un effet d'inertie plus sympa sur l'orgue

    private SerialPort serial;

    void Start()
    {
        serial = new SerialPort("/dev/cu.usbmodem1101", 115200);
        serial.ReadTimeout = 10;

        try
        {
            serial.Open();
            serial.DtrEnable = true;
            serial.RtsEnable = true;
            Debug.Log("Port série ouvert avec succès !");
        }
        catch (Exception e)
        {
            Debug.LogError("Impossible d'ouvrir le port série : " + e.Message);
        }
    }

    void Update()
    {
        // 1. RÉCEPTION DES DONNÉES (Se fait dès que l'Arduino envoie quelque chose)
        if (serial != null && serial.IsOpen)
        {
            try
            {
                string data = serial.ReadLine();
                string[] values = data.Split(',');

                // CORRECTION : Tu as 3 éléments maintenant (vitesse, direction, bouton)


                float rawSpeed = float.Parse(values[0].Trim());

                // Gestion de la zone morte directement à la réception
                if (Mathf.Abs(rawSpeed) <= 0.03f)
                {
                    recievedWheelSpeed = 0f;
                    direction = 0;
                    isStill = true;
                }
                else
                {
                    recievedWheelSpeed = rawSpeed; // On s'assure que la vitesse est positive
                    direction = int.Parse(values[1].Trim());
                    isStill = false;
                }

                buttonState = int.Parse(values[2].Trim());

            }
            catch (TimeoutException) { }
            catch (Exception e) { Debug.LogWarning("Erreur de parsing : " + e.Message); }
        }

        // Debug.Log("Recieved wheel value: " + recievedWheelSpeed);
        finalWheelSpeedFullRange = Mathf.Lerp(finalWheelSpeedFullRange, recievedWheelSpeed, Time.deltaTime * lerpSpeed);
        finalWheelSpeed = Math.Abs(finalWheelSpeedFullRange);
        finalWheelSpeed = Mathf.Lerp(finalWheelSpeed, recievedWheelSpeed, Time.deltaTime * lerpSpeed);

        finalWheelSpeedFullRange = (float)Math.Round(finalWheelSpeedFullRange, 4);


        // CORRECTION DE L'ARRONDI : Si on est extrêmement proche de 0, on force le 0 proprement 
        // pour éviter que le Lerp et l'arrondi se battent indéfiniment.
        if (Math.Abs(finalWheelSpeed) <= 0.005f)
        {
            finalWheelSpeed = 0f;
        }
        else
        {
            // Arrondi à 2 chiffres après la virgule uniquement si on n'est pas à 0
            finalWheelSpeed = (float)Math.Round(finalWheelSpeed, 4);
        }

        // Debug de contrôle
        if (!isStill)
        {
            // Debug.Log($"[Orgue] Vitesse lissée: {finalWheelSpeed} | Direction: {direction} | Bouton: {buttonState}");
        }
    }

    void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen)
        {
            serial.Close();
            Debug.Log("Port série fermé proprement.");
        }
    }
}