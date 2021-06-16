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

    public int Id { get; set; }
    public int PlayerId { get; set; }
    public StorageType storageType { get; set; }
    public int StorageId { get; set; }
    public int ResourceTypeId { get; set; }
    public int BlueprintId { get; set; }
    public int Quantity { get; set; }

    public enum StorageType
    {
        None = 0,
        SpaceShip = 1,
        MainWarehouse = 2
    }

    public PlayerResources() { }

}
