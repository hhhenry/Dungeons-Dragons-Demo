using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using SimpleJSON;

public class Language : MonoBehaviour
{
    public int GetLanguage(){
        return Application.systemLanguage switch
        {
            SystemLanguage.English => 0,
            SystemLanguage.Chinese => 1,
            _ => 0,
        };
    }
}