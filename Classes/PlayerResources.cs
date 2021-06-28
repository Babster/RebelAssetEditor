using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text;


/// <summary>
/// Соответствует набору ресурсов у одного игрока в каком-нибудь одном месте (например, в корабле на миссии, на станции,
/// в торговом челноке и т.д.)
/// </summary>
public class PlayerResources
{

    /* //Пока что отключено чтобы данные не летали на сервер и обратно и чтобы
       //не путаться между переменными в процедурах сохранения и загрузки данных
    public int PlayerId { get; set; }
    public StorageType storageType { get; set; }
    public int StorageId { get; set; }*/

    public List<ResourceUnit> resources { get; set; }

    public enum StorageType
    {
        None = 0,
        SpaceShip = 1,
        MainWarehouse = 2
    }

    public class ResourceUnit
    {
        public int ResourceTypeId { get; set; }
        public int BlueprintId { get; set; }
        public int Quantity { get; set; }

        public ResourceUnit() { }

    }

    public PlayerResources() { }

    /// <summary>
    /// Инициализация объекта на основе того, какой тип и идентификатор хранилища ресурсов передан.
    /// Для дополнительного контроля используется идентификатор игрока
    /// </summary>
    /// <param name="storageType"></param>
    /// <param name="storageId"></param>
    /// <param name="playerId"></param>
    public void LoadData(StorageType storageType, int storageId, int playerId)
    {
        resources = new List<ResourceUnit>();
        string q = $@"
            SELECT
                resource_type,
                blueprint_type,
                quantity
            FROM
                players_resources
            WHERE
                storage_type = {(int)storageType}
                AND storage_id = {storageId}
                AND player_id = {playerId}";

        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                ResourceUnit curRes = new ResourceUnit();
                curRes.ResourceTypeId = (int)r["resource_type"];
                curRes.BlueprintId = (int)r["blueprint_type"];
                curRes.Quantity = (int)r["quantity"];
                resources.Add(curRes);
            }
        }
        r.Close();
    }

    /// <summary>
    /// Обновляет данные в каком-нибудь хранилище имущества игрока
    /// </summary>
    /// <param name="storageType"></param>
    /// <param name="storageId"></param>
    /// <param name="playerId">нужен только для того, чтобы игроки не вандалили имущество других игроков</param>
    public void SaveData(StorageType storageType, int storageId, int playerId)
    {
        //Собираем индентификаторы записей в таблице имущества игрока, которые у него уже есть
        string q;
        q = $@"SELECT id FROM players_resources WHERE storage_type = {(int)storageType} AND storage_id = {storageId} AND player_id = {playerId}";
        List<int> Ids = new List<int>();
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while (r.Read())
            {
                Ids.Add((int)r["id"]);
            }
        }
        r.Close();

        //Заготовка для запроса, который вставит одну запись ресурса в базу и вернёт идентификатор
        string insertQuery = $@"
            INSERT INTO players_resources(
                player_id,
                storage_type, 
                storage_id
            ) VALUES (
                {playerId},
                {(int)storageType},
                {storageId}
            )
            SELECT @@IDENTITY AS Result";

        //Теперь на основе содержимого объекта обновляем записи в базе
        foreach(var res in this.resources)
        {
            int id;
            if(Ids.Count > 0)
            {
                id = Ids[0];
                Ids.Remove(id);
            }
            else
            {
                id = DataConnection.GetResultInt(insertQuery);
            }
            q = $@"
                UPDATE players_resources SET 
                    resource_type = {res.ResourceTypeId},
                    blueprint_type = {res.BlueprintId},
                    quantity = {res.Quantity}
                WHERE
                    id = {id}";
            DataConnection.Execute(q);
        }

        //Исчезнувшие ресурсы убираем из таблицы
        if(Ids.Count > 0)
        {
            string idsToDelete = "";
            foreach(var id in Ids)
            {
                if(idsToDelete != "")
                {
                    idsToDelete += ",";
                }
                idsToDelete += id.ToString();
            }
            q = $@"DELETE FROM players_resources WHERE id IN{idsToDelete}";
            DataConnection.Execute(q);
        }
    }
    
    public void TransferToWarehouse(StorageType storageType, int storageId, int playerId)
    {

        string q;

        string mainQuery = $@"
            SELECT
                id,
                quantity
            FROM
                players_resources
            WHERE
                resource_type = <resource_type>
                AND blueprint_type = <blueprint_type>
                AND storage_type = {(int)StorageType.MainWarehouse}
                AND storage_id = 0
                AND player_id = {playerId}";

        string insertQuery = $@"
            INSERT INTO players_resources (
                player_id,
                storage_type,
                storage_id,
                resource_type,
                blueprint_type,
                quantity
            ) VALUES (
                {playerId},
                {(int)StorageType.MainWarehouse},
                0,
                <resource_type>,
                <blueprint_type>,
                <quantity>
            ";

        foreach(var res in resources)
        {
            q = mainQuery.Replace("<resource_type>", $"{res.ResourceTypeId}");
            q = q.Replace("<blueprint_type>", $"{res.BlueprintId}");
            int id = 0, quantity = 0;
            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                r.Read();
                id = (int)r["id"];
                quantity = (int)r["quantity"];
            }
            r.Close();
            if(id > 0)
            {
                q = $@"UPDATE players_resources SET quantity = {(quantity + res.Quantity).ToString()} WHERE id = {id}";
                DataConnection.Execute(q);
            }
            else
            {
                q = insertQuery.Replace("<resource_type>", $"{res.ResourceTypeId}");
                q = q.Replace("<blueprint_type>", $"{res.BlueprintId}");
                q = q.Replace("<quantity>", $"{res.Quantity})");
                DataConnection.Execute(q);
            }

        }

        q = $@"DELETE FROM players_resources WHERE storage_type = {(int)storageType} AND storage_id = {storageId}";
        DataConnection.Execute(q);

    }

}
