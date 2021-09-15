//
// Created by admin on 27-01-2021.
//

#include "Games.h"



Games::Games(std::vector<Blob>& _persistentBlobs, int& _MATPixelCount) : m_persistentBlobs(_persistentBlobs), m_matPixelCount(_MATPixelCount)
{
    //m_persistentBlobs = _persistentBlobs;
    //m_matPixelCount = _MATPixelCount;
    FMLOG(VERBOSE, "Games", "Constructor created..");
}

bool Games::recogniseBendPattern()
{
    if (m_persistentBlobs.size() == 4 || m_persistentBlobs.size() == 5)
    {
        Blob FirstLeg = m_persistentBlobs[0];
        Blob FirstHand = m_persistentBlobs[1];

        Blob SecondLeg = m_persistentBlobs[2];
        Blob SecondHand = m_persistentBlobs[3];

        if (abs(FirstHand.miny - FirstLeg.miny) <= 1 && abs(SecondHand.miny - SecondLeg.miny) <= 1)
            return false;
        //if( (FirstHand.minx - (FirstLeg.maxx+1)) > 2 && (SecondHand.minx - (SecondLeg.maxx+1)) > 2)
        //    return true;

        //std::cout << "Bending detected";
        return true;
    }
    return false;
}

bool Games::recogniseControlButtonClicked()
{
    if (m_persistentBlobs.size() == 1 && legMovedFlag == 0)
    {
        legMovedFlag = 1;
        singleLegLocation = m_persistentBlobs[0].maxy;
    }
    else if (m_persistentBlobs.size() == 2 && legMovedFlag == 1)
    {
        leftLegLocation = m_persistentBlobs[0].maxy;
        rightLegLocation = m_persistentBlobs[1].maxy;

        if (abs(leftLegLocation - rightLegLocation) > 10)
        {

            if (abs(leftLegLocation - singleLegLocation) < 3)
            {
                ControlButtonType = ActionIdentifierTable::RIGHT;
                legMovedFlag = 0;

                return true;
            }
            else if (abs(rightLegLocation - singleLegLocation) < 3)
            {
                ControlButtonType = ActionIdentifierTable::LEFT;
                legMovedFlag = 0;
                return true;
            }
        }
        legMovedFlag = 0;
    }
    return false;
}


bool Games::recogniseTapButtonClicked()
{
    if (m_persistentBlobs.size() == 1 && legMovedFlag == 0)
    {
        legMovedFlag = 1;
        singleLegLocation = m_persistentBlobs[0].maxy;
    }
    else if (m_persistentBlobs.size() == 2 && legMovedFlag == 1)
    {
        leftLegLocation = m_persistentBlobs[0].maxy;
        rightLegLocation = m_persistentBlobs[1].maxy;

        if (abs(leftLegLocation - rightLegLocation) > 10)
        {

            if (abs(leftLegLocation - singleLegLocation) < 3)
            {
                ControlButtonType = ActionIdentifierTable::RIGHT_TAP;
                legMovedFlag = 0;

                return true;
            }
            else if (abs(rightLegLocation - singleLegLocation) < 3)
            {
                ControlButtonType = ActionIdentifierTable::LEFT_TAP;
                legMovedFlag = 0;
                return true;
            }
        }
        legMovedFlag = 0;
    }
    return false;


}


bool Games::recogniseTouchButtonClicked() {
    if (m_persistentBlobs.size() == 1 && legMovedFlag == 0)
    {
        legMovedFlag = 1;
        singleLegLocation = m_persistentBlobs[0].maxy;
    }
    else if (m_persistentBlobs.size() == 2 && legMovedFlag == 1)
    {
        leftLegLocation = m_persistentBlobs[0].maxy;
        rightLegLocation = m_persistentBlobs[1].maxy;

        if (abs(leftLegLocation - rightLegLocation) > 7)
        {

            if (abs(leftLegLocation - singleLegLocation) < 3)
            {
                ControlButtonType = ActionIdentifierTable::RIGHT_TOUCH;
                legMovedFlag = 0;

                return true;
            }
            else if (abs(rightLegLocation - singleLegLocation) < 3)
            {
                ControlButtonType = ActionIdentifierTable::LEFT_TOUCH;
                legMovedFlag = 0;
                return true;
            }
        }
        legMovedFlag = 0;
    }

    return false;
}

