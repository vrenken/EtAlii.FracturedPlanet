namespace EtAlii.FracturedPlanet.World
{
    using Unity.Entities;

    public struct WorldComponent : IComponentData
    {
        /// <summary>
        /// The radius of the torus itself, i.e. the donut radius. 
        /// </summary>
        public float MajorRadius;

        /// <summary>
        /// The radius of the torus tube, i.e. the thickness of the donut.
        /// </summary>
        public float MinorRadius;

        public int MajorSegmentCount;
        public int MinorSegmentCount;

        //private float _verticalRotation;
        //private float _horizontalRotation;

        public float RotationSpeed;// = 40f;

        public bool AutoRotate;
    }
}