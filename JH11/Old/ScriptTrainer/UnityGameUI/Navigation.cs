using System;
using UnityEngine;

namespace UnityGameUI
{
	// Token: 0x02000009 RID: 9
	public class Navigation
	{
		// Token: 0x0600003D RID: 61 RVA: 0x000040DF File Offset: 0x000022DF
		public Navigation(string key, string button, GameObject panel, bool show)
		{
			this.key = key;
			this.button = button;
			this.panel = panel;
			this.show = show;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004106 File Offset: 0x00002306
		internal void SetActive(bool v)
		{
			this.show = v;
			this.panel.SetActive(v);
		}

		// Token: 0x04000017 RID: 23
		public string key;

		// Token: 0x04000018 RID: 24
		public string button;

		// Token: 0x04000019 RID: 25
		public GameObject panel;

		// Token: 0x0400001A RID: 26
		public bool show;
	}
}
