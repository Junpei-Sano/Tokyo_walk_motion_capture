using System;
using UnityEngine;

namespace PlayerManager
{
    public enum CharacterSelection
    {
        debug,
        sample1,
        sample2,
        sample3,
        sample4,
        mediaPipe
    }

    /* 注意点
     * ・継承禁止
     * 　・子オブジェクトのinspecterでの値に依存してしまうため。
     * 　・Instantiateされたオブジェクトでは値が保持されない？ため。
     */

    public class PlayerInformation : MonoBehaviour
    {
        public static PlayerInformation instance { get; private set; }

        [SerializeField] private string _playerName = "Anonymous";
        [SerializeField] private bool _usePUN2;
        [SerializeField] private bool _showModel;
        [SerializeField] private ControllerType _controllerType;
        [SerializeField] private CharacterSelection _chracterSelection;
        [SerializeField] private Vector3 _spawnPosition = Vector3.zero;
        [SerializeField] private bool _wind = true;
        [SerializeField] private int _frameRate = 0;    // 0のときは設定しない

        public string playerName { get { return _playerName; } }
        public bool usePUN2 { get { return _usePUN2; } }
        public bool showModel { get { return _showModel; } }
        public ControllerType controllerType { get { return _controllerType; } }
        public CharacterSelection characterScript { get { return _chracterSelection; } }
        public Vector3 spawnPosition { get { return _spawnPosition; } }
        public bool wind { get { return _wind;  } }

        public string prefabName { get; private set; } = "";

        private string[] _prefabNames = new string[10];    // とりあえず10個まで

        private void Awake()
        {
            // このオブジェクトは全体で1つだけ
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;

            if (_frameRate != 0) { Application.targetFrameRate = _frameRate; }

            _prefabNames[(int)CharacterSelection.debug] = "Debug";
            _prefabNames[(int)CharacterSelection.sample1] = "Player";
            _prefabNames[(int)CharacterSelection.sample2] = "Player2";
            _prefabNames[(int)CharacterSelection.sample3] = "Player3";
            _prefabNames[(int)CharacterSelection.sample4] = "Player4";
            _prefabNames[(int)CharacterSelection.mediaPipe] = "MediaPipe";

            prefabName = _prefabNames[(int)characterScript];
            DontDestroyOnLoad(this);
        }

        public void setPlayerName(string name)
        {
            this._playerName = name;
        }
        public void setUsePUN2(bool usePun2) { this._usePUN2 = usePun2; }
        public void setIsShowingModel(bool showModel) { this._showModel = showModel; }
        public void setWind(bool wind) { this._wind = wind; }
        public void setMediaPipe(bool useMediapipe)
        {
            if (useMediapipe)
            {
                _controllerType = ControllerType.udp_mediapipe;
            }
            else
            {
                _controllerType = ControllerType.input_system_wing;
            }
        }
    }

    public class PlayerInformationBase : MonoBehaviour
    {
        private PlayerInformation _playerInfo;

        private bool _usePUN2;
        private ControllerType _controllerType;
        private CharacterSelection _chracterSelection;
        private Vector3 _spawnPosition;
        private bool _wind;

        public string playerName { get; protected set; }
        protected bool usePUN2 { get { return _usePUN2; } }
        protected bool showModel { get; set; }
        protected ControllerType controllerType { get { return _controllerType; } }
        protected CharacterSelection characterScript { get { return _chracterSelection; } }
        protected Vector3 spawnPosition { get { return _spawnPosition; } }
        protected bool wind { get { return _wind; } }
        protected string prefabName { get { return _playerInfo.prefabName; } }

        protected virtual void Awake()
        {
            this._playerInfo = GameObject.Find("Player Information").GetComponent<PlayerInformation>();
            this.playerName = _playerInfo.playerName;
            this._usePUN2 = _playerInfo.usePUN2;
            this.showModel = _playerInfo.showModel;
            this._controllerType = _playerInfo.controllerType;
            this._chracterSelection = _playerInfo.characterScript;
            this._spawnPosition = _playerInfo.spawnPosition;
            this._wind = _playerInfo.wind;
        }
    }

    // 上のクラスの継承元が違うだけ
    public class PlayerInformationBasePun2 : Photon.Pun.MonoBehaviourPunCallbacks
    {
        private PlayerInformation _playerInfo;

        private bool _usePUN2;
        private ControllerType _controllerType;
        private CharacterSelection _chracterSelection;
        private Vector3 _spawnPosition;
        private bool _wind;

        public string playerName { get; protected set; }
        protected bool usePUN2 { get { return _usePUN2; } }
        protected bool showModel { get; set; }
        protected ControllerType controllerType { get { return _controllerType; } }
        protected CharacterSelection characterScript { get { return _chracterSelection; } }
        protected Vector3 spawnPosition { get { return _spawnPosition; } }
        protected bool wind { get { return _wind; } }
        protected string prefabName { get { return _playerInfo.prefabName; } }

        protected virtual void Awake()
        {
            this._playerInfo = GameObject.Find("Player Information").GetComponent<PlayerInformation>();
            this.playerName = _playerInfo.playerName;
            this._usePUN2 = _playerInfo.usePUN2;
            this.showModel = _playerInfo.showModel;
            this._controllerType = _playerInfo.controllerType;
            this._chracterSelection = _playerInfo.characterScript;
            this._spawnPosition = _playerInfo.spawnPosition;
            this._wind = _playerInfo.wind;
        }
    }
}