bool Games::recogniseJumpPattern() {
    if (m_persistentBlobs.size() == 2 && m_persistentBlobs[0].areaCovered >= static_cast<int>(Threshold::JUMP_SINGLE_BLOB_AREACOVERED) && m_persistentBlobs[1].areaCovered >= static_cast<int>(Threshold::JUMP_SINGLE_BLOB_AREACOVERED)) {
        twoLegsDetected = 1;
        activateJummpSequence = 1;
        timerHistoryJump = Utils::getCurrentTimestamp();
    }
    else if (m_persistentBlobs.size() == 1 && twoLegsDetected == 0) {
        if (m_persistentBlobs[0].areaCovered >= static_cast<int>(Threshold::JUMP_DOUBLE_BLOB_AREACOVERED) && m_persistentBlobs[0].length >= 2 && m_persistentBlobs[0].width >= 6) {
            twoLegsDetected = 1;
            activateJummpSequence = 1;
            timerHistoryJump = Utils::getCurrentTimestamp();
        }
    }
    else if (m_persistentBlobs.size() == 0 && twoLegsDetected == 1 && m_matPixelCount < static_cast<int>(Threshold::ALLOWED_SHORTED_PIXELS)) {
        timerHistoryJump2 = Utils::getCurrentTimestamp();
        int diff = (int)(timerHistoryJump2 - timerHistoryJump);
        //displayText.setText(diff);
       //if( diff <= 300 && diff >= 50){

        if (diff <= 300 && diff >= 10) {
            return true;
        }
        else
            twoLegsDetected = 0;
        //timerHistoryJump = timerHistoryJump2;
    }
    return false;
}


bool Games::recogniseRunningPattern() {

    if (!runningStartedFlag) {
        if (m_persistentBlobs.size() == 1) {
            if (liftUpStartLocation == 0 && m_persistentBlobs[0].areaCovered <= 20) {
                liftUpStartLocation = m_persistentBlobs[0].maxy;
            }
            else if (abs(liftUpStartLocation - m_persistentBlobs[0].maxy) >= 2) {
                liftUpStartLocation = m_persistentBlobs[0].maxy;
                stepsCount++;
                if (stepsCount == static_cast<int>(Threshold::DETECT_STEPS_COUNT)) {
                    totalStepsCount = 1;
                    //TODO stepCount.setText("SC : "+totalStepsCount);
                    stepsCount = 0;
                    runningStartedFlag = true;
                    return true;
                }
            }

        }
    }
    return false;
}

bool Games::recogniseRunningStopPattern() {


    if (m_persistentBlobs.size() == 1) {
        if (liftUpStartLocation == 0 && m_persistentBlobs[0].areaCovered <= 20) {
            liftUpStartLocation = m_persistentBlobs[0].maxy;
        }
        else if (abs(liftUpStartLocation - m_persistentBlobs[0].maxy) >= 2) {
            liftUpStartLocation = m_persistentBlobs[0].maxy;
            totalStepsCount++;
            //TODO stepCount.setText("SC : "+totalStepsCount);
            twoLegFound = false;
        }
    }
    if (m_persistentBlobs.size() == 2) {
        if (liftUpStartLocation == 0 && m_persistentBlobs[0].areaCovered <= 20) {
            liftUpStartLocation = m_persistentBlobs[0].maxy;
        }
        else if (abs(liftUpStartLocation - m_persistentBlobs[0].maxy) >= 2) {
            liftUpStartLocation = m_persistentBlobs[0].maxy;
            totalStepsCount++;
            //TODO stepCount.setText("SC : "+totalStepsCount);
            twoLegFound = false;
        }
        // totalStepsCount++;
        //stepCount.setText("SC : "+totalStepsCount);
        if (twoLegFound) {
            if (abs(twoLegFoundTimestamp - Utils::getCurrentTimestamp()) >= 500) {
                //if(totalStepsCount - stepsCountWhenTwoLegFound <= 2)
                totalStepsCount = 0;
                stepsCount = 0;
                return true;
            }
        }
        else {
            if (abs(twoLegFoundTimestamp - Utils::getCurrentTimestamp()) >= 10 && abs(twoLegFoundTimestamp - Utils::getCurrentTimestamp()) <= 50) {
                totalStepsCount++;
                //TODO stepCount.setText("SC : "+totalStepsCount);
            }
            twoLegFound = true;
            twoLegFoundTimestamp = Utils::getCurrentTimestamp();
            stepsCountWhenTwoLegFound = totalStepsCount;
        }
    }
    return false;
}



