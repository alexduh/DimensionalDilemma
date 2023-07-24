using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePlayer : MonoBehaviour
{
    Collider box;
    Collider capsule;

    // Start is called before the first frame update
    void Start()
    {
        capsule = GameObject.FindWithTag("Player").GetComponent<CapsuleCollider>();
        box = GetComponent<BoxCollider>();

        Physics.IgnoreCollision(box, capsule, true);
    }

}
