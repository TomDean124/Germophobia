using UnityEngine;
using System.Collections.Generic;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private FactoryTypeSO _factoryType;
    [SerializeField] private Spawner _spawner; 

    public void OnExecute()
    {
        Debug.Log("Executed interaction!" + this.gameObject.name);
        Destroy(this.gameObject); //Generic, change to a more detailed death system later. 
    }

    public void OnConvert(Spawner _spawner)
    {
        if(_factoryType != null && _spawner != null)
        {
        Vector2 _pos = this.gameObject.transform.position;
        Debug.Log("Converting Confirmed");
        _spawner.SpawnFactoryObject(_factoryType, _pos);
        Destroy(this.gameObject);
        }
        else{
            Debug.LogError("Interaction Script is missing the _factoryType Reference or _spawner reference scripts");
        }
    }

    public string GetInteractionPrompt()
    {
        if (_factoryType != null)
        {
            return $"Convert to: {_factoryType.displayName}";
        }
        else
        {
            return "Interact";
        }
    }
}
