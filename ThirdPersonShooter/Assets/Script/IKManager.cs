using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IKManager : MonoBehaviour
{
    Animator anim;
    public GameObject gun;

    public Transform lookAt;

    public Transform leftHandTarget;//left ik target
    public Transform rightHandTarget;//right ik target

    public Transform leftHandIK;//left hand hint
    public Transform rightHandIK;//right hand hint

    public Transform leftLegHint;
    public Transform rightLegHint;

    public Transform AimPose;
    public Transform IdlePose;

    Transform rightShoulder;

    //layer mask to hit everything except the actor's collider
    public LayerMask layerMask;
    public bool ikActive=true;
    public bool Aiming=true;

    public float legRayDist=2f;
    public float footOffset = 0.1f;

    public Vector3 idleOffsetPose;
    public Vector3 aimOffsetPose;
    // Start is called before the first frame update
    void Start()
    {
        //get and set the lef and right hand target positions for holding the gun
        leftHandTarget =gun.GetComponent<gun1>().leftHandIkPose;
        rightHandTarget =gun.GetComponent<gun1>().rightHandIkPose;
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Aiming = Input.GetButton(ControllerStatics.aim);
        if (rightShoulder == null)
        {
            rightShoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
        }
        else
        {
            AimPose.position = rightShoulder.position + AimPose.forward*aimOffsetPose.z+AimPose.right*aimOffsetPose.x+ AimPose.up * aimOffsetPose.y;
            IdlePose.position =rightShoulder.position+ IdlePose.forward * idleOffsetPose.z + IdlePose.right *idleOffsetPose.x + IdlePose.up *idleOffsetPose.y;
            AimPose.LookAt(lookAt.position);
        }

        //if aiming or fire is true then the gun should point at the target
        if (Aiming || Input.GetButton(ControllerStatics.fire))
        {
            gun.transform.position =Vector3.Lerp(gun.transform.position, AimPose.position, Time.deltaTime*10) ;
            gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, AimPose.rotation, Time.deltaTime * 10);
        }
        else
        {
            gun.transform.position = Vector3.Lerp(gun.transform.position, IdlePose.position, Time.deltaTime * 10);
            gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, IdlePose.rotation, Time.deltaTime * 10);
        }
    }

  
    private void OnAnimatorIK(int layerIndex)
    {
        //check if animator and ikActive is set, if it is set then ik targets are handked
        if (anim)
        {
            
            if (ikActive)
            {
               
                if (Aiming)
                {
                    //set look at weight anf position
                    anim.SetLookAtWeight(0.5f,1);
                    anim.SetLookAtPosition(lookAt.position);
                }
                //set weight of each ik target and hint
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
                anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);

                //set ik target position
                anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
                anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);

                
                //set ik hint position
                anim.SetIKHintPosition(AvatarIKHint.LeftElbow,leftHandIK.position);
                anim.SetIKHintPosition(AvatarIKHint.RightElbow,rightHandIK.position);

                
                FootIk();

            }

        }

       
    }


    void FootIk()
    {
        float rweight = anim.GetFloat("rightFootWeight");
        float lweight = anim.GetFloat("leftFootWeight");

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, lweight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, lweight);

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rweight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rweight);
        RaycastHit hit;
        Ray Lray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        Debug.DrawRay(Lray.origin, Lray.direction * legRayDist);
        if (Physics.Raycast(Lray, out hit, legRayDist, layerMask))
        {
            Vector3 footPose = hit.point;
            footPose.y += footOffset;
            //print(hit.distance);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPose);
            Vector3 forward = Vector3.Cross(anim.GetBoneTransform(HumanBodyBones.LeftFoot).right, hit.normal);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward, hit.normal));

        }


        Ray Rray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        Debug.DrawRay(Rray.origin, Rray.direction * legRayDist);
        if (Physics.Raycast(Rray, out hit, legRayDist, layerMask))
        {
            Vector3 footPose = hit.point;
            footPose.y +=footOffset;
            //print(hit.distance);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, footPose);
            Vector3 forward = Vector3.Cross(anim.GetBoneTransform(HumanBodyBones.RightFoot).right, hit.normal);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forward, hit.normal));

        }
        anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rweight);
        anim.SetIKHintPosition(AvatarIKHint.RightKnee, rightLegHint.position);
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, lweight);
        anim.SetIKHintPosition(AvatarIKHint.LeftKnee, leftLegHint.position);
    }
}
