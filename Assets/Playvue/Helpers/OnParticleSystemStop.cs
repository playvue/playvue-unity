using UnityEngine;
using UnityEngine.Events;

public class OnParticleSystemStop : MonoBehaviour {

    public GameObjectEvent OnStopped;

    void OnParticleSystemStopped() {
        OnStopped?.Invoke(gameObject);
    }
}