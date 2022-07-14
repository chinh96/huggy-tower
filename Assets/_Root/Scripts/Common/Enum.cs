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
    Achievement,
    DailyQuest,
    Facebook,
    Leaderboard,
    AchievementDailyQuest,
    RescueParty,
    LuckySpin,
    ThanksGiving,
    ThanksGiving2,
    Factory
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
    Boss
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
    Knife,
    Trap,
    HolyWater,
    Bomb,
    Bow,
    Axe,
    Lock,
    SwordJapan,
    Shuriken,
    SwordBlood,

    // Huggy
    Claws,
    Saw,
    Hammer,
    Key,
    Baseball,

    Ice,
    Fire,
    Electric,
    Poison,
    Mace,
    Bow2,
    Polllaxe
}

public enum ItemTypeHuggy{
    Claws,
    Hammer,
    Saw,
    Key
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

    Lost = 7,
    FightingBoss = 8,
    MoveToEnemy = 9
}

public enum ELevelCondition
{
    KillAll,
    CollectChest,
    SaveKissy,
    CollectGold,
    KillDemon,
    KillDragon,
    KillGhost,
    KillWolf,
    KillBear,
    KillBone,
    KillYeti,
    KillPoliceStick,
    KillSpider,
    KillDragonGold,
    KillFire
}

public enum SkinType
{
    Coin,
    Ads,
    Daily,
    Facebook,
    Achievement,
    Giftcode,
    RescueParty,
    TGLuckySpin,
    ThanksGiving,
    Noel,
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
    PickKey,
    PickFood,
    PickGloves,
    PickShield,
    DragonGoldDie,
    DragonGoldAttack,
    FireDie,
    FireAttack,
    PoliceStickDie,
    PoliceStickAttack,
    SpiderDie,
    SpiderAttack,
    YetiDie,
    YetiAttack,
    Knife,
    Bomb,
    Trap,
    Bow,
    ButtonStart,
    Gloves,
    Axe1,
    Axe2,
    EnemyGoblinDie,
    PrincessStart,
    BombGoblin,
    KappaDie,
    GoblinKappaAttack,
    Elemental,
    Mace,
    Pollaxe,
    Bow2,
    IntroRunStart,
    IntroCutEnemy,
    IntroEnemyStart,
    IntroEnemyDie,
    IntroPrincessStart,
    IntroPrincessScare,
    IntroDragonStart,
    IntroHeroJump,
    IntroHeroFall,
    IntroHeroFallInLove,
    IntroDragonAttack,
    IntroPrincessEnd,
    IntroBackground,
    IntroWolfDie,
    IntroWolfStart,
    IntroEnemySmile,
    LuckySpinRotate,
    LuckySpinHit,
    TurkeyJump,
    // Huggy Kissy Tower
    ElectricTrap,
    ChainUnlocking,
    KissyHelpMe,
    KissyWin,
    KissyWin2,
    HuggyDie,
    HuggyWin,
    HuggyWin2,
    PigAttack,
    PigDie,
    SecretaryAttack,
    SecretaryDie,
    HuggyAttackBaseball, // Pipe
    HuggyAttackClaws,
    HuggyAttackNormal,
    HuggyAttackNormal2,
    HuggyAttackSaw,
    EnemyHandAttack,
    EnemyGunAttack,
    FixingRoom,
    PickWeapon,
    EnemyVenomAttack,
    EnemyVenomDie,
    HuggyAttackHammer,
    HuggyAttackNormal3,
    HuggyAttackNormal4
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

public enum RoomType{
    Receptionist,
    Storage,
    Lounge,
    Showroom,
    ProductionLine,
    Bedroom
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
    Chest,
    BuySkin,
    PlayToLevel,
    CompleteEarth,
    CompleteDesert,
    CompleteIceland,
    CompleteInferno,
    CompleteJade,
    CompleteOlympus,
    JoinGroupFacebookSuccessfully,
    ClaimDailyReward
}

public enum DailyQuestType
{
    NormalEnemy,
    WolfEnemy,
    DemonEnemy,
    GhostEnemy,
    DragonEnemy,
    BearEnemy,
    Princess,
    Chest,
    LevelPassed,
    LoginFacebook,
    CompleteEarth,
    CompleteDesert,
    CompleteIceland,
    CompleteInferno,
    CompleteJade,
    CompleteOlympus,
    BuySkin,
    GetShield,
    GetFood,
    GetSword,
    GetGloves,
    PushWall,
    GetHolyWater,
    DragonGold,
    Fire,
    PoliceStick,
    Spider,
    Yeti,
    LogIntoTheGame,
    WatchVideoReward
}

public enum RescuePartyType
{
    None,
    Hero,
    Princess,
    Top100,
    Hero2,
    Gold
}