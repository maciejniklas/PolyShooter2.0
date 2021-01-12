using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Controls button ability to interaction
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class PlayButtonInteractable : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            // Reference
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            UsernameInputField.Instance.OnUsernameValidation += ButtonInteractable;
        }

        private void ButtonInteractable(bool isValid)
        {
            _button.interactable = isValid;
        }
    }
}