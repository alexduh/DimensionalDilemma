using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private StarterAssetsInputs _input;

    // Start is called before the first frame update
    void Start()
    {
        _input = transform.root.GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.shrink)
        {
            ShrinkBeam();
            _input.shrink = false;
        }
        if (_input.grow)
        {
            GrowBeam();
            _input.grow = false;
        }
    }

    void ShrinkBeam()
    {
        Debug.Log("Shrink!");
    }

    void GrowBeam()
    {
        Debug.Log("Grow!");
    }
}
