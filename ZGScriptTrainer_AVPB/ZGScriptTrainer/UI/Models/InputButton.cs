using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib;
using UniverseLib.UI.Models;

namespace ZGScriptTrainer.UI.Models
{
    public class InputButton
    {
        public Action<string> OnClick;
        public ButtonRef Button { get; }
        public InputFieldRef InputField { get; }
        public string InputText
        {
            get
            {
                return InputField?.Text;
            }
        }
        public InputButton(ButtonRef button, InputFieldRef inputField)
        {
            Button = button;
            InputField = inputField;
            Button.Component.onClick.AddListener(delegate
            {
                OnClick?.Invoke(InputText);
            });
        }
    }
}
