using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTouchObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    // 1
    private GameObject collidingObject; 
    // 2
    private GameObject objectInHand;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void OnCollisionEnter(Collision collision) {
        Destroy (collision.gameObject);
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

    }
    private void SetCollidingObject(Collider col)
    {
        // 1
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // 2
        collidingObject = col.gameObject;
    }

    // 1
    public void OnTriggerEnter(Collider other)
    {
        //SetCollidingObject(other);
    }

    // 2
    public void OnTriggerStay(Collider other)
    {
        //SetCollidingObject(other);
    }

    // 3
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }
}