bool Games::recogniseRunningJumpPattern() {


    if (m_persistentBlobs.size() != 0) {
        timerHistoryRunningJump = Utils::getCurrentTimestamp();
    }

    else if (m_persistentBlobs.size() == 0) {

        if (abs(timerHistoryRunningJump - Utils::getCurrentTimestamp()) >= 200 && abs(timerHistoryRunningJump - Utils::getCurrentTimestamp()) <= 500) {
            return true;
        }

    }
    return false;
}

bool Games::recogniseInOutJumpingJackPattern()
{
    if( !jumpDetected ) {
        if(m_persistentBlobs.size() == 0){
            jumpDetected = true;

        }
    }

    if (m_persistentBlobs.size() == 1  && jumpDetected) {
        if (m_persistentBlobs[0].areaCovered >= 25) {
            if (!messagedRelayed) {

                inOutType = ActionIdentifierTable::JUMPINGJACK_IN;
                inOutFlag = 1;
                messagedRelayed = true;
                jumpDetected = false;
                return true;

            }


        }
    }
    else if (m_persistentBlobs.size() == 2 && jumpDetected) {


        rightLegLocation = m_persistentBlobs[0].maxy;
        leftLegLocation = m_persistentBlobs[1].miny;

        int d = abs(rightLegLocation - leftLegLocation);
        if(firstTimeJIOFlag == 0  && d >= 10) {
            inOutType = ActionIdentifierTable::JUMPINGJACK_OUT;
            inOutFlag = 0;
            firstTimeJIOFlag = 1;
            messagedRelayed = false;

            return true;

        }
        else if (inOutFlag == 1  && d >= 10) {
            inOutType = ActionIdentifierTable::JUMPINGJACK_OUT;
            inOutFlag = 0;
            firstTimeJIOFlag = 0;
            messagedRelayed = false;

            return true;
        } else if( d <= 9 && !messagedRelayed) {
            inOutType = ActionIdentifierTable::JUMPINGJACK_IN;
            inOutFlag = 1;
            messagedRelayed = true;
            jumpDetected = false;
            return true;
        }
    }
    return false;
}

bool Games::recogniseFootMovement()
{
    if(m_persistentBlobs.size() == 2){
        footLocations[0] = m_persistentBlobs[0].miny+1;
        footLocations[1] = m_persistentBlobs[0].maxy+1;
        footLocations[2] = m_persistentBlobs[1].miny+1;
        footLocations[3] = m_persistentBlobs[1].maxy+1;
        return true;
    }
    else if(m_persistentBlobs.size() == 1){
        footLocations[0] = m_persistentBlobs[0].miny+1;
        footLocations[1] = m_persistentBlobs[0].maxy+1;
        footLocations[2] = 0;
        footLocations[3] = 0;
        return true;
    }
    else{
        footLocations[0] = 0;
        footLocations[1] = 0;
        footLocations[2] = 0;
        footLocations[3] = 0;
    }
    
    return false;
}

