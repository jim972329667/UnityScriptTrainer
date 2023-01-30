using System;
using UnityEngine;

namespace UnityGameUI
{
	// Token: 0x02000002 RID: 2
	public class DragAndDrog : MonoBehaviour
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public void Start()
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002053 File Offset: 0x00000253
		public void Update()
		{
			this.GameObjectDragAndDrog();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002060 File Offset: 0x00000260
		private void GameObjectDragAndDrog()
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(0);
			if (mouseButtonDown)
			{
				bool flag = Input.mousePosition.x < this.target.transform.position.x + this.WindowSize.x / 2f && Input.mousePosition.x > this.target.transform.position.x - this.WindowSize.x / 2f;
				if (flag)
				{
					bool flag2 = Input.mousePosition.y < this.target.transform.position.y + this.WindowSize.y / 2f && Input.mousePosition.y > this.target.transform.position.y - this.WindowSize.y / 2f;
					if (flag2)
					{
						this.isMouseDrag = true;
						this.offset = this.target.transform.position - Input.mousePosition;
					}
				}
			}
			bool mouseButtonUp = Input.GetMouseButtonUp(0);
			if (mouseButtonUp)
			{
				this.isMouseDrag = false;
			}
			bool flag3 = this.isMouseDrag;
			if (flag3)
			{
				this.target.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + new Vector2(this.offset.x, this.offset.y);
			}
		}

		// Token: 0x04000001 RID: 1
		public GameObject target;

		// Token: 0x04000002 RID: 2
		public bool isMouseDrag;

		// Token: 0x04000003 RID: 3
		public Vector2 WindowSize;

		// Token: 0x04000004 RID: 4
		private Vector3 offset;
	}
}
