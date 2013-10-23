using UnityEngine;
using System.Collections;

public class DialogueDisplayer : MonoBehaviour 
{
	public int charsPerSecond;

	UILabel uiLabel;
	string text;
	int offSet = 0;

	void Start()
	{
		uiLabel = GetComponent<UILabel>();
		uiLabel.supportEncoding = false;
		uiLabel.symbolStyle = UIFont.SymbolStyle.None;
		text = uiLabel.font.WrapText(uiLabel.text, uiLabel.lineWidth / uiLabel.cachedTransform.localScale.x, uiLabel.maxLineCount, false, UIFont.SymbolStyle.None);
	}

	/// <summary>
	/// Reads through the dialogue and displays it on screen.
	/// Must be matched with calls to StopDisplayingText()
	/// </summary>
	/// <returns>An array of lines of text.</returns>
	/// <param name="textLines">Text lines.</param>
	public IEnumerator DisplayText(string[] textLines)
	{
		foreach(var text in textLines)
		{
			while(offSet < text.Length)
			{
				charsPerSecond = Mathf.Max(1, charsPerSecond);

				// Periods and end-of-line characters should pause for a longer time.
				float delay = (float)charsPerSecond / 100f;

				char c = text[offSet];
				if(c == '.' || c == '\n' || c == '!' || c == '?')
				{
					delay *= 2f;
				}

				uiLabel.text = text.Substring(0, ++offSet);
				yield return new WaitForSeconds(delay);
			}
			uiLabel.text = "";
			offSet = 0;
		}
	}

	public void StopDisplayingText()
	{
		StopAllCoroutines();
		uiLabel.text = "";
		offSet = 0;
	}
}
