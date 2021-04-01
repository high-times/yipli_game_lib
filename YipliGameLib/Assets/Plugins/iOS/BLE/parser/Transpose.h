//
// Created by admin on 27-01-2021.
//
#pragma once
#include <iostream>

#ifndef FMINTERFACE_TRANSPOSE_H
#define FMINTERFACE_TRANSPOSE_H

enum EncodingAlgorithms{
    LEGACY,
    MSA
};

class Transpose {
public:
    void parseHexString(bool *_MAT_ARRAY, std::string const& _input);
    void decodeMSA(bool *_MAT_ARRAY, std::string const& _input);
    void decodeLegacy(bool *_MAT_ARRAY, std::string const&  _input);
    EncodingAlgorithms hashit(std::string const& inString);
};


#endif //FMINTERFACE_TRANSPOSE_H
