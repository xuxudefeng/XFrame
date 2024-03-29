﻿using UnityEngine;

namespace RLD
{
    public interface IGizmoSlider
    {
        Gizmo Gizmo { get; }
        int HandleId { get; }
        Priority HoverPriority3D { get; }
        Priority HoverPriority2D { get; }
        Priority GenericHoverPriority { get; }

        void SetHoverable(bool isHoverable);
        void SetVisible(bool isVisible);
        void SetSnapEnabled(bool isEnabled);
        void Render(Camera camera);
    }
}
