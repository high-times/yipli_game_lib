//
// Created by admin on 27-01-2021.
//

#include "Processor.h"

Processor::Processor() {

    FMLOG(VERBOSE, "Processor", "Constructor created..") ;
}


/*
Processor::Processor(DriverControl *_dc) {
    DC = _dc;
    std::cout << "Processor(DriverControl* _DC)";
}
*/

void Processor::removeNoise(std::vector<Blob>& _persistentBlobs)
{
    // SOLUTION TO NOISE (Arching)
    if (_persistentBlobs.size() == 2) {
        int FirstLegUpperPart = _persistentBlobs[0].width;
        int FirstLegLowerPart = _persistentBlobs[1].width;
        int FirstLegUpperPartYCord = _persistentBlobs[0].maxy;
        int FirstLegLowerPartYCord = _persistentBlobs[1].maxy;
        int FirstLegFreeSpaceWidth = (( _persistentBlobs[1].maxy + 1) - _persistentBlobs[0].miny);
        int FirstLegFreeSpaceLength = (( _persistentBlobs[1].minx - 1) - _persistentBlobs[0].maxx);
        if (abs(FirstLegUpperPart - FirstLegLowerPart) <= 1 && FirstLegFreeSpaceLength <= 2 && abs(FirstLegUpperPartYCord - FirstLegLowerPartYCord) <= 1) {
            _persistentBlobs[0].maxx = _persistentBlobs[1].maxx;
            // _persistentBlobs[2].maxx =  _persistentBlobs[3].maxx;
            _persistentBlobs[0].length = ( _persistentBlobs[1].maxx + 1) - _persistentBlobs[0].minx;
            //  _persistentBlobs[2].length =  ( _persistentBlobs[2].maxx+1) -  _persistentBlobs[2].minx;
            _persistentBlobs[0].areaCovered = _persistentBlobs[0].areaCovered + _persistentBlobs[1].areaCovered + (FirstLegFreeSpaceWidth * FirstLegFreeSpaceLength);
            //  _persistentBlobs[2].areaCovered =   _persistentBlobs[2].areaCovered +  _persistentBlobs[3].areaCovered + (SecondLegFreeSpaceWidth*SecondLegFreeSpaceLength);
            _persistentBlobs.erase(_persistentBlobs.begin() + 1);
            // _persistentBlobs.remove(2);
            // Log.i(Logger.FMBLOBS, "NOISE CANCELLATION : deleting single blob due to arching");
        }
    }
    if (_persistentBlobs.size() == 4) {
        int FirstLegUpperPart = _persistentBlobs[0].width;
        int FirstLegLowerPart = _persistentBlobs[1].width;
        int SecondLegUpperPart =  _persistentBlobs[2].width;
        int SecondLegLowerPart =  _persistentBlobs[3].width;
        int FirstLegFreeSpaceWidth = (( _persistentBlobs[1].maxy + 1) - _persistentBlobs[0].miny);
        int FirstLegFreeSpaceLength = (( _persistentBlobs[1].minx - 1) - _persistentBlobs[0].maxx);
        int SecondLegFreeSpaceWidth = (( _persistentBlobs[1].maxy + 1) - _persistentBlobs[0].miny);
        int SecondLegFreeSpaceLength = (( _persistentBlobs[1].minx - 1) - _persistentBlobs[0].maxx);
        //Log.i(Logger.FMBLOBS, "ARCH CHECK: "+FirstLegUpperPart+" "+FirstLegLowerPart+" "+SecondLegUpperPart+" "+SecondLegLowerPart+" "+FirstLegFreeSpaceLength+" "+SecondLegFreeSpaceLength+" "+FirstLegFreeSpaceWidth+" "+SecondLegFreeSpaceWidth);
        //if((FirstLegUpperPart == FirstLegLowerPart) && (SecondLegUpperPart == SecondLegLowerPart) && (FirstLegFreeSpaceLength <= 2  && SecondLegFreeSpaceLength <= 2)){
        if ((abs(FirstLegUpperPart - FirstLegLowerPart) <= 1 && abs(SecondLegUpperPart - SecondLegLowerPart) <= 1) && (FirstLegFreeSpaceLength <= 2 && SecondLegFreeSpaceLength <= 2)) {

            //if((Math.abs(FirstLegUpperPart - FirstLegLowerPart) <= 1 && Math.abs(SecondLegUpperPart - SecondLegLowerPart) <= 1) && (FirstLegFreeSpaceLength <= 2  && SecondLegFreeSpaceLength <= 2)){
            _persistentBlobs[0].maxx = _persistentBlobs[1].maxx;
            _persistentBlobs[2].maxx =  _persistentBlobs[3].maxx;
            _persistentBlobs[0].length = ( _persistentBlobs[1].maxx + 1) - _persistentBlobs[0].minx;
            _persistentBlobs[2].length = ( _persistentBlobs[2].maxx + 1) -  _persistentBlobs[2].minx;
            _persistentBlobs[0].width = ( _persistentBlobs[1].maxy + 1) - _persistentBlobs[0].miny;
            _persistentBlobs[2].width = ( _persistentBlobs[2].maxy + 1) -  _persistentBlobs[2].miny;
            _persistentBlobs[0].areaCovered = _persistentBlobs[0].areaCovered + _persistentBlobs[1].areaCovered + (FirstLegFreeSpaceWidth * FirstLegFreeSpaceLength);
            _persistentBlobs[2].areaCovered =  _persistentBlobs[2].areaCovered +  _persistentBlobs[3].areaCovered + (SecondLegFreeSpaceWidth * SecondLegFreeSpaceLength);
            _persistentBlobs.erase(_persistentBlobs.begin() + 1);
            _persistentBlobs.erase(_persistentBlobs.begin() + 2);
            //Log.i(Logger.FMBLOBS, "NOISE CANCELLATION : deleting double blobs due to arching");
        }
    }
    

}



