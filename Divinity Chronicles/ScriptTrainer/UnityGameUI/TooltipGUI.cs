
using ScriptTrainer;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityGameUI
{
    internal class TooltipGUI : MonoBehaviour
    {
        public static TooltipGUI instance = null;
        public GameObject TooltipText = null;
        public string TooltipTextValue = string.Empty;
        private bool initialized = false;
        public void Start()
        {
            instance = this;
        }
        public void Update()
        {
            var mouse = Input.mousePosition;
            if (gameObject != null && initialized)
            {
                Vector2 size = gameObject.GetComponent<RectTransform>().rect.size;
                if (mouse.x < gameObject.transform.position.x + size.x / 2 && mouse.x > gameObject.transform.position.x - size.x / 2)
                {
                    if (mouse.y < gameObject.transform.position.y + size.y / 2 && mouse.y > gameObject.transform.position.y - size.y / 2)
                    {
                        //TooltipText.transform.position = new Vector2(mouse.x + 15, Screen.height - mouse.y + 15);
                        TooltipText.GetComponent<TooltipTextScript>().ShowTooltip(true, 0.2f);
                        return;
                    }
                }
                TooltipText.GetComponent<TooltipTextScript>().HideTooltip(true, 0f, false);
            }
        }
        public void Init(string text)
        {
            TooltipText = Instantiate(Resources.Load<GameObject>("UI/TooltipText"));
            TooltipTextScript component = TooltipText.GetComponent<TooltipTextScript>();
            TooltipTextValue = text;
            component.SetText(TooltipTextValue, new TooltipTextScript.PopupTextLayoutInfo() { minWidth = 165});
            component.SetFontSize(18);
            Popup(TooltipText.GetComponent<TooltipTextScript>(), MainWindow.dragAndDrog.gameObject, new Vector2(-440, 0));
            initialized = true;
        }
        public void Popup(TooltipTextScript anchorObject, GameObject ui, Vector2 offset)
        {
            GameObject sceneDialogCanvas = ui;
            if (sceneDialogCanvas == null)
            {
                return;
            }

            RectTransform component = ui.GetComponent<RectTransform>();
            Vector3 vector;
            if (component)
            {
                Vector3 position = default(Vector3);
                position.x = (anchorObject.CustomAnchorPoint.x - component.pivot.x) * component.sizeDelta.x;
                position.y = (anchorObject.CustomAnchorPoint.y - component.pivot.y) * component.sizeDelta.y;
                position.z = 0f;
                vector = sceneDialogCanvas.transform.InverseTransformPoint(ui.transform.TransformPoint(position));
            }
            else
            {
                vector = sceneDialogCanvas.transform.InverseTransformPoint(ui.transform.position);
            }
            RectTransform component2 = anchorObject.GetComponent<RectTransform>();
            component2.anchorMin = new Vector2(0.5f, 0.5f);
            component2.anchorMax = new Vector2(0.5f, 0.5f);
            Vector2 vector2 = offset;

            component2.SetParent(sceneDialogCanvas.transform, false);
            component2.anchoredPosition = new Vector2(vector.x, vector.y) + vector2;

        }
    }
}
