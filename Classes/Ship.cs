using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class Ship
{

    public int Id { get; set; }
    public int PlayerId { get; set; }
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

    public int RigId { get; set; }

    public Ship(SqlDataReader r)
    {
        LoadFromReader(r);
    }

    public Ship(int id)
    {
        string q = ShipQuery();
        q += $" WHERE id = {Id}";
        SqlDataReader r = DataConnection.GetReader(q);
        r.Read();
        LoadFromReader(r);
        r.Close();
    }

    private void LoadFromReader(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        PlayerId = Convert.ToInt32(r["player_id"]);
        ModelId = Convert.ToInt32(r["ss_design_id"]);
        Experience = Convert.ToInt32(r["experience"]);
        Level = Convert.ToInt32(r["ship_level"]);

    }

    public static string ShipQuery()
    {
        string q = $@"
        SELECT
            id,
            player_id,
            ss_design_id,
            experience,
            ship_level
        FROM
            admirals_ships";

        return q;

    }

    public static List<Ship> PlayerShips(int playerId)
    {
        List<Ship> ships = new List<Ship>();
        string q = ShipQuery();
        q += $@" WHERE player_id = {playerId}";
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                ships.Add(new Ship(r));
            }
        }
        r.Close();
        return ships;
    }

    public void Save()
    {
        string q;
        if(Id == 0)
        {
            q = $@"
                INSERT INTO admirals_ships(player_id) VALUES({PlayerId})
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"UPDATE admirals_ships SET 
                player_id = {PlayerId},
                ss_design_id = {ModelId},
                experience = {Experience},
                ship_level = {Level},
                rig_id = {RigId}
            WHERE
                id = {Id}";
        DataConnection.Execute(q);

    }

}
