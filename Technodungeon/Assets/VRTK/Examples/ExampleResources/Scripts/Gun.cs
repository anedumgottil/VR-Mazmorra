using UnityEngine;
using VRTK;

public class Gun : VRTK_InteractableObject
{
    public GameObject flashlight = null;
    public AudioClip flashlightClickNoise = null;
    public GameObject bullet = null;
    public float bulletSpeed = 1000f;
    public float bulletLife = 5f;
    public bool flashlightStartOff = true;//start flashlight in off state?

    private bool flashlightState = false;
    private AudioSource audioSource = null;

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);
        FireBullet();
    }

    protected void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource> ();
        if (flashlight != null) {//setup flashlight
            GetComponent<VRTK_ControllerEvents> ().StartMenuReleased += new ControllerInteractionEventHandler (DoStartMenuReleased);
            flashlight.SetActive (!flashlightStartOff);
            flashlightState = !flashlightStartOff;
        }
        if (bullet != null) {//setup bullets
            bullet.SetActive (false);
        }
    }

    private void FireBullet()
    {
        if (bullet == null)
            return;
        GameObject bulletClone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
        bulletClone.SetActive(true);
        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
        rb.AddForce(bullet.transform.forward * bulletSpeed);
        Destroy(bulletClone, bulletLife);
    }

    //flashlight button
    private void DoStartMenuReleased(System.Object sender, ControllerInteractionEventArgs e) {
        //are we holding the gun?
        if (base.IsGrabbed ()) {
            //is this the same controller that we're being grabbed by?
            if (e.controllerReference.actual.Equals (base.GetGrabbingObject ())) {
                //toggle the flashlight state
                flashlight.SetActive (!flashlightState);
                flashlightState = !flashlightState;
                //play click noise
                audioSource.PlayOneShot (flashlightClickNoise);
            }
        }
    }

}