using UnityEngine;

namespace Logging
{
    [AddComponentMenu("Assets/Scripts/Logging")]
    public class Logger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool showLogs;
        [SerializeField] private string prefix;
        [SerializeField] private Color prefixColor;
        private string _hexColor;
        [SerializeField] private LogType logType;
    

#if UNITY_EDITOR
        private void OnValidate() 
        {
            _hexColor = "#" + ColorUtility.ToHtmlStringRGBA(prefixColor);    
        }
#endif

        public void Log(string message, Object sender)
        {
            if(!showLogs) return;

            //Debug.Log($"{_prefix}: {message}", sender);
            Debug.Log($"<color={_hexColor}>{logType} {prefix}: {message}</color>", sender);
        }
    }
}

