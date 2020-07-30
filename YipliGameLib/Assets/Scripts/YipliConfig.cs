using UnityEngine;


[CreateAssetMenu]
public class YipliConfig : ScriptableObject
{
    [HideInInspector]
    public string callbackLevel;

    [HideInInspector]
    public YipliPlayerInfo playerInfo;

    [HideInInspector]
    public YipliMatInfo matInfo;

    [HideInInspector]
    public string userId;
}
