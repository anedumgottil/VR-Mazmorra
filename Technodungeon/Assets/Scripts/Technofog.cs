using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

//apply this script to a MicroBlock to make it fade away similar to fog
//it also makes it Touchable.
public class Technofog : VRTK_InteractableObject {
    private static WaitForSeconds waits = new WaitForSeconds(0.25f);
    private bool alive = true;
    public int dispersionAmount = 4;
    public int evaporationMagnitude = 16;
    Renderer mbRenderer;
    int currentAlpha = 255;

	// Use this for initialization
	void Start () {
        this.pointerActivatesUseAction = false;
        this.isGrabbable = false;
        this.isUsable = false;
        mbRenderer = this.GetComponent<Renderer> ();
        if (mbRenderer == null) {
            this.alive = false;
            Destroy (this.gameObject);
        }
        StartCoroutine (evaporate ());
	}
	
    public override void StartTouching(VRTK_InteractTouch currentTouchingObject) {
        Destroy (this.gameObject);
    }

    public bool makeTransparent(int alpha) {
        if (alpha <= 0 || this.currentAlpha <= 0) {
            Destroy (this.gameObject);
        }
        Material material = mbRenderer.material;

        material.SetFloat("_Mode", 4f);

        //0f - opacity
        //1f - cutout
        //2f - fade
        //3f - transparent
        Color32 col = mbRenderer.material.GetColor("_Color");
        col.a = (byte)alpha;
        currentAlpha = alpha;
        mbRenderer.material.SetColor("_Color", col);

        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        return true;
    }

    IEnumerator evaporate()
    {
        while (alive) {
            yield return waits;
            //Debug.Log ("countdown: " + this.currentAlpha);
            makeTransparent (this.currentAlpha - Random.Range(1, evaporationMagnitude));
            this.gameObject.transform.Translate (new Vector3 (((float)(Random.Range (-dispersionAmount, dispersionAmount))) / 100f, ((float) (Random.Range (0, dispersionAmount))) / 100f, ((float) (Random.Range (-dispersionAmount, dispersionAmount))) / 100f));
        }
    }
}
