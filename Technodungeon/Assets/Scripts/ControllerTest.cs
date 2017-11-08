using UnityEngine;
using VRTK;
public class PlayerControllerTest : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        if(GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError("Need to have VRTK_ControllerEvents component attached to the controller");
            return;
        }
        GetComponent<VRTK_ControllerEvents>().ButtonOnePressed += new ControllerInteractionEventHandler(handleOneButtonPressed);
        GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += new ControllerInteractionEventHandler(handleMenuButtonPressed);
        GetComponent<VRTK_ControllerEvents>().TriggerPressed += new ControllerInteractionEventHandler(triggerPressed);
    }

    private void handleMenuButtonPressed(object sender, ControllerInteractionEventArgs e)
    {
        Debug.Log("Menu Button Seems to be working!");
    }

    private void handleOneButtonPressed(object sender, ControllerInteractionEventArgs e)
    {
        Debug.Log("One Button Seems to be working!");
    }

    private void triggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        Debug.Log("Trigger Seems to be working!");
    }
}