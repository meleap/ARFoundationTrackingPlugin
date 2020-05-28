# ARFoundationTrackingPlugin
ARFoundationのImageTrackingを使ったAR座標系補正のサンプル
# サンプルシーン ImageTracking
シーンの場所はScenes/ImageTracking

このサンプルは以下の機能を持つ
1. マーカーを使ったAR座標系の補正
1. ステレオ表示
1. ~~ARセッションの操作（リセット/リロード/ポーズ/レジューム）（ARFoundationの公式サンプル）~~ ← 絶対に必要とは感じなかったので実装しなかった

## マーカーを使ったAR座標系の補正
自分で用意したシーン・画像で補正を行うためには、以下の４つの手順の設定が必要
1. 検出画像のライブラリの作成
1. 画像検出スクリプトの設定
1. 画像を検出したときに補正するための位置・姿勢の設定
1. AR座標系補正スクリプトの設定

### 1. 検出画像のライブラリの作成
1. ProjectViewで右クリック - Create - XR - `Reference Image Library`
1. 作成した参照画像ライブラリに検出したい画像を設定し、各種パラメータを設定（ただし、`Name`は後で説明する`AROriginManager`が使用するため、画像毎に異なる名前をつけること）

### 2. 画像検出スクリプトの設定
1. 「AR Session Origin」スクリプトをアタッチしたオブジェクト（`AR Session Origin`）に`ARTrackedImageManager`(ARFoundationのスクリプト)をアタッチ
1. `ARTrackedImageManager`の`Serialized Library`に作成した参照画像ライブラリをセット
1. `ARTrackedImageManager`の`Max Numbur of Moving Images`に検出したい画像の枚数をセット
1. (デバッグ用)`ARTrackedImageManager`の`Tracked Image Prefab`に画像が検出された時に画像の位置に表示したプレハブをセット（`AROriginManager`がデバッグモードの時は上書きされる）

### 3. 画像を検出したときに補正するための位置・姿勢の設定
1. 1で登録した画像の`Name`と同じ名前のGameObject(以下オブジェクトAとする)を作成
1. （オプション）`Qaud`に画像を貼り付けたものをオブジェクトAの子オブジェクトとして入れておくと調整する時に便利。この時、`RotationのX`を90にしておくこと。
1. オブジェクトAを実空間に対応した位置にセットする。この時、画像を壁に貼る場合は`RotationのX`を-90にしておくこと。
1. 1-3を登録した画像の数だけ繰り返す

### 4. AR座標系補正スクリプトの設定
1. `AR Session Origin`スクリプトをアタッチしたオブジェクト（`AR Session Origin`）に`AROriginManager`をアタッチ
1. `AROriginManager`の`MarkerTransformOnUnityCoordinate`に3で作成したオブジェクトをセット
<img src="https://github.com/meleap/ARFoundationImageTrackingCaribrationOrigin/blob/master/images/MarkerSetting.png" width="512">

デバッグ用
1. AROriginManagerの`isDebug`にチェックを入れる
1. AROriginManagerの`worldSpaceCanvasCamera`にAR `Camera`オブジェクトをセット
1. AROriginManagerの`Tracked Image Prefab`に`Prefabs/ImageInfoPrefab`をセット

## ステレオ表示
１つのカメラでレンダリングした画像を左右に2分割してステレオ表示する.物理的なサイズと画素単位の変換にはunityのAPIから取得できるデバイスの画素密度[ppi]を用いる。


<img src="https://github.com/meleap/ARFoundationImageTrackingCaribrationOrigin/blob/master/images/StereoOverview.png" width="512">

1. `Stereo Camera Manager`をステレオ表示したい`Camera`を持つオブジェクトにアタッチ
1. `Stereo Camera Manager`の３つのパラメータを設定する。パラメータはIPD(眼幅)、中心のY座標（0〜1）、画面サイズに対する拡大率。
<img src="https://github.com/meleap/ARFoundationImageTrackingCaribrationOrigin/blob/master/images/StereoSettings.png" width="512">
