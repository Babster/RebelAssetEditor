using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

class ResourceType
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; }
    public string DescriptionEng { get; set; }
    public string DescriptionRus { get; set; }
    public int ImgId { get; set; }

    public ResourceType() { }

    public ResourceType(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        ParentId = Convert.ToInt32(r["parent_id"]);
        Name = Convert.ToString(r["name"]);
        DescriptionEng = Convert.ToString(r["description_english"]);
        DescriptionRus = Convert.ToString(r["description_russian"]);
        ImgId = Convert.ToInt32(r["img_id"]);
    }

    public static List<ResourceType> GetResouceList()
    {
        List<ResourceType> resList = new List<ResourceType>();
        string q = ResourceQuery();
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while (r.Read())
            {
                resList.Add(new ResourceType(r));
            }
        }
        r.Close();
        return resList;
    }

    private static string ResourceQuery()
    {
        string q = $@"
            SELECT
                id,
                parent_id,
                name,
                description_english,
                description_russian,
                img_id
            FROM
                resource_types
            ";
        return q;
    }

    
    private static void FillResDict()
    {
        StaticMembers.resDict = new Dictionary<int, ResourceType>();
        List<ResourceType> resList = GetResouceList();
        if(resList.Count > 0)
        {
            foreach(ResourceType res in resList)
            {
                StaticMembers.resDict.Add(res.Id, res);
            }
        }
    }
    public static ResourceType ResourceById(int Id)
    {
        if (StaticMembers.resDict == null)
            FillResDict();
        if(StaticMembers.resDict.ContainsKey(Id))
        {
            return StaticMembers.resDict[Id];
        }
        else
        {
            return null;
        }
    }

    public void Delete()
    {
        if (Id == 0)
            return;
        string q = $@"DELETE FROM resource_types WHERE id = {Id}";
        DataConnection.Execute(q);
    }

    public void Save()
    {

        string q;
        if(Id == 0)
        {
            q = $@"
                INSERT INTO resource_types(parent_id) VALUES({ParentId})
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
            UPDATE resource_types SET
                parent_id = {ParentId},
                name = @str1,
                description_english = @str2,
                description_russian = @str3,
                img_id = {ImgId}
            WHERE 
                id = {Id}";
        List<string> names = new List<string> { Name, DescriptionEng, DescriptionRus };
        DataConnection.Execute(q, names);
    }

    public override string ToString()
    {
        return $"{Name} ({Id})";
    }

}
