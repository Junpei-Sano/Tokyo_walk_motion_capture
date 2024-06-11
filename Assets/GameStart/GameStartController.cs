using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using PlayerManager;

namespace Gamestart
{
    public class GameStartController : MonoBehaviour
    {
        //[SerializeField] private InputField _inputField;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Toggle _showModel;
        [SerializeField] private Toggle _wind;
        [SerializeField] private Toggle _useMediaPipe;
        [SerializeField] private GameObject _startMenu;
        [SerializeField] private GameObject _optionMenu;
        private PlayerInformation _info;
        private string _defaultFieldText;


        private void Start()
        {
            _info = GameObject.Find("Player Information").GetComponent<PlayerInformation>();
            _defaultFieldText = _inputField.text;
            ReflectSettings();
        }

        private void ReflectSettings()
        {
            if (string.Compare(_info.playerName, "Anonymous") != 0)
            {
                _inputField.text = _info.playerName;
            }
            _showModel.isOn = _info.showModel;
            _wind.isOn = _info.wind;
            _useMediaPipe.isOn = (_info.controllerType == ControllerType.udp_mediapipe);
        }

        private void LoadGameScene()
        {
            SceneManager.LoadScene("SamplePhotonScene");
        }

        public void StartGame()
        {
            _info.setUsePUN2(false);
            LoadGameScene();
        }

        public void StartPUN2Game()
        {
            _info.setUsePUN2(true);
            LoadGameScene();
        }

        public void SetPlayerName()
        {
            string newName = _inputField.text;
            if (string.Compare(_defaultFieldText, newName) != 0)
            {
                _info.setPlayerName(_inputField.text);
            }
        }

        public void QuitGame()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
        #endif
        }

        public void OptionScreen()
        {
            _startMenu.SetActive(false);
            _optionMenu.SetActive(true);
        }

        // à»â∫ÇÕê›íËâÊñ óp

        public void BackStartMenu()
        {
            _optionMenu.SetActive(false);
            _startMenu.SetActive(true);
        }

        public void SetShowModel()
        {
            _info.setIsShowingModel(_showModel.isOn);
        }

        public void SetWind()
        {
            _info.setWind(_wind.isOn);
        }

        public void SetUseMediaPipe()
        {
            _info.setMediaPipe(_useMediaPipe.isOn);
        }
    }
}