using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Manipulating the camera with standard inputs
    /// </summary>
    public class FreeCam : MonoBehaviour
    {
        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 15F;
        public float sensitivityY = 15F;

        public float minimumX = -360F;
        public float maximumX = 360F;

        public float minimumY = -60F;
        public float maximumY = 60F;

        public float moveSpeed = 1.0f;

        public bool lockHeight = false;

        private float rotationY = 0F;

        private void Update()
        {
            var t = transform;
            if (axes == RotationAxes.MouseXAndY)
            {
                var rotationX = t.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                t.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                t.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                t.localEulerAngles = new Vector3(-rotationY, t.localEulerAngles.y, 0);
            }

            var xAxisValue = Input.GetAxis("Horizontal");
            var zAxisValue = Input.GetAxis("Vertical");
            if (lockHeight)
            {
                var dir = t.TransformDirection(new Vector3(xAxisValue, 0.0f, zAxisValue) * moveSpeed);
                dir.y = 0.0f;
                t.position += dir;
            }
            else
            {
                t.Translate(new Vector3(xAxisValue, 0.0f, zAxisValue) * moveSpeed);
            }
        }
    }
}
