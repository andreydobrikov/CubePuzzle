using UnityEngine;
using System.Collections;

public enum ColorBlindMode
{
	None
};

public class ColorManager
{
	public static ColorBlindMode currentColorBlindMode;

	public static Color errorColor = new Color(1, 0.24f, 0.83f);

	public static Color GetObjectRealColor(Colour objColour)
	{
		switch(objColour)
		{
			case Colour.Red:
				return ConvertRed();
			case Colour.Green:
				return ConvertGreen();
			case Colour.Blue:
				return ConvertBlue();
			case Colour.None:
				return Color.white;
		}
		return errorColor;
	}

	static Color ConvertRed()
	{
		switch(currentColorBlindMode)
		{
			case ColorBlindMode.None:
				return Color.red;
		}
		return errorColor;
	}

	static Color ConvertGreen()
	{
		switch(currentColorBlindMode)
		{
			case ColorBlindMode.None:
				return Color.green;
		}
		return errorColor;
	}

	static Color ConvertBlue()
	{
		switch(currentColorBlindMode)
		{
			case ColorBlindMode.None:
				return Color.blue;
		}
		return errorColor;
	}
}