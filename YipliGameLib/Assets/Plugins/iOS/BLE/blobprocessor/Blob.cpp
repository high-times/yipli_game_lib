//
// Created by admin on 27-01-2021.
//

#include "Blob.h"
#include <iostream>

#include <algorithm>
#include <cmath>

Blob::Blob()
{
	std::cout << "Blob()";
}

Blob::Blob(int _x, int _y)
{
	minx = _x;
	miny = _y;
	maxx = _x;
	maxy = _y;

	length = (maxx + 1) - minx;
	width = (maxy + 1) - miny;
	areaCovered++;
}

void Blob::add(int x, int y)
{
	minx = std::min(minx, x);
	miny = std::min(miny, y);
	maxx = std::max(maxx, x);
	maxy = std::max(maxy, y);


	length = (maxx + 1) - minx;
	width = (maxy + 1) - miny;
	areaCovered++;
}

int Blob::size()
{
	return (maxx - minx + 1) * (maxy - miny + 1);
}

std::vector<int> Blob::getCenter()
{
	int x = (int)ceil((float)((maxx - minx) / 2)) + minx;
	int y = (int)ceil((float)((maxy - miny) / 2)) + miny;
	std::vector<int> _center_points{ x + 1, y + 1 };
	return _center_points;
}


bool Blob::isNear(int _x, int _y)
{
	int cx = std::max(std::min(_x, maxx), minx);
	int cy = std::max(std::min(_y, maxy), miny);
	int d = distSq(cx, cy, _x, _y);

	if (d <= distThreshold * distThreshold) {
		return true;
	}
	else {
		return false;
	}
}

int Blob::distSq(int _x1, int _y1, int _x2, int _y2)
{
	int d = (_x2 - _x1) * (_x2 - _x1) + (_y2 - _y1) * (_y2 - _y1);
	return d;
}


int Blob::findEuDistance(int _x1, int _y1, int _x2, int _y2) {
	int d = (_x2 - _x1) + (_y2 - _y1);
	return d;
}