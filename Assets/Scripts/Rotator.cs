using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    [SerializeField]
    private float speed;

    //Update is called every frame
    void Update()
    {
        //Rotate thet transform of the game object this is attached to by 45 degrees, taking into account the time elapsed since last frame.
        transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime * speed);
    }
}
