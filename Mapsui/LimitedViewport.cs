﻿using System.Collections.Generic;
using System.ComponentModel;
using Mapsui.UI;
using Mapsui.Utilities;

namespace Mapsui;

public class LimitedViewport : IViewport
{
    private readonly Viewport _viewport = new Viewport();

    public LimitedViewport()
    {
        _viewport.ViewportChanged += (sender, args) => ViewportChanged?.Invoke(sender, args);
    }

    public IViewportLimiter Limiter
    {
        get => _viewport.Limiter;
        set => _viewport.Limiter = value;
    }

    public ViewportState State => _viewport.State;

    public event PropertyChangedEventHandler? ViewportChanged;
 
    public void Transform(MPoint position, MPoint previousPosition, double deltaResolution = 1, double deltaRotation = 0)
        => _viewport.Transform(position, previousPosition, deltaResolution, deltaRotation);

    public void SetSize(double width, double height)
        => _viewport.SetSize(width, height);
    
    public virtual void SetCenter(double x, double y, long duration = 0, Easing? easing = default)
        => _viewport.SetCenter(x, y, duration, easing);

    public virtual void SetCenterAndResolution(double x, double y, double resolution, long duration = 0, Easing? easing = default)
        => _viewport.SetCenterAndResolution(x, y, resolution, duration, easing);

    public void SetCenter(MPoint center, long duration = 0, Easing? easing = default)
        => _viewport.SetCenter(center, duration, easing);

    public void SetResolution(double resolution, long duration = 0, Easing? easing = default)
        => _viewport.SetResolution(resolution, duration, easing);

    public void SetRotation(double rotation, long duration = 0, Easing? easing = default)
        => _viewport.SetRotation(rotation, duration, easing);

    public bool UpdateAnimations()
    {
        return _viewport.UpdateAnimations();
    }

    public void SetAnimations(List<AnimationEntry<Viewport>> animations)
    {
        _viewport.SetAnimations(animations);
    }
}
