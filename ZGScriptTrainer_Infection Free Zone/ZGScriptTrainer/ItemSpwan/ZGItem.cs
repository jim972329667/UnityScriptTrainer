using Gameplay.GameResources;
using Gameplay.Vehicles;
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
        public ResourceID ID { get; set; }
        public VehicleType VehicleType { get; set; }
        public int Count { get; set; }
    }
}
