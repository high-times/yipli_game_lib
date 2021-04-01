

#pragma once
#ifndef FMINTERFACE_ACTION_IDENTIFIER_H
#define FMINTERFACE_ACTION_IDENTIFIER_H

#include<string>

enum class ActionIdentifierTable {
    // Constants
    NULL_ID,
    SEPARATOR,

    
    //Trouble shooting
    TROUBLESHOOTING,
    
    // Actions mat-controls
    LEFT,
    RIGHT,
    ENTER,
    PAUSE,

    // Actions game
    LEFT_MOVE,
    RIGHT_MOVE,
    JUMP,
    RUNNING,
    RUNNING_STOPPED,
    JUMPINGJACK_OUT,
    JUMPINGJACK_IN,
    TILES,
    LEFT_TAP,
    RIGHT_TAP,
    LEFT_TOUCH,
    RIGHT_TOUCH,

    // LC Kids
    VERTICAL_JUMP,
    JUMPING_JACK,
    SKIER_JACK,
    SKIER_JACK_VERTICAL,
    CROSS_OVER_JACK,
    OBLIQUE_JACK,
    LUNGES,
    LUNGES_RUN,
    MOUNTAIN_CLIMBING,
    PLANK_STARTED,
    PLANK_STOPPED,
    PLANK_JUMP_INOUT,
    SHOULDER_TAP,
    MULE_KICK,
    RIGHT_LEG_HOPPING,
    LEFT_LEG_HOPPING,
    BURPEES,
    JUMPS_180,
    DIAGONAL_JUMP,
    FORWARD_JUMP,
    BACKWARD_JUMP,
    RIGHT_SIDE_JUMP,
    LEFT_SIDE_JUMP,
    STAR_JUMP,
    CHEST_JUMP,
    HOPSCOTCH,
    BALANCE_STARTED,
    BALANCE_STOPPED,
    ONE_ARM_STARTED,
    ONE_ARM_STOPPED,
    NINJA_KICKS,
    FAST_FEET,
    FAST_FEET_STOPPED,
    HIGH_KNEE_RUN,
    SQUATS,
    SQUAT_AND_KICK,
    SQUAT_AND_JUMP,
    SQUAT_180,
    JUMPING_JACK_AND_SQUAT,
    LATERAL_SQUAT_JUMP,
    THREE_LEG_DOG ,
    BANARSANA,
    AEROPLANE_POSE,
    VIKRASANA,
    ARDHA_CHANDRASANA,
    MALASANA,
};


