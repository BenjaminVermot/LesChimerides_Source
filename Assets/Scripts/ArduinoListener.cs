using System;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.InputSystem;

public class ArduinoListener : MonoBehaviour
{

    [Header("Datas")]
    public WheelObject wheelDatas;
    public bool isStill = true;
    private float recievedWheelSpeed = 0;
    public int direction = 0;

    [Header("Lissage de la vitesse")]
    public float finalWheelSpeed = 0;
    public float finalWheelSpeedFullRange = 0;

    [Header("Configuration Clavier (Fallback)")]
    [Tooltip("Vitesse maximale atteinte en simulant au clavier")]
    public float keyboardMaxSpeed = 1.0f;

    private bool isArduinoConnected = true;
    private SerialPort serial;

    private InputAction wheelInputRight;
    private InputAction wheelInputLeft;

    void Awake()
    {
        // Récupération des actions depuis l'Input System
        wheelInputRight = InputSystem.actions.FindAction("WheelRight");
        wheelInputLeft = InputSystem.actions.FindAction("WheelLeft");
    }

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
            isArduinoConnected = true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Impossible d'ouvrir le port série : " + e.Message + " -> Passage en mode Clavier.");
            isArduinoConnected = false;
        }
    }

    void Update()
    {
        // --- 1. OBTENTION DE LA VITESSE CIBLE (Arduino OU Clavier) ---
        if (isArduinoConnected && serial != null && serial.IsOpen)
        {
            ReadArduino();
        }
        else
        {
            ReadKeyboardFallback();
        }

        // --- 2. CALCUL DU LISSAGE (L'inertie s'applique dans les deux modes) ---

        // Lissage de la valeur FullRange (qui gère le signe/la direction)
        finalWheelSpeedFullRange = Mathf.Lerp(finalWheelSpeedFullRange, recievedWheelSpeed, Time.deltaTime * wheelDatas.statesValues[wheelDatas.stateIndex].lerpSpeed);

        // Snap à zéro pour éviter de rester bloqué sur une petite valeur résiduelle.
        if (Mathf.Abs(finalWheelSpeedFullRange) <= 0.01f)
        {
            finalWheelSpeedFullRange = 0f;
            finalWheelSpeed = 0f;
            SetStill(true);
            direction = 0;
        }
        else
        {
            SetStill(false);
            finalWheelSpeedFullRange = (float)Math.Round(finalWheelSpeedFullRange, 4);

            // La vitesse absolue suit la valeur lissée absolue
            finalWheelSpeed = Mathf.Abs(finalWheelSpeedFullRange);
        }

        updateWheelObjectDatas();
    }

    private void ReadArduino()
    {
        try
        {
            string data = serial.ReadLine();
            string[] values = data.Split(',');

            float rawSpeed = Mathf.Abs(float.Parse(values[0].Trim()));
            rawSpeed = Mathf.Clamp01(rawSpeed);
            int rawDirection = Math.Sign(int.Parse(values[1].Trim()));
            int buttonState = Math.Sign(int.Parse(values[2].Trim()));
            wheelDatas.resetButtonState = buttonState;

            if (rawDirection == 0 || rawSpeed <= 0.03f)
            {
                recievedWheelSpeed = 0f;
                direction = 0;
                SetStill(true);
            }
            else
            {
                direction = rawDirection;
                // La vitesse est signée uniquement par la direction.
                recievedWheelSpeed = rawSpeed * direction;
                SetStill(false);
            }



        }
        catch (TimeoutException) { }
        catch (Exception e) { Debug.LogWarning("Erreur de parsing : " + e.Message); }
    }

    private void ReadKeyboardFallback()
    {
        // On vérifie si les touches sont pressées (Input System renvoie 1 si pressé, 0 sinon)
        float rightPressed = wheelInputRight != null ? wheelInputRight.ReadValue<float>() : 0f;
        float leftPressed = wheelInputLeft != null ? wheelInputLeft.ReadValue<float>() : 0f;

        // Calcul de la direction : droite = +1, gauche = -1, les deux ou aucun = 0
        float inputDirection = rightPressed - leftPressed;

        if (Mathf.Abs(inputDirection) > 0.1f)
        {
            // On définit la vitesse cible vers laquelle le Lerp va vouloir aller
            recievedWheelSpeed = inputDirection * keyboardMaxSpeed;
            direction = inputDirection > 0 ? 1 : -1;
        }
        else
        {
            // Si rien n'est pressé, la cible devient 0. Le Lerp dans l'Update fera descendre la vitesse proprement.
            recievedWheelSpeed = 0f;
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

    void updateWheelObjectDatas()
    {
        wheelDatas.finalWheelSpeed = finalWheelSpeed;
        wheelDatas.finalWheelSpeedFullRange = finalWheelSpeedFullRange;
        wheelDatas.wheelDirection = direction;
    }

    void SetStill(bool value)
    {
        if (wheelDatas != null && wheelDatas.isStill != value)
        {
            wheelDatas.isStill = value;
        }
    }
}