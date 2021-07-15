using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Station
{
    //Доступны или нет солнечные батареи
    public int SolarArrays { get; set; }

    //Список слотов под реакторы
    public ReactorSlots reactorSlots { get; set; }

    //Доступные слоты под ангары
    public HangarSlots hangarSlots { get; set; }

    //Какие работают стыковочные модули
    public DockingBays dockingBays { get; set; }

    //Какие есть слоты под помещения для экипажа
    public CrewQuartersSlots crewQuartersSlots { get; set; }

    //Радарные установки
    public RadarSystemsSlots radarSystemsSlots { get; set; }

    #region Объекты, которые соответствуют слотам и наборам слотов, которые есть на базе

    public enum StationSlotType
    {
        Reactor = 1,
        Hangar = 2,
        DockingBay = 3,
        CrewQuarters = 4,
        Workshop = 5,
        Laboratory = 6,
        RadarSystem = 7
    }

    public class ReactorSlots
    {

        public ReactorSlots()
        {
            Reactors = new List<ReactorSlot>();
            Reactors.Add(new ReactorSlot(1, 1));
            Reactors.Add(new ReactorSlot(1, 2));
            Reactors.Add(new ReactorSlot(1, 3));
            Reactors.Add(new ReactorSlot(1, 4));
        }


        public List<ReactorSlot> Reactors { get; set; }

        public int TotalPower()
        {
            return 0;
        }

    }
    public class ReactorSlot
    {
        public int SectorNumber { get; set; }
        public int Number { get; set; }
        public int IsBroken { get; set; }
        public ReactorSlot(int sectorNumber, int number)
        {
            SectorNumber = sectorNumber;
            Number = number;
        }

    }
    public class HangarSlots
    {
        public List<HangarSlot> Slots { get; set; }

        public HangarSlots()
        {
            Slots = new List<HangarSlot>();
            Slots.Add(new HangarSlot(1, 1));
            Slots.Add(new HangarSlot(1, 2));
            Slots.Add(new HangarSlot(1, 3));
            Slots.Add(new HangarSlot(1, 4));
            Slots.Add(new HangarSlot(2, 1));
            Slots.Add(new HangarSlot(2, 2));
            Slots.Add(new HangarSlot(2, 3));
            Slots.Add(new HangarSlot(2, 4));
        }

    }
    public class HangarSlot
    {
        public int Section { get; set; }
        public int Number { get; set; }
        public int IsBroken { get; set; }
        public HangarSlot(int section, int number)
        {
            Section = section;
            Number = number;
        }

    }
    public class DockingBays
    {
        public int PassengerAvailable { get; set; }
        public int CargoAvailable { get; set; }
        public int LuxuryAvailable { get; set; }
    }
    public class CrewQuartersSlots
    {
        public List<CrewQuartersSlot> Slots { get; set; }
        public CrewQuartersSlots()
        {
            Slots = new List<CrewQuartersSlot>();
            Slots.Add(new CrewQuartersSlot(1, 1));
            Slots.Add(new CrewQuartersSlot(1, 2));
            Slots.Add(new CrewQuartersSlot(1, 3));
            Slots.Add(new CrewQuartersSlot(1, 4));
            Slots.Add(new CrewQuartersSlot(2, 1));
            Slots.Add(new CrewQuartersSlot(2, 2));
            Slots.Add(new CrewQuartersSlot(2, 3));
            Slots.Add(new CrewQuartersSlot(2, 4));

        }
    }
    public class CrewQuartersSlot
    {
        public int Section { get; set; }
        public int Number { get; set; }
        public CrewQuartersSlot(int section, int number)
        {
            Section = section;
            Number = number;
        }
    }
    public class Workshops
    {
        public int HasBig { get; set; }
        public int HasMedium { get; set; }
        public int HasSmall1 { get; set; }
        public int HasSmall2 { get; set; }
    }
    public class Laboratories
    {
        public int HasFirst { get; set; }
        public int HasSecond { get; set; }
    }
    public class RadarSystemsSlots
    {
        public List<RadarSystemsSlot> Slots { get; set; }
        public RadarSystemsSlots()
        {
            Slots = new List<RadarSystemsSlot>();
            Slots.Add(new RadarSystemsSlot(1, 1));
            Slots.Add(new RadarSystemsSlot(1, 2));
            Slots.Add(new RadarSystemsSlot(1, 3));
            Slots.Add(new RadarSystemsSlot(1, 4));
            Slots.Add(new RadarSystemsSlot(2, 1));
            Slots.Add(new RadarSystemsSlot(2, 2));
            Slots.Add(new RadarSystemsSlot(2, 3));
            Slots.Add(new RadarSystemsSlot(2, 4));
        }
    }
    public class RadarSystemsSlot
    {
        public int Sector { get; set; }
        public int Number { get; set; }
        public RadarSystemsSlot(int sector, int number)
        {
            Sector = sector;
            Number = number;
        }
    }

    #endregion

    #region Типы модулей

    public enum CrewQuartersTypes
    {
        Officers = 1, //До трех офицеров на модуль
        ShipCrew = 2,  //Команда для космического корабля (это всегда 20 существ)
        StationCrew = 3 //Для размещения персонала базы (до 20 существ)
    }

    public class ReactorType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Power { get; set; }
        public int Resource { get; set; }
        public int Consumption { get; set; }
    }

    #endregion

    #region Модули, которые установлены на базе игрока

    #endregion

}

