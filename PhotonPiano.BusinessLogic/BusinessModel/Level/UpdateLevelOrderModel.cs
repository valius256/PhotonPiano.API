using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotonPiano.BusinessLogic.BusinessModel.Level
{
    public class UpdateLevelOrderModel
    {
        public List<LevelOrderModel> LevelOrders { get; set; } = []; // Must include all level
    }
}
