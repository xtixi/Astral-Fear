//---------------------------------------------------------------------------------------------------------------
//This is a little program of Draw lines with physics. Enjoy!
//MouseDraw v1.0 by Unicorn
//---------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class DrawingPhysics : MonoBehaviour {

	public GameObject linePrefab;
	public Material lineMaterial;
	public GameObject colliderPrefab;
	private GameObject colliderClone;
	public float width = 1;
	private Camera cameras;
	private GameObject brushDrawingClone;
	private LineRenderer lineRenderer;
	private Vector3 initdrawPos;
	private Vector3 lastdrawPos;
	private Vector3 drawPos;
	private int vertexCount;
	private GameObject newLineMesh;
	private float cameraFar=20;
	private bool startDraw = false;
	private bool endDraw = false;
	public float destroyTime = 10;
	private GameObject lineCollection;
	public bool drawStable = false;
	public bool isDrawing;
	
	void Start (){
		//Find MainCamera
		if (cameras==null && GameObject.FindWithTag("MainCamera"))
		{
			cameras = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		}
		cameraFar = Mathf.Abs(cameras.transform.position.z-transform.position.z);
		
		lineCollection = GameObject.Find("PhysicLines");
		if (lineCollection == null)
		{
			lineCollection = new GameObject("PhysicLines");
		}
	}
	
	void Update ()
	{
		#if UNITY_EDITOR  ||  UNITY_STANDALONE  ||  UNITY_WEBPLAYER 
			startDraw = Input.GetKeyDown(KeyCode.Mouse0);
			endDraw = Input.GetKeyUp(KeyCode.Mouse0);
		#else  // For touch-devices
			if (Input.touchCount > 0) 
			{
				Touch touch = Input.GetTouch (0);
				startDraw = (touch.phase == TouchPhase.Began);
				endDraw = (touch.phase == TouchPhase.Ended);
			}
		#endif

		//When Start Draw
		if (startDraw)
		{
			if (isDrawing==false)
			{
				//Get first point
				initdrawPos=cameras.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraFar));
				//Create LineRenderer gameobject
				brushDrawingClone = Instantiate(linePrefab, initdrawPos, cameras.transform.rotation) as GameObject;
				lineRenderer = brushDrawingClone.GetComponent<LineRenderer>();
				lastdrawPos = initdrawPos;
				//Set drawing flag
				isDrawing = true;
				vertexCount = 1;
				//Create a new GameObject to be parent of LineRenderer and it's colliders
				newLineMesh = new GameObject ("newline");
				lineCollection = GameObject.Find("PhysicLines");
				if (lineCollection == null)
				{
					lineCollection = new GameObject("PhysicLines");
				}
				newLineMesh.transform.position = transform.position;
				newLineMesh.transform.SetParent(lineCollection.transform);
			}
		}
		//When End Draw
		if (endDraw)
		{
			//Set drawing flag
			isDrawing = false;
			//Set LineRenderer gameobject to be child of newLineMesh
			brushDrawingClone.transform.parent = newLineMesh.transform;
			//Add Rigidbody to newLineMesh
			Rigidbody newRigbody = newLineMesh.AddComponent<Rigidbody>();
			//Rigdbody constraints
			newRigbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
			//Hold leftShift to draw soild lines
			if (drawStable)
			{
				newRigbody.isKinematic = true;
			}
			//Add delay destroy
			Destroy(newLineMesh, destroyTime);
		}
		
		//Set LineRenderer and create it's colliders
		if (isDrawing && brushDrawingClone)
		{
			//Screen to WorldPoint
			drawPos = cameras.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraFar));
			//Calculate distance between this draw point and last
			float drawdistance = Vector3.Distance(drawPos, lastdrawPos);
			//When draw a new point which far enough
			if (drawdistance>=cameraFar*width/80)
			{
				//Add a new LineRenderer node
				vertexCount = vertexCount + 1;
				lineRenderer.positionCount = vertexCount;
				lineRenderer.SetPosition(vertexCount-1, brushDrawingClone.transform.InverseTransformPoint(drawPos));
				lineRenderer.startWidth = cameraFar*width/60;
				lineRenderer.endWidth = cameraFar*width/60;
				lineRenderer.sharedMaterial = lineMaterial;
				//Create colliders belongs to it
				colliderClone = Instantiate(colliderPrefab, (drawPos+lastdrawPos)/2, Quaternion.LookRotation(drawPos-lastdrawPos)) as GameObject;
				CapsuleCollider capsulecollider = colliderClone.GetComponent<CapsuleCollider>();
				capsulecollider.radius = cameraFar*width/120;
				capsulecollider.height = drawdistance;
				//Make colliders to be childs of newLineMesh
				colliderClone.transform.parent = newLineMesh.transform;
				//Save last draw point
				lastdrawPos = drawPos;
			}
		}
		
	}
	
	public void SetStable()
	{
		drawStable = !drawStable;
	}

	public void ClearLines()
	{
		Destroy(lineCollection);
	}
	
	//Draw white box, help you to locate the draw plane.
	void OnDrawGizmos() 
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube (transform.position, new Vector3(50, 50, 0));
	}
	
}

