using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPData : MonoBehaviour {

	/* Private objects and components */
	private GameManager gameManager;
	private RayCaster rayCastComp;

	/* Health bar stuff */
	public RectTransform healthTransform; // the health bar rectangle
	private float cachedY; 					//health bar y value will always be the same
	private float minX;					  // min position of visual health bar (green one)
	private float maxX;					  // max position of visual health bar
	private int currentHealth; 
	private int CurrentHealth{            // getters/setters for current health
		get {
			return currentHealth;}
		set {
			currentHealth=value;
			HandleHealth();               // if health is changing, need to handle it
		}
	}
	public int maxHealth = 100;                 // set to something like 100 in inspector
	public Text healthText;               // text to read Health: x
	public Image visualHealth;            // green bar that slides around
	public float coolDown;                // frame rate stuff
	private bool onCD;                    // also frame rate stuff


	public float armLength = 2;
	public bool dead = false;
	public Transform lastCheckpoint;
	public GameObject mouseOverObject;
	[HideInInspector]
	public ObjectData mouseOverObjectData;
	[HideInInspector]
	public bool showText = false;
	[HideInInspector]
	public string onscreenText;




	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		rayCastComp = gameObject.GetComponent<RayCaster> ();

		/* Health Bar Stuff */
		cachedY = healthTransform.position.y;  // y position of health bar- never changes
		maxX = healthTransform.position.x;     // x position- start at the max
		minX = healthTransform.position.x - healthTransform.rect.width; //cause it's a sliding bar
		currentHealth = maxHealth;             // start at the max
		onCD = false;



	}

	// Update is called once per frame
	void Update () {

				/* If health is zero, declare dead */
				if (currentHealth <= 0) {
						dead = true;
				}

				/* If player is dead */
				if (dead) {
						if (lastCheckpoint != null) {
								dead = false;
								CurrentHealth = 100;
								gameObject.transform.position = lastCheckpoint.position;
						} else {
								dead = false;
								CurrentHealth = 100;
								gameManager.RestartLevel ();
						}
				}

				/* if looking at something */
				if (rayCastComp.hit) {
					/* if object is new */
						if (mouseOverObject!= null && rayCastComp.gameObjectHit != mouseOverObject) {
								mouseOverObject.renderer.material.shader = mouseOverObjectData.initialShader;
								showText = false;
								mouseOverObject = null;
								mouseOverObjectData = null;
						}
						/* if has objectData */
						if (rayCastComp.gameObjectHit != null && rayCastComp.gameObjectHit.GetComponent<ObjectData> () != null) {
								mouseOverObject = rayCastComp.gameObjectHit;
								mouseOverObjectData = mouseOverObject.GetComponent<ObjectData> ();
								/* if within arms reach */
								if (rayCastComp.distanceToHit <= armLength) {
										mouseOverObjectData.withinArmsReach = true;
										if(mouseOverObjectData.pickupable){
											mouseOverObject.renderer.material.shader = mouseOverObjectData.highlight;
											onscreenText = mouseOverObjectData.overlayText;
											showText = true;
										}
								/* not within arms reach */
								} else {
										mouseOverObject.renderer.material.shader = mouseOverObjectData.initialShader;
										showText = false;
										mouseOverObject = null;
										mouseOverObjectData = null;
								}
						}
				/* not looking at something */
		} else {
			if(mouseOverObject != null){
				mouseOverObjectData.withinArmsReach = false;
				mouseOverObject = null;
				mouseOverObjectData = null;
			}
				
						
		}	
	}

	
	/* so that we are not taking damage every frame */
	IEnumerator CoolDownDmg(){
		onCD = true;
		yield return new WaitForSeconds (coolDown);
		onCD = false;
	}

	/* Function to handle health including:
	 Health Text (i.e. Health: 100)
	 Changing the color of the bar so it's more yellow in the middle, red when we're low */
	private void HandleHealth(){
		healthText.text = "Health: " + currentHealth;
		float currentXValue = MapValues (currentHealth, 0, maxHealth, minX, maxX); // need to know where to put the slider
		healthTransform.position=new Vector3(currentXValue, cachedY); // and put the slider there based on health
		if(currentHealth>maxHealth/2){
			visualHealth.color = new Color32((byte)MapValues(currentHealth, maxHealth/2,maxHealth, 255,0),255,0,255);
		}
		else{
			visualHealth.color = new Color32(255,(byte)MapValues(currentHealth,0,maxHealth/2,0,255),0,255);
		}

	}

	void OnTriggerStay(Collider other){
		if (other.name == "Enemy") {               // we've touched an enemy!
			if (!onCD && currentHealth > 0) {      // subtract health appropriately
				StartCoroutine(CoolDownDmg()); 
				CurrentHealth -=1;
			}
		}
		if (other.name == "Friend") {              // hey a friend!
			if (!onCD && currentHealth < maxHealth) {  // add health appropriately
				StartCoroutine (CoolDownDmg ());
				CurrentHealth += 1;
			}
		}
	}


	/* function to determine based on how much health we have, 
	 how far to move the health bar over */
	private float MapValues(float currentHealth, float minHealth, float maxHealth, float minXpos, float maxXpos){
			// minHealth=0, maxHealth=100
			// minXpos=-10 (health bar all the way moved to the left)
			// maxXpos=0 (health bar where it started at
			// linear interpolation
			return(currentHealth - minHealth) * (maxXpos - minXpos) / (maxHealth - minHealth) + minXpos; 
	}


	void OnGUI(){
		if (showText) {
			GUI.Label (new Rect((float)Screen.width/2,(float)Screen.height/1.5f,100,100), onscreenText);
		}
	}
}
