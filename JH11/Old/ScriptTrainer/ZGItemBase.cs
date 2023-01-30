using System;

namespace ScriptTrainer
{
	// Token: 0x0200000E RID: 14
	public class ZGItemBase
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00007238 File Offset: 0x00005438
		public ZGItemBase(string name, string description, Type itemType, Item item)
		{
			this.Name = name;
			this.Description = description;
			this.ItemType = itemType;
			this.Item = item;
		}

		// Token: 0x0400003B RID: 59
		public string Name;

		// Token: 0x0400003C RID: 60
		public string Description;

		// Token: 0x0400003D RID: 61
		public Type ItemType;

		// Token: 0x0400003E RID: 62
		public Item Item;
	}
}