class ActionIdentifier {

public:
    static std::string getActionName(ActionIdentifierTable _actionID) {
        switch (_actionID) {

        case ActionIdentifierTable::NULL_ID:
            return "null";

        case ActionIdentifierTable::TROUBLESHOOTING:
            return "Mat Data";
                
        case ActionIdentifierTable::LEFT:
            return "Left";

        case ActionIdentifierTable::RIGHT:
            return "Right";

        case ActionIdentifierTable::ENTER:
            return "Enter";

        case ActionIdentifierTable::PAUSE:
            return "Pause";

        case ActionIdentifierTable::LEFT_MOVE:
            return "Left Move";

        case ActionIdentifierTable::RIGHT_MOVE:
            return "Right Move";

        case ActionIdentifierTable::JUMP:
            return "Jump";

        case ActionIdentifierTable::VERTICAL_JUMP:
            return "Vertical Jump";

        case ActionIdentifierTable::RUNNING:
            return "Running";

        case ActionIdentifierTable::RUNNING_STOPPED:
            return "Running Stopped";

        case ActionIdentifierTable::JUMPINGJACK_IN:
            return "Jump In";

        case ActionIdentifierTable::JUMPINGJACK_OUT:
            return "Jump Out";

        case ActionIdentifierTable::JUMPING_JACK:
            return "Jumping Jack";

        case ActionIdentifierTable::SKIER_JACK:
            return "Skier Jack";

        case ActionIdentifierTable::SKIER_JACK_VERTICAL:
            return "SJ Vertical";

        case ActionIdentifierTable::OBLIQUE_JACK:
            return "Oblique Jack";

        case ActionIdentifierTable::CROSS_OVER_JACK:
            return "Crossover JJ";

        case ActionIdentifierTable::LUNGES:
            return "Lunges";

        case ActionIdentifierTable::LUNGES_RUN:
            return "Lunges Run";

        case ActionIdentifierTable::MOUNTAIN_CLIMBING:
            return "Mountain Climbing";

        case ActionIdentifierTable::PLANK_STARTED:
            return "Plank Started";


        case ActionIdentifierTable::PLANK_STOPPED:
            return "Plank Stopped";

        case ActionIdentifierTable::PLANK_JUMP_INOUT:
            return "Plank Jump Ins";

        case ActionIdentifierTable::SHOULDER_TAP:
            return "Shoulder Tap";

        case ActionIdentifierTable::MULE_KICK:
            return "Mule Kick";

        case ActionIdentifierTable::RIGHT_LEG_HOPPING:
            return "R Leg Hopping";

        case ActionIdentifierTable::LEFT_LEG_HOPPING:
            return "L Leg Hopping";

        case ActionIdentifierTable::BURPEES:
            return "Burpee";

        case ActionIdentifierTable::JUMPS_180:
            return "180 Jumps";

        case ActionIdentifierTable::DIAGONAL_JUMP:
            return "Diagonal Jump";

        case ActionIdentifierTable::FORWARD_JUMP:
            return "Forward Jump";

        case ActionIdentifierTable::BACKWARD_JUMP:
            return "Backward Jump";

        case ActionIdentifierTable::RIGHT_SIDE_JUMP:
            return "Right Jump";

        case ActionIdentifierTable::LEFT_SIDE_JUMP:
            return "Left Jump";

        case ActionIdentifierTable::STAR_JUMP:
            return "Star Jump";

        case ActionIdentifierTable::CHEST_JUMP:
            return "Chest Jump";

        case ActionIdentifierTable::HOPSCOTCH:
            return "Hopscotch";

        case ActionIdentifierTable::BALANCE_STARTED:
            return "Balance Started";

        case ActionIdentifierTable::BALANCE_STOPPED:
            return "Balance Stopped";

        case ActionIdentifierTable::ONE_ARM_STARTED:
            return "1-Arm Started";

        case ActionIdentifierTable::ONE_ARM_STOPPED:
            return "1-Arm Stopped";


        case ActionIdentifierTable::NINJA_KICKS:
            return "Ninja Kick";

        case ActionIdentifierTable::FAST_FEET:
            return "Fast Feet";

        case ActionIdentifierTable::FAST_FEET_STOPPED:
            return "Fast Feet Stopped";

        case ActionIdentifierTable::HIGH_KNEE_RUN:
            return "High Knee";


        case ActionIdentifierTable::SQUAT_180:
            return "180 Squats";

        case ActionIdentifierTable::SQUAT_AND_JUMP:
            return "Squat & Jump";

        case ActionIdentifierTable::SQUAT_AND_KICK:
            return "Squat & Kick";

        case ActionIdentifierTable::SQUATS:
            return "Squats";


        case ActionIdentifierTable::JUMPING_JACK_AND_SQUAT:
            return "Squat & JJ";

        case ActionIdentifierTable::LATERAL_SQUAT_JUMP:
            return "Lateral Squats";

        case ActionIdentifierTable::THREE_LEG_DOG:
            return "3 Leg Dog";

        case ActionIdentifierTable::BANARSANA:
            return "Banarsana";

        case ActionIdentifierTable::AEROPLANE_POSE:
            return "Aeroplane pose";

        case ActionIdentifierTable::VIKRASANA:
            return "Vikrasana";

        case ActionIdentifierTable::ARDHA_CHANDRASANA:
            return "Ardha Chandrasana";

        case ActionIdentifierTable::MALASANA:
            return "Malasana";

        case ActionIdentifierTable::TILES:
            return "Tiles";

        case ActionIdentifierTable::LEFT_TAP:
            return "Left Tap";

        case ActionIdentifierTable::RIGHT_TAP:
            return "Rigth Tap";

        case ActionIdentifierTable::LEFT_TOUCH:
            return "Left Touch";

        case ActionIdentifierTable::RIGHT_TOUCH:
            return "Rigth Touch";



        }
        return "null";

    }


