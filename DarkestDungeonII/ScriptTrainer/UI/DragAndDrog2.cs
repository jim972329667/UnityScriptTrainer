using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScriptTrainer.UI
{
    public class DragAndDrog2 : MonoBehaviour
    {
        public bool isMouseDrag { get; set; } = false;
        public Vector2 offset { get; set; }
        public void Start()
        {

        }

        // Update is called once per frame
        public void Update()
        {
            GameObjectDragAndDrog();
        }

        //任意拖拽


        //拖拽Updata
        private void GameObjectDragAndDrog()
        {
            var mos = Mouse.current.position.ReadValue();

            if (isMouseDrag)
            {
                base.gameObject.transform.position = new Vector2(mos.x, mos.y) + new Vector2(offset.x, offset.y);
                isMouseDrag = false;
            }

            if (Mouse.current.leftButton.isPressed)
            {
                var pos = base.gameObject.transform.position;
                Vector2 WindowSize = base.gameObject.GetComponent<RectTransform>().rect.size;
                if (mos.x < pos.x + WindowSize.x / 2 && mos.x > pos.x - WindowSize.x / 2)
                {
                    if (mos.y < pos.y + WindowSize.y / 2 && mos.y > pos.y - WindowSize.y / 2)
                    {
                        isMouseDrag = true;
                        offset = new Vector2(pos.x, pos.y) - mos;
                    }
                }
            }
        }
    }
}
