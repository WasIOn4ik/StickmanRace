using UnityEngine;

namespace YG.Example
{
    public class OpenAuthorizationDialog : MonoBehaviour
    {
#if UNITY_WEBGL
        public void OpenAuthDialog()
        {
            YandexGame.AuthDialog();
        }
#endif
    }
}