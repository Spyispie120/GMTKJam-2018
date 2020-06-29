using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float dampening;
    private Vector2 origin;
    private bool shaking;
    private float shakeMag, shakeTimeEnd;
    private Vector3 shakeVector;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        shaking = false;
        //dampening = 1f;
    }

    private void FixedUpdate()
    {
        Vector3 target = UpdateTargetPos();
        UpdateCameraPosition(target, new Vector3());
    }

    Vector3 UpdateTargetPos()
    {
        Vector3 ret = origin;// player.transform.position;// + mouseOffset; //find position as it relates to the player
        ret += UpdateShake(); //add the screen shake vector to the target
        ret.z = -10;//zStart; //make sure camera stays at same Z coord
        return ret;
    }

    void UpdateCameraPosition(Vector3 target, Vector3 refVel)
    {
        Vector3 tempPos;
        tempPos = Vector3.SmoothDamp(transform.position, target, ref refVel, dampening); //smoothly move towards the target
        transform.position = tempPos; //update the position
    }

    // shaking
    public void Shake(Vector3 direction, float magnitude, float length)
    {
        shaking = true;
        shakeVector = direction;
        shakeMag = magnitude;
        shakeTimeEnd = Time.time + length;
    }

    Vector3 UpdateShake()
    {
        if (!shaking || Time.time > shakeTimeEnd)
        {
            shaking = false;
            return Vector3.zero;
        }
        Vector3 tempOffset = shakeVector;
        tempOffset *= shakeMag;
        return tempOffset;
    }
}
