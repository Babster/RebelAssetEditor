using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.SqlClient;

class Battle
{

    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int Ongoing { get; set; }
    public int BattleSceneTypeId { get; set; }
    public int RigId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateComplete { get; set; }
    public int Successfull { get; set; }
    public int CurrentCycle { get; set; }
    public int CurrentStage { get; set; }
    public int MaxOpenedCycle { get; set; }
    public int MaxOpenedStage { get; set; }
    public int ShipExperience { get; set; }
    public string ShipOpenedSkills { get; set; }

    public Battle() { }

    public Battle(SqlDataReader r)
    {

    }

    /*id int Unchecked
    player_id int Checked
    ongoing int Checked
    battle_scene_type_id int Checked
    rig_id int Checked
    date_start datetime    Checked
    date_complete   datetime Checked
    successfull int Checked
    current_cycle int Checked
    current_stage
    max_opened_cycle	int	Checked
    max_opened_stage	int	Checked
    ship_experience int Checked
    ship_opened_skills varchar(200)    Checked*/
}
