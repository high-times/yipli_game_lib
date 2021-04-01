//
// Created by admin on 27-01-2021.
//
#pragma once
#ifndef FMINTERFACE_CLUSTER_H
#define FMINTERFACE_CLUSTER_H

#include "Games.h"
#include "../connection/ResponsePackager.h"
#include "../support/Utils.h"

class Cluster {

public:

    /*********************
    * Member variables
    *********************/
    int m_clusterId = 0;
    std::vector<Blob>& m_persistentBlobs;
    int& m_matPixelCount;
    Games m_games = Games(m_persistentBlobs, m_matPixelCount);
    //LiveClassKids lckids;

    /*********************
    * Constructors
    *********************/
    Cluster(std::vector<Blob>& _persistentBlobs, int& _MATPixelCount);

    /*********************
    * Member functions
    *********************/
    void setClusterID(const int _clusterId);
    int getClusterID();
    ResponsePackager clusterCheckSequence(ResponsePackager& responsePackager);

};
#endif //FMINTERFACE_CLUSTER_H