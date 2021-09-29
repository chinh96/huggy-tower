using UnityEngine;
using DG.Tweening;
using Cinemachine;
using System;
using UnityEngine.SceneManagement;

public class IntroController : Singleton<IntroController>
{
    public HeroIntro HeroIntro;
    public GoblinIntro GoblinIntro;
    public GoblinIntro WolfIntro;
    public PrincessIntro PrincessIntro;
    public DragonIntro DragonIntro;
    public GameObject HeroTower;
    public GameObject EnemyTower;
    public CinemachineVirtualCamera VirtualCamera;
    public DragonIntro DragonIntro2;
    public PrincessIntro PrincessIntro2;
    [NonSerialized] public CinemachineTransposer Transposer;
    public ParticleSystem HitHeroFx;
    public ParticleSystem BloodWolfFx;
    public ParticleSystem BloodGoblinFx;
    public ParticleSystem Exploison;
    public ParticleSystem FireFx;
    public DragonIntro DragonIntro0;
    public FireFull FireFull;
    public SpriteRenderer Overlay;

    private void Start()
    {
        Transposer = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        DragonIntro.gameObject.SetActive(false);
        HeroIntro.transform.DOMoveX(PrincessIntro.transform.position.x - 4f, 1).SetEase(Ease.Linear).OnComplete(() =>
        {
            HeroVsGoblin();
        });
        DragonIntro0.transform.DOMoveX(-6, 4);
    }

    public void HeroVsGoblin()
    {
        HeroIntro.PlayIdle();
        GoblinIntro.PlayAttack();
        WolfIntro.PlayAttack();
        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        {
            HitHeroFx.Play();
            HeroIntro.PlayHurt();
        });

        DOTween.Sequence().AppendInterval(1.1f).AppendCallback(() =>
        {
            GoblinIntro.PlayIdle();
            GoblinIntro.GetComponent<MeshRenderer>().sortingOrder = 1;
            WolfIntro.PlayIdle();
            WolfIntro.GetComponent<MeshRenderer>().sortingOrder = -1;
            HeroIntro.PlayAttack();
            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
            {
                BloodWolfFx.Play();
                BloodGoblinFx.Play();
                GoblinIntro.PlayDie();
                WolfIntro.PlayDie();
            });
            DOTween.Sequence().AppendInterval(1.2f).AppendCallback(() =>
            {
                HeroVsDragon();
            });
        });
    }

    public void HeroVsDragon()
    {
        HeroIntro.PlayRun();
        HeroIntro.transform.DOMoveX(PrincessIntro.transform.position.x, 1).SetEase(Ease.Linear).OnComplete(() =>
        {
            HeroIntro.PlayJump();
            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
            {
                HeroIntro.PlayWait();
            });
            DOTween.Sequence().AppendInterval(.15f).AppendCallback(() =>
            {
                HeroIntro.transform.SetParent(DragonIntro.transform);
            });
            PrincessIntro.PlayTalk();
        });

        DragonIntro.gameObject.SetActive(true);
        DragonIntro.transform.DOMove(PrincessIntro.transform.position + Vector3.up * 1.5f + Vector3.right * 1.5f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            PrincessIntro.transform.SetParent(DragonIntro.transform);
            DragonIntro.transform.DOMove(EnemyTower.transform.position + Vector3.up * 12, 3);
            DOTween.Sequence().AppendInterval(1.3f).AppendCallback(() =>
            {
                HeroIntro.transform.SetParent(transform.parent);
                HeroIntro.gameObject.AddComponent<Rigidbody2D>().gravityScale = 2;
                HeroIntro.PlayFall();
            });
        });
    }

    public void PlayExploison()
    {
        Exploison.Play();
    }

    public void LookBackToDragon()
    {
        DragonIntro.gameObject.SetActive(false);
        DragonIntro2.gameObject.SetActive(true);
        PrincessIntro.gameObject.SetActive(false);
        PrincessIntro2.gameObject.SetActive(true);
        VirtualCamera.Follow = DragonIntro2.transform;
        Transposer.m_FollowOffset = new Vector3(-2, 0, -10);
        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        {
            FireFx.Play();
            FireFull.Show();
            DragonIntro2.PlayAttack();
            DOTween.To(() => Transposer.m_FollowOffset, (x) => Transposer.m_FollowOffset = x, new Vector3(-3, -12, -10), 1);
            DOTween.To(() => VirtualCamera.m_Lens.OrthographicSize, (x) => VirtualCamera.m_Lens.OrthographicSize = x, 18, 1).OnComplete(() =>
            {
                DOTween.Sequence().AppendInterval(3).AppendCallback(() =>
                {
                    float alpha = 0;
                    DOTween.To(() => alpha, (x) =>
                    {
                        alpha = x;
                        Overlay.color = new Color(0, 0, 0, x);
                    }, 1, 1).OnComplete(() =>
                    {
                        Data.IsIntro = false;
                        SceneManager.LoadScene(Constants.HOME_SCENE);
                    });
                });
            });
        });
    }
}
