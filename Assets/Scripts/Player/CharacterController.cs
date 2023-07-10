using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CharacterController : MonoBehaviour
{
    public Vector3 velocity;
    private RaycastHit hit;
    // Start is called before the first frame update

    public void Move(Vector3 moveVector)
    {
        transform.position += AdjustVelocityToSlope(moveVector); ;
        velocity = moveVector/Time.deltaTime;
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {

        if (Physics.Raycast(transform.position, Vector3.down, out hit, .2f, GetComponent<FirstPersonController>().GroundLayers))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }
        return velocity;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
