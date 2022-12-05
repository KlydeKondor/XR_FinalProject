using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRoverUser : MonoBehaviour
{
    // Public WheelColliders
    public WheelCollider wheelFR;
    public WheelCollider wheelMR;
    public WheelCollider wheelBR;
    public WheelCollider wheelFL;
    public WheelCollider wheelML;
    public WheelCollider wheelBL;

    public float moveSpeed;
    public float rotSpeed;

    private float curAngle = 0f;
    private float maxAngle = 45f;

    // Start is called before the first frame update
    void Start()
    {
        wheelFR.steerAngle = 0.45f;
        wheelMR.steerAngle = 0.45f;
        wheelBR.steerAngle = 0.45f;
        wheelFL.steerAngle = 0.45f;
        wheelML.steerAngle = 0.45f;
        wheelBL.steerAngle = 0.45f;
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
            MoveRover(dT, false, true);
        }

        // Apply rotational component
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            //this.transform.Rotate(0, -rotSpeed * dT, 0);
            if (Input.GetKey(KeyCode.RightArrow))
            {
                TurnRover(dT);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                TurnRover(dT, false);
            }
        }
        else
        {
            TurnRover(dT, false, true);
        }

        // Apply brakes
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyBrakes(dT);
        }
        else
        {
            ApplyBrakes(dT, true);
        }
    }

    private void MoveRover(float dT, bool isFwd = true, bool isNeut = false)
    {
        float torque = moveSpeed * dT;

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

    private void TurnRover(float dT, bool isRt = true, bool isNeut = false)
    {
        float incrAngle = rotSpeed * dT;

        // Check if the user is turning left or right, or returning to neutral
        if (!isNeut && Mathf.Abs(curAngle) < maxAngle)
        {
            // Not neutral; check if right or left
            if (isRt)
            {
                // Increase the current angle
                curAngle = (curAngle + incrAngle > maxAngle ? maxAngle : curAngle + incrAngle);

                // TODO: Toggle steering for middle and back wheels
                // Right = positive steering angle
                wheelFR.steerAngle += incrAngle;
                //wheelMR.steerAngle += incrAngle;
                //wheelBR.steerAngle += incrAngle;
                wheelFL.steerAngle += incrAngle;
                //wheelML.steerAngle += incrAngle;
                //wheelBL.steerAngle += incrAngle;
            }
            else
            {
                // Decrease the current angle
                curAngle = (curAngle - incrAngle < -maxAngle ? -maxAngle : curAngle - incrAngle);

                // TODO: Toggle steering for middle and back wheels
                wheelFR.steerAngle -= incrAngle;
                //wheelMR.steerAngle -= incrAngle;
                //wheelBR.steerAngle -= incrAngle;
                wheelFL.steerAngle -= incrAngle;
                //wheelML.steerAngle -= incrAngle;
                //wheelBL.steerAngle -= incrAngle;
            }
        }
        else if (isNeut && Mathf.Abs(curAngle) > 0)
        {
            // Decrease the current angle if positive; increase if negative
            if (curAngle > 0)
            {
                curAngle = (curAngle - incrAngle < 0 ? 0 : curAngle - incrAngle);
            }
            else
            {
                curAngle = (curAngle + incrAngle > 0 ? 0 : curAngle + incrAngle);
            }

            // Update the steering angle with curAngle
            wheelFR.steerAngle = curAngle;
            wheelMR.steerAngle = curAngle;
            wheelBR.steerAngle = curAngle;
            wheelFL.steerAngle = curAngle;
            wheelML.steerAngle = curAngle;
            wheelBL.steerAngle = curAngle;
        }
    }

    private void ApplyBrakes(float dT, bool releaseBrakes = false)
    {
        float torque = moveSpeed * dT;

        if (!releaseBrakes)
        {
            wheelFR.brakeTorque = torque;
            wheelMR.brakeTorque = torque;
            wheelBR.brakeTorque = torque;
            wheelFL.brakeTorque = torque;
            wheelML.brakeTorque = torque;
            wheelBL.brakeTorque = torque;
        }
        else
        {
            wheelFR.brakeTorque = 0;
            wheelMR.brakeTorque = 0;
            wheelBR.brakeTorque = 0;
            wheelFL.brakeTorque = 0;
            wheelML.brakeTorque = 0;
            wheelBL.brakeTorque = 0;
        }
    }
}
