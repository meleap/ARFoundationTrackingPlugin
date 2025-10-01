using UnityEngine.XR.ARSubsystems;

namespace LapsaAR.Helpers
{
    /// <summary>
    /// 既知のARカメラの解像度とフレームレートの組み合わせ
    /// </summary>
    public static class KnownARCameraConfigurations
    {
        // TODO check
        // ReSharper disable once InconsistentNaming
        public static readonly ARCameraConfigurationValue[] iPhone8 =
        {
            new(1280, 720, 30, Supported.Unsupported),
            new(1280, 720, 60, Supported.Unsupported),
            new(1920, 1080, 30, Supported.Unsupported),
            new(1920, 1080, 60, Supported.Unsupported),
            new(1920, 1440, 30, Supported.Unsupported),
            new(1920, 1440, 60, Supported.Unsupported)
        };

        // ReSharper disable once InconsistentNaming
        public static readonly ARCameraConfigurationValue[] iPhoneSe2 =
        {
            new(1920, 1440, 60, Supported.Unsupported), // default
            new(1920, 1440, 30, Supported.Unsupported),
            new(1920, 1080, 60, Supported.Unsupported),
            new(1920, 1080, 30, Supported.Unsupported),
            new(1280, 720, 60, Supported.Unsupported),
            new(1280, 720, 30, Supported.Unsupported),
        };

        // TODO check
        // ReSharper disable once InconsistentNaming
        public static readonly ARCameraConfigurationValue[] iPhoneSe3 =
        {
            new(1280, 720, 30, Supported.Unsupported),
            new(1280, 720, 60, Supported.Unsupported),
            new(1920, 1080, 30, Supported.Unsupported),
            new(1920, 1080, 60, Supported.Unsupported),
            new(1920, 1440, 30, Supported.Unsupported),
            new(1920, 1440, 60, Supported.Unsupported)
        };

        // TODO check
        // ReSharper disable once InconsistentNaming
        public static readonly ARCameraConfigurationValue[] iPhone12Pro =
        {
            new(1280, 720, 30, Supported.Supported),
            new(1280, 720, 60, Supported.Supported),
            new(1920, 1080, 30, Supported.Supported),
            new(1920, 1080, 60, Supported.Supported),
            new(1920, 1440, 30, Supported.Supported),
            new(1920, 1440, 60, Supported.Supported),
            new(3840, 2160, 30, Supported.Supported)
        };

        // ReSharper disable once InconsistentNaming
        public static readonly ARCameraConfigurationValue[] iPhone13Pro =
        {
            // ARカメラをディスプレイ全体で描画する通常のレンダリングの場合、1920x1440と1920*1080で大きな見た目の変化はありませんが、若干1920*1080の方がFOVが小さく描画される領域が狭くなります
            // 1080だとマーカーがちょうど画面の範囲内に入っていると認識します
            // 1440だとマーカーが画面の上下に少しはみ出していても認識できるため、マーカー認識には1440の方が有利です
            new(1920, 1440, 60, Supported.Supported), // default
            new(1920, 1440, 30, Supported.Supported),
            new(1920, 1080, 60, Supported.Supported),
            new(1920, 1080, 30, Supported.Supported),
            new(1280, 720, 60, Supported.Supported),
            new(1280, 720, 30, Supported.Supported),
            new(3840, 2160, 30, Supported.Supported),
        };

        // ReSharper disable once InconsistentNaming
        public static readonly ARCameraConfigurationValue[] iPhone16e =
        {
            new(1920, 1440, 60, Supported.Unsupported), // default
            new(1920, 1440, 30, Supported.Unsupported),
            new(1920, 1080, 60, Supported.Unsupported),
            new(1920, 1080, 30, Supported.Unsupported),
            new(1280, 720, 60, Supported.Unsupported),
            new(1280, 720, 30, Supported.Unsupported),
        };

        // ReSharper disable once InconsistentNaming
        public static readonly ARCameraConfigurationValue[] iPhone16ProMax =
        {
            new(1920, 1440, 60, Supported.Supported), // default
            new(1920, 1440, 30, Supported.Supported),
            new(1920, 1080, 60, Supported.Supported),
            new(1920, 1080, 30, Supported.Supported),
            new(1280, 720, 60, Supported.Supported),
            new(1280, 720, 30, Supported.Supported),
            new(3840, 2160, 30, Supported.Supported),
        };
    }
}
