#pragma once

#include <string>
#include <vector>

#ifndef FMINTERFACE_DRIVERCONTROL_H
#define FMINTERFACE_DRIVERCONTROL_H

//#define DRIVER_VERSION "CPP_TV_0.0.10"
#define DRIVER_VERSION "2.7.4"

#include "blobprocessor/Processor.h"
#include "blobprocessor/Blob.h"
#include "recognition/Cluster.h"
#include "parser/Transpose.h"
#include "support/Const.h"
#include "support/Utils.h"

const int MULTI_PLAYER = 0;
const int SINGLE_PLAYER = 1;


class DriverControl {

public:

    /*********************
    * Member variables
    *********************/
    std::string FMData{ "" };
    std::string LastFMData{ "" };
    std::vector<Blob> persistentBlobs;
    std::string FMResponse{ "" };
    int MATPixelCount{ 0 };
    static int gameMode;
    int playerID{ 0 };
    static int playerCounter;
    int responseCount{ 0 };
    bool newFrameFlag{ false };
    bool gamePaused{ false };
    long long pauseTimestamp{ 0 };

    Processor processor;
    Cluster cluster = Cluster(persistentBlobs, MATPixelCount);
    Transpose transpose;
    Utils utils;
    ResponsePackager responsePackager;

public:

    /*********************
    * Constructors
    *********************/
    DriverControl();
    DriverControl(int _playerCount);

    /*********************
    * Member functions
    *********************/
    int getFeedSize();
    bool initDriverProcessing(std::string FMData);
    void setClusterID(int GameID);
    int getClusterID();
    static int getTotalPlayerCount();
    void getMATArrayBasedOnPlayerType(bool* _matArray);
    int getActiveMATPixelCount();
    void setActiveMATPixelCount(int foundNonBlankPattern);
    std::string getFMDriverVersion();
};



#endif //FMINTERFACE_DRIVERCONTROL_H
