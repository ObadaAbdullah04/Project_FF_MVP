namespace Project.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public static class UIHelper
    {
        public static bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;

            if (Input.touchCount > 0)
            {
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
