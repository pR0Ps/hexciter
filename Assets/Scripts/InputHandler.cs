using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {
	
	public static InputHandler Instance {get; private set;} // Singleton Instance
	
	#region Public Fields
	public Vector2 inputSignalDelta;	
	public bool inputSignalHold;
	public bool inputSignalDown;
	public bool inputSignalUp;
	public Vector2 inputVector;
	
	public float inputEndMagnitude; // the distance of the most recent input drag
	
	public Vector2 inputPosition;
	public float inputZoom;
	#endregion

	#region Private Fields
	private bool oneTouchLastFrame;
	
	private Vector2 inputPrevFrame; // inputVector for previous frame
	private Vector2 inputOrigin; // inputVector when input (click or single finger tap) is received
	
	private float pinchDistance; // distance between two fingers
	private float previousPinchDistance; // distance between two fingers last frame
	private float deltaPinch; // delta distance between two frames
	private float quarterScreenHypotenuse; // used for pinch precision
	
	private bool twoTouchLock; // A bool to make a single touch after a two touch ignored until zero touches
	//Used so that when the user releases both fingers after zooming, they don't fire off a signle unintended touch
	
	private bool inputSignalDownLastFrame;
	private bool inputSignalUpLastFrame;
	
	private float totalPinch;
	public bool useMobile = false;
	#endregion

	const int LAYER_MASK = 768;
	
	void Awake () {
		Instance = this;
		quarterScreenHypotenuse = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height)/8f;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
			useMobile = true;
		else
			useMobile = false;
	}

	void Update () {
		
		inputSignalDown = false;
		inputSignalUp = false;
		
		if (useMobile)
			MobileInput();
		else
			PCInput();
		
		//Clamp the zoom value: 70% out 170% in
		//what is this inputZoom = Mathf.Clamp(inputZoom, -.7f, 1.3f);

		//Deal with a touch/click start
		if (inputSignalDown) {
			inputOrigin = inputVector;
			inputPrevFrame = inputVector;	
		}	

		//Deal with a touch/click release
		if (inputSignalUp) {
			inputEndMagnitude = Vector2.Distance(inputOrigin, inputVector);
		}
		
		//Determine delta position from start of input until now
		if (inputSignalHold) {
			inputSignalDelta = (inputVector - inputPrevFrame);// * (1 - inputZoom/3f);
			inputPrevFrame = inputVector;
		}
		else
			inputSignalDelta = Vector2.zero;

		Interactions ();

		//Put this somewhere else?
		//Android back button
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Application.loadedLevelName == "game"){
				Application.LoadLevel("menu");
			}
			else if (Application.loadedLevelName == "menu"){
				Debug.Log("Exiting");
				Application.Quit();
			}
		}
	}

	private GameObject downObject; // this is the object last clicked down on, use when you need to know if a full click (up and down) on the same object was performed

	// put all the clicking/releasing for classes that inherit InteractableObject here
	// the physics2D overlap will ONLY detect objects which are on the "Interactable" layer
	// in this way, you can always infer the the collider returned is something that inherits InteractiveObject
	void Interactions () {
		if (inputSignalDown) {
			Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(inputVector), LAYER_MASK);
			if (col) {
				col.GetComponent<InteractiveObject>().DownAction();
				downObject = col.gameObject;
			}
		}
		if (inputSignalUp) {
			Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(inputVector), LAYER_MASK);
			if (col) {
				col.GetComponent<InteractiveObject>().UpAction();
				if (col.gameObject == downObject) {
					col.GetComponent<InteractiveObject>().TapAction();
				}
			}
			downObject = null;
		}
	}
	
	void MobileInput () {
		if (Input.touchCount == 0) {
			twoTouchLock = false;
			totalPinch = 0;
			if (oneTouchLastFrame) {
				inputSignalUp = true;
			}
		}
		
		if (Input.touchCount == 1 && !twoTouchLock) {
			inputVector = Input.touches[0].position;
			inputSignalHold = true;
			if (!oneTouchLastFrame) {		
				inputSignalDown = true;
				oneTouchLastFrame = true;
			}		
		}
		else {
			inputSignalHold = false;
			oneTouchLastFrame = false;
		}
		
		if (Input.touchCount == 2) {
			twoTouchLock = true;
			pinchDistance = Vector2.Distance(Input.touches[0].position,Input.touches[1].position);
			if (Input.touches[1].phase == TouchPhase.Began)
				previousPinchDistance = pinchDistance;
			deltaPinch = pinchDistance - previousPinchDistance;
			previousPinchDistance = pinchDistance;		
			inputZoom += deltaPinch/quarterScreenHypotenuse;		
			totalPinch += deltaPinch;
		}
	}
	
	void PCInput () {
		inputVector = Input.mousePosition;
		inputSignalDown = Input.GetMouseButtonDown(0);
		inputSignalHold = Input.GetMouseButton(0);
		inputSignalUp = Input.GetMouseButtonUp(0);
		inputZoom += Input.GetAxis("Mouse ScrollWheel");
	}
}
