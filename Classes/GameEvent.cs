using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class GameEvent
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; }

    public List<EventElement> Elements { get; set; }

    public GameEvent() { }
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
                name
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

    public class EventElement
    {
        public int Id { get; set; }
        public int GameEventId { get; set; }
        public string ElementType { get; set; }
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
                    INSERT INTO game_events_elements(game_event_id) VALUES{gameEventId}
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

    }

    public EventElement AddElement()
    {
        EventElement newElement = new EventElement();
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
                name = @str1
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
            q += $@" AND id NOT IN({idsDoNotDelete}";
        }
        DataConnection.Execute(q);

    }

}

