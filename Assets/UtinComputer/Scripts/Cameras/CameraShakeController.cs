using System;
namespace UtinComputer.Cameras
{
    public class CameraShakeController : IDisposable
    {
        private readonly CameraView _camera;

        public CameraShakeController(CameraView camera)
        {
            _camera = camera;
        }

        public void Dispose()
        {
        }

        public void Shake()
        {
        }
    }
}
