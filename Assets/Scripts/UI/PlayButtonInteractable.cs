using System.Collections;
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
            _button = GetComponent<Button>();
        }

        private void OnDisable()
        {
            UsernameInputField.Instance.OnUsernameValidation -= ButtonInteractable;
        }

        private void OnEnable()
        {
            StartCoroutine(AddButtonInteractableListener());
        }

        private IEnumerator AddButtonInteractableListener()
        {
            yield return new WaitUntil(() => UsernameInputField.Instance != null);

            UsernameInputField.Instance.OnUsernameValidation += ButtonInteractable;
        }

        private void ButtonInteractable(bool isValid)
        {
            _button.interactable = isValid;
        }
    }
}