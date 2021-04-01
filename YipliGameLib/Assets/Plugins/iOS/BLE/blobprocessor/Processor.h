
#pragma once
#ifndef FMINTERFACE_PROCESSOR_H
#define FMINTERFACE_PROCESSOR_H


#include "../support/Const.h"
#include "../support/Utils.h"
#include "Blob.h"

#include <iostream>
#include <vector>


class Processor {
    
public:
    /*********************
    * Member variables
    *********************/
    

    //DriverControl* DC = nullptr;

    /*********************
    * Constructors
    *********************/
    Processor();
    //Processor(DriverControl* _dc);

   

    /*********************
    * Member functions
    *********************/
    bool foundNewFrame(const bool* _MAT_ARRAY, std::vector<Blob>& _persistentBlobs, int& _MAT_PixelCount, int _playerCount);

    void removeNoise(std::vector<Blob>& _persistentBlobs);


};


#endif //FMINTERFACE_PROCESSOR_H
