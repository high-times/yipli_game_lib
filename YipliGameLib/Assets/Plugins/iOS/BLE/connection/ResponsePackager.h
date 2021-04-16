#pragma once
#ifndef FMINTERFACE_RESPONSE_PACKAGER_H
#define FMINTERFACE_RESPONSE_PACKAGER_H

#include<map>
#include <string>

#include "../support/Utils.h"
#include "../support/ActionIdentifier.h"

class ResponsePackager {

public:
    static int m_responseCount;
    std::string m_player{ "" };;

    ActionIdentifierTable m_actionId;
    std::string m_actionName{ "" };
    std::map<std::string, std::string> m_properties;
    

    //ResponsePackager(int _params);
    void setResponsePackager(ActionIdentifierTable _actionID, std::map<std::string, std::string> _property);
    void setResponsePackager(ActionIdentifierTable _actionID); 
    static std::string packageFMresponse(std::string _playersData);
    void setPlayerData(int _playerCount);
    void resestVariables();


};
#endif //FMINTERFACE_RESPONSE_PACKAGER_H
