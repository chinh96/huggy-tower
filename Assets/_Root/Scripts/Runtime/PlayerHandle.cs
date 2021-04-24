using System;
using Lean.Touch;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHandle : UnitHandle
{
   [SerializeField] private Rigidbody2D rigid2D;
   [SerializeField] private Collider2D coll2D;
   [SerializeField] private LeanSelectableByFinger leanSelectableByFinger;

   public void OnSelected()
   {
      rigid2D.gravityScale = 0;
      coll2D.enabled = false;
   }
   
   public void OnDeSelected()
   {
      rigid2D.gravityScale = 1;
      coll2D.enabled = true;
   }

   private void OnMouseDown()
   {
      OnSelected();
      leanSelectableByFinger.SelfSelected = true;
   }

   private void OnMouseUp()
   {
      OnDeSelected();
      leanSelectableByFinger.Deselect();
   }
}
