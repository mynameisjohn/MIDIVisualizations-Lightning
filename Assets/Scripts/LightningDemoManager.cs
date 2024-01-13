using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningDemoManager : NoteReceiver
{
    public ParticleSystem _LightningParticleSystem;
    public int _LightningParticlesToEmit = 10;

    public ParticleSystem _RainParticleSystem;

    public Light _LightningFlashLight;
    public float _LightingFlashAmount;
    public float _LightningFlashDecayRate;
    private Coroutine _lightningFlashCoroutine;
    private float _initialLightningAmount;
    private void Start()
    {
        _initialLightningAmount = _LightningFlashLight.intensity;

        ParticleSystem.EmissionModule em = _LightningParticleSystem.emission;
        em.rate = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FlashLightning();
        }        
    }

    public void FlashLightning()
    {
        _LightningParticleSystem.Emit(_LightningParticlesToEmit);
        _LightningFlashLight.intensity += _LightingFlashAmount;
        if (_lightningFlashCoroutine == null)
        {
            StartCoroutine(lightingFlashCoro());
        }
    }

    IEnumerator lightingFlashCoro ()
    {
        while (_LightningFlashLight.intensity > _initialLightningAmount)
        {
            _LightningFlashLight.intensity *= _LightningFlashDecayRate;
            yield return new WaitForEndOfFrame();
        }

        _LightningFlashLight.intensity = _initialLightningAmount;
        _lightningFlashCoroutine = null;
        yield return null;
    }
    public override void HandleNote(int noteNumber)
    {
        FlashLightning();
    }
}
