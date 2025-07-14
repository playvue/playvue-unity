using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject> {}

[System.Serializable]
public class Vector3Event : UnityEvent<Vector3> {}

[System.Serializable]
public class Vector2Event : UnityEvent<Vector2> {}

[System.Serializable]
public class TransformEvent : UnityEvent<Transform> {}

[System.Serializable]
public class FloatEvent : UnityEvent<float> {}

[System.Serializable]
public class IntEvent : UnityEvent<int> {}
