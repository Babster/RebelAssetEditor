using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

class Ship
{

    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int ModelId { get; set; }
    public ShipModel Model 
    { 
        get
        {
            if (ModelId == 0)
                return null;
            return ShipModel.ModelById(ModelId);
        }
    }
    public int Level { get; set; }
    public int Experience { get; set; }

    public Ship(r)

    public Ship(int Id)
    {
        string q;
        q = $@"";
    }

}
