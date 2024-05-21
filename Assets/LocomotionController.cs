using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController : MonoBehaviour
{
    public XRController leftController;
    public XRController rightController;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;

    public bool EnableLeftTeleport { get; set; } = true;
    public bool EnableRightTeleport { get; set; } = true;

    void Update()
    {
        if (leftController)
        {
            leftController.gameObject.SetActive(EnableLeftTeleport && CheckIfActivated(leftController));
        }
        if (rightController)
        {
            rightController.gameObject.SetActive(EnableRightTeleport && CheckIfActivated(rightController));
        }
    }

    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    }
}
