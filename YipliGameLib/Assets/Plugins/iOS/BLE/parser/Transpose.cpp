//
// Created by admin on 27-01-2021.
//

#include "Transpose.h"
#include "../support/Const.h"
#include "../support/Utils.h"
#include <sstream>

#include <bitset>



void Transpose::parseHexString(bool* _MAT_ARRAY, std::string const& _input) {
    
    switch (hashit(_input.substr(2,2))) {
        case LEGACY:
            decodeLegacy(_MAT_ARRAY, _input);
            break;
            
        default:
            break;
    }
}


void Transpose::decodeMSA(bool *_MAT_ARRAY, std::string const& _input) {
    
}

void Transpose::decodeLegacy(bool *_MAT_ARRAY, std::string const& _input) {
    std::vector<std::string> bytes;
    Utils::splitEqually(_input, BUFFER_SIZE, bytes);

    int num_start_bytes = 2;        // number of start bytes in the stream
    int num_stop_bytes = 0;         // number of stop bytes in the stream
    int total_bytes = BUFFER_SIZE; // totals bytes in the stream

    int offset = 1;                 // indexing starts with 0 in code, hence to map it to real indexing

    std::string FMData = "";

    for (int index = 0 + num_start_bytes; index <= (total_bytes - offset) - num_stop_bytes; index++) {
        // converting the hex string input to binary string
        std::string binString = "0x" + bytes[index];
       

        std::stringstream bin;
        bin << std::hex << binString;
        unsigned n;
        bin >> n;
        std::bitset<8> b(n);
        binString = b.to_string();

        reverse(binString.begin(), binString.end());
        FMData += binString;

    }

    int temp_array[MAT_ROWS * MAT_COLUMNS];

    for (int i = 0; i < MAT_COLUMNS; i++) {
        for (int j = 0; j < MAT_ROWS; j++) {
            temp_array[i + j * MAT_COLUMNS] = (FMData[static_cast<std::basic_string<char, std::char_traits<char>, std::allocator<char>>::size_type>(i) * MAT_ROWS + j]) - '0';
            _MAT_ARRAY[(j)*MAT_COLUMNS + (i)] = temp_array[j * MAT_COLUMNS + i];

        }
    }
}

EncodingAlgorithms Transpose::hashit(std::string const& inString) {
    
    if (inString == "A5")
        return LEGACY;
    
    if (inString == "B6")
        return MSA;
    
    return LEGACY;
}

