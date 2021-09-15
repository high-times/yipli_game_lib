//
// Created by admin on 25-01-2021.
//

#include "DriverControl.h"
#include <iostream>
#include "support/ClusterIdentifier.h"

int DriverControl::playerCounter = 0;
int DriverControl::gameMode = SINGLE_PLAYER;

DriverControl::DriverControl() {
    FMLOG(VERBOSE, "DriverControl", "Constructor created..");
}

DriverControl::DriverControl(int _playerCount) {
    //FMLOG(VERBOSE, "DriverControl", "Constructor created..");
    //std::cout << "DriverControl()" << std::endl;
    //processor = Processor();
    //responsePackager = ResponsePackager();
    transpose = Transpose();
    playerCounter = _playerCount;
    playerID = playerCounter;
}


bool DriverControl::initDriverProcessing(std::string _FMData) {

    bool MAT_ARRAY[ MAT_SIZE ];
    FMData = _FMData;

    if( FMData != LastFMData){

        //Producing FMData

        //Utils::resetTable(MAT_ARRAY);
        transpose.parseHexString(MAT_ARRAY, FMData);
        getMATArrayBasedOnPlayerType(MAT_ARRAY);
        Utils::ghostMatAlgo(MAT_ARRAY);

        // Setting flags
        LastFMData = FMData;
        newFrameFlag = false;
        gamePaused = false;


        if(processor.foundNewFrame(MAT_ARRAY, persistentBlobs, MATPixelCount, getTotalPlayerCount()))
            processor.removeNoise(persistentBlobs);


        //Process cluster
        cluster.clusterCheckSequence(responsePackager);

        if( cluster.getClusterID() == ClusterID::TROUBLESHOOTING ){

            std::map<std::string, std::string> property;
            std::string mat_array_string;
            for (int i: MAT_ARRAY) {
                mat_array_string += std::to_string(i);
            }

            property.emplace("array", mat_array_string);

            responsePackager.setResponsePackager( ActionIdentifier::getActionName(responsePackager.m_actionId) == "null" ? ActionIdentifierTable::TROUBLESHOOTING : ActionIdentifierTable::JUMPING_JACK, property);
            responsePackager.setPlayerData(playerID);
            responseCount++;
            return true;

        }

        if ( ActionIdentifier::getActionName(responsePackager.m_actionId) != "null" ) {
            responsePackager.setPlayerData(playerID);
            responseCount++;

            //Log.i("FMResponse :", ActionIdentifiers.getActionName(responsePackager.ActionId));
            return true;
        }else{
            return false;
        }


    }

    else{


        if (!newFrameFlag) {
            pauseTimestamp = Utils::getCurrentTimestamp();
            newFrameFlag = true;

            //Log.i("MAT_PAUSE :", ""+timerHistoryPause1);
        }
        else{



            //----------- NEW CHECK FOR NO OF ACTIVE PIXELS -----------------
            setActiveMATPixelCount(0);
            transpose.parseHexString(MAT_ARRAY, FMData);
            int col = MAT_COLUMNS;
            int row = MAT_ROWS;
            for (int i = 0; i < row; ++i) {
                for (int j = 0; j < col; ++j) {
                    if(MAT_ARRAY[i * col + j] == 1) {
                        //FMLOG(VERBOSE, "FMLog",("Setting pixel count : "+std::to_string(i)+" "+std::to_string(j)).c_str());
                        setActiveMATPixelCount(getActiveMATPixelCount() + 1);
                    }
                }
            }
            //----------------------------------------------------------------


            auto newPauseTimestamp = Utils::getCurrentTimestamp();

            auto diff =  (newPauseTimestamp - pauseTimestamp);
            FMLOG(VERBOSE, "FMResponse",("Pause Stats-"+std::to_string(getActiveMATPixelCount())+ std::string(" ")+std::to_string(diff)).c_str());

            if (diff >= static_cast<long>(Threshold::GAME_PAUSE_TIMESTAMP) && !gamePaused && getClusterID() != 0) {

                if(getActiveMATPixelCount() < static_cast<int>(Threshold::ALLOWED_SHORTED_PIXELS) ) {

                    gamePaused = true;
                    FMLOG(2, "FMResponse :", "GAME PAUSED");
                    responsePackager.setResponsePackager(ActionIdentifierTable::PAUSE);
                    responsePackager.setPlayerData(playerID);
                    responseCount++;

                    if(cluster.getClusterID() == static_cast<int>(JOYFUL_JUMP)){
                        cluster.m_games.runningStartedFlag = false;
                    }
                    return true;

                }
            }
            if(cluster.getClusterID() == static_cast<int>(JOYFUL_JUMP)){

                //if(getActiveMATPixelCount() < static_cast<int>(Threshold::ALLOWED_SHORTED_PIXELS))
                {
                    FMLOG(2, "FMResponse :", ("Wildcard entry to cluster "+ std::to_string(cluster.getClusterID())).c_str());
                    cluster.clusterCheckSequence(responsePackager);

                    if (ActionIdentifier::getActionName(responsePackager.m_actionId) != "null") {
                        responsePackager.setPlayerData(playerID);
                        responseCount++;
                        return true;
                    }else{
                        return false;
                    }
                }
            }
        }


    }
    return false;
}

void DriverControl::setClusterID(int GameID){
    cluster.setClusterID(GameID);
}

int DriverControl::getClusterID(){
    return cluster.getClusterID();
}

int DriverControl::getTotalPlayerCount() {
    return playerCounter;
}

void DriverControl::getMATArrayBasedOnPlayerType(bool* _matArray) {

    if (playerID == 2) {
        //std::copy(_matArray + (MAT_ROWS * MAT_COLUMNS)/2, _matArray + (MAT_ROWS * MAT_COLUMNS), _matArray);
        for(int i = (MAT_ROWS * MAT_COLUMNS)/2; i < (MAT_ROWS * MAT_COLUMNS); i++)
            _matArray[i - (MAT_ROWS * MAT_COLUMNS)/2] = _matArray[i];
    }
}


int DriverControl::getFeedSize() {
    return FEED_SIZE;
}



int DriverControl::getActiveMATPixelCount() {
    return MATPixelCount;
}

void DriverControl::setActiveMATPixelCount(int pixels) {
    MATPixelCount = pixels;
}

std::string DriverControl::getFMDriverVersion() {
    return DRIVER_VERSION;;
}



