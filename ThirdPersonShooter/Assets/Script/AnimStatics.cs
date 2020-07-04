using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStatics
{
    public static string walking = "walk";
    public static string running = "run";
    public static string aim = "isAiming";
    public static string angle = "aimAngle";
    public static string crouch ="crouch";
    public static string horizontal ="horizontal";
    public static string vertical ="vertical";
    public static string isInAngle="isInAngle";
    public static string onGround ="onGround";

    public enum animLayers
    {
        //enumerator for each layer
         normalLayer = 0,aimLayer = 1, crouchLayer =2
    }

}