bool Games::recogniseHoppingPattern()
{

    if (m_persistentBlobs.size() == 2) {

        rightLegLocationHopping = m_persistentBlobs[0].maxy;
        leftLegLocationHopping = m_persistentBlobs[1].maxy;
        hoppingFlag = 1;

    }
    else if (m_persistentBlobs.size() == 1 && hoppingFlag == 2) {
        int legLocation = m_persistentBlobs[0].maxy;

        if ( abs(legLocation- rightLegLocationHopping) <= 2) {
            hoppingFlag = 1;
            hoppingLegType = "Left";

            return true;
        }

        if (abs(legLocation - leftLegLocationHopping) <= 2) {
            //detectedText.setText("Left Hopping Started");
            hoppingFlag = 1;
            hoppingLegType = "Right";
            return true;
        }
    }
    else if (m_persistentBlobs.size() == 0) {
        hoppingFlag = 2;
    }
    return false;
}

bool Games::recogniseLegMovementPattern() {

    // Logic for single blob detect which has two foots

    if ((m_persistentBlobs.size() == 2 && legMovedFlag == 0)) {

        //if(abs(persistentBlobs.get(0].miny-persistentBlobs.get(1].miny) >= TWO_LEG_IDEAL_DISTANCE){
        rightLegLocation = m_persistentBlobs[0].maxy;
        leftLegLocation = m_persistentBlobs[1].maxy;
        legMovedFlag = 1;
        //}
    }
    else if (m_persistentBlobs.size() == 1 && legMovedFlag == 0 && m_persistentBlobs[0].areaCovered >= 30) {
        rightLegLocation = m_persistentBlobs[0].miny;
        leftLegLocation = m_persistentBlobs[0].maxy;
        legMovedFlag = 1;
    }
    else if (m_persistentBlobs.size() == 2 && legMovedFlag == 1) {

        if (abs(m_persistentBlobs[0].miny - m_persistentBlobs[1].miny) >= static_cast<int>(Threshold::TWO_LEG_IDEAL_DISTANCE)) {

            int new_rightLegLocation = m_persistentBlobs[0].maxy;
            int new_leftLegLocation = m_persistentBlobs[1].maxy;


            if ((rightLegLocation > new_rightLegLocation) && (rightLegLocation - new_rightLegLocation) >= static_cast<int>(static_cast<int>(Threshold::LEG_MOVE_COUNT) && (abs(new_leftLegLocation - leftLegLocation) <= static_cast<int>(Threshold::LEG_MOVE_COUNT)))) {
                legMovedFlag = 2;
                LegMovementType = ActionIdentifierTable::LEFT_MOVE;




                return true;
            }
            else if ((leftLegLocation < new_leftLegLocation) && (new_leftLegLocation - leftLegLocation) >= static_cast<int>(Threshold::LEG_MOVE_COUNT) && (abs(new_rightLegLocation - rightLegLocation) <= static_cast<int>(Threshold::LEG_MOVE_COUNT))) {
                legMovedFlag = 2;
                LegMovementType = ActionIdentifierTable::RIGHT_MOVE;


                return true;
            }
            else {
                rightLegLocation = new_rightLegLocation;
                leftLegLocation = new_leftLegLocation;
            }
        }

    }
    else if ((m_persistentBlobs.size() == 2 && legMovedFlag == 2)) {

        if (abs(m_persistentBlobs[0].miny - m_persistentBlobs[1].miny) >= static_cast<int>(Threshold::TWO_LEG_IDEAL_DISTANCE)) {
            rightLegLocation = m_persistentBlobs[0].maxy;
            leftLegLocation = m_persistentBlobs[1].maxy;
            legMovedFlag = 0;
        }
    }
    else if (m_persistentBlobs.size() == 1 && legMovedFlag == 2 && m_persistentBlobs[0].areaCovered >= 30) {
        rightLegLocation = m_persistentBlobs[0].miny;
        leftLegLocation = m_persistentBlobs[1].maxy;
        legMovedFlag = 1;
    }
    return false;
}

