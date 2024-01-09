using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScriptTrainer.UI
{
    public class TooltipGUI : MonoBehaviour
    {
        public TooltipGUI(IntPtr handle) : base(handle) { }
        public bool EnableTooltip = false;
        public List<string> Tooltip = new List<string>();
        public float WindowSizeFactor = 1;
        private GUIStyle tooltipStyle;

        public void Update()
        {
            var mousepos = UnityEngine.Input.mousePosition;

            if (base.gameObject != null)
            {
                Vector2 size = base.gameObject.GetComponent<RectTransform>().rect.size;
                if (mousepos.x < base.gameObject.transform.position.x + size.x * WindowSizeFactor / 2 && mousepos.x > base.gameObject.transform.position.x - size.x * WindowSizeFactor / 2)
                {
                    if (mousepos.y < base.gameObject.transform.position.y + size.y * WindowSizeFactor / 2 && mousepos.y > base.gameObject.transform.position.y - size.y * WindowSizeFactor / 2)
                    {
                        EnableTooltip = true;
                        return;
                    }
                }
            }

            EnableTooltip = false;
        }
        public void OnGUI()
        {
            //var mousepos = EventSystem.current.currentInputModule.input.mousePosition; // Instead of Input.mousePosition
            var mousepos = UnityEngine.Input.mousePosition;
            if (Tooltip.Count != 0 && EnableTooltip == true)
            {
                GetGUIStyle();
                float RectX = mousepos.x + 15;
                float RectY = Screen.height - mousepos.y + 15;
                float width = GetMaxText();
                // The +15 are cursor offsets
                for (int i = 0; i < Tooltip.Count; i++)
                {
                    GUI.backgroundColor = Color.black;
                    GUIContent content = new GUIContent(Tooltip[i]);
                    //float width = tooltipStyle.CalcSize(content).x;
                    float height = tooltipStyle.CalcSize(content).y;

                    GUI.Box(new Rect(RectX, RectY, width, Math.Max(25f, height)), content, tooltipStyle);
                    RectY += Math.Max(25f, height);
                }
            }
        }
        private float GetMaxText()
        {
            float value = 0;
            if (Tooltip.Count != 0)
            {
                for (int i = 0; i < Tooltip.Count; i++)
                {
                    GUIContent content = new GUIContent(Tooltip[i]);
                    float width = tooltipStyle.CalcSize(content).x;
                    if (width > value)
                        value = width;
                }
            }
            return value;
        }
        private void GetGUIStyle()
        {
            tooltipStyle = new GUIStyle(GUI.skin.box)
            {
                richText = true,
                alignment = TextAnchor.MiddleLeft
            };
            tooltipStyle.normal.textColor = Color.white;
            tooltipStyle.fontSize = (int)(18 * WindowSizeFactor);
        }
    }
}