bool Processor::foundNewFrame(const bool* _MAT_ARRAY, std::vector<Blob>& _persistentBlobs, int& _MATPixelCount, int _playerCount)
{
    int col = MAT_COLUMNS;
    int row = MAT_ROWS / _playerCount;
    
    


    std::vector<Blob> currentBlobs;

    _MATPixelCount = 0;

    // Begin loop to walk through every pixel
    for (int i = 0; i < row; ++i) {
        for (int j = 0; j < col; ++j) {
            if (_MAT_ARRAY[i * col + j] == 1) {

                _MATPixelCount++;
                bool found = false;
                for (auto &b : currentBlobs) {
                    if (b.isNear(j, i)) {
                        b.add(j, i);
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    Blob b(j, i);
                    currentBlobs.push_back(b);
                }
            }
        }
    }

    //std::cout << "Size of current blobs : " << currentBlobs.size() << std::endl;

    for (int i = currentBlobs.size() - 1; i >= 0; i--) {
        if (currentBlobs[i].size() < 4)
            currentBlobs.erase(currentBlobs.begin() + i);
    }
   
    //std::cout << "Size of current blobs : " << currentBlobs.size() << std::endl;

    if (_persistentBlobs.size() == 0 && currentBlobs.size() == 0)
        return false;

    if (_persistentBlobs.size() == 0 && currentBlobs.size() > 0) {
        for (auto b : currentBlobs) {
            _persistentBlobs.push_back(b);
        }
        return true;
    }


    else if (_persistentBlobs.size() == currentBlobs.size()) {


        bool changeInPattern = false;

        for (int i = 0; i <= currentBlobs.size() - 1; i++) {
            std::vector<int> centerB = _persistentBlobs[i].getCenter();
            std::vector<int> centerCB = currentBlobs[i].getCenter();


            int d = Utils::findEuDistance((int)centerB[0], (int)centerB[1], (int)centerCB[0], (int)centerCB[1]);

            if (d >= 2) {

                changeInPattern = true;
                break;
            }
        }

        if (changeInPattern) {
            _persistentBlobs.clear();
            for (Blob b : currentBlobs) {
                _persistentBlobs.push_back(b);
            }
            return true;
        }
        else {
            return false;
        }

    }
    else {

        _persistentBlobs.clear();
        for (Blob b : currentBlobs) {
            _persistentBlobs.push_back(b);
        }
        return true;
    }
}