    static std::string getActionID(ActionIdentifierTable _actionID)
    {
        switch (_actionID)
        {
            case ActionIdentifierTable::NULL_ID:
                return  "null";
                
            case ActionIdentifierTable::TROUBLESHOOTING:
                return "TRBL";
                
                // Actions mat-controls
            case ActionIdentifierTable::LEFT:
                return  "9GO5";
            case ActionIdentifierTable::RIGHT:
                return  "3KWN";
            case ActionIdentifierTable::ENTER:
                return  "PLW3";
            case ActionIdentifierTable::PAUSE:
                return  "UDH0";

                // Actions game
            case ActionIdentifierTable::LEFT_MOVE:
                return  "38UF";
            case ActionIdentifierTable::RIGHT_MOVE:
                return  "DMEY";
            case ActionIdentifierTable::JUMP:
                return  "9D6O";
            case ActionIdentifierTable::RUNNING:
                return  "SWLO";
            case ActionIdentifierTable::RUNNING_STOPPED:
                return  "7RCE";
            case ActionIdentifierTable::JUMPINGJACK_OUT:
                return  "QRTY";
            case ActionIdentifierTable::JUMPINGJACK_IN:
                return  "EUOA";
            case ActionIdentifierTable::TILES:
                return  "TIL3";
            case ActionIdentifierTable::LEFT_TAP:
                return  "9015";
            case ActionIdentifierTable::RIGHT_TAP:
                return  "3L1N";
            case ActionIdentifierTable::LEFT_TOUCH:
                return  "A075";
            case ActionIdentifierTable::RIGHT_TOUCH:
                return  "AL0N";

                // LC Kids
            case ActionIdentifierTable::VERTICAL_JUMP:
                return  "9LSO";
            case ActionIdentifierTable::JUMPING_JACK:
                return  "99XR";
            case ActionIdentifierTable::SKIER_JACK:
                return  "NWCH";
            case ActionIdentifierTable::SKIER_JACK_VERTICAL:
                return  "01Q4";
            case ActionIdentifierTable::CROSS_OVER_JACK:
                return  "VUFO";
            case ActionIdentifierTable::OBLIQUE_JACK:
                return  "SG45";
            case ActionIdentifierTable::LUNGES:
                return  "CY55";
            case ActionIdentifierTable::LUNGES_RUN:
                return  "386I";
            case ActionIdentifierTable::MOUNTAIN_CLIMBING:
                return  "BGM4";
            case ActionIdentifierTable::PLANK_STARTED:
                return  "58GH";
            case ActionIdentifierTable::PLANK_STOPPED:
                return  "0DLA";
            case ActionIdentifierTable::PLANK_JUMP_INOUT:
                return  "WBTW";
            case ActionIdentifierTable::SHOULDER_TAP:
                return  "GH10";
            case ActionIdentifierTable::MULE_KICK:
                return  "WBUT";
            case ActionIdentifierTable::RIGHT_LEG_HOPPING:
                return  "3DIN";
            case ActionIdentifierTable::LEFT_LEG_HOPPING:
                return  "3DI1";
            case ActionIdentifierTable::BURPEES:
                return  "FN1S";
            case ActionIdentifierTable::JUMPS_180:
                return  "V56G";
            case ActionIdentifierTable::DIAGONAL_JUMP:
                return  "6JJR";
            case ActionIdentifierTable::FORWARD_JUMP:
                return  "UJ3J";
            case ActionIdentifierTable::BACKWARD_JUMP:
                return  "U10J";
            case ActionIdentifierTable::RIGHT_SIDE_JUMP:
                return  "B8X7";
            case ActionIdentifierTable::LEFT_SIDE_JUMP:
                return  "18X7";
            case ActionIdentifierTable::STAR_JUMP:
                return  "LPM0";
            case ActionIdentifierTable::CHEST_JUMP:
                return  "JASL";
            case ActionIdentifierTable::HOPSCOTCH:
                return  "U8W2";
            case ActionIdentifierTable::BALANCE_STARTED:
                return  "UWC6";
            case ActionIdentifierTable::BALANCE_STOPPED:
                return  "1WC1";
            case ActionIdentifierTable::ONE_ARM_STARTED:
                return  "ISJD";
            case ActionIdentifierTable::ONE_ARM_STOPPED:
                return  "EJ02";
            case ActionIdentifierTable::NINJA_KICKS:
                return  "90DM";
            case ActionIdentifierTable::FAST_FEET:
                return  "RDDJ";
            case ActionIdentifierTable::FAST_FEET_STOPPED:
                return  "137E";
            case ActionIdentifierTable::HIGH_KNEE_RUN:
                return  "HXCQ";
            case ActionIdentifierTable::SQUATS:
                return  "OYMP";
            case ActionIdentifierTable::SQUAT_AND_KICK:
                return  "E0CB";
            case ActionIdentifierTable::SQUAT_AND_JUMP:
                return  "6CTM";
            case ActionIdentifierTable::SQUAT_180:
                return  "FYN1";
            case ActionIdentifierTable::JUMPING_JACK_AND_SQUAT:
                return  "O12U";
            case ActionIdentifierTable::LATERAL_SQUAT_JUMP:
                return  "X5IW";
            case ActionIdentifierTable::THREE_LEG_DOG:
                return  "8G3J";
            case ActionIdentifierTable::BANARSANA:
                return  "UWHX";
            case ActionIdentifierTable::AEROPLANE_POSE:
                return  "WHXW";
            case ActionIdentifierTable::VIKRASANA:
                return  "LP07";
            case ActionIdentifierTable::ARDHA_CHANDRASANA:
                return  "3JCQ";
            case ActionIdentifierTable::MALASANA:
                return  "3J11";

            default:   
                return "null";

        }
    }
};
#endif