bool Games::recogniseJumpingJackPattern () {

    //matchPattern();

    if (m_persistentBlobs.size() == 1 ) {
        if(m_persistentBlobs[0].areaCovered >= 30){
            rightLegLocation = m_persistentBlobs[0].maxy;
            leftLegLocation = m_persistentBlobs[0].miny;

            int d = abs(rightLegLocation - leftLegLocation);

            if (jumpingJackFlag == 1 && d >= 10) {

                jumpingJackFlag = 0;
                return true;
            } else if( d <= 9) {
                jumpingJackFlag = 1;
            }
        }
    }


    if (m_persistentBlobs.size() == 2 ) {

        rightLegLocation = m_persistentBlobs[0].maxy;
        leftLegLocation = m_persistentBlobs[1].miny;

        int d = abs(rightLegLocation - leftLegLocation);
        if (jumpingJackFlag == 1 && d >= 10) {
            jumpingJackFlag = 0;
            return true;
        } else if( d <= 9) {
            jumpingJackFlag = 1;
        }
    }
    return false;
}


bool Games::recogniseHighKneeRunningPattern() {

    if(m_persistentBlobs.size() == 1 && jumpFlag == 0){

        leftLegLocation = m_persistentBlobs[0].miny;
        jumpFlag = 1;
        //Log.i("MAT_KNEE :", " 1 Blob - "+leftLegLocation);
        timerHistoryHighKnee = Utils::getCurrentTimestamp();
    }
    else if(m_persistentBlobs.size() == 0 && jumpFlag == 1){
        jumpFlag = 2;
        //Log.i("MAT_KNEE :", " 0 Blob");
    }
    else if(m_persistentBlobs.size() == 1 && jumpFlag == 2){

        long diffT =  abs(timerHistoryHighKnee - Utils::getCurrentTimestamp());

        int diff = abs( leftLegLocation -  m_persistentBlobs[0].miny);
        //Log.i("MAT_KNEE :", " 1 Blob - "+leftLegLocation+" "+m_persistentBlobs[0].miny+" "+diff+" "+diffT);
        if( diff <= 15 && diffT >= 100) {

            return true;
        }
        jumpFlag = 0;
    }
    
    return false;
}


bool Games::recogniseNinjaKickPattern() {
    if(m_persistentBlobs.size() == 1 && jumpFlag==0){

        leftLegLocation = m_persistentBlobs[0].miny;
        jumpFlag = 1;
        //Log.i("MAT_NINJA :", " 1 Blob - "+leftLegLocation);
    }
    else if(m_persistentBlobs.size() == 0 && jumpFlag == 1){
        jumpFlag = 2;
        //Log.i("MAT_NINJA :", " 0 Blob");
    }
    else if(m_persistentBlobs.size() == 1 && jumpFlag == 2){

        int diff = abs( leftLegLocation -  m_persistentBlobs[0].miny);
        //Log.i("MAT_NINJA :", " 1 Blob - "+leftLegLocation+" "+m_persistentBlobs[0].miny+" "+diff);
        if( diff <= 15) {

            return true;
        }
        jumpFlag = 0;
    }
    return false;
}


bool Games::recogniseSkierJumpingJackPattern() {

    if( m_persistentBlobs.size() == 0){
        sjjFlag = 1;
        //Log.i("SJJ :", "Found blank pattern");
    }

    else if (m_persistentBlobs.size() == 2 ) {

        rightLegLocation = m_persistentBlobs[0].miny;
        leftLegLocation = m_persistentBlobs[1].maxy;

        int d = abs(rightLegLocation - leftLegLocation);

        //Log.i("SJJ :", "" + (d));
        if ( sjjFlag == 1 && d >= 10 ) {
            sjjFlag = 0;
            return true;
        }
    }
    return false;
}


void Games::resetAllActionFlags()
{
    ControlButtonType = ActionIdentifierTable::NULL_ID;

    rightLegLocation = 0;
    leftLegLocation = 0;
    legMovedFlag = 0;

    twoLegsDetected = 0;

    jumpDetected = false;
    std::fill(footLocations, footLocations + 4, 0);
    timerHistoryRunningJump = 0;


}



