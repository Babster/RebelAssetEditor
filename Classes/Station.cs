using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Station
{

    public class ReactorSlots
    {
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
        public int Size { get; set; }

        public ReactorSlot(int sectorNumber, int number, int size)
        {
            SectorNumber = sectorNumber;
            Number = number;
            Size = size;
        }

    }
}

