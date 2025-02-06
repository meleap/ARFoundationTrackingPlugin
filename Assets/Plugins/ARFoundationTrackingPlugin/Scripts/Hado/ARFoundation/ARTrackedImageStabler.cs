using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Hado.ARFoundation
{
    public class ARTrackedImageStabler : IDisposable
    {
        // marker name -> Anchor
        private readonly Dictionary<string, Anchor> _detectedAnchorDic = new();
        private readonly Subject<ARTrackedImage> _subject = new();
        private IDisposable _disposable;

        private float movingNoiseThreshold = 0.05f;

        /// フレーム間の移動距離がこの値より大きい場合はノイズとして捨てる
        public float MovingNoiseThreshold
        {
            get => movingNoiseThreshold;
            set
            {
                movingNoiseThreshold = value;
                Clear();
            }
        }

        private int noiseCheckSampleCount = 2;

        /// MovingNoiseThresholdのチェックを何回ぶん行うか
        public int NoiseCheckSampleCount
        {
            get => noiseCheckSampleCount;
            set
            {
                noiseCheckSampleCount = value;
                Clear();
            }
        }

        private readonly Subject<(Vector3, Quaternion)> _trackedImageSubject = new();

        // このクラスはマーカーだけの責務にしたいが、今の実装上やむなくWorldAnchorの存在に依存している
        /// ImageTrackingで認識したマーカーの座標が複数フレームにわたって安定したとき、移動すべきWorldAnchorの座標を返します
        public IObservable<(Vector3, Quaternion)> TrackedImageObservable => _trackedImageSubject;

        public ARTrackedImageStabler()
        {
            Clear();
        }

        public void OnTrackedImage(ARTrackedImage trackedImage)
        {
            _subject.OnNext(trackedImage);
        }

        public void TryInitAnchorTransformIfNotExists(ARTrackedImage trackedImage,
            Func<ARTrackedImage, Anchor> initAnchorTransform)
        {
            var imageName = trackedImage.referenceImage.name;
            if (!_detectedAnchorDic.ContainsKey(imageName))
            {
                _detectedAnchorDic[imageName] = initAnchorTransform(trackedImage);
            }
        }

        // フレーム間の移動距離が大きすぎる場合はノイズとして判定する
        private bool IsNoiseData(IList<(Vector3, Quaternion)> positionAndRotations)
        {
            var threshold = MovingNoiseThreshold * MovingNoiseThreshold;
            for (var i = 0; i < positionAndRotations.Count - 1; i++)
            {
                var d = Vector3.SqrMagnitude(positionAndRotations[i].Item1 - positionAndRotations[i + 1].Item1);
                if (d > threshold) return true;
            }

            return false;
        }

        private Anchor GetOrNullAnchorWithClear(string imageName)
        {
            // 初回マーカー認識後にNative側で"UnityARKit: Updating ARSession configuration"があると、keyはあるのにAnchorがnullという状態が発生する
            // その場合は一度クリアして再度Anchorを設定する
            var anchor = _detectedAnchorDic.GetValueOrDefault(imageName);
            if (anchor == null) Clear();
            return anchor;
        }

        public void Clear()
        {
            _detectedAnchorDic.Clear();
            _disposable?.Dispose();
            // パラメータ変更したら反映、ClearしたときにBufferを初期化するためにここでsubscribe
            _disposable = _subject
                .Select(trackedImage => GetOrNullAnchorWithClear(trackedImage.referenceImage.name))
                .Where(anchor => anchor != null)
                .Select(x =>
                {
                    x.transform.GetPositionAndRotation(out var pos, out var rot);
                    return (pos, rot);
                })
                .Buffer(NoiseCheckSampleCount + 1)
                .Where(l => !IsNoiseData(l))
                .Select(l => l.Last()) // 最新のデータを取得
                .Subscribe(_trackedImageSubject);
        }

        public void Dispose()
        {
            _subject.Dispose();
            _trackedImageSubject.Dispose();
            _disposable?.Dispose();
        }
    }
}
