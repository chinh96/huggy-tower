using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
public class Furniture : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _priceTextCanFix;
    [SerializeField] private TextMeshProUGUI _priceTextCantFix;
    // [SerializeField] private GameObject _spanner; // chưa tối ưu(mỗi furniture có 1 spanner riêng => có thể dùng chung)
    [SerializeField] private GameObject _canFixButton;
    [SerializeField] private GameObject _cantFixButton;
    [SerializeField] private Image _furnitureImage;

    [SerializeField] private int _furnitureIndex;

    private ParticleSystem _smoke;
    private GameObject _spanner;
    private FurnitureData _nextFurnitureData;
    private FurnitureData _currentFurnitureData;
    private FurnitureResources _furnitureResources;

    private Room _room;
    private RoomPopup _roomPopup;
    public bool IsUpgrading { get; set; }

    public void Init(RoomResources roomCurrent, Room room, RoomPopup roomPopup)
    {
        IsUpgrading = false;
        _furnitureResources = roomCurrent.Funitures[_furnitureIndex - 1];

        this._room = room;
        this._roomPopup = roomPopup;
        _smoke = Instantiate(roomPopup.Smoke, transform);
        _spanner = Instantiate(roomPopup.Spanner, transform);
        _spanner.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        HideAll();
        /*
        for(int idx = 1; idx < _furnitureResources.FurnitureLevels.Count; idx++){
             FurnitureData item = _furnitureResources.FurnitureLevels[idx];
             item.IsUnlocked = false;
        }*/

        _furnitureResources.FurnitureLevels[0].IsUnlocked = true;
        Reset();
    }
    public void Reset()
    { // Trong trường hợp có thể upgrade nhiều lần
        if(IsUpgrading) return;
        bool done = true;
        for (int idx = 1; idx < _furnitureResources.FurnitureLevels.Count; idx++)
        { // Vì idx = 0 luôn luôn unlocked
            FurnitureData item = _furnitureResources.FurnitureLevels[idx];
            if (!item.IsUnlocked)
            {
                done = false;

                _currentFurnitureData = _furnitureResources.FurnitureLevels[idx - 1];
                _nextFurnitureData = item;

                if (Data.CoinTotal >= _nextFurnitureData.Cost) SwitchFixButtonActive(true);
                else SwitchFixButtonActive(false);

                CorrectPriceText();
                break;
            }
        }
        if (done)
        {
            HideAll();
            _currentFurnitureData = _furnitureResources.FurnitureLevels[_furnitureResources.FurnitureLevels.Count - 1];
        }
        UpdateFurnitureImage();
    }

    private void UpdateFurnitureImage()
    {
        _furnitureImage.sprite = _currentFurnitureData.Sprite;
        if (transform.GetSiblingIndex() != 0) _furnitureImage.SetNativeSize();
    }

    private void HideAll()
    {
        _spanner.SetActive(IsUpgrading);
        _canFixButton.SetActive(false);
        _cantFixButton.SetActive(false);
    }
    private void SwitchFixButtonActive(bool isCanFix)
    {
        _canFixButton.SetActive(isCanFix);
        _cantFixButton.SetActive(!isCanFix);
    }
    private void CorrectPriceText()
    {
        _priceTextCanFix.SetText(_nextFurnitureData.Cost.ToString());
        _priceTextCantFix.SetText(_nextFurnitureData.Cost.ToString());
    }

    /*
        Note: Khi đang upgrade mà chuyển sang room mới, trước khi code kịp chạy dòng lệnh _furnitureResources.Upgrade();
              thì furniture chưa kịp upgrade
    */
    public void OnClickFixButton()
    {
        if(Data.CoinTotal >= _nextFurnitureData.Cost){
            IsUpgrading = true;
            _furnitureResources.Upgrade();
            Data.CoinTotal -= _nextFurnitureData.Cost; // call noti

            _room.Reset(); // Reset all furniture except funiture is upgrading

            HideAll();
            _spanner.transform.position = _canFixButton.transform.position;

            _spanner.SetActive(true);

            DOTween.Sequence().AppendInterval(2.2f).AppendCallback(() =>
            {
                _spanner.SetActive(false);
                // _roomPopup.
                // _roomPopup. = castles[castleIndex].transform.position;
                // smoke.transform.localPosition += new Vector3(0, 100, 0);
                _smoke.transform.position = _canFixButton.transform.position;
                _smoke.Play();
                IsUpgrading = false;
                Reset();
                _room.Reset();
                SoundController.Instance.PlayOnce(SoundType.FixingRoom);
            });
        }
    }

    public void SetBackground()
    {
        HideAll();
    }
}
