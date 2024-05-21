using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using PDollarGestureRecognizer;
using Unity.XR.CoreUtils;

public class MovementRecognizer : MonoBehaviour
{
    public XRNode inputSource;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;
    public Transform movementSource;
    public Camera cam;

    public float newPositionThresholdDistance = 0.05f;
    public GameObject debugCubePrefab;
    public bool creationMode = true;
    public string newGestureName;

    public float recognitionThreshold = 0.9f;

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent onRecognized;

    private List<Gesture> trainingSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionList = new List<Vector3>();
    private int movingPoints = 0;
    [SerializeField]
    private int pointsToMove = 15;


    public float speed = 1;

    public float gravity = -9.81f;
    public LayerMask groundLayer;
    public float additionalHeight = 0.2f;
    private float fallingSpeed;
    private XROrigin rig;
    private CharacterController character;


    void Start()
    {
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (var item in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();

        //Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);
        //Start the Movement
        if (!isMoving && isPressed )
        {
            StartMovemenet();
        }
        else if (isMoving && isPressed && movingPoints < pointsToMove)
        {
            UpdateMovemnet();
        }
        else if (isPressed)
        {
            EndMovement();
        }
    }

    void StartMovemenet()
    {
        isMoving = true;
        positionList.Clear();
        positionList.Add(movementSource.position);
        movingPoints++;
        if (debugCubePrefab)
        {
            Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
        }
    }

    public void EndMovement()
    {
        isMoving = false;
        movingPoints = 0;

        Point[] pointArray = new Point[positionList.Count];
        for (int i = 0; i < positionList.Count; i++)
        {
            Vector2 screenPoints = cam.WorldToScreenPoint(positionList[i]);
            pointArray[i] = new Point(screenPoints.x, screenPoints.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);
        //Agrega un nuevo gesto a trainginSet
        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log(result.GestureClass + result.Score);
            if (result.GestureClass.ToLower() == "walk" && inputButton.ToString() == "PrimaryButton")
            {
                Debug.Log("Caminar");
                characterMovement();
            }
            if (result.GestureClass.ToLower() == "lamp" && inputButton.ToString() == "SecondaryButton")
            {
                Debug.Log("LUMUS");
                if (GameManager.timeFlashLight < 100)
                {
                    GameManager.timeFlashLight += 5;
                }
                if (GameManager.timeFlashLight >= 100)
                {
                    GameManager.timeFlashLight = 100;
                }
            }
        }
        Debug.Log(inputButton);
    }

    void UpdateMovemnet()
    {
        Vector3 lastPosition = positionList[positionList.Count - 1];

        if (Vector3.Distance(movementSource.position, lastPosition) > newPositionThresholdDistance)
        {
            positionList.Add(movementSource.position);
            movingPoints++;
            if (debugCubePrefab)
            {
                Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
            }
        }

    }
    public string getNameGesture()
    {
        return newGestureName;
    }

    private void characterMovement()
    {
        CapsuleFollowHeadset();
        Vector3 direction = rig.Camera.transform.forward;
        Debug.Log(direction);

        character.Move(direction * speed);

        bool isGrounded = CheckifGrounded();

        if (isGrounded)
        {
            fallingSpeed = 0;
        }
        else
        {
            fallingSpeed += gravity;
        }
        character.Move(Vector3.up * fallingSpeed);
    }

    void CapsuleFollowHeadset()
    {
        character.height = rig.CameraInOriginSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.Camera.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2 + character.skinWidth, capsuleCenter.z);
    }

    bool CheckifGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLenght = character.center.y + 0.1f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLenght, groundLayer);
        return hasHit;
    }
}




