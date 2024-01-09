using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRule;

namespace ScriptTrainer.UI
{
    public class TooltipGUI : MonoBehaviour
    {
        public TooltipGUI(IntPtr handle) : base(handle) { }
        public bool EnableTooltip
        {
            get
            {
                return background?.active ?? false;
            }
            set
            {
                background?.SetActive(value);
            }
        }

        public static readonly Color DefaultBackgroundColor = new Color(0.45f, 0.45f, 0.45f, 1);
        public static readonly Color DefaultTextColor = new Color(1f, 1f, 1f, 1);
        public string Tooltip = string.Empty;
        public GameObject canvas = null;
        public GameObject background = null;
        public void Initialize(string tip, int sortingOrder, Color backgroundColor = default(Color), Color textColor = default(Color))
        {
            Tooltip = tip;
            canvas = UIControls.createUICanvas(ScriptTrainer.WindowSizeFactor.Value);
            canvas.GetComponent<Canvas>().overrideSorting = true;
            canvas.GetComponent<Canvas>().sortingOrder = sortingOrder;

            background = UIControls.createUIPanel(canvas, "40", "150", null);
            background.GetComponent<Image>().color = ((backgroundColor == default) ? DefaultBackgroundColor : backgroundColor);
            

            GameObject uiText = UIControls.createUIText(background, "#FFFFFFFF");
            uiText.GetComponent<Text>().color = ((textColor == default) ? DefaultTextColor : textColor);
            uiText.GetComponent<Text>().text = Tooltip;
            int size = 40;
            uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(150, size);
            size = (int)(uiText.GetComponent<Text>().preferredHeight + 10);
            uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(150, size);
            background.GetComponent<RectTransform>().sizeDelta = new Vector2(160, size+10);

            EnableTooltip = false;
        }
        public void Update()
        {
            var mousepos = UnityEngine.Input.mousePosition;

            if (background != null && !string.IsNullOrEmpty(Tooltip))
            {
                //Vector2 size = base.gameObject.GetComponent<RectTransform>().rect.size;
                //if (mousepos.x < base.gameObject.transform.position.x + size.x * WindowSizeFactor / 2 && mousepos.x > base.gameObject.transform.position.x - size.x * WindowSizeFactor / 2)
                //{
                //    if (mousepos.y < base.gameObject.transform.position.y + size.y * WindowSizeFactor / 2 && mousepos.y > base.gameObject.transform.position.y - size.y * WindowSizeFactor / 2)
                //    {
                //        
                //    }
                //}

                if(RectTransformUtility.RectangleContainsScreenPoint(base.gameObject.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    var x = background.GetComponent<RectTransform>().sizeDelta;
                    background.transform.position = mousepos + new Vector3(x.x/2, -x.y/2, 0) + new Vector3(10,0,0);
                    EnableTooltip = true;
                    return;
                }
            }

            EnableTooltip = false;
        }
        public void OnDestroy()
        {
            UnityEngine.Object.Destroy(canvas);
            UnityEngine.Object.Destroy(base.gameObject);
        }
        
    }
}
