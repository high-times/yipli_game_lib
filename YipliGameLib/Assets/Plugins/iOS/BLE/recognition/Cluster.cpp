//
// Created by admin on 27-01-2021.
//

#include "Cluster.h"
#include "../support/ClusterIdentifier.h"
#include <iostream>
#include <map>



Cluster::Cluster(std::vector<Blob>& _persistentBlobs, int& _MATPixelCount): m_persistentBlobs(_persistentBlobs),m_matPixelCount(_MATPixelCount)
{
    FMLOG(VERBOSE, "Cluster", "Constructor created..");
}


int Cluster::getClusterID() 
{
    return m_clusterId;
}

void Cluster::setClusterID(int _clusterId) 
{
    m_clusterId = _clusterId;
}



ResponsePackager Cluster::clusterCheckSequence(ResponsePackager& responsePackager)
{

    switch (m_clusterId)
    {

        case MAT_CONTROLS:
        {
            if (m_games.recogniseJumpPattern())
            {
                //detectedText.setText((DETECTION_COUNT++) + " : Jumping");
                responsePackager.setResponsePackager(ActionIdentifierTable::ENTER);
                m_games.resetAllActionFlags();
                return responsePackager;
            }
            else if (m_games.recogniseControlButtonClicked())
            {
                responsePackager.setResponsePackager(m_games.ControlButtonType);
                m_games.resetAllActionFlags();
                return responsePackager;
            }
            break;
        }

        case JOYFUL_JUMP:
        {
            if (m_games.recogniseRunningJumpPattern())
            {
                m_games.timerHistoryRunningJump = 0;
                m_games.resetAllActionFlags();
                responsePackager.setResponsePackager(ActionIdentifierTable::JUMP);
                return responsePackager;
            }
            if (!m_games.runningStartedFlag)
            {
                if (m_games.recogniseRunningPattern())
                {
                    m_games.resetAllActionFlags();

                    m_games.runningStartTime = Utils::getCurrentTimestamp();

                    std::map<std::string, std::string> property;
                    property.emplace("speed", "0");
                    property.emplace("totalStepsCount", std::to_string(m_games.totalStepsCount));
                    responsePackager.setResponsePackager(ActionIdentifierTable::RUNNING, property);

                    return responsePackager;
                }
            }
            else if (m_games.runningStartedFlag)
            {
                if (m_games.recogniseRunningStopPattern())
                {
                    m_games.runningStartedFlag = false;
                    m_games.resetAllActionFlags();
                    responsePackager.setResponsePackager(ActionIdentifierTable::RUNNING_STOPPED);
                    return responsePackager;
                }
                else if (m_matPixelCount > 1)
                {
                    auto diff = (Utils::getCurrentTimestamp() - m_games.runningStartTime)/1000;


                    if (diff < 5)
                        diff = 4;

                    float speed = 0;
                    speed = (m_games.totalStepsCount * 0.7142) / diff;

                    //FMLOG(VERBOSE,"speed : ", (std::to_string(speed)+ " "+ std::to_string(diff)).c_str());
                    std::string returnSpeed(std::to_string(speed * 3.6));
                    std::map<std::string, std::string> property;
                    property.emplace("totalStepsCount", std::to_string(m_games.totalStepsCount));
                    property.emplace("speed", returnSpeed);
                    
                    responsePackager.setResponsePackager(ActionIdentifierTable::RUNNING, property);
                    return responsePackager;
                }
            }

            break;
        }

        case YIPLI_RUNNER:
        {
            if (m_games.recogniseJumpPattern())
            {
                m_games.resetAllActionFlags();
                responsePackager.setResponsePackager(ActionIdentifierTable::JUMP);
                return responsePackager;
            }
            else if (m_games.recogniseLegMovementPattern())
            {
                m_games.resetAllActionFlags();
                responsePackager.setResponsePackager(m_games.LegMovementType);
                return responsePackager;
            }
            break;
        }

        case SKATER:
        {

            if (m_games.recogniseInOutJumpingJackPattern())
            {
                responsePackager.setResponsePackager(m_games.inOutType);
                m_games.resetAllActionFlags();
                return responsePackager;


            }
            break;
        }

        case PENGUIN_POP: {
            if (m_games.recogniseJumpPattern()) {
                m_games.resetAllActionFlags();
                responsePackager.setResponsePackager(ActionIdentifierTable::JUMP);
                return responsePackager;

            }

            break;
        }

        case TILES:
        {
            if (m_games.recogniseFootMovement())
            {
                std::map<std::string, std::string> property;
                property.emplace("x1", std::to_string(m_games.footLocations[0]));
                property.emplace("x2", std::to_string(m_games.footLocations[1]));
                property.emplace("x3", std::to_string(m_games.footLocations[2]));
                property.emplace("x4", std::to_string(m_games.footLocations[3]));

                responsePackager.setResponsePackager(ActionIdentifierTable::TILES, property);
                m_games.resetAllActionFlags();
                return responsePackager;
            }

            break;
        }

        case TREE_WARRIOR:
        {
            if (m_games.recogniseJumpPattern())
            {
                responsePackager.setResponsePackager(ActionIdentifierTable::JUMP);
                m_games.resetAllActionFlags();
                return responsePackager;
            }
            else if (m_games.recogniseTapButtonClicked())
            {
                responsePackager.setResponsePackager(m_games.ControlButtonType);
                m_games.resetAllActionFlags();
                return responsePackager;
            }
            break;
        }

        case TOUCH_BUTTON:{
            if (m_games.recogniseTouchButtonClicked()) {

                //Log.i("DETECTED :", "Controls clicked : " + games.ControlButtonType);
                responsePackager.setResponsePackager(m_games.ControlButtonType);
                m_games.resetAllActionFlags();
                return responsePackager;
            }
            break;

        }

        case TROUBLESHOOTING:
        {
            if (m_games.recogniseJumpingJackPattern())
            {
                responsePackager.setResponsePackager(ActionIdentifierTable::JUMPING_JACK);
                m_games.resetAllActionFlags();
                return responsePackager;
            }

            break;
        }

        case LCK_02:{


            if (!m_games.runningStartedFlag)
            {
                if (m_games.recogniseRunningPattern())
                {
                    m_games.resetAllActionFlags();

                    m_games.runningStartTime = Utils::getCurrentTimestamp();

                    std::map<std::string, std::string> property;
                    property.emplace("speed", "0");
                    property.emplace("totalStepsCount", std::to_string(m_games.totalStepsCount));
                    responsePackager.setResponsePackager(ActionIdentifierTable::RUNNING, property);

                    return responsePackager;
                }
            }
            else
            {
                if (m_games.recogniseRunningStopPattern())
                {
                    m_games.runningStartedFlag = false;
                    m_games.resetAllActionFlags();
                    responsePackager.setResponsePackager(ActionIdentifierTable::RUNNING_STOPPED);
                    return responsePackager;
                }
                else if (m_matPixelCount > 1)
                {
                    auto diff = (Utils::getCurrentTimestamp() - m_games.runningStartTime)/1000;


                    if (diff < 5)
                        diff = 4;

                    float speed = 0;
                    speed = (m_games.totalStepsCount * 0.7142) / diff;

                    //FMLOG(VERBOSE,"speed : ", (std::to_string(speed)+ " "+ std::to_string(diff)).c_str());
                    std::string returnSpeed(std::to_string(speed * 3.6));
                    std::map<std::string, std::string> property;
                    property.emplace("totalStepsCount", std::to_string(m_games.totalStepsCount));
                    property.emplace("speed", returnSpeed);

                    responsePackager.setResponsePackager(ActionIdentifierTable::RUNNING, property);
                    return responsePackager;
                }
            }
            break;
        }

        case LCK_03:{

            if (m_games.recogniseHighKneeRunningPattern()) {
                m_games.resetAllActionFlags();
                responsePackager.setResponsePackager(ActionIdentifierTable::HIGH_KNEE_RUN);
                return responsePackager;
            }

            break;
        }

        case LCK_04:{
            if (m_games.recogniseSkierJumpingJackPattern()) {
                m_games.resetAllActionFlags();
                responsePackager.setResponsePackager(ActionIdentifierTable::SKIER_JACK);
                return responsePackager;
            }

            break;
        }

        case LCK_05:{
            if (m_games.recogniseJumpPattern()) {
                m_games.resetAllActionFlags();
                responsePackager.setResponsePackager(ActionIdentifierTable::JUMP);
                return responsePackager;
            }
            break;
        }

        case LCK_07: {
            if (m_games.recogniseNinjaKickPattern()) {
                m_games.resetAllActionFlags();
                responsePackager.setResponsePackager(ActionIdentifierTable::NINJA_KICKS);
                return responsePackager;
            }
            break;
        }

        case LCK_11:{
            if (m_games.recogniseHoppingPattern()) {
                std::string legType = m_games.hoppingLegType;
                m_games.resetAllActionFlags();
                if(legType=="Right") {
                    responsePackager.setResponsePackager(ActionIdentifierTable::RIGHT_LEG_HOPPING);
                }
                else{
                    responsePackager.setResponsePackager(ActionIdentifierTable::LEFT_LEG_HOPPING);
                }
                return responsePackager;

            }
            break;
        }

            /*  case THE_RAFT:
              {
                  if (m_games.recogniseHoppingPattern())
                  {
                      std::string legType = m_games.hoppingLegType;
                      m_games.resetAllActionFlags();
                      if (legType == "Right")
                      {
                          responsePackager.setResponsePackager(ActionIdentifierTable::RIGHT_LEG_HOPPING);
                      }
                      else
                      {
                          responsePackager.setResponsePackager(ActionIdentifierTable::LEFT_LEG_HOPPING);
                      }
                      return responsePackager;
                  }

                  else if (m_games.recogniseJumpPattern())
                  {
                      m_games.resetAllActionFlags();
                      responsePackager.setResponsePackager(ActionIdentifierTable::JUMP);
                      return responsePackager;
                  }
                  break;
              }*/




    }
    return responsePackager;
    
}
