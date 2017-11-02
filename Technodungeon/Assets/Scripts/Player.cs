using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class allows you to do player related classes, like move him/her around the map or access his items which we might need to add in the future
public class Player : MobileEntity {
    private static Player instance = null;//now with 100% more singleton!

    private int gridPosX = -1;
    private int gridPosY = -1;

    private int generatorX = 5;
    private int generatorY = 5;

    public float clickThresh = 0.3f;//30% of trackpad distance
    public GameObject rController;
    public GameObject lController;

    private SteamVR_TrackedController rightController;
    private SteamVR_TrackedController leftController;

    private void OnEnable()
    {
        rightController = rController.gameObject.GetComponent<SteamVR_TrackedController>(); 
        leftController = lController.gameObject.GetComponent<SteamVR_TrackedController>();
        rightController.PadClicked += HandleRightPadClicked;
        leftController.PadClicked += HandleLeftPadClicked;
    }

    private void OnDisable()
    {
        rightController.PadClicked -= HandleRightPadClicked;
        leftController.PadClicked -= HandleLeftPadClicked;
    }

    private void HandleRightPadClicked(object sender, ClickedEventArgs e)
    {
        if (e.padX < -1 + clickThresh && (e.padY > -1 + clickThresh && e.padY < 1 - clickThresh)) {
            //left pad click
            genTile(--generatorX, generatorY);
        } else if (e.padX > 1 - clickThresh && (e.padY > -1 + clickThresh && e.padY < 1 - clickThresh)) {
            //right pad click
            genTile(++generatorX, generatorY);
        } else if (e.padY < -1 + clickThresh && (e.padX > -1 + clickThresh && e.padX < 1 - clickThresh)) {
            //down pad click
            genTile(generatorX, --generatorY);
        } else if (e.padY > 1 - clickThresh && (e.padX > -1 + clickThresh && e.padX < 1 - clickThresh)) {
            //up pad click
            genTile(generatorX, ++generatorY);
        }
            
    }

    private void genTile(int x, int y) {
        if (Grid.getInstance ().getGridSpace (x, y) != null) {
            MapGenerator.Instance.setGridSpace (x, y, GridSpace.GridSpaceType.Corridor);
        }
    }

    private void HandleLeftPadClicked(object sender, ClickedEventArgs e)
    {
        if (e.padX < -1 + clickThresh && (e.padY > (-1 + clickThresh) && e.padY < 1 - clickThresh)) {
            //left pad click
            //teleportToGridCoords(gridPosX-1, gridPosY);
            this.gameObject.transform.Translate (new Vector3(2,0,0));
        } else if (e.padX > 1 - clickThresh && (e.padY > (-1 + clickThresh) && e.padY < 1 - clickThresh)) {
            //right pad click
            //teleportToGridCoords(gridPosX+1, gridPosY);
            this.gameObject.transform.Translate (new Vector3(-2,0,0));
        } else if (e.padY < -1 + clickThresh && (e.padX > (-1 + clickThresh) && e.padX < 1 - clickThresh)) {
            //down pad click
            //teleportToGridCoords(gridPosX, gridPosY-1);
            this.gameObject.transform.Translate (new Vector3(0,-2,0));
        } else if (e.padY > 1 - clickThresh && (e.padX > (-1 + clickThresh) && e.padX < 1 - clickThresh)) {
            //up pad click
            //teleportToGridCoords(gridPosX, gridPosY+1);
            this.gameObject.transform.Translate (new Vector3(0,2,0));

        }

    }

    void Awake() {
        //Check if instance already exists
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a Grid.
            Destroy (gameObject); 
    }


    public static Player getInstance() {
        return instance;
    }
}
