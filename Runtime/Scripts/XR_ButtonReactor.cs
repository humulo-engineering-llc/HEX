using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public enum XRHandType
{
    Left,
    Right,
    Any
}


public class XR_ButtonReactor : MonoBehaviour
{
    bool[] bButtonIsPressed;
    bool[] bButtonWasPressed;

    List<InputDevice> gameControllers;
    InputDevice controllerLeft;
    InputDevice controllerRight;

    [SerializeField] XRHandType handType = XRHandType.Any;
    [SerializeField] InputHelpers.Button buttonToCheck = InputHelpers.Button.None;

    [SerializeField] UnityEvent OnButtonPress = null;
    [SerializeField] UnityEvent OnButtonRelease = null;

    bool bIsInitialized = false;

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
            //Debug.Log("Button check: " + bButtonIsPressed[i]);

            if (!bButtonWasPressed[i] && bButtonIsPressed[i])
            {
                //Debug.Log("Button pressed");
                OnButtonPress.Invoke();
            }

            if (bButtonWasPressed[i] && !bButtonIsPressed[i])
            {
                //Debug.Log("Button released");
                OnButtonRelease.Invoke();
            }
        }
    }
}
