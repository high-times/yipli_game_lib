using Firebase.Database;
using System;
using UnityEngine;

public class YipliPlayerInfo
{
    public string playerName;
    public string playerId;
    public string playerDob;
    public string playerAge; //Current age of the player
    public string playerHeight; //Current height of the player
    public string playerWeight; //Current height of the player
    //public string playerExpertyLevel;//The Experty level of the player at time of playing the game.
    public string gender;
    public string profilePicUrl;
    //public string difficultyLevel; // to be decided by the game.

    public YipliPlayerInfo() { }

    public YipliPlayerInfo(string playerId, string playerName, string playerDob, string playerHeight, string playerWeight, string gender = "")
    {
        this.playerId = playerId;
        this.playerName = char.ToUpper(playerName[0]) + playerName.Substring(1);
        this.playerDob = playerDob;
        if (playerDob == null || playerDob == "")
        {
            playerAge = "";
        }
        else
        {
            Debug.Log("Calculation player age for DOB : " + playerDob);
            playerAge = CalculateAge(playerDob);
        }
        this.playerHeight = playerHeight;
        this.playerWeight = playerWeight;
        this.gender = gender;
    }

    public YipliPlayerInfo(DataSnapshot snapshot, string key)
    {
        try
        {
            if (snapshot != null)
            {
                Debug.Log("filling the YipliPlayerInfo from Snapshot.");
                playerId = key.ToString();
                playerName = snapshot.Child("name").Value?.ToString() ?? "";
                playerName = char.ToUpper(playerName[0]) + playerName.Substring(1);
                playerWeight = snapshot.Child("weight").Value?.ToString() ?? "";
                playerHeight = snapshot.Child("height").Value?.ToString() ?? "";
                playerDob = snapshot.Child("dob").Value?.ToString() ?? "";

                //DOB is stored in the format "mm-dd-yyyy" in the backend
                Debug.Log("DOB recieved from backend : " + playerDob);
                if (playerDob == "")
                {
                    Debug.Log("Player age is null.");
                    playerAge = "";
                }
                else
                {
                    playerAge = CalculateAge(playerDob);
                    Debug.Log("Got Player age : " + playerAge);
                }
                gender = snapshot.Child("gender").Value?.ToString() ?? "";

                profilePicUrl = snapshot.Child("profile-pic-url").Value?.ToString() ?? "";

                //If playername is not found, set PlayerId to null
                if (playerName== "")
                {
                    playerId = null;
                }

                Debug.Log("Player Found with details :" + playerAge + " " + playerHeight + " " + playerId + " " + playerWeight + " " + playerName + " ProfilePicUrl:" + profilePicUrl);
            }
            else
            {
                Debug.Log("DataSnapshot is null. Can't create YipliPlayerInfo instance.");
                playerId = null;
            }
        }
        catch (Exception exp)
        {
            Debug.Log("Exception in creating YipliPlayerInfo object from DataSnapshot : " + exp.Message);
            playerId = null;
        }
    }

    private string CalculateAge(string strDob /* 'mm-dd-yyyy' format */)
    {
        DateTime now = DateTime.Now;

        DateTime dob = DateTime.Parse(strDob);

        var years = now.Year - dob.Year;

        if (now.Month < dob.Month)
        {
            years--;
        }

        if (now.Month == dob.Month)
        {
            if (now.Day < dob.Day)
            {
                years--;
            }
        }

        return years.ToString();
    }
}

public class YipliMatInfo
{
    public string matName;
    public string matId;
    public string macAddress;

    public YipliMatInfo() { }

    public YipliMatInfo(string matId, string macAddress)
    {
        this.matId = matId;
        this.macAddress = macAddress;
    }

    public YipliMatInfo(DataSnapshot snapshot, string key)
    {
        try
        {
            if (snapshot != null)
            {
                Debug.Log("filling the YipliPlayerInfo from Snapshot.");
                matId = key.ToString();
                matName = snapshot.Child("display-name").Value?.ToString() ?? "";
                macAddress = snapshot.Child("mac-address").Value?.ToString() ?? "";
                Debug.Log("Mat Found with details :" + matName + " " + macAddress + " " + matId);
            }
            else
            {
                Debug.Log("DataSnapshot is null. Can't create YipliMatInfo instance.");
                matId = null;
            }
        }
        catch (Exception exp)
        {
            Debug.Log("Exception in creating YipliMatInfo object from DataSnapshot : " + exp.Message);
            matId = null;
        }
    }
}

