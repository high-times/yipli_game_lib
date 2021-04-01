//
// Created by admin on 27-01-2021.
//
#pragma once
#include <string>
#include <vector>
#ifndef FMINTERFACE_BLOB_H
#define FMINTERFACE_BLOB_H


class Blob {
private:
public:
    //Plot parameters
    int minx{ 0 };
    int miny{ 0 };
    int maxx{ 0 };
    int maxy{ 0 };

    //Area parameters
    int areaCovered{ 0 };
    int length{ 0 };
    int width{ 0 };

    //feature parameters
    float circularity{ 0 };
    std::string shapeType = "null";
    std::string orientation = "null";

    //property parameters
    int distThreshold{ 1 };
    int id{ 0 };
    bool taken{ false };
    int lifespan{ 100 };



    Blob();
    Blob(int _x, int _y);
    void add(int x, int y);
    int size();
    std::vector<int> getCenter();
    bool isNear(int _x, int _y);
    int distSq(int _x1, int _y1, int _x2, int _y2);
    int findEuDistance(int _x1, int _y1, int _x2, int _y2);
   
};


#endif //FMINTERFACE_BLOB_H
