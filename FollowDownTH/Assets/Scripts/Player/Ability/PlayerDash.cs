using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDash : AbilityBase
{
    [Header("Connections")]
    [SerializeField] private CinemachineVirtualCamera originalCam = default;
    [Header("Visuals")]
    [SerializeField] private ParticleSystem dashParticle = default;
    [SerializeField] private Volume dashVolume = default;
    public override void Ability()
    {
        dashParticle.Play();
        Sequence dash = DOTween.Sequence()
        .Insert(0, transform.DOMove(transform.position + (transform.forward * 5), .2f))
        .AppendCallback(() => dashParticle.Stop());

        DOVirtual.Float(0, 1, .1f, SetDashVolumeWeight)
            .OnComplete(() => DOVirtual.Float(1, 0, .5f, SetDashVolumeWeight));

        DOVirtual.Float(40, 50, .1f, SetCameraFOV)
            .OnComplete(() => DOVirtual.Float(50, 40, .5f, SetCameraFOV));
    }

    private void Start()
    {
        dashParticle.Stop();
    }
    void SetDashVolumeWeight(float weight)
    {
        dashVolume.weight = weight;
    }

    void SetCameraFOV(float fov)
    {
        originalCam.m_Lens.FieldOfView = fov;
    }
}
