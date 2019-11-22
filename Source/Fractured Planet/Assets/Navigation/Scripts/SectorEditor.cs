namespace EtAlii.FracturedPlanet.Navigation
{
    using UnityEngine;

    public class SectorEditor : MonoBehaviour
    {
        public float force = 2f;
        public float range = 2f;
        public float maxReachDistance = 100f;
        public AnimationCurve forceOverDistance = AnimationCurve.Constant(0, 1, 1);
        public Sector sector;
        public Transform playerCamera;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            TryEditSector();
        }

        private void TryEditSector()
        {
            if (force <= 0 || range <= 0)
            {
                return;
            }

            if (Input.GetButton("Fire1"))
            {
                RaycastToSector(true);
            }
            else if (Input.GetButton("Fire2"))
            {
                RaycastToSector(false);
            }
        }

        private void RaycastToSector(bool addMaterial)
        {
            var startP = playerCamera.position;
            var destP = startP + playerCamera.forward;
            var direction = destP - startP;

            var ray = new Ray(startP, direction);

            if (!Physics.Raycast(ray, out var hitInfo, maxReachDistance)) return;

            var hitPoint = hitInfo.point;

            if (addMaterial)
            {
                var hits = Physics.OverlapSphere(hitPoint, range / 2f * 0.8f);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Player"))
                    {
                        return;
                    }
                }
            }

            EditSector(hitPoint, addMaterial);
        }

        private void EditSector(Vector3 point, bool addMaterial)
        {
            var buildModifier = addMaterial ? 1 : -1;

            var hitX = Math3d.Round(point.x);
            var hitY = Math3d.Round(point.y);
            var hitZ = Math3d.Round(point.z);

            var intRange = Math3d.Ceil(range);

            for (var x = -intRange; x <= intRange; x++)
            {
                for (var y = -intRange; y <= intRange; y++)
                {
                    for (var z = -intRange; z <= intRange; z++)
                    {
                        var offsetX = hitX - x;
                        var offsetY = hitY - y;
                        var offsetZ = hitZ - z;

                        if (!sector.IsPointInsideSector(offsetX, offsetY, offsetZ))
                            continue;

                        var distance = Math3d.Distance(offsetX, offsetY, offsetZ, point);
                        if (!(distance <= range)) continue;

                        var modificationAmount = force / distance *
                                                 forceOverDistance.Evaluate(1 - Math3d.Map(distance, 0, force, 0, 1)) *
                                                 buildModifier;

                        var oldDensity = sector.GetDensity(offsetX, offsetY, offsetZ);
                        var newDensity = oldDensity - modificationAmount;

                        newDensity = Math3d.Clamp01(newDensity);

                        sector.SetDensity(newDensity, offsetX, offsetY, offsetZ, true);
                    }
                }
            }
        }
    }
}