using UnityEngine;
using System.Collections;

enum DragAndDropNotification
{
	MenuItemPressed, MapObjectPressed, ObjectPlaced, DoubleClicked, MapObjectRemoved, MenuItemRightClicked
};

public class DragAndDropController : MonoBehaviour 
{
	Vector3 lastMousePos;
	LevelCreator levelCreator;
	
	public GameObject playerSpherePrefab;
	
	public GameObject draggingObj;
	LevelAssetManager assetManager;

	GameObject selectedMenuItem;
	GameObject selectedMenuItemPrefab;
	GameObject selectedMenuItemHighlight;
	
	void Start()
	{
		levelCreator = GameObject.Find("LevelCreator").GetComponent<LevelCreator>();
		assetManager = GameObject.Find("LevelCreator").GetComponent<LevelAssetManager>();

		Messenger<GameObject>.AddListener(DragAndDropNotification.MenuItemPressed.ToString(), MenuItemPressed);
		Messenger<GameObject>.AddListener(DragAndDropNotification.MapObjectPressed.ToString(), MapObjectPressed);
		Messenger<Vector3>.AddListener(DragAndDropNotification.DoubleClicked.ToString(), DoubleClicked);
		Messenger<GameObject>.AddListener(DragAndDropNotification.MenuItemRightClicked.ToString(), MenuItemRightClicked);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateChangeData>.AddListener(LevelCreatorStateNotification.TestingMapEnter.ToString(), TestingMapEnter);
	}

	void OnDestroy()
	{
		Messenger<GameObject>.RemoveListener(DragAndDropNotification.MenuItemPressed.ToString(), MenuItemPressed);
		Messenger<GameObject>.RemoveListener(DragAndDropNotification.MapObjectPressed.ToString(), MapObjectPressed);
		Messenger<Vector3>.RemoveListener(DragAndDropNotification.DoubleClicked.ToString(), DoubleClicked);
		Messenger<GameObject>.RemoveListener(DragAndDropNotification.MenuItemRightClicked.ToString(), MenuItemRightClicked);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateChangeData>.RemoveListener(LevelCreatorStateNotification.TestingMapEnter.ToString(), TestingMapEnter);
	}
	
	void MenuItemPressed(GameObject prefab)
	{
		if(draggingObj != null)
			Destroy(draggingObj);
		
		draggingObj = (GameObject)Instantiate(prefab);
		draggingObj.name = draggingObj.name.Replace("(Clone)","");

		if(draggingObj.name.Contains("Door"))
		{
			SetupDoorPiece(draggingObj);
		}
	else if(draggingObj.name.Contains("PlayerStart"))
		{
			SetupStartPiece(draggingObj);
		}

		if(draggingObj.transform.childCount > 0)
		{
			foreach(Transform child in draggingObj.transform)
			{
				if(child.collider)
					child.collider.enabled = false;
			}
		}
		var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		worldPos.z = 2.5f;
		draggingObj.transform.position = worldPos;
	}

	void MenuItemRightClicked(GameObject prefab)
	{
		if(selectedMenuItemPrefab == prefab)
		{
			DeselectMenuItem();
		}
		else
		{
			DeselectMenuItem();

			SelectMenuItem(UICamera.lastHit.collider.gameObject);
			selectedMenuItemPrefab = prefab;
		}
	}

	void SelectMenuItem(GameObject newSelection)
	{
		var highlight = newSelection.transform.Find("HighlightSprite").gameObject;

		if(selectedMenuItemHighlight != null)
		{
			TweenAlpha.Begin(selectedMenuItemHighlight, 0.5f, 0.0f);
		}
		selectedMenuItemHighlight = highlight;
		selectedMenuItem = newSelection;

		TweenAlpha.Begin(selectedMenuItemHighlight, 0.5f, 1.0f);
	}

	void DeselectMenuItem()
	{
		if(selectedMenuItemHighlight != null)
		{
			TweenAlpha.Begin(selectedMenuItemHighlight, 0.5f, 0.0f);
			selectedMenuItemHighlight = null;
			selectedMenuItemPrefab = null;
		}
	}

	void MapObjectPressed(GameObject objPressed)
	{		
		if(draggingObj != null && objPressed != draggingObj)
		{
			Destroy(draggingObj);
		}

		ReplaceFloorPiece(objPressed.transform.position);
		
		draggingObj = objPressed;
	}

	void TestingMapEnter(StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateChangeData changeData)
	{
		DeselectMenuItem();
	}
	
	void Update()
	{
		if(draggingObj != null)
		{
			if(Input.GetMouseButtonUp(0))
			{
				FinishDragging();
				return;
			}
			if(Input.GetMouseButton(0))
			{
				if(lastMousePos != Vector3.zero)
				{
					var delta = Input.mousePosition - lastMousePos;
					DoDragging(delta);
				}
			}
			lastMousePos = Input.mousePosition;
		}
		if(selectedMenuItemPrefab != null)
		{
			if(UICamera.hoveredObject != null && UICamera.hoveredObject != selectedMenuItem)
			{
				if(Input.GetMouseButtonDown(1))
				{
					PlaceSelectedItem();
				}
			}
		}
	}
	
