using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolLed_XuanCuong.Objects
{
    public class LedCollection : CollectionBase
    {
        // Constructor
        public LedCollection()
        {

        }

        public LED this[int index]
        {
            get { return (LED)InnerList[index]; }
        }
        // Get zone by it's zoneID
        public LED GetLed(string ledID)
        {
            foreach (LED led in InnerList)
            {
                if (led.ID == ledID)
                {
                    return led;
                }
            }
            return null;
        }

        // Add
        public void Add(LED led)
        {
            InnerList.Add(led);
        }

        // Remove
        public void Remove(LED led)
        {
            InnerList.Remove(led);
        }
    }
}


