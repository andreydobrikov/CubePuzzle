using UnityEngine;
using System;
using System.Collections;

public class DoorPiece : ColorCollisionObject 
{
	public Door theDoor;
	
	
	protected override void Start()
	{
		base.Start ();

		theDoor = GetComponentInChildren<Door>();
		
		CheckDoor();
	}
	
	public override void ChangeColour(Colour colorToChangeTo)
	{
		base.ChangeColour (colorToChangeTo);
		
		CheckDoor();
	}

	public override void RotateColour (bool forward)
	{
		base.RotateColour (forward);

		CheckDoor();
	}
	
	public void RotateDoorColour()
	{
		int currentColourIndex = (int)theDoor.objColour;
		var values = Enum.GetValues(typeof(Colour));

		currentColourIndex++;
			
		if(currentColourIndex == values.Length)
		{
			currentColourIndex = 1;
		}
		SetDoorColour((Colour)currentColourIndex);
	}
	
	public void SetDoorColour(Colour colourToSet)
	{		
		theDoor.objColour = colourToSet;
		theDoor.renderer.material.color = theDoor.GetObjectRealColor(theDoor.objColour);
		
		CheckDoor();
	}
	
	void CheckDoor()
	{
		if(theDoor.objColour == objColour)
		{			
			theDoor.OpenDoor();
		}
		else
		{
			theDoor.CloseDoor();
		}
	}
}