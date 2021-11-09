using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class XR_ButtonHoldTimer : MonoBehaviour
{
    bool[] bButtonIsPressed;
    bool[] bButtonWasPressed;

    List<InputDevice> gameControllers;
    InputDevice controllerLeft;
    InputDevice controllerRight;

    [SerializeField] XRHandType handType = XRHandType.Any;
    [SerializeField] InputHelpers.Button buttonToCheck = InputHelpers.Button.None;

    //[SerializeField] UnityEvent OnButtonPress = null;
    //[SerializeField] UnityEvent OnButtonRelease = null;
    [SerializeField] UnityEvent OnTimerEnded = null;
    [SerializeField] bool b_resetTimerOnRelease = false;
    [SerializeField] float f_timeNeeded = 2.0f;
    float[] f_timers;    

    bool bIsInitialized = false;
    bool b_hasTimerFinished = false;

    public float Percent
    {
        get
        {
            return 1.0f - (Mathf.Min(f_timers[0], f_timers[1]) / f_timeNeeded);
        }
    }

    //This could potentially cause issues if a controller is disconnected or reconnected mid-gameplay.
    void Start()
    {
        Invoke("Initialize", 0.25f);    //this avoids any issues with script execution order
    }

    void Initialize()
    {
        //gameControllers = new List<InputDevice>();
        //InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, gameControllers);

        controllerLeft = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        controllerRight = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        gameControllers = new List<InputDevice>();
        gameControllers.Add(controllerLeft);
        gameControllers.Add(controllerRight);
        bButtonIsPressed = new bool[gameControllers.Count];
        bButtonWasPressed = new bool[gameControllers.Count];
        f_timers = new float[2];
        f_timers[0] = f_timeNeeded;
        f_timers[1] = f_timeNeeded;

        bIsInitialized = true;
    }

    //This just feels like a bad way of doing this.
    //There must be a simpler more efficient way.
    void Update()
    {
        if (!bIsInitialized)
        {
            return;
        }

        if (b_hasTimerFinished)
        {
            enabled = false;
            return;
        }

        if (handType == XRHandType.Left)
        {
            CheckInput(0);
        }
        else if (handType == XRHandType.Right)
        {
            CheckInput(1);
        }
        else
        {
            CheckInput(0);
            CheckInput(1);
        }
    }

    void CheckInput(int i)
    {
        bButtonWasPressed[i] = bButtonIsPressed[i];

        if (gameControllers[i].IsPressed(buttonToCheck, out bButtonIsPressed[i]))
        {
            if (bButtonIsPressed[i])
            {
                f_timers[i] -= Time.deltaTime;
                if (f_timers[i] <= 0)
                {
                    b_hasTimerFinished = true;
                    OnTimerEnded.Invoke();
                }
            }

            if ((bButtonWasPressed[i] && !bButtonIsPressed[i]) && b_resetTimerOnRelease)
            {
                f_timers[i] = f_timeNeeded;
            }
        }
    }
}
