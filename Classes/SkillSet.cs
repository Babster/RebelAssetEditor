using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class SkillSetSql : SkillSet
{

    public SkillSetSql() { }

    public SkillSetSql(SqlDataReader r) 
    {
        Id = (int)r["id"];
        ParentId = (int)r["parent_id"];
        Name = (string)r["name"];
        Description = (string)r["description"];
        OwnerType = (SkillSet.SkillsetOwnerTypes)r["owner_type"];
        OpenCost = (int)r["open_cost"];
        AvailableForPlayer = (int)r["available_for_player"];
        ExpType = (ExperiencingType)r["experience_type"];
        LoadElements();

    }

    private void LoadElements()
    {
        Elements = new List<SkillSetElement>();
        string q = $@"
            SELECT
                id,
                skill_set_id,
                ISNULL(skill_type_id, 0) AS skill_type_id,
                skill_level,
                skill_column,
                available_at_start,
                predecessor_1,
                predecessor_2,
                ISNULL(replaces_skill, 0) AS replaces_skill
            FROM
                skill_sets_structure
            WHERE
                skill_set_id = {Id}";
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                Elements.Add(new SkillSetElementSql(r));
            }
        }
        r.Close();

    }

    public SkillSetElementSql AddElement()
    {
        SkillSetElementSql newElement = new SkillSetElementSql();
        newElement.SkillSetId = Id;
        Elements.Add(newElement);
        return newElement;
    }

    public void SaveData()
    {
        string q;
        if(Id == 0)
        {
            q = $@"INSERT INTO skill_sets(name) VALUES ('')
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }
        q = $@"UPDATE skill_sets SET 
                parent_id = {ParentId},
                name = @str1,
                description = @str2, 
                owner_type = {(int)OwnerType},
                open_cost = {OpenCost},
                available_for_player = {AvailableForPlayer},
                experience_type = {(int)ExpType}
            WHERE
                id = {Id}
            ";
        List<string> names = new List<string> { Name, Description };
        DataConnection.Execute(q, names);

        string idsDoNotDelete = "";

        if(Elements.Count > 0)
        {
            foreach(var element in Elements)
            {
                SkillSetElementSql curElement = (SkillSetElementSql)element;
                curElement.SaveData(Id);
                if(idsDoNotDelete != "")
                {
                    idsDoNotDelete += ",";
                }
                idsDoNotDelete += curElement.Id.ToString();
            }
        }

        q = $@"DELETE FROM skill_sets_structure WHERE skill_set_id = {Id}";
        if(idsDoNotDelete != "")
        {
            q += $@" AND id NOT IN({idsDoNotDelete})";
        }
        DataConnection.Execute(q);

    }

    private static string SkillSetQuery()
    {
        return $@"
        SELECT
            id,
            parent_id,
            name,
            ISNULL(description, '') AS description,
            owner_type,
            open_cost,
            ISNULL(available_for_player, 0) AS available_for_player,
            ISNULL(experience_type, 0) AS experience_type
        FROM
            skill_sets";
    }

    public static List<SkillSetSql> GetSkillsetList()
    {
        List<SkillSetSql> list = new List<SkillSetSql>();
        string q = SkillSetQuery();
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                list.Add(new SkillSetSql(r));
            }
        }
        r.Close();
        return list;

    }

}

public class SkillSetElementSql:SkillSetElement
{
    public SkillSetElementSql() { }

    public SkillSetElementSql(SqlDataReader r)
    {
        Id = (int)r["id"];
        SkillSetId = (int)r["skill_set_id"];
        SkillTypeId = (int)r["skill_type_id"];
        SkillLevel = (int)r["skill_level"];
        SkillColumn = (int)r["skill_column"];
        AvailableAtStart = (int)r["available_at_start"];
        Predecessor1 = (int)r["predecessor_1"];
        Predecessor2 = (int)r["predecessor_2"];
        ReplacesSkill = (int)r["replaces_skill"];
}

    public void SaveData(int SkillSetId)
    {
        string q;
        if(Id == 0)
        {
            q = $@"
                INSERT INTO skill_sets_structure(skill_set_id) VALUES({SkillSetId})
                SELECT @@IDENTITY AS Result";

            Id = DataConnection.GetResultInt(q);

        }

        q = $@"
            UPDATE skill_sets_structure SET 
                skill_type_id = {SkillTypeId},
                skill_level = {SkillLevel},
                skill_column = {SkillColumn},
                available_at_start = {AvailableAtStart},
                predecessor_1 = {Predecessor1},
                predecessor_2 = {Predecessor2},
                replaces_skill = {ReplacesSkill}
            WHERE
                id = {Id}";

        DataConnection.Execute(q);

    }

}

public class SkillSet
{

    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public SkillsetOwnerTypes OwnerType { get; set; }
    public int OpenCost { get; set; } //Сколько опыта требуется для активации данного набора скиллов
    public int AvailableForPlayer { get; set; }
    public List<SkillSetElement> Elements { get; set; }
    public ExperiencingType ExpType { get; set; }

    public enum SkillsetOwnerTypes
    {
        None = 0,
        Officer = 1,
        Spaceship = 2,
        Module = 3
    }

    public enum ExperiencingType
    {
        None = 0,
        KillEnemy = 1,
        DamageEnemy = 2,
        GetDamage = 3,
        UseEnergy = 4
    }

    public SkillSet() { Elements = new List<SkillSetElement>(); }

}

public class SkillSetElement
{
    //Структура набора скиллов следующая.
    //1. Сам набор - SkillSet - соответствует одной записи в таблице skill_sets и соответствует одному набору
    //скиллов. Например, базовая ветка в космических коряблях - починка брони и реген щита
    //2. Таблица связей между SkillSet и SkillType - к какому набору скиллов SkillSet какие скиллы SkillType привязаны
    //и сколько стоят при активации во время миссии + за сколько могут быть открыты во время миссии. Таблица skill_sets_structure
    //Как раз этот класс и есть 2, его экземпляр соответствует привязке одного скилла к набору
    //3. SkillTypes - сами скиллы. Описано какой у них тип, сколько стоят и каков эффект от активации
    
    public int Id { get; set; }
    public int SkillSetId { get; set; }
    public int SkillTypeId { get; set; }
    public int SkillLevel { get; set; }
    public int SkillColumn { get; set; }
    public int AvailableAtStart { get; set; }
    public int Predecessor1 { get; set; }
    public int Predecessor2 { get; set; }
    public int ReplacesSkill { get; set; }
    public SkillSetElement() { }

    public override string ToString()
    {
        SkillType t = SkillTypeSql.SkillTypeById(SkillTypeId);
        if(t == null)
        {
            return $"[empty type] : {SkillLevel}, {SkillColumn}";
        }
        else
        {
            return $"{t.Name}: {SkillLevel}, {SkillColumn}";
        }
    }

}