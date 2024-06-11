using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Butterfly_NPC
{
    public class ButterflyFormation : MonoBehaviour
    {
        public ButterflyFormation next = null;    // �v���C���[������ݒ�
        public ButterflyFormation prev = null;
        public ButterflyFormation player = null;

        public int GetScore()
        {
            if (next == null) { return 0; }
            else { return next.GetScore() + 1; }
        }
    }

    public class NPC_Butterfly : ButterflyFormation
    {
        private�@readonly Vector3 _worldMax = new Vector3(450, -150, 800);
        private readonly Vector3 _worldMin = new Vector3(-450, -200, -800);

        private readonly float _distance = 20.0f;    // ���X���m�̊Ԋu
        private readonly int _frameSplit = 5;    // ���̃t���[�����������đO�̒���Ǐ]����
        private Vector3 _nextPos;
        private Quaternion _nextRot;

        private void Awake()
        {
            ReplaceObject();
        }

        private void Update()
        {
            if (prev == null) { return; }

            // �ꏊ���X�V
            this.transform.position = _nextPos;
            this.transform.rotation = _nextRot;
            CalcNextPosition();
        }

        // ���̍��W�i_goalPos, _goalRot�j���v�Z
        private void CalcNextPosition()
        {
            Vector3 goalPos = prev.transform.position - (prev.transform.forward * _distance);
            // ���݈ʒu�ƖڕW�ʒu�̊Ԃ�������_���v�Z����
            //_nextPos = ((_frameSplit - 1) * this.transform.position + goalPos) / _frameSplit;
            _nextPos = this.transform.position + (goalPos - this.transform.position) / _frameSplit;
            _nextRot = prev.transform.rotation;    // ��]��1�t���[���Ŕ��f����
        }

        public void SetFollowing()
        {
            CalcNextPosition();
        }

        // �Ĕz�u
        private void ReplaceObject()
        {
            Vector3 pos;
            Vector3 max = _worldMax - _worldMin;
            pos.x = Random.Range(0, max.x);
            pos.y = Random.Range(0, max.y);
            pos.z = Random.Range(0, max.z);
            pos += _worldMin;
            this.transform.position = pos;
            this.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
        }
    }
}
