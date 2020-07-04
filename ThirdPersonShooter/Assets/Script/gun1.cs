using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun1 : MonoBehaviour
{

    public GameObject bullet;
    public Transform leftHandIkPose;
    public Transform rightHandIkPose;

    public float recoilAngle=1f;
    public float lastImpulse = 0;
    public float impulseInterval=0.01f;
    private void Update()
    {

        if (Input.GetButton(ControllerStatics.fire))
        {
            //stupid ass code to shoot big ass sphere bullets
            GameObject b = Instantiate(bullet);
            b.transform.position = transform.position + transform.forward;
            b.AddComponent<Rigidbody>();
            b.GetComponent<Rigidbody>().velocity = transform.forward * 100;
            b.GetComponent<Rigidbody>().useGravity = false;
            Destroy(b, 3);

        }
        
    }
}
