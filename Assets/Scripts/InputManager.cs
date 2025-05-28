using UnityEngine;
using System.Collections.Generic;


public static class InputManager
{
    public static KeyCode RecallKey = KeyCode.R;
    public static KeyCode ExecuteKey = KeyCode.E; 
    public static KeyCode ConvertKey = KeyCode.F;

    public static bool RecallKeyPress()
    {
        return Input.GetKeyDown(RecallKey);
    }

    public static Dictionary<PlayerAction, KeyCode> actionKeys = new Dictionary<PlayerAction, KeyCode>()
    {
        {PlayerAction.Recall, RecallKey},
        {PlayerAction.Execute, ExecuteKey},
        {PlayerAction.Convert, ConvertKey}
    };

    public static string GetKeyForAction(PlayerAction _action)
    {
        if(actionKeys.TryGetValue(_action, out KeyCode key))
        {
        return key.ToString();
        }
        else{
            return "Undefined"; 
        }
    }

    public enum PlayerAction
    {
        Recall,
        Execute,
        Convert
    }
}