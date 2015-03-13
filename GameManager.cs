using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private GameObject fpc;
	private GameObject mainCamera;
	private int level;
	private CharacterMotor fpMotor;
	private MouseLook mouseLookCam;
	private MouseLook mouseLookFPC; 
	private float camSensitivity;
	private float fpcSensitivity;
	public KeyCode pauseKey = KeyCode.Escape;
	public KeyCode restartKey = KeyCode.R;
	public KeyCode inventoryKey = KeyCode.Tab;
	public bool inventoryMode = false;
	public bool isPaused = false; 
	private GameObject inventory;
	private GameObject dropbox;


	// Use this for initialization
	void Start () {
		//Initialize variables 
		fpc = GameObject.Find ("First Person Controller");
		mainCamera = GameObject.Find ("Main Camera");
		fpMotor = fpc.GetComponent<CharacterMotor> ();
		mouseLookCam = mainCamera.GetComponent<MouseLook> ();
		mouseLookFPC = fpc.GetComponent<MouseLook> ();
		inventory = GameObject.FindGameObjectWithTag ("Inventory");
		dropbox = GameObject.FindGameObjectWithTag ("Dropbox");
		// Start disabled
		dropbox.SetActive (false);
		inventory.SetActive (false);
		//Start unpaused
		isPaused = false; 
		Screen.showCursor = false;
		Screen.lockCursor = true;
		//Save camera sensitivity
		camSensitivity = mouseLookCam.sensitivityY;
		fpcSensitivity = mouseLookFPC.sensitivityX;
		//Save level
		level = Application.loadedLevel;


	}
	
	// Update is called once per frame
	void Update () {

		// Inventory mode pauses game
		if (inventoryMode) {
			isPaused = true;
			// Hitting key closes inventory
			if(Input.GetKeyUp (inventoryKey)){
				isPaused = false;
				inventoryMode = false;
				inventory.SetActive (false);
				dropbox.SetActive(false);
			}
		// Open inventory
		} else if (Input.GetKeyUp (inventoryKey)) {
			isPaused = true;
			inventoryMode = true;
			inventory.SetActive(true);
			dropbox.SetActive(true);
		}

		// Restart level
		if(Input.GetKeyUp (restartKey)){
			RestartLevel();
		}

		// Pause game
		if (Input.GetKeyUp (pauseKey)) {
			if (isPaused) {
				isPaused = false;
				Screen.showCursor = false;
				Screen.lockCursor = true;
			} else {
				isPaused = true;
				Screen.showCursor = true;
				Screen.lockCursor = false;

			}
		}

		// State if not paused
		if(!isPaused){
			Screen.showCursor = false;
			Screen.lockCursor = true;
			fpMotor.canControl = true;
			mouseLookCam.sensitivityY = camSensitivity;
			mouseLookFPC.sensitivityX = fpcSensitivity;
		// State if paused
		} else {
			Screen.showCursor = true;
			Screen.lockCursor = false;
			fpMotor.canControl = false;
			mouseLookCam.sensitivityY = 0;
			mouseLookFPC.sensitivityX = 0;
			if(Input.GetMouseButtonUp(0)){
				isPaused = false;
			}
		}
	
	}

	// Reloads level
	public void RestartLevel(){
		Application.LoadLevel(level);
	}
}
