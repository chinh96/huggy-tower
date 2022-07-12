using UnityEngine;
using DG.Tweening;

public class ItemTeleport : Item
{
    [SerializeField] private ItemTeleport itemTeleport;

    public CanvasGroup canvasGroup;

    public override void Collect(IUnit affectTarget)
    {
        var player = (Player)affectTarget;
        if (player != null)
        {
            player.Skeleton.DOColor(new Color(0, 0, 0, 0), .5f).OnComplete(() =>
            {
                player.transform.position = itemTeleport.transform.position;
                player.transform.SetParent(itemTeleport.transform.parent);
                // This is an error because it didn't use UpdateDefault() method of the player PhucVH
                player.Skeleton.DOColor(new Color(1, 1, 1, 1), .5f).OnComplete(() =>
                {
                    player.PlayIdle(true);
                    canvasGroup.DOFade(0, .5f).OnComplete(() =>
                    {
                        State = EUnitState.Invalid;
                        gameObject.SetActive(false);
                    });
                    itemTeleport.canvasGroup.DOFade(0, .5f).OnComplete(() =>
                    {
                        itemTeleport.State = EUnitState.Invalid;
                        itemTeleport.gameObject.SetActive(false);

                        //transform.parent.GetComponent<RoomTower>().UpdateUnitCollection();

                        //var parentRoom = itemTeleport.transform.parent.GetComponent<RoomTower>();
                        //parentRoom.UpdateUnitCollection();

                        //player.SetParentRoom(parentRoom);
                        //player.StartSearchingTurn();
                    });
                });
            });
        }
    }
}