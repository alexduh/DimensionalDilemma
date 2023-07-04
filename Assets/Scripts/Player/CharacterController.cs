using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Vector3 velocity;
    // Start is called before the first frame update

    public void Move(Vector3 moveVector)
    {
        transform.position += moveVector;
        velocity = moveVector/Time.deltaTime;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
