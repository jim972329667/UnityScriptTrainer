using UnityEngine;
using System.Collections;

namespace UnityGameUI
{
    public class DragAndDrog : MonoBehaviour
    {

        public GameObject target;
        public bool isMouseDrag;
        public Vector2 WindowSize;
        //private Vector3 screenPosition;
        private Vector3 offset;
        // Use this for initialization
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
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.mousePosition.x < target.transform.position.x + WindowSize.x / 2 && Input.mousePosition.x > target.transform.position.x - WindowSize.x / 2)
                {
                    if (Input.mousePosition.y < target.transform.position.y + WindowSize.y / 2 && Input.mousePosition.y > target.transform.position.y - WindowSize.y / 2)
                    {
                        isMouseDrag = true;
                        offset = target.transform.position - Input.mousePosition;
                    }
                }

                //screenPosition = Camera.main.WorldToScreenPoint(target.transform.position);
                //ScriptTrainer.ScriptTrainer.Instance.Log($"ZG:");
                //ScriptTrainer.ScriptTrainer.Instance.Log($"ZG:面板位置:{target.transform.position.x};{target.transform.position.y}");
                //ScriptTrainer.ScriptTrainer.Instance.Log($"ZG:鼠标位置:{Input.mousePosition.x};{Input.mousePosition.y}");
                //ScriptTrainer.ScriptTrainer.Instance.Log($"ZG:面板大小:{target.GetComponent<RectTransform>().sizeDelta.x};{target.GetComponent<RectTransform>().sizeDelta.y}");

            }

            if (Input.GetMouseButtonUp(0))
            {
                isMouseDrag = false;
            }

            if (isMouseDrag)
            {

                //Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
                //Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;
                //target.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
                target.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + new Vector2(offset.x, offset.y);
            }
        }

    }
}

