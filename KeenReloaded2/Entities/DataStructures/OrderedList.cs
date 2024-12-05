using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.DataStructures
{
    public class OrderedList<T> : List<T>
    {
        private readonly Func<T, T, int> _comparatorFunction;
        public OrderedList(Func<T, T, int> comparatorFunction)
        {
            _comparatorFunction = comparatorFunction;
        }

        public static OrderedList<T> FromEnumerable(IEnumerable<T> items, Func<T, T, int> comparatorFunction, bool ascending)
        {
            var list = new OrderedList<T>(comparatorFunction);
            if (items == null || !items.Any())
                return list;


            foreach (var item in items)
            {
                if (ascending)
                {
                    list.InsertAscending(item);
                }
                else
                {
                    list.InsertDescending(item);
                }
            }

            return list;
        }

        public void InsertAscending(T item)
        {
            this.InsertOrdered(item, true);
        }

        public void InsertDescending(T item)
        {
            this.InsertOrdered(item, false);
        }

        private void InsertOrdered(T item, bool ascending)
        {
            int insertIndex = -1;
            if (ascending)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    int result = _comparatorFunction(item, this[i]);
                    if (result == -1)
                    {
                        insertIndex = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.Count; i++)
                {
                    int result = _comparatorFunction(item, this[i]);
                    if (result == 1)
                    {
                        insertIndex = i + 1;
                        break;
                    }
                }
            }

            if (insertIndex == -1 || insertIndex >= this.Count)
            {
                this.Add(item);
            }
            else
            {
                this.Insert(insertIndex, item);
            }
        }
    }
}
