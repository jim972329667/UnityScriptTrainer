using System;
using UnityEngine;

namespace ScriptTrainer
{
	// Token: 0x02000014 RID: 20
	public class ZGGameObject : MonoBehaviour
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x0000A31E File Offset: 0x0000851E
		public void Start()
		{
			this.mw = new MainWindow();
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000A32C File Offset: 0x0000852C
		public void Update()
		{
			bool flag = !MainWindow.initialized;
			if (flag)
			{
				MainWindow.Initialize();
			}
			bool keyDown = Input.GetKeyDown(ScriptTrainer.ShowCounter.Value);
			if (keyDown)
			{
				bool flag2 = !MainWindow.initialized;
				if (!flag2)
				{
					MainWindow.optionToggle = !MainWindow.optionToggle;
					MainWindow.canvas.SetActive(MainWindow.optionToggle);
					Event.current.Use();
				}
			}
		}

		// Token: 0x04000070 RID: 112
		public MainWindow mw;
	}
}
