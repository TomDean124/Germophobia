using UnityEngine;

public interface IInteractable
{
	void OnExecute(); //Call when the execute choice is picked
	void OnConvert(Spawner _spawner); //Call when the convert choice is picked.
	string GetInteractionPrompt(); //Returns the prompt for execute or convert.
}
