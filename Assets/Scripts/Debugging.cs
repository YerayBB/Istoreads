using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UtilsUnknown
{
    public static class Debugging<T>
    {
        public static void List(List<T> list)
        {
            StringBuilder line = new StringBuilder("");
            foreach(T item in list)
            {
                line.AppendFormat("{0}, ",item.ToString());
            }
            Debug.Log(line.ToString());
        }
    }
}
