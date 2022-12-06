using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRoverUser : MonoBehaviour
{
    // PUBLIC DATA MEMBERS
    // Public GameObject for outermost SEV object
    public GameObject sev;
    
    // Public WheelColliders
    public WheelCollider wheelFR;
    public WheelCollider wheelMR;
    public WheelCollider wheelBR;
    public WheelCollider wheelFL;
    public WheelCollider wheelML;
    public WheelCollider wheelBL;

    // Public meshes
    public GameObject wheelMeshFR_In;
    public GameObject wheelMeshFR_Out;
    public GameObject wheelMeshMR_In;
    public GameObject wheelMeshMR_Out;
    public GameObject wheelMeshBR_In;
    public GameObject wheelMeshBR_Out;
    public GameObject wheelMeshFL_In;
    public GameObject wheelMeshFL_Out;
    public GameObject wheelMeshML_In;
    public GameObject wheelMeshML_Out;
    public GameObject wheelMeshBL_In;
    public GameObject wheelMeshBL_Out;

    // Public speed variables
    public float moveSpeed;
    public float rotSpeed;

    // PRIVATE DATA MEMBERS
    private float[] curAngle = new float[3];
    private float maxAngle = 45f;
    private const int MAX_GEAR = 5;
    private const int MIN_GEAR = 1;
    private int gear = 1;
    private float[] torqueArray = new float[MAX_GEAR];
    private bool[] wheelArray = new bool[3];

    // Start is called before the first frame update
    void Start()
    {
        // Set the outer SEV object's transform to be a child of this transform
        sev.transform.parent = this.transform;
        sev.GetComponent<BoxCollider>().transform.parent = this.transform;
        
        // Set the torque for each gear
        float scale = 1f;
        for (int i = 0; i < torqueArray.Length; i++)
        {
            // Set this torque according to moveSpeed and scale
            torqueArray[i] = scale * moveSpeed;
            scale += 0.5f;
        }

        // Set whether each wheel can turn
        wheelArray[0] = false; // Back
        wheelArray[1] = false; // Middle
        wheelArray[2] = true; // Front

        // Set the elements of curAngle to zero
        curAngle[0] = 0;
        curAngle[1] = 0;
        curAngle[2] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float dT = Time.deltaTime;
        
        // Check whether the rover should move forward or backward
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            // Determine if the SEV should move forward or back
            if (Input.GetKey(KeyCode.UpArrow))
            {
                // Move the rover forward
                MoveRover(dT);
            }
            
            if (Input.GetKey(KeyCode.DownArrow))
            {
                // Move the rover backward
                MoveRover(dT, false);
            }
        }
        else
        {
            // Set torque to zero and coast
            MoveRover(dT, false, true);
        }

        // Check which pairs of wheels should be turning
        for (int i = 0; i < wheelArray.Length; i++)
        {
            // If true, turn
            if (wheelArray[i])
            {
                // Apply rotational component
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                {
                    // Not neutral; figure out if right or left
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        TurnRover(dT, i);
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        TurnRover(dT, i, false);
                    }
                }
                else
                {
                    // Neutral; return wheels to a zero angle
                    TurnRover(dT, i, false, true);
                }
            }
            else
            {
                // Neutral; return wheels to a zero angle
                TurnRover(dT, i, false, true);
            }
        }

        // Apply brakes
        if (Input.GetKey(KeyCode.B))
        {
            ApplyBrakes(dT);
        }
        else
        {
            ApplyBrakes(dT, true);
        }

        // Grab or drop a gear
        if (Input.GetKeyDown(KeyCode.G) && gear < MAX_GEAR)
        {
            gear++;
            Debug.Log("Up to " + gear.ToString());

        }
        else if (Input.GetKeyDown(KeyCode.D) && gear > MIN_GEAR)
        {
            gear--;
            Debug.Log("Down to " + gear.ToString());
        }

        // Change which pairs of wheels turn
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Front and middle
            wheelArray[0] = false;
            wheelArray[1] = true;
            wheelArray[2] = true;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            // Back and middle
            wheelArray[0] = true;
            wheelArray[1] = true;
            wheelArray[2] = false;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            // Front
            wheelArray[0] = false;
            wheelArray[1] = false;
            wheelArray[2] = true;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            // Back
            wheelArray[0] = true;
            wheelArray[1] = false;
            wheelArray[2] = false;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            // All
            wheelArray[0] = true;
            wheelArray[1] = true;
            wheelArray[2] = true;
        }

        // Update the wheel meshes
        //UpdateMeshes(dT);
    }

    // Move the SEV forward or backward, or turn the engine off
    private void MoveRover(float dT, bool isFwd = true, bool isNeut = false)
    {
        float torque = torqueArray[gear - 1] * dT;

        // Apply torque according to input
        if (isFwd)
        {
            // Set to maximum positive torque (forwards)
            wheelFR.motorTorque = torque;
            wheelMR.motorTorque = torque;
            wheelBR.motorTorque = torque;
            wheelFL.motorTorque = torque;
            wheelML.motorTorque = torque;
            wheelBL.motorTorque = torque;
        }
        else if (!isNeut)
        {
            // Set to maximum negative torque (reverse)
            wheelFR.motorTorque = -torque;
            wheelMR.motorTorque = -torque;
            wheelBR.motorTorque = -torque;
            wheelFL.motorTorque = -torque;
            wheelML.motorTorque = -torque;
            wheelBL.motorTorque = -torque;
        }
        else
        {
            // Set to zero torque (coasting)
            wheelFR.motorTorque = 0;
            wheelMR.motorTorque = 0;
            wheelBR.motorTorque = 0;
            wheelFL.motorTorque = 0;
            wheelML.motorTorque = 0;
            wheelBL.motorTorque = 0;
        }
    }

    // Turn a pair of wheels incrementally
    private void TurnRover(float dT, int pair, bool isRt = true, bool isNeut = false)
    {
        float incrAngle = rotSpeed * dT;

        // Check if the user is turning left or right, or returning to neutral
        if (!isNeut && Mathf.Abs(curAngle[pair]) < maxAngle)
        {
            // Not neutral; check if right or left
            if (isRt)
            {
                // Increase the current angle
                curAngle[pair] = (curAngle[pair] + incrAngle > maxAngle ? maxAngle : curAngle[pair] + incrAngle);

                // Right = positive steering angle
                if (pair == 0)
                {
                    // Back
                    wheelBL.steerAngle += incrAngle;
                    wheelBR.steerAngle += incrAngle;
                }
                else if (pair == 1)
                {
                    // Middle
                    wheelML.steerAngle += incrAngle;
                    wheelMR.steerAngle += incrAngle;
                }
                else
                {
                    // Front
                    wheelFL.steerAngle += incrAngle;
                    wheelFR.steerAngle += incrAngle;
                }
            }
            else
            {
                // Decrease the current angle
                curAngle[pair] = (curAngle[pair] - incrAngle < -maxAngle ? -maxAngle : curAngle[pair] - incrAngle);

                // Check which pair is being turned
                if (pair == 0)
                {
                    // Back
                    wheelBL.steerAngle -= incrAngle;
                    wheelBR.steerAngle -= incrAngle;
                }
                else if (pair == 1)
                {
                    // Middle
                    wheelML.steerAngle -= incrAngle;
                    wheelMR.steerAngle -= incrAngle;
                }
                else
                {
                    // Front
                    wheelFL.steerAngle -= incrAngle;
                    wheelFR.steerAngle -= incrAngle;
                }
            }
        }
        else if (isNeut && Mathf.Abs(curAngle[pair]) > 0)
        {
            // Decrease the current angle if positive; increase if negative
            if (curAngle[pair] > 0)
            {
                curAngle[pair] = (curAngle[pair] - incrAngle < 0 ? 0 : curAngle[pair] - incrAngle);
            }
            else
            {
                curAngle[pair] = (curAngle[pair] + incrAngle > 0 ? 0 : curAngle[pair] + incrAngle);
            }

            // Update the steering angle with the curAngle for this pair of wheels
            if (pair == 0)
            {
                // Back
                wheelBL.steerAngle = curAngle[pair];
                wheelBR.steerAngle = curAngle[pair];
            }
            else if (pair == 1)
            {
                // Middle
                wheelML.steerAngle = curAngle[pair];
                wheelMR.steerAngle = curAngle[pair];
            }
            else
            {
                // Front
                wheelFL.steerAngle = curAngle[pair];
                wheelFR.steerAngle = curAngle[pair];
            }
        }
    }

    // Stop the SEV's forward or backward movement
    private void ApplyBrakes(float dT, bool releaseBrakes = false)
    {
        float torque = torqueArray[gear - 1] * dT;

        // Apply or release the brakes
        if (!releaseBrakes)
        {
            // Apply, equal to the current torque
            wheelFR.brakeTorque = torque;
            wheelMR.brakeTorque = torque;
            wheelBR.brakeTorque = torque;
            wheelFL.brakeTorque = torque;
            wheelML.brakeTorque = torque;
            wheelBL.brakeTorque = torque;
        }
        else
        {
            // Disengage brakes
            wheelFR.brakeTorque = 0;
            wheelMR.brakeTorque = 0;
            wheelBR.brakeTorque = 0;
            wheelFL.brakeTorque = 0;
            wheelML.brakeTorque = 0;
            wheelBL.brakeTorque = 0;
        }
    }

    void UpdateMeshes(float dT)
    {
        // Spin the wheels (https://forum.unity.com/threads/simple-way-of-getting-wheel-mesh-to-rotate-with-wheel-colliders.480316/)
        // RPM / 60 = Revolutions per second; RPS * 360 = Degrees per second; DPS * dT = Degrees at a given frame
        wheelMeshFL_In.transform.Rotate(0, wheelFL.rpm * 6 * dT, 0);
        wheelMeshFL_Out.transform.Rotate(0, wheelFL.rpm * 6 * dT, 0);
        wheelMeshFR_In.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);
        wheelMeshFR_Out.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);
        wheelMeshML_In.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);
        wheelMeshML_Out.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);
        wheelMeshMR_In.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);
        wheelMeshMR_Out.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);
        wheelMeshBL_In.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);
        wheelMeshBL_Out.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);
        wheelMeshBR_In.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);
        wheelMeshBR_Out.transform.Rotate(0, wheelFR.rpm * 6 * dT, 0);


        // Update the azimuth of the wheels
        /*
        wheelMeshFL_In.transform.rotation.Set() = wheelFL.transform.rotation;
        wheelMeshFL_Out.transform.rotation = wheelFL.transform.rotation;
        wheelMeshFR_In.transform.rotation = wheelFR.transform.rotation;
        wheelMeshFR_Out.transform.rotation = wheelFR.transform.rotation;
        wheelMeshML_In.transform.rotation = wheelML.transform.rotation;
        wheelMeshML_Out.transform.rotation = wheelML.transform.rotation;
        wheelMeshMR_In.transform.rotation = wheelMR.transform.rotation;
        wheelMeshMR_Out.transform.rotation = wheelMR.transform.rotation;
        wheelMeshBL_In.transform.rotation = wheelBL.transform.rotation;
        wheelMeshBL_Out.transform.rotation = wheelBL.transform.rotation;
        wheelMeshBR_In.transform.rotation = wheelBR.transform.rotation;
        wheelMeshBR_Out.transform.rotation = wheelBR.transform.rotation;
        */
    }
}
