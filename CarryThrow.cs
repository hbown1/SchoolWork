using UnityEngine;
using System.Collections;

public class CarryThrow : MonoBehaviour {

	[Tooltip("Position of player's hand")]
	public Transform handPos;
	public Transform centerPos;
	private GameObject fpc;
	private GameObject mainCamera;
	private RayCaster rayCastComp;
	private FPData fpData;
	[Tooltip("Strength of player")]
	public float throwForce = 1;
	public float additionalForce = 0;
	private float xTorque = 1;
	private float yTorque = 1;
	private float zTorque = 1;
	private float step;
	[Tooltip("Object being carried")]
	public GameObject currentObject;
	private ObjectData currentObjectData;
	public bool carryingAnObject = false;
	private KeyCode cancelThrow = KeyCode.LeftControl;
	private KeyCode throwKey = KeyCode.T;
	Inventory inventoryScript;


	// Use this for initialization
	void Start () {
		fpc = GameObject.Find ("First Person Controller");
		mainCamera = GameObject.Find ("Main Camera");
		inventoryScript = GameObject.FindGameObjectWithTag ("Inventory").GetComponent<Inventory> ();
		rayCastComp = fpc.GetComponent<RayCaster> ();
		fpData = fpc.GetComponent<FPData> ();

	}
	
	// Update is called once per frame
	void Update () {
		step = additionalForce * Time.deltaTime;

		if (currentObject == null){
			carryingAnObject = false;
			currentObjectData = null;	
		}
		/* Carrying an object */
		if (carryingAnObject) {
			currentObject.transform.position = handPos.position;
			currentObject.transform.rotation = handPos.rotation;

			/* Drop or throw object */
			if(Input.GetKey(throwKey)){
				currentObject.transform.position = centerPos.position;
				currentObject.transform.position = Vector3.MoveTowards(currentObject.transform.position, transform.position, step);
				if(additionalForce <= 50){
					additionalForce+=0.1f;
				}
			}

			/* Throws object */
			if(Input.GetKeyUp(throwKey) && !Input.GetKey (cancelThrow)){
				StopInteracting();
			/* Cancels throw and resets throw force */
			} else if (Input.GetKeyUp(throwKey) && Input.GetKey (cancelThrow)){
				additionalForce = 0;
			}

			/* Use object */
			if(Input.GetMouseButtonUp(0)){
				useObject();
			}

		/* Not carrying an Object */
		} else if (!carryingAnObject) {		

			if (fpData.mouseOverObject != null && fpData.mouseOverObjectData.withinArmsReach) {			//Ray has hit something within arms reach

				if (fpData.mouseOverObjectData.pickupable && Input.GetMouseButtonUp(0)) {		//can be picked up and mouse is clicked
					fpData.mouseOverObject.renderer.material.shader = fpData.mouseOverObjectData.initialShader;
					currentObject = rayCastComp.gameObjectHit;
					currentObjectData = currentObject.GetComponent<ObjectData> ();
					carryingAnObject = true; 
					audio.PlayOneShot (currentObjectData.pickupFX);
					currentObject.collider.enabled = false;
					rayCastComp.gameObjectHit.transform.position = handPos.transform.position;
					inventoryScript.addExistingItem(currentObject.GetComponent<DroppedItem>().item);
				}
			}
		}
	
	}
	/* Calls object's use function */
	void useObject(){
		currentObjectData.use();
	}

	/* Resets variables */
	void StopInteracting(){

		currentObject.collider.enabled = true;

		if (carryingAnObject) {
			DropObject();
		}

		currentObject.renderer.material.shader = currentObjectData.initialShader;
		fpData.showText = false;
		currentObject = null;
		currentObjectData = null;
		fpData.mouseOverObject = null;
		fpData.mouseOverObjectData = null;


	}

	/* Throws object */
	void DropObject(){
		currentObject.transform.position = centerPos.position;
		currentObject.rigidbody.AddTorque (xTorque, yTorque, zTorque);
		// Force is determined by how long throw button was held
		currentObject.rigidbody.velocity = mainCamera.transform.TransformDirection(0,0,(throwForce+additionalForce)/currentObject.rigidbody.mass);
		carryingAnObject = false; 
		additionalForce = 0;
	}


}
