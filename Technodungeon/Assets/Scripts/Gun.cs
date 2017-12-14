using UnityEngine;
using VRTK;

public class Gun : VRTK_InteractableObject
{
    [Header("Gun Options", order = 4)]
    public GameObject flashlight = null;
    public GameObject muzzleFlash = null;
    public AudioSource audioSource = null;
    public AudioClip flashlightClickNoise = null;
    public AudioClip grabNoise = null;
    public AudioClip ungrabNoise = null;
    public GameObject bullet = null;
    public float fireRate = 0;//set to zero for no fire rate enforcement.
    public float bulletSpeed = 1000f;
    public float bulletLife = 5f;
    public int bulletDamage = 35;
    public bool flashlightStartOff = true;//start flashlight in off state?
    public AudioClip[] fireSounds;

    private bool flashlightState = false;
    private GameObject grabbingObject = null;
    private float lastFireTime = 0;

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);
        fireBullet();
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
        if (grabNoise != null && audioSource != null) {
            audioSource.PlayOneShot (grabNoise);
        }

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
        if (ungrabNoise != null && audioSource != null) {
            audioSource.PlayOneShot (ungrabNoise);
        }

        //deregister flashlight button
        previousGrabbingObject.GetComponent<VRTK_ControllerEvents> ().ButtonTwoPressed -= new ControllerInteractionEventHandler (DoButtonTwoPressed);
    }

    protected void Start()
    {
        if (audioSource == null) {
            Debug.LogWarning ("Cannot find Gun audiosource!");
        }
        if (flashlight != null) {//setup flashlight
            flashlight.SetActive (!flashlightStartOff);
            flashlightState = !flashlightStartOff;
        }
        if (bullet != null) {//setup bullets
            bullet.SetActive (false);
        }
        lastFireTime = fireRate;
    }

    public void fireBullet()
    {
        if (bullet == null)
            return;
        if (fireRate + lastFireTime > Time.time)
            return;
        lastFireTime = Time.time;
        GameObject bulletClone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
        bulletClone.SetActive(true);
        NormalProjectile np = bulletClone.GetComponent<NormalProjectile>();

        if (audioSource != null) {
            float pitchmod = 0.0f;
            AudioClip selectedSound = Utility.randomlySelectAudioClipAndPitch (fireSounds, 0.15f, out pitchmod);
            //play fire sound
            audioSource.pitch += pitchmod;
            audioSource.PlayOneShot (selectedSound);
            audioSource.pitch = 1.0f;//reset pitch
        }

        //play muzzleflash ps
        if (muzzleFlash != null) {
            muzzleFlash.GetComponent<ParticleSystem> ().Play();
        }
        np.setSpeed (bulletSpeed);
        np.FireProjectile (this.gameObject, bullet.transform.up, bulletDamage, 10);
    }

    //flashlight button
    private void DoButtonTwoPressed(System.Object sender, ControllerInteractionEventArgs e) {
        //are we holding the gun?
        if (base.IsGrabbed ()) {
            //is this the same controller that we're being grabbed by?
            if (e.controllerReference.scriptAlias.Equals (this.grabbingObject)) {
                //toggle the flashlight state
                toggleFlashlight();
                //play click noise
                if (audioSource != null) {
                    audioSource.PlayOneShot (flashlightClickNoise);
                }
            } else {
                Debug.Log("Gun: Tried to activate a flashlight with the wrong controller. This should not happen: "+e.controllerReference.scriptAlias.name.ToString () + " =/= "+ this.grabbingObject.name.ToString());
            }
        }
    }

    public void toggleFlashlight() {
        flashlight.SetActive (!flashlightState);
        flashlightState = !flashlightState;
    }

    public void setFlashlightState(bool state) {
        flashlight.SetActive (state);
        flashlightState = state;
    }

}