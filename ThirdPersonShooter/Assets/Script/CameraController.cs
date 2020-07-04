using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float rotX;//stores rotation in the X axis
    float rotY;//stores rotation in the Y axis
    public float mouseSmoothness = 100;//basically mouse Sensitivity
    public float mouseRotAcceleration = 20;
    public float followSpeed = 100;
    public float maxPitch = 80;
    public float fovAim;
    public float fovNorm;
    public Transform yaw;//yaw gameObject's transform
    public Transform pitch;//pitch gameObject's transform
    public Transform shake;//shake gameObject's transform
    public Transform target;//player target trasnform
    public Transform globalLookTarget;//look at target transform for player to point his gun at
    public Vector3 camOffset = new Vector3(0, 0, 0);
    public Vector3 aimOffset = new Vector3(0, 0, 0);
    private Vector3 currCamOffset;
    Camera cam;
    public LayerMask layerMask;//to mask out actor colliders like players
    bool aiming = false;


    //camera recoil parameters
    public float incrementRate = 0.1f;
    public float increment = 1;
    public float currTimeRecoil = 0;
    // Start is called before the first frame update
    void Start()
    {
       
        rotX = pitch.rotation.eulerAngles.x;
        rotY = yaw.rotation.eulerAngles.y;
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        getMouseParam();

    }
    private void FixedUpdate()
    {
       
        cameraRotation();
        cameraTranslation();
        //HandleRecoil();//ignore this cause this is for recoil and this method is shit

        
    }
    void HandleRecoil()
    {
        //if ya really wanna look at this then go ahead
        if (Input.GetButton(ControllerStatics.fire))
        {
            //it just adds a small amount to the pitch each time the player shoots with regular intervals which is the increment
            if (currTimeRecoil < incrementRate)
                currTimeRecoil += Time.deltaTime;
            else
            {
                
                currTimeRecoil = 0;
                rotX += increment;
            }



        }
    }
    void getMouseParam()
    {
        //gets delta mouse change
        float mouseX = Input.GetAxis(ControllerStatics.MouseX);
        float mouseY = Input.GetAxis(ControllerStatics.MouseY);

        rotX += mouseY * mouseSmoothness * Time.deltaTime;
        rotY += mouseX * mouseSmoothness * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -maxPitch, maxPitch);

        if (Mathf.Abs(rotY) > 360)
            rotY = 0;

        if (Input.GetButton(ControllerStatics.aim))
            aiming = true;
        else
            aiming = false;
    }


    void cameraRotation()
    {
        //Smoothly rotate the Yaw in the Y axis
        Quaternion yAngle = Quaternion.Euler(yaw.rotation.eulerAngles.x, rotY, yaw.rotation.eulerAngles.z);
        yaw.rotation = Quaternion.Slerp(yaw.rotation, yAngle, Time.deltaTime * mouseRotAcceleration);

        //Smoothly rotate the pitch in the X axis
        Quaternion xAngle = Quaternion.Euler(rotX, yaw.rotation.eulerAngles.y, yaw.rotation.eulerAngles.z);
        pitch.rotation = Quaternion.Slerp(pitch.rotation, xAngle, Time.deltaTime * mouseRotAcceleration);


    }

    void cameraTranslation()
    {
        //follow player

        yaw.transform.position = Vector3.Lerp(yaw.transform.position, target.transform.position, Time.deltaTime * followSpeed);

        //aim
        if (aiming)
        {
            currCamOffset = aimOffset;

            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovAim, Time.deltaTime * 10);

        }
        else
        {
            currCamOffset = camOffset;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovNorm, Time.deltaTime * 10);
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, currCamOffset, Time.deltaTime * 5);

       

        cameraCollision();
    }

    void cameraCollision()
    {
        Vector3 origin = target.position;
        Ray r = new Ray(origin, -(origin - transform.position).normalized);
        RaycastHit hit;
        if (Physics.Linecast(origin, transform.position, out hit, layerMask))
        {
            transform.position = origin + r.direction * hit.distance;
        }

        //set the far point with raycast(i.e)set the look position
        if (Physics.Raycast(transform.position, transform.forward, out hit, 200, layerMask))
        {
            //if obstructed then set the poin of obstruction as the look position
            globalLookTarget.transform.position = hit.point;
        }
        else
        {
            //if nothing is obstructed then set the look position to a large distance
            globalLookTarget.localPosition = new Vector3(0, 0, 100);
        }

    }
}
