using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptTrainer
{
    [Serializable]
    public class PresetData
    {
        // Token: 0x04000015 RID: 21
        public int version;

        // Token: 0x04000016 RID: 22
        public string presetName = "Preset0";

        // Token: 0x04000017 RID: 23
        public string skinColor = "";

        // Token: 0x04000018 RID: 24
        public string bodyTexture = "Default";

        // Token: 0x04000019 RID: 25
        public bool hideCloth = false;

        // Token: 0x0400001A RID: 26
        public string clothSkin = "Default";

        // Token: 0x0400001B RID: 27
        public string crownModel = "Default";

        // Token: 0x0400001C RID: 28
        public float crownPositionX;

        // Token: 0x0400001D RID: 29
        public float crownPositionY;

        // Token: 0x0400001E RID: 30
        public float crownPositionZ;

        // Token: 0x0400001F RID: 31
        public float crownRotationX;

        // Token: 0x04000020 RID: 32
        public float crownRotationY;

        // Token: 0x04000021 RID: 33
        public float crownRotationZ;

        // Token: 0x04000022 RID: 34
        public float crownScale = 1f;

        // Token: 0x04000023 RID: 35
        public float bodySmoothness;
    }
}
