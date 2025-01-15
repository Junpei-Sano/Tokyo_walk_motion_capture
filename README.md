# Tokyo_walk_motion_capture
- モーションキャプチャから得られたデータでプレイヤーを操作するゲーム
- [https://www.notion.so/RNN-7db886672ad24f2ba01a8fef4446d488?pvs=4](https://marred-seatbelt-edc.notion.site/RNN-7db886672ad24f2ba01a8fef4446d488)

## PyTorch/
- Pythonで動かすプログラム類
- モーション分類結果をUDPでUnityに送信する

### PyTorch/MediaPipe/
- モーションキャプチャとしてMediaPipeを使用する場合
- 実行ファイル：`LSTM_unity.ipynb`
- 2台のカメラが必要

### PyTorch/OptiTrack/
- モーションキャプチャとしてOptiTrackを使用する場合
- 実行ファイル：`mocap_LSTM_unity.ipynb`
- 注：リアルタイム処理は未対応
  - 現状、Optitrackで録画したCSVファイルを再生することで再現

## その他ファイル
- Unityのプロジェクトファイル
