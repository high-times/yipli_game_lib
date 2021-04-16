#include <jni.h>
#include <string>
#include "DriverControl.cpp"
#include <android/log.h>
#include "support/Utils.h"


DriverControl d1(1);
std::unique_ptr<DriverControl> d2;
Utils utils;
std::string FMResponse="";

void processForMP(std::string basicString);

extern "C"
JNIEXPORT jstring JNICALL
Java_com_fitmat_fmjavainterface_DeviceControlActivity_stringFromJNI(
        JNIEnv* env,
        jobject /* this */) {
    std::string hello = "Test OK";
    return env->NewStringUTF(hello.c_str());
}



extern "C"
JNIEXPORT void JNICALL
Java_com_fitmat_fmjavainterface_DeviceControlActivity_setGameMode(JNIEnv *env, jobject thiz, jint _game_mode) {
    if(DriverControl::gameMode == SINGLE_PLAYER && _game_mode == MULTI_PLAYER){
        d2.reset(new DriverControl(2));
    }
    else if(DriverControl::gameMode == MULTI_PLAYER && _game_mode == SINGLE_PLAYER){
        DriverControl::playerCounter--;
    }

    DriverControl::gameMode = _game_mode;
}

extern "C"
JNIEXPORT jint JNICALL
Java_com_fitmat_fmjavainterface_DeviceControlActivity_getGameMode(JNIEnv *env, jobject thiz) {
    return DriverControl::gameMode;
}

extern "C"
JNIEXPORT void JNICALL
Java_com_fitmat_fmjavainterface_DeviceControlActivity_setClusterID__I(JNIEnv *env, jobject thiz, jint _cluster_id) {
    d1.setClusterID(_cluster_id);
}


extern "C"
JNIEXPORT void JNICALL
Java_com_fitmat_fmjavainterface_DeviceControlActivity_setClusterID__II(JNIEnv *env, jobject thiz,
                                                                       jint __p1_game_id,
                                                                       jint __p2_game_id) {
        d2->setClusterID(__p2_game_id);
        d1.setClusterID(__p1_game_id);
}


extern "C"
JNIEXPORT jint JNICALL
Java_com_fitmat_fmjavainterface_DeviceControlActivity_getClusterID__(JNIEnv *env, jobject thiz) {
    return d1.getClusterID();
}


extern "C"
JNIEXPORT jint JNICALL
Java_com_fitmat_fmjavainterface_DeviceControlActivity_getClusterID__I(JNIEnv *env, jobject thiz,
                                                                      jint _player_id) {
    if(_player_id == 1){
        return d1.getClusterID();
    }else{
        return d2->getClusterID();
    }
}

extern "C"
JNIEXPORT jstring JNICALL
Java_com_fitmat_fmjavainterface_DeviceControlActivity_getFMDriverVersion(JNIEnv *env, jobject thiz) {

        std::string hello = DRIVER_VERSION;
        return env->NewStringUTF(hello.c_str());

}


extern "C"
JNIEXPORT void JNICALL
Java_com_fitmat_fmjavainterface_BluetoothLeService_initFMDataProcessing(JNIEnv *env, jobject thiz,jstring _fmdata) {

    std::string _FMData = NDKUtils::jstring2string(env, _fmdata);

    //FMLOG(VERBOSE, "packageFMResponse", (_FMData+" "+std::to_string(_FMData.length())).c_str());
    if(DriverControl::gameMode == SINGLE_PLAYER){

        bool result = d1.initDriverProcessing(_FMData);
        if (result) {
            std::vector<std::string> players;
            players.push_back(d1.responsePackager.m_player);
            FMResponse = ResponsePackager::packageFMresponse(players[0]);
            FMLOG(VERBOSE, "packageFMResponse", FMResponse.c_str());
            d1.responsePackager.resestVariables();
        }
    }
    else{
        processForMP(_FMData);
    }
}

void processForMP(std::string _data) {
    //TODO : _Multiplayer
    bool result = d1.initDriverProcessing(_data);
    bool result1 = d2->initDriverProcessing(_data);

    //FMLOG(VERBOSE, "packageFMResponse", (std::to_string(result) + " " + std::to_string(result1)).c_str());

    if(!result){
        if(d1.responsePackager.m_player == ""){
            d1.responsePackager.m_actionId = ActionIdentifierTable::NULL_ID;
            d1.responsePackager.setPlayerData(1);
        }
    }
    if(!result1){
        if(d2->responsePackager.m_player == ""){
            d2->responsePackager.m_actionId = ActionIdentifierTable::NULL_ID;
            d2->responsePackager.setPlayerData(2);
        }

    }
    if(result || result1){

        //FMLOG(VERBOSE, "packageFMResponse", (d1.responsePackager.m_player + " -- data -- " + d2->responsePackager.m_player).c_str());
        std::vector<std::string> players;

        d1.responsePackager.m_player = d1.responsePackager.m_player.substr(0,7) +
                ", \"count\":" + std::to_string(d1.responseCount) +
                d1.responsePackager.m_player.substr(7);

        d2->responsePackager.m_player = d2->responsePackager.m_player.substr(0,7) +
                ", \"count\":" + std::to_string(d2->responseCount) +
                d2->responsePackager.m_player.substr(7);

        players.push_back(d1.responsePackager.m_player);
        d1.responsePackager.resestVariables();

        players.push_back(d2->responsePackager.m_player);
        d2->responsePackager.resestVariables();

        //FMLOG(VERBOSE, "packageFMResponse", (players[0] + " " + players[1]).c_str());

        std::string _playersData = players[0] + "," + players[1];
        FMResponse = ResponsePackager::packageFMresponse(_playersData);
        FMLOG(VERBOSE, "packageFMResponse", FMResponse.c_str());


    }

}


extern "C"
JNIEXPORT jint JNICALL
Java_com_fitmat_fmjavainterface_BluetoothLeService_getFeedSize(JNIEnv *env, jobject thiz) {
    return d1.getFeedSize();

}


extern "C"
JNIEXPORT jstring JNICALL
Java_com_fitmat_fmjavainterface_DeviceControlActivity_getFMResponse(JNIEnv *env, jobject thiz) {
    return  env->NewStringUTF(FMResponse.c_str());;
}