using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yipli.HttpMpdule
{
    public class HTTPAnimationFunctionActivator : MonoBehaviour
    {
        [SerializeField] HTTPMatController httpMatController = null;

        public void UpdatePlayerSelectionInfo()
        {
            httpMatController.StartScrolling();
        }
    }
}