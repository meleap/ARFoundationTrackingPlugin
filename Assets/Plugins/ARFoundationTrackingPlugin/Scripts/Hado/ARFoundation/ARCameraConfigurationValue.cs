using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace LapsaAR
{
    /// <summary>
    /// ARCameraの解像度やフレームレート、深度センサーのサポート状況を表す値
    /// <see cref="UnityEngine.XR.ARSubsystems.XRCameraConfiguration"/> を単純な値として扱うためのstruct
    /// </summary>
    /// <param name="Resolution">解像度</param>
    /// <param name="Framerate">Editorならnull</param>
    /// <param name="DepthSensorSupported">LiDAR搭載しているiPhoneであればSupported、LiDAR搭載してないiPhoneはUnsupported、EditorならUnknown</param>
    public record struct ARCameraConfigurationValue(
        Vector2Int Resolution,
        int? Framerate,
        Supported DepthSensorSupported)
    {
        public ARCameraConfigurationValue(int width, int height, int? framerate, Supported depthSensorSupported)
            : this(new Vector2Int(width, height), framerate, depthSensorSupported)
        {
        }

        public static ARCameraConfigurationValue From(in XRCameraConfiguration c) =>
            new(c.resolution, c.framerate, c.depthSensorSupported);

        public static ARCameraConfigurationValue? From(in XRCameraConfiguration? c) => c != null ? From(c.Value) : null;
    }
}
