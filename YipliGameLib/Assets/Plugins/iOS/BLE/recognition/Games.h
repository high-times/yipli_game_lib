#pragma once
#ifndef FMINTERFACE_GAMES_H
#define FMINTERFACE_GAMES_H

#include<string>
#include<iostream>
#include<vector>
#include<cstdlib>

#include "../blobprocessor/Blob.h"
#include "../support/ActionIdentifier.h"
#include "../blobprocessor/Processor.h"
#include "../support/Utils.h"
#include    "../support/Const.h"

class Processor;

class Games {
public:

    /*********************
    * Member variables
    *********************/
    std::vector<Blob>& m_persistentBlobs;
    int& m_matPixelCount;

    //Control button
    int legMovedFlag { 0 };
    int rightLegLocationController { 0 };
    int leftLegLocationController { 0 };
    ActionIdentifierTable ControlButtonType;

    //Leg Movement
    int rightLegLocation { 0 };
    int leftLegLocation { 0 };

    // Hopping
    int rightLegLocationHopping { 0 };
    int leftLegLocationHopping { 0 };
    int hoppingFlag { 0 };
    std::string hoppingLegType = "";

    //Jump
    int twoLegsDetected { 0 };
    int activateJummpSequence { 0 };
    long long timerHistoryJump{ 0 };
    long long timerHistoryJump2{ 0 };

    //Runnning
    int globalStopTimeDifference { 0 };
    int runnningStopFlag { 0 };
    long long timerHistoryRunning1{ 0 };
    long long timerHistoryRunning2{ 0 };
    bool runningStartedFlag = false;
    int singleLegMovedFlag { 0 };
    int singleLegLocation { 0 };
    long long runningStartTime { 0 };
    int stepsCount { 0 };
    int totalStepsCount { 0 };
    int liftUpStartLocation { 0 };
    bool twoLegFound = false;
    long long twoLegFoundTimestamp { 0 };
    int stepsCountWhenTwoLegFound { 0 };
    long long timerHistoryRunningJump { 0 };
    bool runningJumpFlag = false;

    //Leg Movement
    ActionIdentifierTable LegMovementType ;

    //Jump IN/OUT
    ActionIdentifierTable inOutType;
    int inOutFlag { 0 };
    bool messagedRelayed = false;
    bool jumpDetected = false;
    int jumpingJackFlag { 0 };
    int firstTimeJIOFlag{ 0 };;

    //Beat
    int footLocations[4]{ 0 };

    //Skier Jack
    int sjjFlag { 0 };
    int jumpFlag { 0 };

    //High Knee
    long long timerHistoryHighKnee { 0 };
    
    /*****************************************
    * Member functions / Constructors
    *****************************************/
    Games(std::vector<Blob>& _persistentBlobs, int& _MATPixelCount);

    void resetAllActionFlags();
    bool recogniseBendPattern();
    bool recogniseControlButtonClicked();
    bool recogniseTapButtonClicked();
    bool recogniseJumpPattern();
    bool recogniseFootMovement();
    bool recogniseHoppingPattern();
    bool recogniseInOutJumpingJackPattern();
    bool recogniseRunningPattern();
    bool recogniseRunningStopPattern();
    bool recogniseRunningJumpPattern();
    bool recogniseLegMovementPattern();
    bool recogniseJumpingJackPattern();
    bool recogniseTouchButtonClicked();

    bool recogniseHighKneeRunningPattern();

    bool recogniseNinjaKickPattern();

    bool recogniseSkierJumpingJackPattern();
};

#endif