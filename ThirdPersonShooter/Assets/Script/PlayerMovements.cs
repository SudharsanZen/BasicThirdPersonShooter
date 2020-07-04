using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public GameObject cam;
    bool isWalking;
    bool isRunning;
    bool locomotion;
    float horizontal;
    float vertical;
    bool isCrouching;
    bool isInAngle;
    bool onGround;
    bool aiming;
    bool firing;
    Vector3 moveDir;

    float moveSpeed=0;
    public float walkSpeed=1;
    public float runSpeed=3;
    public float movementSmoothness=10;
    public float rotationSmoothness = 20;
    public Transform lookAt;

    public bool gravity=true;
    public int numOfRays=4;
    public float disFromGround;
    public float radiusGroundRays;
    public float crouchdisFromGround=0.5f;
    float currDist=0;
    public LayerMask layerMask;
    Transform pitch;

    Animator anim;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
        Cursor.lockState =CursorLockMode.Locked;
        Cursor.visible = false;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        pitch = cam.GetComponent<CameraController>().pitch.transform;
    }

    // Update is called once per frame
    void Update()
    {
        getParam();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        Ground();
        setParam();
        
    }
   
    void getParam()
    {
        //gets movement parameters
        horizontal = Input.GetAxis(ControllerStatics.horizontal);
        vertical = Input.GetAxis(ControllerStatics.Vertical);
        locomotion =(Mathf.Abs(horizontal)>0||Mathf.Abs(vertical)>0);
        isCrouching = Input.GetKey(ControllerStatics.crouch);
        aiming = Input.GetButton(ControllerStatics.aim);
        firing =Input.GetButton(ControllerStatics.fire);
        if (locomotion && Input.GetButton(ControllerStatics.Sprint) && !aiming && !firing)
        {
            isRunning = locomotion;
            isWalking = false;
        }
        else if (locomotion)
        {
            isWalking = true;
            isRunning = false;

        }
        else
        {
            isWalking = false;
            isRunning = false;
        }

        
    }
    void setLayerWeight(AnimStatics.animLayers layerID,float weight)
    {
        anim.SetLayerWeight((int)layerID, Mathf.LerpUnclamped(anim.GetLayerWeight((int)layerID), weight, Time.deltaTime * 7));
    }
    void setParam()
    {
        //set the layerWeights
        setLayerWeight(AnimStatics.animLayers.crouchLayer, (isCrouching)?1:0);
        setLayerWeight(AnimStatics.animLayers.aimLayer, (aiming)?1:0);
        if (aiming)
            AimPitch();
        //sets the animator parameter
        anim.SetFloat(AnimStatics.horizontal,horizontal);
        anim.SetFloat(AnimStatics.vertical,vertical);
        anim.SetBool(AnimStatics.walking,isWalking);
        anim.SetBool(AnimStatics.running,isRunning);
        anim.SetBool(AnimStatics.aim,aiming);
        anim.SetBool(AnimStatics.crouch,isCrouching);
        anim.SetBool(AnimStatics.isInAngle,isInAngle);
        anim.SetBool(AnimStatics.onGround,onGround);

        
    }

    

    void HandleMovement()
    {
        moveDir = moveDirection();
        moveSpeed =(isWalking)?walkSpeed:runSpeed;

        Vector3 targetVelocity=moveDir*moveSpeed;
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity,Time.deltaTime*movementSmoothness);
    }
    Vector3 moveDirection()
    {
        Vector3 dir = cam.transform.forward * vertical + cam.transform.right * horizontal;
        dir.y = 0;
        dir = dir.normalized;
        return dir;
    }

    void HandleRotation()
    {
        Vector3 lookDir = Quaternion.Euler(0,cam.transform.rotation.eulerAngles.y,0)*Vector3.forward;
        if (locomotion && !aiming && !firing)
        {
            Quaternion lookAng = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAng, Time.deltaTime*rotationSmoothness);

            //still din't use this crap so don't worry about this part
            float ang = Vector3.Angle(transform.forward, moveDir);
            //print(ang);
            if (ang > 8)
            {
                //still din't use this crap so don't worry about this part
                isInAngle = false;
            }
            else
            {
                //still din't use this crap so don't worry about this part
                isInAngle = true;
            }
        }
        else if(aiming || firing)
        {
            Quaternion lookAng = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAng, Time.deltaTime * rotationSmoothness);
        }

       
    }

   
    void AimPitch()
    {
        //calculate the look angle with the pitch and set the angle in animator for blending between look-up and look-down animation
        anim.SetFloat(AnimStatics.angle, Vector3.SignedAngle(pitch.forward,Vector3.up,-pitch.right));
    }


    void Ground()
    {
       
        Ray r =new Ray(transform.position+new Vector3(0,1,0),Vector3.down);
        Debug.DrawRay(r.origin, r.direction* disFromGround);

        RaycastHit hit;
        bool gHit = Physics.Raycast(r, out hit, disFromGround, layerMask);
        
        for (int i = 0;i<numOfRays; i++)
        {
            if (gHit)
                break;
            Vector3 newOrigin=r.origin+Quaternion.Euler(0,360/numOfRays*i,0)* transform.forward*radiusGroundRays ;
            Debug.DrawRay(newOrigin, r.direction * disFromGround,Color.cyan);
            Ray newR =new Ray(newOrigin,Vector3.down);
            gHit = Physics.Raycast(newR, out hit, disFromGround, layerMask);
        }


        if (gHit)
        {
            currDist =Mathf.Lerp(currDist,(Input.GetKey(ControllerStatics.crouch)?crouchdisFromGround:disFromGround),Time.deltaTime*10);
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, -(hit.distance - currDist + 0.2f), 0), Time.deltaTime * 100);
        }
        else
        {
            if(gravity)
                transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * 5f, transform.position.z);
        }
        onGround = gHit;
    }
}
