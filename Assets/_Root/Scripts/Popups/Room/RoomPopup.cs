using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPopup : Popup
{
    [SerializeField] List<Room> _roomList;
    [SerializeField] List<RoomCollectionItem> _roomColectionItemList;
    [SerializeField] ScrollRectScript scrollRectScript;
    [SerializeField] GameObject spanner;
    [SerializeField] ParticleSystem smokeUpgrade;
    
    public GameObject Spanner {
        get {return spanner;}
    }
    public ParticleSystem Smoke{
        get {return smokeUpgrade;}
    }
    private Room _currentRoom;
    // Start is called before the first frame update
    protected override void AfterInstantiate()
    {
        Init();
    }

    private void Init()
    {
        // Chỉ khởi tạo roomCurent
        _roomList.ForEach(item =>
        {
            if (item.RoomType == Data.RoomCurrent)
            {
                _currentRoom = item;
                item.gameObject.SetActive(true);
                item.Init();
            }
            else item.gameObject.SetActive(false);
        });

        // Khởi tạo roomCollection
        for (int idx = 0; idx < _roomColectionItemList.Count; idx++)
        {
            if (idx > 0)
            {
                _roomColectionItemList[idx].Init(ResourcesController.Factory.Rooms[idx], ResourcesController.Factory.Rooms[idx - 1].IsComplete);
            }
            else _roomColectionItemList[idx].Init(ResourcesController.Factory.Rooms[idx], true);
        }
    }

    public void OpenNewRoom()
    {
        for (int idx = 0; idx < _roomColectionItemList.Count; idx++)
        {
            if (!ResourcesController.Factory.Rooms[idx].IsComplete)
            {
                _roomColectionItemList[idx].Reset(true);
                break;
            }
        }
    }

    // Reset RoomPopup sau khi mở khóa được room mới và chuyển sang room đó hoặc chuyển qua lại giữa các room.
    private void ResetRoomCurent()
    {
        _roomList.ForEach(item =>
       {
           if (item.RoomType == Data.RoomCurrent)
           {
               _currentRoom = item;
               item.gameObject.SetActive(true);
               item.Init();

               //scrollRectScript.FocusOnCurrentRoom(item.transform.GetSiblingIndex());
           }
           else item.gameObject.SetActive(false);
       });
    }

    // Hide Huggy Image on all collections and resetRoomCurrent;
    public void ChangeToAnotherRoom()
    {
        _roomColectionItemList.ForEach(item =>
        {
            item.HideHuggyImage();
        });
        ResetRoomCurent();
    }

    public GameObject GetRoomCurrentObject()
    {
        return this._currentRoom.gameObject;
    }

    public void ReturnCurrentRoomToOriginalPosition()
    {
        this._currentRoom.GetComponent<Room>().ReturnOriginalPosition(); // from background to Popup
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();
        ReturnCurrentRoomToOriginalPosition();
        // Reset room vì ở chế độ debug sau khi tăng tiền room phải được cập nhật
        _roomList.ForEach(item =>
        {
            if (item.RoomType == Data.RoomCurrent)
            {
                item.Reset();
            }
        });

    }

    protected override void AfterShown()
    {
        base.AfterShown();
        for (int idx = 0; idx < _roomColectionItemList.Count; idx++)
        {
            if (_roomColectionItemList[idx].GetRoomType() == Data.RoomCurrent) scrollRectScript.FocusOnCurrentRoom(_roomColectionItemList[idx].transform.GetSiblingIndex());
        }
    }
    public override void Close()
    {
        if (this._currentRoom.IsUpgrading() == false)
        {
            base.Close();

            if (GameController.Instance != null)
            {
                SoundController.Instance.PlayBackground(SoundType.BackgroundInGame);
            }
            else
            {
                SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
            }
            //HomeController.Instance.ResetBackground();
        }
    }
}
