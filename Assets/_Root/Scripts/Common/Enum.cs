public enum DirectionMoveOut
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
}

public enum NotiType
{
    Universe,
    World,
    Build,
    Skin,
    Daily,
    Achievement
}

public enum EGameState
{
    None,
    Playing,
    Win,
    Lose,
}

public enum ShowAction
{
    DoNothing,
    DismissCurrent,
    PauseCurrent
}

public enum EUnitState
{
    Normal,
    Invalid,
}

public enum EUnitType
{
    Hero,
    Enemy,
    Item,
    Gem,
    Princess,
}

public enum ItemType
{
    BrokenBrick,
    Chest,
    Sword,
    None,
    Gem,
    Gloves,
    Food,
    Shield,
    Key,
    Knife
}

public enum ETurn
{
    None = 0,
    Drag = 1,
    Searching = 2,
    Attacking = 3,
    UsingItem = 4,
    MoveToItem = 5,
    SavingPrincess = 6,
}

public enum ELevelCondition
{
    KillAll,
    CollectChest,
    SavePrincess,
    CollectGold,
    KillDemon,
    KillDragon,
    KillGhost,
    KillWolf,
    KillBear
}

public enum SkinType
{
    Coin,
    Ads,
    Daily,
    Facebook
}

public enum SoundType
{
    BackgroundInGame,
    BackgroundHome,
    BackgroundCastle,
    ButtonClick,
    Win,
    Lose,
    OpenPopup,
    HeroDrag,
    HeroDrop,
    HeroHit,
    HeroCut,
    HeroUpLevel,
    HeroDie,
    HeroYeah,
    EnemyHit,
    EnemyCut,
    EnemyShoot,
    EnemyBite,
    OpenChest,
    RescuePrincess,
    BlockWallBreak,
    CoinMove,
    BuildItem,
    HeroDownLevel,
    EnemyDie,
    HeroPickSword,
    EnemyDogDie,
    EnemyDie2,
    EnemyDie3,
    DemonAttack,
    EnemyStart,
    HeroHit2,
    HeroHit3,
    HeroCut2,
    HeroCut3,
    HeroPushWall,
    BuildItemDone,
    TowerLevelUp,
    BearDie,
    BearStart,
    DragonDie,
    DragonStart,
    PickKey
}

public enum WorldType
{
    Desert,
    Earth,
    Iceland,
    Inferno,
    Jade,
    Olympus
}

public enum AchievementType
{
    NormalEnemy,
    WolfEnemy,
    DemonEnemy,
    GhostEnemy,
    DragonEnemy,
    BearEnemy,
    Princess,
    Chest
}