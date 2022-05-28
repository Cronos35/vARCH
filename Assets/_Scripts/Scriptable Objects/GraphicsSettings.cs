using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Graphics Settings", menuName ="Graphics Settings")]
public class GraphicsSettings : ScriptableObject
{
    private Vector2Int _resolution;
    private int _reflectionResolution;
    private bool _enableRealtimeReflections;
    private int _antiAliasingQuality;

    public Vector2Int Resolution { get { return _resolution;  } set { _resolution = value;  } }
    public int ReflectionResolution { get { return _reflectionResolution; } set { _reflectionResolution = value; } }
    public bool EnableRealtimeResolution { get { return _enableRealtimeReflections; } set { _enableRealtimeReflections = value; } }
    public int AntiAliasingQuality { get { return _antiAliasingQuality;  } set { _antiAliasingQuality = value; } }
}
