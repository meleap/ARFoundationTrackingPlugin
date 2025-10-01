using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;

namespace LapsaAR.Extensions
{
    public static class ARCameraManagerExtensions
    {
        /// <summary>
        /// 次の ARCameraManager.frameReceived イベントが呼ばれるまで待ちます
        /// </summary>
        /// <param name="cameraManager"></param>
        /// <param name="cancellationToken"></param>
        public static async UniTask WaitForFrameReceivedAsync(this ARCameraManager cameraManager,
            CancellationToken cancellationToken = default)
        {
            var tcs = new UniTaskCompletionSource();
            try
            {
                cameraManager.frameReceived += Handler;
                await using (cancellationToken.Register(() => tcs.TrySetCanceled())) await tcs.Task;
            }
            finally
            {
                cameraManager.frameReceived -= Handler;
            }

            return;

            void Handler(ARCameraFrameEventArgs _) => tcs.TrySetResult();
        }

        /// <summary>
        /// ARCameraManager.currentConfiguration を ARCameraConfigurationValue に変換して返します
        /// 初めてframeReceivedイベントが呼ばれるまで、nullを返します
        /// </summary>
        /// <param name="cameraManager"></param>
        /// <returns></returns>
        public static ARCameraConfigurationValue? GetCurrentConfigurationValue(this ARCameraManager cameraManager) =>
            ARCameraConfigurationValue.From(cameraManager.currentConfiguration);

        /// <summary>
        /// ARCameraManager.GetConfigurations()を呼び出して ARCameraConfigurationValue[] に変換して返します
        /// 初めてframeReceivedイベントが呼ばれるまで、空配列を返します
        /// </summary>
        /// <param name="cameraManager"></param>
        /// <returns></returns>
        public static ARCameraConfigurationValue[] GetConfigurationValues(this ARCameraManager cameraManager)
        {
            using var configurations = cameraManager.GetConfigurations(Allocator.Temp);
            var result = new ARCameraConfigurationValue[configurations.Length];
            for (var i = 0; i < configurations.Length; i++)
                result[i] = ARCameraConfigurationValue.From(configurations[i]);
            return result;
        }

        /// <summary>
        /// ARCameraConfigurationValue を介して ARCameraManager.currentConfiguration を設定します
        /// 初めてframeReceivedイベントが呼ばれる前に呼び出した場合、 InvalidOperationException がスローされます
        /// 実際に変更が反映されるまで6 frame程度かかります
        /// </summary>
        /// <param name="cameraManager"></param>
        /// <param name="configurationValue"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ApplyConfigurationValue(this ARCameraManager cameraManager,
            in ARCameraConfigurationValue configurationValue)
        {
            // frameReceivedイベントが呼ばれるまで空配列が返ります
            using var configurations = cameraManager.GetConfigurations(Allocator.Temp);
            foreach (var c in configurations)
            {
                if (ARCameraConfigurationValue.From(c) != configurationValue) continue;
                cameraManager.currentConfiguration = c;
                return;
            }

            throw new InvalidOperationException(
                $"Requested camera configuration is not supported. Use GetConfigurationValues() with the current device as parameter. configuration:  {configurationValue}");
        }
    }
}
