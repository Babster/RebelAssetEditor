using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Crew;

public class GameEvent
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; }
    public int Repeatable { get; set; }

    public List<EventElement> Elements { get; set; }

    public GameEvent() 
    {
        Elements = new List<EventElement>();
    }
    public GameEvent(int parentId)
    {
        this.ParentId = parentId;
        Name = "new game event";
        Elements = new List<EventElement>();
    }
    public GameEvent(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        ParentId = Convert.ToInt32(r["parent_id"]);
        Name = Convert.ToString(r["name"]);
        Repeatable = Convert.ToInt32(r["repeatable"]);

        Elements = new List<EventElement>();
        string q = EventElementsQuery(Id);
        SqlDataReader r2 = DataConnection.GetReader(q);
        if(r2.HasRows)
        {
            while(r2.Read())
            {
                Elements.Add(new EventElement(r2));
            }
        }
        r2.Close();
    }

    private static string EventElementsQuery(int elementId)
    {
        string q = $@"
            SELECT 
                id,
                game_event_id,
                element_type,
                element_info 
            FROM
                game_events_elements
            WHERE
                game_event_id = {elementId}";
        return q;
    }

    public static string EventsQuery()
    {
        string q = $@"
            SELECT
                id,
                parent_id,
                name,
                ISNULL(repeatable, 0) AS repeatable
            FROM
                game_events";
        return q;
    }

    public static List<GameEvent> EventList()
    {
        List<GameEvent> eventList = new List<GameEvent>();
        string q = EventsQuery();
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                GameEvent curEvent = new GameEvent(r);
                eventList.Add(curEvent);
            }
        }
        r.Close();

        return eventList;

    }
    private static Dictionary<int, GameEvent> EventDict;
    private static void FillEventsDictionary()
    {
        EventDict = new Dictionary<int, GameEvent>();
        List<GameEvent> events = EventList();
        foreach(GameEvent evnt in events)
        {
            EventDict.Add(evnt.Id, evnt);
        }
    }
    public static GameEvent EventById(int Id)
    {
        if (EventDict == null)
            FillEventsDictionary();
        if(EventDict.ContainsKey(Id))
        {
            return EventDict[Id];
        }
        else
        {
            return null;
        }
    }

    public class EventElement
    {
        public int Id { get; set; }
        public int GameEventId { get; set; }
        public string ElementInfo { get; set; }

        public EventElement() { }

        public EventElement(SqlDataReader r)
        {
            Id = Convert.ToInt32(r["id"]);
            GameEventId = Convert.ToInt32(r["game_event_id"]);
            ElementType = Convert.ToString(r["element_type"]);
            ElementInfo = Convert.ToString(r["element_info"]);
        }

        public void Save(int gameEventId)
        {
            string q;
            if(Id == 0)
            {
                q = $@"
                    INSERT INTO game_events_elements(game_event_id) VALUES({gameEventId})
                    SELECT @@IDENTITY AS Result";
                Id = DataConnection.GetResultInt(q);
            }

            q = $@"
                UPDATE game_events_elements SET 
                    element_type = @str1,
                    element_info = @str2
                WHERE
                    id = {Id}";
            List<String> names = new List<string> { ElementType, ElementInfo };

            DataConnection.Execute(q, names);

        }

        public void Delete()
        {
            if (Id == 0)
                return;
            string q;
            q = $@"DELETE FROM game_events_elements WHERE id = {Id}";
            DataConnection.Execute(q);
        }

        public override string ToString()
        {
            return ElementType;
        }

        #region Specific parameters

        private void ConvertStringArrayToElementInfo(string[] s)
        {
            ElementInfo = "";
            if (s == null)
                return;
            if (s.Length == 0)
                return;
            for(int i = 0; i < s.Length; i++)
            {
                if (i > 0)
                    ElementInfo += ";";
                ElementInfo += s[i];
            }
        }

        private string pElementType;
        public string ElementType 
        { 
            get
            {
                return pElementType;
            }
            set
            {
                pElementType = value;
                if (String.IsNullOrEmpty(ElementInfo))
                    return;
                string[] s = ElementInfo.Split(';');
                s[0] = "";
                //s[1] = "";

                ConvertStringArrayToElementInfo(s);

            }
        }

        public ShipModel ShipModel
        {
            get
            {
                if (pElementType != "Give spaceship")
                    return null;
                if (String.IsNullOrEmpty(ElementInfo))
                    return null;
                string[] s = ElementInfo.Split(';');
                if (s[0] != "")
                {
                    return ShipModel.ModelById(Convert.ToInt32(s[0]));
                }
                else
                {
                    return null;
                }
            }
            set
            {
                string[] s;
                if (String.IsNullOrEmpty(ElementInfo))
                {
                    s = new string[2];
                    s[1] = "";
                }
                else
                {
                    s = ElementInfo.Split(';');
                }
                if(value == null)
                {
                    s[0] = "0";
                }
                else
                {
                    s[0] = value.Id.ToString();
                }
                

                ConvertStringArrayToElementInfo(s);

            }
        }

        public ShipModuleType ModuleType
        {
            get
            {
                if (pElementType != "Give module")
                    return null;
                if (String.IsNullOrEmpty(ElementInfo))
                    return null;
                string[] s = ElementInfo.Split(';');
                if (s[0] != "")
                {
                    return ShipModuleType.ModuleById(Convert.ToInt32(s[0]));
                }
                else
                {
                    return null;
                }
            }
            set
            {
                string[] s;
                if (String.IsNullOrEmpty(ElementInfo))
                {
                    s = new string[2];
                    s[1] = "";
                }
                else
                {
                    s = ElementInfo.Split(';');
                }

                if (value == null)
                {
                    s[0] = "0";
                }
                else
                {
                    s[0] = value.Id.ToString();
                }

                ConvertStringArrayToElementInfo(s);

            }
        }

        public CrewOfficerType Officer
        {
            get
            {
                if (pElementType != "Create officer")
                    return null;
                if (String.IsNullOrEmpty(ElementInfo))
                    return null;
                string[] s = ElementInfo.Split(';');
                if (s[0] != "")
                {
                    return CrewOfficerType.OfficerById(Convert.ToInt32(s[0]));
                }
                else
                {
                    return null;
                }
            }
            set
            {
                string[] s;
                if (String.IsNullOrEmpty(ElementInfo))
                {
                    s = new string[2];
                    s[1] = "";
                }
                else
                {
                    s = ElementInfo.Split(';');
                }

                if (value == null)
                {
                    s[0] = "0";
                }
                else
                {
                    s[0] = value.Id.ToString();
                }

                ConvertStringArrayToElementInfo(s);

            }
        }

        public int Experience
        {
            get
            {
                if (String.IsNullOrEmpty(ElementInfo))
                    return 0;
                string[] s = ElementInfo.Split(';');
                if (s[1] != "")
                {
                    return Convert.ToInt32(s[1]);
                }
                else
                {
                    return 0;
                }

            }
            set
            {
                string[] s;
                if (String.IsNullOrEmpty(ElementInfo))
                {
                    s = new string[2];
                    s[0] = "";
                }
                else
                {
                    s = ElementInfo.Split(';');
                }

                s[1] = value.ToString();

                ConvertStringArrayToElementInfo(s);
            }
        }

        #endregion

    }

    public EventElement AddElement()
    {
        EventElement newElement = new EventElement();
        newElement.ElementType = "Give spaceship";
        Elements.Add(newElement);
        return newElement;
    }

    public void Save()
    {
        string q;
        if(Id == 0)
        {
            q = $@"
                INSERT INTO game_events(parent_id) VALUES({ParentId})
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
            UPDATE game_events SET
                name = @str1,
                repeatable = {Repeatable}
            WHERE
                id = {Id}";
        List<string> names = new List<string> { Name };
        DataConnection.Execute(q, names);

        string idsDoNotDelete = "";
        if(Elements.Count > 0)
        {
            foreach(EventElement element in Elements)
            {
                element.Save(Id);
                if (idsDoNotDelete != "")
                    idsDoNotDelete += ",";
                idsDoNotDelete += element.Id;
            }
        }
        q = $@"DELETE FROM game_events_elements WHERE game_event_id = {Id}";
        if(idsDoNotDelete != "")
        {
            q += $@" AND id NOT IN({idsDoNotDelete})";
        }
        DataConnection.Execute(q);

    }

    public void Delete()
    {
        if (Id == 0)
            return;
        string q = $@"
            DELETE FROM game_events WHERE id = {Id};
            DELETE FROM game_events_elements WHERE game_event_id = {Id}
            ";
        DataConnection.Execute(q);
    }

    public void ExecuteEvent(AdmiralNamespace.AccountData player)
    {

        if(Elements.Count == 0)
        {
            return;
        }

        //Repeatable events - check if already happend
        if(Repeatable == 0)
        {
            if (EventHappened(player.Id, Id))
                return;
        }

        string q;

        foreach (EventElement element in Elements)
        {

            if(element.ShipModel != null)
            {
                q = $@"
                    INSERT INTO admirals_ships
                    (
                        player_id,
                        ss_design_id,
                        experience,
                        ship_level
                    ) VALUES (
                        {player.Id},
                        {element.ShipModel.Id},
                        {element.Experience},
                        1
                    )";
                DataConnection.Execute(q);
            }

            if(element.ModuleType != null)
            {
                q = $@"
                    INSERT INTO admirals_modules (
                        player_id,
                        module_id,
                        experience,
                        module_level
                    ) VALUES (
                        {player.Id},
                        {element.ModuleType.Id},
                        {element.Experience},
                        1
                    )";
                DataConnection.Execute(q);
            }

            if(element.Officer != null)
            {
                CrewOfficer officer = new CrewOfficer(element.Officer, player.Id);
                officer.Save();
            }

        }

        q = $@"INSERT INTO game_events_log(
                player_id,
                event_id,
                occurs
            ) VALUES (
                {player.Id},
                {Id},
                GETDATE()
            )";
        DataConnection.Execute(q);
    }

    public static bool EventHappened(int playerId, int eventId)
    {
        string q = $"SELECT id FROM game_events_log WHERE player_id = {playerId} AND event_id = {eventId}";
        int Id = DataConnection.GetResultInt(q, null, 0);
        return Id > 0;
    }

}

