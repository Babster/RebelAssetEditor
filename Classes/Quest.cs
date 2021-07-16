using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public partial class Quest
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    public int AvailableAtStart { get; set; }
    public string PredecessorQuests { get; set; }
    public string ScenesOnStart { get; set; }
    public string ScenesOnEnd { get; set; }
    public int BattleTypeId { get; set; }
    public int GameEventOnEnd { get; set; }
    public EventManager.EventType EventType { get; set; } //Как поя
    public int AppearVariety { get; set; }
    
    public Quest() { }

}

public partial class Quest
{

    public Quest(SqlDataReader r)
    {
        Id = (int)r["id"];
        ParentId = (int)r["parent_id"];
        Name = (string)r["name"];
        Text = (string)r["text"];
        AvailableAtStart = (int)r["available_at_start"];
        PredecessorQuests = (string)r["predecessor_quests"];
        ScenesOnStart = (string)r["scenes_on_start"];
        ScenesOnEnd = (string)r["scenes_on_end"];
        BattleTypeId = (int)r["battle_type"];
        GameEventOnEnd = (int)r["game_event_on_end"];
        EventType = (EventManager.EventType)r["event_type"];
        AppearVariety = (int)r["appear_variety"];
    }

    public void SaveData()
    {
        string q;
        if(Id == 0)
        {
            q = $@"
                INSERT INTO quests(name) VALUES('')
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
            UPDATE quests SET
                parent_id = {ParentId},
                name = @str1,
                text = @str2,
                available_at_start = {AvailableAtStart},
                predecessor_quests = @str3,
                scenes_on_start = @str4,
                scenes_on_end = @str5,
                battle_type = {BattleTypeId},
                game_event_on_end = {GameEventOnEnd},
                event_type = {(int)EventType},
                appear_variety = {AppearVariety}
            WHERE
                id = {Id}";
        List<string> names = new List<string>();
        names.Add(Name);
        names.Add(Text);
        names.Add(PredecessorQuests);
        names.Add(ScenesOnStart);
        names.Add(ScenesOnEnd);
        DataConnection.Execute(q, names);

    }

    public static string QuestsQuery()
    {
        return $@"
            SELECT
                id,
                parent_id,
                name,
                text,
                available_at_start,
                predecessor_quests,
                scenes_on_start,
                scenes_on_end,
                battle_type,
                game_event_on_end,
                event_type,
                appear_variety
            FROM
                story_quests";
    }

    public static List<Quest> QuestList()
    {
        List<Quest> quests = new List<Quest>();
        SqlDataReader r = DataConnection.GetReader(QuestsQuery());
        if(r.HasRows)
        {
            while(r.Read())
            {
                quests.Add(new Quest(r));
            }
        }
        r.Close();
        return quests;
    }

}