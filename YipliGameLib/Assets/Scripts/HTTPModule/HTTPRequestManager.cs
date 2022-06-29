using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Access a website and use UnityWebRequest.Get to download a page.
// Also try to download a non-existing page. Display the error.
namespace Yipli.HttpMpdule 
{
    public class HTTPRequestManager : MonoBehaviour
    {
        string getUserIDUrl = "https://us-central1-yipli-project.cloudfunctions.net/adminPanel/user/details?userId=lC4qqZCFEaMogYswKjd0ObE6nD43&detail=fullProfile&apiTocken=bfFzw8p9LgZIXc7N";

        // Unity Oprations



        // Data Operations
        public string GetWindowsYipliAppDownloadURL() {
            return null;
        }

        public void SetGameData() {
            
        }

        // Request Operations
        IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        break;

                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;

                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        break;

                    default:
                        Debug.Log("Default case");
                        break;
                }
            }
        }        
    }
}
