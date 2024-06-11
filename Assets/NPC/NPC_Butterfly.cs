using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Butterfly_NPC
{
    public class ButterflyFormation : MonoBehaviour
    {
        public ButterflyFormation next = null;    // プレイヤー側から設定
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
        private　readonly Vector3 _worldMax = new Vector3(450, -150, 800);
        private readonly Vector3 _worldMin = new Vector3(-450, -200, -800);

        private readonly float _distance = 20.0f;    // 蝶々同士の間隔
        private readonly int _frameSplit = 5;    // このフレーム数をかけて前の蝶を追従する
        private Vector3 _nextPos;
        private Quaternion _nextRot;

        private void Awake()
        {
            ReplaceObject();
        }

        private void Update()
        {
            if (prev == null) { return; }

            // 場所を更新
            this.transform.position = _nextPos;
            this.transform.rotation = _nextRot;
            CalcNextPosition();
        }

        // 次の座標（_goalPos, _goalRot）を計算
        private void CalcNextPosition()
        {
            Vector3 goalPos = prev.transform.position - (prev.transform.forward * _distance);
            // 現在位置と目標位置の間から内分点を計算する
            //_nextPos = ((_frameSplit - 1) * this.transform.position + goalPos) / _frameSplit;
            _nextPos = this.transform.position + (goalPos - this.transform.position) / _frameSplit;
            _nextRot = prev.transform.rotation;    // 回転は1フレームで反映する
        }

        public void SetFollowing()
        {
            CalcNextPosition();
        }

        // 再配置
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
