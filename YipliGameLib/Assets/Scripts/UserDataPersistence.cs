using UnityEngine;

public static class UserDataPersistence
{
    public static void SavePropertyValue(string strProperty, string strValue)
    {
        if (strValue.Length > 0)
        {
            PlayerPrefs.SetString(strProperty, strValue);
        }
    }

    public static string GetPropertyValue(string strProperty)
    {
        if(PlayerPrefs.HasKey(strProperty) && PlayerPrefs.GetString(strProperty).Length > 0)
            return PlayerPrefs.GetString(strProperty);
        return null;
    }

    public static void SavePlayerToDevice(YipliPlayerInfo playerInfo)
    {
        Debug.Log("Saving player to device with properties : " + playerInfo.playerId + " " + playerInfo.playerName + " " + playerInfo.playerDob + " " + playerInfo.playerHeight + " " + playerInfo.playerWeight);
        SavePropertyValue("player-id", playerInfo.playerId);
        SavePropertyValue("player-name", playerInfo.playerName);
        SavePropertyValue("player-dob", playerInfo.playerDob);
        SavePropertyValue("player-height", playerInfo.playerHeight);
        SavePropertyValue("player-weight", playerInfo.playerWeight);
        PlayerPrefs.Save();
    }

    public static YipliPlayerInfo GetSavedPlayer()
    {
        Debug.Log("Getting saved player from device.");
        if(GetPropertyValue("player-id") != null && GetPropertyValue("player-name") != null)
            return new YipliPlayerInfo(GetPropertyValue("player-id"),
                GetPropertyValue("player-name"),
                GetPropertyValue("player-dob"), 
                GetPropertyValue("player-height"), 
                GetPropertyValue("player-weight"));

        Debug.Log("Return null for GetSavedPlayer");
        return null;
    }

    public static void SaveMatToDevice(YipliMatInfo matInfo)
    {
        Debug.Log("Saving mat to device with properties : " + matInfo.matId + " " + matInfo.macAddress);
        SavePropertyValue("mat-id", matInfo.matId);
        SavePropertyValue("mac-address", matInfo.macAddress);
        PlayerPrefs.Save();
    }

    public static YipliMatInfo GetSavedMat()
    {
        Debug.Log("Getting saved mat from device.");
        if (GetPropertyValue("mat-id") != null && GetPropertyValue("mac-address") != null)
        {
            return new YipliMatInfo(GetPropertyValue("mat-id"),
                GetPropertyValue("mac-address"));
        }

        Debug.Log("Return null for GetSavedMat");
        return null;
    }
}
