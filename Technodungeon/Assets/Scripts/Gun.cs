using UnityEngine;
using VRTK;

public class Gun : VRTK_InteractableObject
{
    [Header("Gun Options", order = 4)]
    public GameObject flashlight = null;
    public AudioClip flashlightClickNoise = null;
    public GameObject bullet = null;
    public float bulletSpeed = 1000f;
    public float bulletLife = 5f;
    public bool flashlightStartOff = true;//start flashlight in off state?
    public AudioClip[] fireSounds;

    private bool flashlightState = false;
    private AudioSource audioSource = null;
    private GameObject grabbingObject = null;

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);
        FireBullet();
    }

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject) {
        //VRTK Grab code
        GameObject currentGrabbingGameObject = (currentGrabbingObject != null ? currentGrabbingObject.gameObject : null);
        ToggleEnableState(true);
        if (!IsGrabbed() || IsSwappable())
        {
            PrimaryControllerGrab(currentGrabbingGameObject);
        }
        else
        {
            SecondaryControllerGrab(currentGrabbingGameObject);
        }
        OnInteractableObjectGrabbed(SetInteractableObjectEvent(currentGrabbingGameObject));

        //OUR grab code

        this.grabbingObject = currentGrabbingObject.gameObject;
        //play pickup sound

        //register flashlight button
        grabbingObject.GetComponent<VRTK_ControllerEvents> ().ButtonTwoPressed += new ControllerInteractionEventHandler (DoButtonTwoPressed);
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject) {
        //VRTK Ungrab Code:
        GameObject previousGrabbingGameObject = (previousGrabbingObject != null ? previousGrabbingObject.gameObject : null);
        GameObject secondaryGrabbingObject = GetSecondaryGrabbingObject();
        if (!secondaryGrabbingObject || secondaryGrabbingObject != previousGrabbingGameObject)
        {
            SecondaryControllerUngrab(secondaryGrabbingObject);
            PrimaryControllerUngrab(previousGrabbingGameObject, secondaryGrabbingObject);
        }
        else
        {
            SecondaryControllerUngrab(previousGrabbingGameObject);
        }
        OnInteractableObjectUngrabbed(SetInteractableObjectEvent(previousGrabbingGameObject));

        //OUR ungrab code
        this.grabbingObject = null;
        //play drop sound

        //deregister flashlight button
        previousGrabbingObject.GetComponent<VRTK_ControllerEvents> ().ButtonTwoPressed -= new ControllerInteractionEventHandler (DoButtonTwoPressed);
    }

    protected void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource> ();
        if (flashlight != null) {//setup flashlight
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
        NormalProjectile np = bulletClone.GetComponent<NormalProjectile>();

        //play fire sound
        audioSource.pitch += ((float) (Random.Range (0,11)*0.01f)) + (-0.05f);//tweak pitch from range (0.05 to -0.05)
        if (fireSounds != null && fireSounds.Length > 0) {
            int selectedsound = Random.Range (0, fireSounds.Length);
            audioSource.PlayOneShot (fireSounds[selectedsound]);
        }
        audioSource.pitch = 1.0f;//reset pitch

        np.FireProjectile (this.gameObject, bullet.transform.up, 100, 10);
    }

    //flashlight button
    private void DoButtonTwoPressed(System.Object sender, ControllerInteractionEventArgs e) {
        //are we holding the gun?
        if (base.IsGrabbed ()) {
            //is this the same controller that we're being grabbed by?
            if (e.controllerReference.scriptAlias.Equals (this.grabbingObject)) {
                //toggle the flashlight state
                flashlight.SetActive (!flashlightState);
                flashlightState = !flashlightState;
                //play click noise
                audioSource.PlayOneShot (flashlightClickNoise);
            } else {
                Debug.Log("Gun: Tried to activate a flashlight with the wrong controller. This should not happen: "+e.controllerReference.scriptAlias.name.ToString () + " =/= "+ this.grabbingObject.name.ToString());
            }
        }
    }

}