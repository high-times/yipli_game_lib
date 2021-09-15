#include "ResponsePackager.h"
#include "../support/Utils.h"


int ResponsePackager::m_responseCount = 0;

/*ResponsePackager::ResponsePackager(int _params)
{

}*/

void ResponsePackager::setResponsePackager(ActionIdentifierTable _actionID)
{
	m_actionId = _actionID;
}


void ResponsePackager::setResponsePackager(ActionIdentifierTable _actionID, std::map<std::string, std::string> _property)
{
   /*
   {
        "count": 1,
            "timestamp": 1597237057689,
            "playerdata": [# Array containing player data
        {
          "id": 1,                         # Player ID(For Single - player - 1 , Multiplayer it could be 1 or 2)
          "fmresponse": {
            "action_id": "9D6O",           # Action ID - Unique ID for each action.Refer below table for all action IDs
            "action_name": "Jump",         # Action Name for debugging(Gamers should strictly check action ID)
            "properties": "null"           # Any properties action has - ex.Running could have Step Count, Speed
          }
        }
            ]
    }
    */
    m_actionId = _actionID;
    m_properties = _property;
}






std::string ResponsePackager::packageFMresponse(std::string _playersData)
{
    std::string FMResponse = "{ \"count\":" + std::to_string(++m_responseCount) +
                             ", \"timestamp\":" + std::to_string(Utils::getCurrentTimestamp()) +
                             ", \"playerdata\":["+_playersData+"]}";
    
	return FMResponse;
}

void ResponsePackager::setPlayerData(int _playerCount)
{
    std::string FMplayer = "{\"id\":"+ std::to_string(_playerCount) +
                            ", \"fmresponse\":{\"action_id\":\""+ ActionIdentifier::getActionID(m_actionId) +
                            "\", \"action_name\" : \""+ ActionIdentifier::getActionName(m_actionId) +"\"" ;
    
    std::string properties;
    if(m_properties.size() == 0){
        properties = ", \"properties\": \"null\"";
    }else{
        int j = 0;
        properties += ", \"properties\": \"";
        for ( const auto &myPair : m_properties ) {
            if(j++!=0)
                properties+=",";
            std::cout << myPair.first << "\n";
            properties += myPair.first + ":" + myPair.second;
        }
        properties += "\"";
    }

    FMplayer += properties;
    FMplayer +=  "}}";
    m_player = FMplayer;

}

void ResponsePackager::resestVariables()
{
    m_actionId = ActionIdentifierTable::NULL_ID;
    m_actionName = "";
    m_player = "";
    m_properties = {};
}


