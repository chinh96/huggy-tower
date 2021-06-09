using Spine.Unity;

public interface IAnim
{
    public SkeletonGraphic Skeleton { get; }

    #region animation

    void PlayIdle(bool isLoop);

    void PlayAttack();

    void PLayMove(bool isLoop);

    void PlayDead();

    void PlayWin(bool isLoop);

    void PlayLose(bool isLoop);

    #endregion
}