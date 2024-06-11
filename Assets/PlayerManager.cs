using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace PlayerManager
{
    public class PlayerManager : PlayerInformationBase
    {
        public GameObject playerObject { get; private set; } = null;

        GameObject _photonController;
        private SimplePun _simplePun;

        protected virtual void OnEnable()
        {
            _photonController = GameObject.Find("Photon Controller").gameObject;
            _simplePun = _photonController.GetComponent<SimplePun>();
        }

        protected virtual void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            if (base.usePUN2)
            {
                _simplePun.enabled = true;
            }
            else
            {
                GameObject player = _photonController.GetComponent<MyPhotonPrefabPool>().Instantiate(base.prefabName, base.spawnPosition, Quaternion.identity);
                player.SetActive(true);
                StartGame(player);
            }
        }

        private void StopAllChildrenRender(Transform parent)    // �������\���i�ċA�ŏI�[�I�u�W�F�N�g�܂Łj
        {
            foreach (Transform child in parent)
            {
                MeshRenderer m = child.GetComponent<MeshRenderer>();    // �q�E���Ȃǂ��ׂĂ�
                if (m != null)
                {
                    m.enabled = false;
                    //Debug.Log("Stop Renderer: " + child.name);
                }
                StopAllChildrenRender(child);    // �ċA
            }
        }

        public void StartGame(GameObject player)
        {
            this.playerObject = player;

            if (!base.showModel)
            {
                /* �������\�� */
                GameObject myModel = player.transform.Find("Model/Model").gameObject;
                StopAllChildrenRender(myModel.transform);
                /* �J���������̈ʒu�Ɉړ� */
                player.transform.Find("Model/Model").transform.position += new Vector3(0, 0, -3);
            }

            /* ��������������ł���悤�ɃX�N���v�g�A�J������L���ɂ��� */
            switch (base.characterScript)
            {
                case CharacterSelection.debug:
                    ForDebugging debugScript = player.GetComponent<ForDebugging>();
                    debugScript.enabled = true;
                    break;
                case CharacterSelection.sample1:
                    SampleCharacter1 characterScript1 = player.GetComponent<SampleCharacter1>();
                    characterScript1.controllerType = base.controllerType;
                    characterScript1.enabled = true;
                    break;
                case CharacterSelection.sample2:
                    SampleCharacter2 characterScript2 = player.GetComponent<SampleCharacter2>();
                    characterScript2.controllerType = base.controllerType;
                    characterScript2.enabled = true;
                    break;
                case CharacterSelection.sample3:
                    SampleCharacter3 characterScript3 = player.GetComponent<SampleCharacter3>();
                    characterScript3.enabled = true;
                    break;
                case CharacterSelection.sample4:
                    SampleCharacter3 characterScript4 = player.GetComponent<SampleCharacter3>();
                    characterScript4.enabled = true;
                    break;
                case CharacterSelection.mediaPipe:
                    SampleCharacter3 characterScript5 = player.GetComponent<SampleCharacter3>();
                    characterScript5.enabled = true;
                    break;
                default:
                    Debug.LogError("Undefined Character Script");
                    break;
            }

            // Open XR�ύX�ɂ�
            //GameObject camera = player.transform.Find("OVRPlayerController").gameObject;
            GameObject camera = player.transform.Find("XR Origin").gameObject;
            camera.SetActive(true);

            //player.transform.Find("UIHelpers").gameObject.SetActive(true);
        }

        protected void Respawn()
        {
            if (base.usePUN2)    // �����炭���ݕs�g�p
            {
                _simplePun.DisconnectPlayer();
            }
            else
            {
                SceneManager.LoadScene("SamplePhotonScene");
            }
        }

        protected void LoadStartScene()
        {
            if (base.usePUN2)
            {
                _simplePun.DisconnectPlayer();
            }
            SceneManager.LoadScene("StartSceneSample");
        }
    }
}
