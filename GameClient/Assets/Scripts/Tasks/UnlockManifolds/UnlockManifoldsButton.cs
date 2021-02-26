using UnityEngine;
using UnityEngine.UI;

public class UnlockManifoldsButton : MonoBehaviour {

    private int _value;
    private Text _buttonText;
    private Button _button;
    private UnlockManifoldsTask _parentTask;

    public void Initialize(int value, UnlockManifoldsTask parentTask) {
        if (_buttonText == null) {
            _buttonText = GetComponentInChildren<Text>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonPressed);
        }

        _value = value;
        _buttonText.text = value.ToString();
        _parentTask = parentTask;
    }

    public void ToggleButton(bool isOn) {
        _button.interactable = isOn;
    }

    private void OnButtonPressed() {
        _parentTask.OnButtonPressed(_value, this);
    }

}