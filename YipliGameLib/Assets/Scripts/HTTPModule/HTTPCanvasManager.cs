using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yipli.HttpMpdule
{
    public class HTTPCanvasManager : MonoBehaviour
    {
        [Header("UI Objects")]
        [SerializeField] private Canvas httpCanvas = null;

        void Awake()
        {
            httpCanvas.worldCamera = Camera.main;
        }
    }
}