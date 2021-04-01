//
// Created by admin on 27-01-2021.
//
#pragma once
#ifndef FMINTERFACE_UTILS_H
#define FMINTERFACE_UTILS_H

#include "Const.h"
#include <chrono>
#include <algorithm>
#include <vector>
#include <string>
#include <iostream>

#define VERBOSE 2
#define DEBUG 3
#define INFO 4
#define WARN 5
#define ERROR 6

//#define ANDROID


#ifdef ANDROID
#include <android/log.h>
#include <jni.h>

class NDKUtils {

public:

    static std::string jstring2string(JNIEnv* env, jstring jStr) {
        if (!jStr)
            return "";

        const jclass stringClass = env->GetObjectClass(jStr);
        const jmethodID getBytes = env->GetMethodID(stringClass, "getBytes", "(Ljava/lang/String;)[B");
        const jbyteArray stringJbytes = (jbyteArray)env->CallObjectMethod(jStr, getBytes, env->NewStringUTF("UTF-8"));

        size_t length = (size_t)env->GetArrayLength(stringJbytes);
        jbyte* pBytes = env->GetByteArrayElements(stringJbytes, NULL);

        std::string ret = std::string((char*)pBytes, length);
        env->ReleaseByteArrayElements(stringJbytes, pBytes, JNI_ABORT);

        env->DeleteLocalRef(stringJbytes);
        env->DeleteLocalRef(stringClass);


        return ret;
    }


};

#define FMLOG(LOG_LEVEL, TAG, DATA) __android_log_print(LOG_LEVEL, TAG,"%s", DATA)
#else
#define FMLOG(LOG_LEVEL, TAG, DATA) std::cout << __FUNCTION__ << " : " << __LINE__ << " : " << DATA << std::endl
#endif



class Utils {

public:

    inline static const char* const BoolToString(bool b)
    {
        return b ? "true" : "false";

    }
    static void splitEqually(std::string str, int n, std::vector<std::string>& bytes)
    {
        int str_size = str.length();
        int i;
        int part_size;


        // Calculate the size of parts to 
        // find the division points 
        part_size = str_size / n;
        std::string temp = "";
        for (i = 0; i <= str_size; i++)
        {
            if (i % part_size == 0) {

                if (i > 0) {
                    bytes.push_back(temp);
                    temp = "";
                }

            }

            //cout << str[i] << endl;
            temp += str[i];

        }
    }


    static void resetTable(bool* _MAT_ARRAY) {
        int _size = MAT_SIZE;
        for (int i = 0; i < _size; i++)
            _MAT_ARRAY[i] = 0;
    }


    static long long getCurrentTimestamp() {
        using namespace std::chrono;
        return duration_cast<milliseconds>(system_clock::now().time_since_epoch()).count();
    }



    inline static int findEuDistance(int _x1, int _y1, int _x2, int _y2) {
        int d = (_x2 - _x1) + (_y2 - _y1);
        return d;
    }

    static void ghostMatAlgo(bool* _MAT_ARRAY) {
        int col = MAT_COLUMNS;
        int row = MAT_ROWS;

        for (int i = 0; i < row; ++i) {
            for (int j = 0; j < col; ++j) {

                if (_MAT_ARRAY[i * col + j] == 1) {
                    if (i == 0 && j == 0) {
                        if ((_MAT_ARRAY[(i + 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        continue;
                    }

                    if (i == row - 1 && j == col - 1) {
                        if ((_MAT_ARRAY[(i - 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }

                        continue;
                    }

                    if (i == 0) {
                        if ((_MAT_ARRAY[(i + 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        if ((_MAT_ARRAY[(i + 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        continue;
                    }
                    if (i == row - 1) {
                        if ((_MAT_ARRAY[(i - 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }
                        if ((_MAT_ARRAY[(i - 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }
                        continue;
                    }

                    if (j == 0) {
                        if ((_MAT_ARRAY[(i + 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        if ((_MAT_ARRAY[(i - 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }
                        continue;
                    }
                    if (j == col - 1) {
                        if ((_MAT_ARRAY[(i - 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }
                        if ((_MAT_ARRAY[(i + 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        continue;
                    }


                    if ((_MAT_ARRAY[(i + 1) * col + (j + 1)] == 1)) {
                        _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                        _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                    }
                    if ((_MAT_ARRAY[(i - 1) * col + (j - 1)] == 1)) {
                        _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                        _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                    }
                    if ((_MAT_ARRAY[(i + 1) * col + (j - 1)] == 1)) {
                        _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                        _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                    }
                    if ((_MAT_ARRAY[(i - 1) * col + (j + 1)] == 1)) {
                        _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                        _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                    }
                }
            }
        }


        for (int i = row - 1; i > 0; --i) {
            for (int j = col - 1; j > 0; --j) {

                if (_MAT_ARRAY[i * col + j] == 1) {

                    if (i == 0 && j == 0) {
                        if ((_MAT_ARRAY[(i + 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        continue;
                    }

                    if (i == row - 1 && j == col - 1) {
                        if ((_MAT_ARRAY[(i - 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }

                        continue;
                    }

                    if (i == 0) {
                        if ((_MAT_ARRAY[(i + 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        if ((_MAT_ARRAY[(i + 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        continue;
                    }
                    if (i == row - 1) {
                        if ((_MAT_ARRAY[(i - 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }
                        if ((_MAT_ARRAY[(i - 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }
                        continue;
                    }

                    if (j == 0) {
                        if ((_MAT_ARRAY[(i + 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        if ((_MAT_ARRAY[(i - 1) * col + (j + 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }
                        continue;
                    }
                    if (j == col - 1) {
                        if ((_MAT_ARRAY[(i - 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                        }
                        if ((_MAT_ARRAY[(i + 1) * col + (j - 1)] == 1)) {
                            _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                            _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                        }
                        continue;
                    }

                    if ((_MAT_ARRAY[(i + 1) * col + (j + 1)] == 1)) {
                        _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                        _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                    }
                    if ((_MAT_ARRAY[(i - 1) * col + (j - 1)] == 1)) {
                        _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                        _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                    }
                    if ((_MAT_ARRAY[(i + 1) * col + (j - 1)] == 1)) {
                        _MAT_ARRAY[(i)*col + (j - 1)] = 1;
                        _MAT_ARRAY[(i + 1) * col + (j)] = 1;
                    }
                    if ((_MAT_ARRAY[(i - 1) * col + (j + 1)] == 1)) {
                        _MAT_ARRAY[(i)*col + (j + 1)] = 1;
                        _MAT_ARRAY[(i - 1) * col + (j)] = 1;
                    }


                }

            }
        }

    }

};
#endif //FMINTERFACE_UTILS_H
