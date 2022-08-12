using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MainSystem.Common
{
    public class MainSystemCommon
    {
        Vector2 scroll = Vector2.zero;

        public bool onEnter()
        {
            var mouse = Mouse.current.leftButton;
            if (mouse.wasPressedThisFrame)
            {
                Debug.Log("Press");
                return true;
            }
            return false;
        }

        public float wheelEvent()
        {
            var mouse = Mouse.current;

            scroll = mouse.scroll.ReadValue();

            float wheel = scroll.y;
            return wheel;
        }

    }
}
