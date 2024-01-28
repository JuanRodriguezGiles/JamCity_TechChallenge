using UnityEngine;

using TMPro;

namespace TechChallenge.Scripts.UI
{
    public class TestsUI : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TextMeshProUGUI txtStatus = null;
        #endregion

        #region UNITY_CALLS
        public void Start()
        {
            txtStatus.text = string.Empty;
        }
        #endregion
        
        #region PUBLIC_METHODS
        public void UpdateText(string text)
        {
            txtStatus.text += "\n" + text;
        }
        #endregion
    }
}