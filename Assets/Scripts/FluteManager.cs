using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class FluteManager : NoteReceiver
{
    public Light _LightToModify;
    public ParticleSystem _ParticleSystemToModify;
    public Color[] _AlternateColors;
    public float _IncrementDelta = 0.1f;

    Color _defaultLightColor;
    Color _defaultParticleSystemColor;

    Color _initialLightColor;
    Color _initialParticleSystemColor;

    int _targetLightColorIndex = -1;
    int _targetParticleSystemColorIndex = -1;

    float _interpPosition;
    Coroutine _transitionColorCoro;

    int _noteCount;

    private void Start()
    {
        _defaultLightColor = _LightToModify.color;
        _defaultParticleSystemColor = _ParticleSystemToModify.main.startColor.color;
    }

    public override void HandleNote(int noteNumber)
    {
        _noteCount++;
        updateNoteCount();
    }

    public override void HandleNoteOff(int noteNumber)
    {
        _noteCount--;
        if (_noteCount < 0)
        {
            _noteCount = 0;
        }
        updateNoteCount();
    }

    void updateNoteCount()
    {
        if (_transitionColorCoro != null)
        {
            StopCoroutine(_transitionColorCoro);
        }

        if (_noteCount == 0)
        {
            _transitionColorCoro = StartCoroutine(transitionColorCoro(_defaultLightColor, _defaultParticleSystemColor));
            return;
        }

        int targetColorLightIndex = Random.Range(0, _AlternateColors.Length);
        int targetColorPSIndex = Random.Range(0, _AlternateColors.Length);
        while (targetColorLightIndex == _targetLightColorIndex || targetColorPSIndex == _targetParticleSystemColorIndex || targetColorLightIndex == targetColorPSIndex)
        {
            targetColorLightIndex = Random.Range(0, _AlternateColors.Length);
            targetColorPSIndex = Random.Range(0, _AlternateColors.Length);

        }
        _targetLightColorIndex = targetColorLightIndex;
        _targetParticleSystemColorIndex = targetColorPSIndex;

        Color targetLightColor = _AlternateColors[_targetLightColorIndex];
        Color targetPSColor = _AlternateColors[_targetParticleSystemColorIndex];

        _transitionColorCoro = StartCoroutine(transitionColorCoro(targetLightColor, targetPSColor));
    }

    IEnumerator transitionColorCoro(Color targetLightColor, Color targetParticleSystemColor)
    {
        _interpPosition = 0;
        Color initialLightColor = _LightToModify.color;

        ParticleSystem.MainModule mm = _ParticleSystemToModify.main;
        ParticleSystem.MinMaxGradient mmC = mm.startColor;
        Color initialParticleSystemColor = mmC.color;

        while (_interpPosition < 1)
        {
            _LightToModify.color = Color.Lerp(initialLightColor, targetLightColor, _interpPosition);

            mmC.color = Color.Lerp(initialParticleSystemColor, targetParticleSystemColor, _interpPosition);
            mm.startColor = mmC;

            _interpPosition += _IncrementDelta;
            yield return new WaitForEndOfFrame();
        }


        _transitionColorCoro = null;
    }
}
