using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private bool startState;
    private bool currentState;

    public bool Unlocked()
    {
        return startState == currentState;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //return startState == currentState;
    }
}
