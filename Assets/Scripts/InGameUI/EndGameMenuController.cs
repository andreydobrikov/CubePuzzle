using UnityEngine;
using System.Collections;



public class EndGameMenuController : MonoBehaviour 
{
	public GameObject endGameMenuPanel;
	
	void Start()
	{
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.EndGameEnter);
		
		NotificationCenter<EndGameMenuNotification>.DefaultCenter.AddObserver(this, EndGameMenuNotification.NextLevelPressed);
		NotificationCenter<EndGameMenuNotification>.DefaultCenter.AddObserver(this, EndGameMenuNotification.QuitAndSavePressed);
		NotificationCenter<EndGameMenuNotification>.DefaultCenter.AddObserver(this, EndGameMenuNotification.QuitPressed); 
	}
	
	void EndGameEnter()
	{
		NGUITools.SetActive(endGameMenuPanel, true);
		
		if(!LevelController.Instance.isStoryMode)
		{
			var nextLevelButton = endGameMenuPanel.transform.Find("NextLevelButton").gameObject;
			var quitAndSaveButton = endGameMenuPanel.transform.Find("QuitAndSaveButton").gameObject;
			
			NGUITools.SetActive(nextLevelButton, false);
			NGUITools.SetActive(quitAndSaveButton, false);
		}
	}
	
	void NextLevelPressed()
	{
		
	}
	
	void QuitAndSavePressed()
	{
		
	}
	
	void QuitPressed()
	{
		Application.LoadLevel("FrontMenu");
		if(Time.timeScale != 1)
			Time.timeScale = 1;
	}
}