	void DoDragging(Vector3 delta)
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		int layerMask = 1 << 10;
		
		if(draggingObj != null)
		{
			if(draggingObj.transform.localScale.x < 1.1f)
				draggingObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			
			if(draggingObj.transform.childCount > 0)
			{
				foreach(Transform child in draggingObj.transform)
				{
					child.gameObject.layer = 11;
				}
			}
			draggingObj.layer = 11;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				if(hit.collider != null)
				{
					draggingObj.transform.position = hit.collider.transform.position;
				}
			}
			else
			{
				draggingObj.transform.position += (Vector3)(delta * Time.deltaTime);
				draggingObj.transform.parent = null;
			}
		}
	}
	
	void FinishDragging()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(lastMousePos);

		int layerMask = 1 << 10;
		
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if(hit.collider != null)
			{
				GameObject objToReplace = hit.collider.gameObject;

				if(draggingObj.GetComponent<DraggableMapObject>() == null)
				{
					draggingObj.AddComponent<DraggableMapObject>();
				}

				if(draggingObj.transform.childCount > 0)
				{
					foreach(Transform child in draggingObj.transform)
					{
						child.gameObject.layer = 10;
					}
				}
				draggingObj.layer = 10;
				draggingObj.transform.localScale = Vector3.one;
				draggingObj.transform.parent = levelCreator.mapRoot.transform;
				Messenger<GameObject>.Invoke(DragAndDropNotification.ObjectPlaced.ToString(), draggingObj);
				draggingObj = null;
				Destroy(objToReplace);
			}
		}
		else
		{
			Destroy(draggingObj);
		}
	}

	void PlaceSelectedItem()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 10))
		{
			if(hit.collider != null)
			{
				var objToReplace = hit.collider.gameObject;

				var clone = (GameObject)Instantiate(selectedMenuItemPrefab);
				clone.name = clone.name.Remove(clone.name.IndexOf("(Clone)"), 7);

				if(clone.name.Contains("Door"))
				{
					SetupDoorPiece(clone);
				}
				else if(clone.name.Contains("PlayerStart"))
				{
					SetupStartPiece(clone);
					DeselectMenuItem();
				}
				else if(clone.name.Contains("End"))
				{
					DeselectMenuItem();
				}
				else if(clone.GetComponent<DraggableMapObject>() == null)
				{
					clone.AddComponent<DraggableMapObject>();
				}


				if(clone.transform.childCount > 0)
				{
					foreach(Transform child in clone.transform)
					{
						child.gameObject.layer = 10;
						if(child.collider)
						{
							child.collider.enabled = false;
						}
					}
				}
				clone.layer = 10;
				clone.transform.localScale = Vector3.one;
				clone.transform.position = hit.collider.transform.position;
				clone.transform.parent = levelCreator.mapRoot.transform;
				Messenger<GameObject>.Invoke(DragAndDropNotification.ObjectPlaced.ToString(), clone);

				Destroy(objToReplace);
			}
		}
	}

	void SetupDoorPiece(GameObject doorGo)
	{
		var doorMenuPieceGo = GameObject.Find("DoorCubeMenuObj");

		var doorMenuPiece = doorMenuPieceGo.GetComponent<DoorPiece>();
		var doorPiece = doorGo.GetComponent<DoorPiece>();

		doorPiece.SetDoorColour(doorMenuPiece.theDoor.objColour);
		doorPiece.theDoor.initialColour = doorMenuPiece.theDoor.objColour;
		doorPiece.ChangeColour(doorMenuPiece.objColour);
		doorGo.AddComponent<DraggableRotatableMapObject>();
	}

	void SetupStartPiece(GameObject startGo)
	{
		var menuPlayerSphere = GameObject.Find("PlayerStartCubeMenuObj");
		var menuPlayerObj = menuPlayerSphere.GetComponent<PlayerStartPiece>();

		var playerSphere = (GameObject)Instantiate(playerSpherePrefab);

		var playerCharObj = playerSphere.GetComponent<PlayerCharacter>();

		playerSphere.transform.parent = startGo.transform;
		playerSphere.transform.localPosition = new Vector3(0, 1.3f, 0);
		playerCharObj.rigidbody.useGravity = false;
		playerCharObj.playerMovement.canMove = false;

		startGo.GetComponent<PlayerStartPiece>().ChangeColour(menuPlayerObj.objColour);
		playerSphere.collider.enabled = false;
		playerCharObj.SilentlyChangeColour(menuPlayerObj.objColour);
		startGo.AddComponent<DraggableMapObject>();
	}

	void SetupEndPiece(GameObject endGo)
	{

	}
	
	void ReplaceFloorPiece(Vector3 pos)
	{
		GameObject nullPiece = (GameObject)Instantiate(assetManager.nullCubePrefab, pos, Quaternion.identity);
		nullPiece.transform.parent = levelCreator.mapRoot.transform;
		nullPiece.name = nullPiece.name.Replace("(Clone)", "");
	}

	void DoubleClicked(Vector3 pos)
	{
		ReplaceFloorPiece (pos);
	}
}
