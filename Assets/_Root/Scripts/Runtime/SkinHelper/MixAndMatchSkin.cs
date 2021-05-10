using Lance.TowerWar.Data;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;

namespace Lance.TowerWar.Helper
{
    using UnityEngine;

    public class MixAndMatchSkin : MonoBehaviour
    {
        // here we use arrays of strings to be able to cycle between them easily.
        [SpineSkin] public string[] swordSkins = { };
        private SkeletonGraphic _skeletonGraphic;

        // This "naked body" skin will likely change only once upon character creation,
        // so we store this combined set of non-equipment Skins for later re-use.
        private Skin _characterSkin;

        // for repacking the skin to a new atlas texture
        public Material runtimeMaterial;
        public Texture2D runtimeAtlas;

        private void Awake() { _skeletonGraphic = this.GetComponent<SkeletonGraphic>(); }

        private void Start() { Refresh(); }

        public void Refresh(int index = 0)
        {
            UpdateCharacterSkin(index);
            UpdateCombinedSkin();
        }

        public void OptimizeSkin()
        {
            // Create a repacked skin.
            var previousSkin = _skeletonGraphic.Skeleton.Skin;
            // Note: materials and textures returned by GetRepackedSkin() behave like 'new Texture2D()' and need to be destroyed
            if (runtimeMaterial)
                Destroy(runtimeMaterial);
            if (runtimeAtlas)
                Destroy(runtimeAtlas);
            Skin repackedSkin = previousSkin.GetRepackedSkin("Repacked skin",
                _skeletonGraphic.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial,
                out runtimeMaterial,
                out runtimeAtlas);
            previousSkin.Clear();

            // Use the repacked skin.
            _skeletonGraphic.Skeleton.Skin = repackedSkin;
            _skeletonGraphic.Skeleton.SetSlotsToSetupPose();
            _skeletonGraphic.AnimationState.Apply(_skeletonGraphic.Skeleton);

            // You can optionally clear the cache after multiple repack operations.
            AtlasUtilities.ClearCache();
        }

        private void UpdateCharacterSkin(int index)
        {
            var skeleton = _skeletonGraphic.Skeleton;
            var skeletonData = skeleton.Data;
            _characterSkin = new Skin("character-base");
            // Note that the result Skin returned by calls to skeletonData.FindSkin()
            // could be cached once in Start() instead of searching for the same skin
            // every time. For demonstration purposes we keep it simple here.
            _characterSkin.AddSkin(skeletonData.FindSkin(swordSkins[index]));
            _characterSkin.AddSkin(skeletonData.FindSkin(HeroSkinData.SkinHeroByIndex(Data.Data.CurrentSkinHero)));
        }

        private void UpdateCombinedSkin()
        {
            var skeleton = _skeletonGraphic.Skeleton;
            var resultCombinedSkin = new Skin("character-combined");

            resultCombinedSkin.AddSkin(_characterSkin);

            skeleton.SetSkin(resultCombinedSkin);
            skeleton.SetSlotsToSetupPose();
        }
    }
}