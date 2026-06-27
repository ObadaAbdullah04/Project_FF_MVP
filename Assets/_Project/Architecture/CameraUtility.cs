namespace Project.Architecture
{
    using UnityEngine;

    public static class CameraUtility
    {
        public static Camera ResolveCamera(Camera current, Component self)
        {
            if (current != null) return current;
            current = self.GetComponent<Camera>();
            if (current == null) current = Camera.main;
            return current;
        }
    }
}
