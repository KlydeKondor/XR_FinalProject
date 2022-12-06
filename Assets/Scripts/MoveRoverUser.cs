using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRoverUser : MonoBehaviour
{
    // PUBLIC DATA MEMBERS
    // Public WheelColliders
    public WheelCollider wheelFR;
    public WheelCollider wheelMR;
    public WheelCollider wheelBR;
    public WheelCollider wheelFL;
    public WheelCollider wheelML;
    public WheelCollider wheelBL;

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
    }

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

                // TODO: Toggle steering for middle and back wheels
                // Right = positive steering angle
                if (pair == 0)
                {
                    wheelBL.steerAngle += incrAngle;
                    wheelBR.steerAngle += incrAngle;
                }
                else if (pair == 1)
                {
                    wheelML.steerAngle += incrAngle;
                    wheelMR.steerAngle += incrAngle;
                }
                else
                {
                    wheelFL.steerAngle += incrAngle;
                    wheelFR.steerAngle += incrAngle;
                }
            }
            else
            {
                // Decrease the current angle
                curAngle[pair] = (curAngle[pair] - incrAngle < -maxAngle ? -maxAngle : curAngle[pair] - incrAngle);

                // TODO: Toggle steering for middle and back wheels
                if (pair == 0)
                {
                    wheelBL.steerAngle -= incrAngle;
                    wheelBR.steerAngle -= incrAngle;
                }
                else if (pair == 1)
                {
                    wheelML.steerAngle -= incrAngle;
                    wheelMR.steerAngle -= incrAngle;
                }
                else
                {
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
                wheelBL.steerAngle = curAngle[pair];
                wheelBR.steerAngle = curAngle[pair];
            }
            else if (pair == 1)
            {
                wheelML.steerAngle = curAngle[pair];
                wheelMR.steerAngle = curAngle[pair];
            }
            else
            {
                wheelFL.steerAngle = curAngle[pair];
                wheelFR.steerAngle = curAngle[pair];
            }
        }
    }

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
}
