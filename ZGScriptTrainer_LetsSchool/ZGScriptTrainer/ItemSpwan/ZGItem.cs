using ProjectSchoolNs.GameItemNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGScriptTrainer.ItemSpwan
{
    public class ZGItem
    {
        public string Type { get; set; }
        public long ID { get; set; }
        public int Count { get; set; }
        public GameItem BaseItem { get; set; }
    }
}
