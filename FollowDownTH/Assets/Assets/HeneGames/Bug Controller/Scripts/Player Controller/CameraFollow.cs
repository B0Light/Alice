using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject CameraFollowObj;
    public float CameraMoveSpeed = 120.0f;
    public float clampAgle = 80.0f;
    public float inputSensitivity = 150.0f;
    public bool invertY = false;

    private float mouseX;
    private float mouseY;
    private float finalInputX;
    private float finalInputZ;
    private float rotY = 0.0f;
    private float rotX = 0.0f;

    void Start()
    {
        transform.SetParent(null);

        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");

        if(invertY)
        {
            mouseY = Input.GetAxis("Mouse Y");
        }
        else
        {
            mouseY = -Input.GetAxis("Mouse Y");
        }

        finalInputX = mouseX;
        finalInputZ = mouseY;

        rotY += finalInputX * inputSensitivity * Time.deltaTime;
        rotX += finalInputZ * inputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAgle, clampAgle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    void LateUpdate()
    {
        CameraUpdater();   
    }

    void CameraUpdater()
    {
        Transform target = CameraFollowObj.transform;

        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
