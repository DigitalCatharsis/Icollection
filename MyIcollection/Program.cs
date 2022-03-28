using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace test
{


    //Написать контейнер, который поддерживает добавление эллементов конец
    //-=- обращение эллемента по индексу, удаление по индексу
    //Insert в указанную позицию
    //Сортировка
    //Вся херня должна быть iterable?  (чтобы работали foreach, as)
    //должна иметь реализованый интерфейс icollection

    //
    //iterable? 
    //контейнер
    //Где будут жить обхекты в контейнере?
    // IEnumerator
    // IEnumerable
    //Загуглить "геттер члена класса C#",  сделать _length только getter

    //универсальный тип = объекты

    //   Незя:
    // наследоваться от list



    //выделить память
    //добавить эл (что делать если не хватает памяти надо еще)

    //очистить все


    public class MyFuckinCollection<T> : ICollection<T>
    {
        // The generic enumerator obtained from IEnumerator<T>
        // by GetEnumerator can also be used with the non-generic IEnumerator.
        // To avoid a naming conflict, the non-generic IEnumerable method
        // is explicitly implemented.

        // The inner collection to store objects.
        private T[] _myCollection;

        private int _length = 0;        //Кол-во сузествующих эллементов
        private int _oldCapacity;  //Предыдущее кол-во слотов для увеличения размера        
        private int _capacity = 10;       //Текущее кол-во слотов для увеличения размера
        public int Count => _length; //Get - Set как защита от Null??????????



        public MyFuckinCollection()
        {
            // иницилизировать _myStorage на куче 
            _myCollection = new T[_capacity];
        }

        public void Add(T item)   //Добавить эллемент в конец списка
        {
            if (_length == _capacity)
            {
                Extend();
                _myCollection[_capacity] = item;
            }
            else
            {
                _myCollection[_length] = item;
            }
            _length++;
        }

        public void Add(T[] items)    //insert range
        {
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }


        private void Extend()
        {
            _oldCapacity = _capacity;
            _capacity = (_oldCapacity * 3) / 2 + 1;

            var _myTempStorage = new T[_capacity];

            for (var i = 0; i < _length; i++)
            {
                _myTempStorage[i] = _myCollection[i];
            }

            _myCollection = _myTempStorage;

        }

        public void Clear()
        {
            for (int i = 0; i < _length; i++)
            {
                _myCollection[i] = default;   //Правильно?
            }
        }


        // Determines if an item is in the collection
        // by using the FuckinObjectSameDimensions equality comparer.
        public bool Contains(T item)    //Поиск эллемента в коллекции
        {

            foreach (T item2 in this)
            {
                // Equality defined by the FuckinObject
                // class's implmentation of IEquatable<T>.
                if (item2.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)   //from my _collection to array starting from array.index
        {
            if (array == null)
                throw new ArgumentNullException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (_length > array.Length - arrayIndex + 1) //Сравнивает, что длинна масивов одинаковыя
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            for (int i = 0; i < _length; i++)  //_length() в оригинале без () очему?
            {
                array[i + arrayIndex] = _myCollection[i];
            }
        }

        public bool IsReadOnly => false;  //инфа для юзверя, что коллекцию можно модифицировать 

        public bool Remove(T item)    //Удаление эллемента по Значению (со сдвигом влево)
        {
            for (int i = _length - 1; i > -1; i--)
            {
                if (_myCollection[i].Equals(item))
                {
                    for (int y = i; y < _length - 1; y++)
                    {
                        _myCollection[y] = _myCollection[y + 1];
                    }
                    _myCollection[_length - 1] = default;
                    break;
                }

            }
            return true;
        }


        public bool RemoveByIndex(int index)    //Удаление эллемента по Значению (со сдвигом влево)
        {

            for (int y = index; y < _length - 1; y++)
            {
                _myCollection[y] = _myCollection[y + 1];
            }
            _myCollection[_length - 1] = default;

            return true;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new FuckinObjEnumerator<T>(this);
            //Надо пошагово обсудить последовательность создания и работы энумератора.
            //Что за чем вызывается, делает и зачем.
        }

        IEnumerator IEnumerable.GetEnumerator()
        {

            return new FuckinObjEnumerator<T>(this);
        }

        // Adds an index to the collection.
        public T this[int index]
        {
            get { return (T)_myCollection[index]; }
            set { _myCollection[index] = value; }
        }

    }

    // Defines the enumerator for the Ts collection.
    // (Some prefer this class nested in the collection class.)
    public class FuckinObjEnumerator<T> : IEnumerator<T>
    {
        private MyFuckinCollection<T> _collection;
        private int curIndex;
        private T curObject;

        public FuckinObjEnumerator(MyFuckinCollection<T> collection)
        {
            _collection = collection;
            curIndex = -1;
            curObject = default(T);

        }

        public bool MoveNext()
        {
            //Avoids going beyond the end of the collection.
            if (++curIndex >= _collection.Count)
            {
                return false;
            }
            else
            {
                // Set current T to next item in collection.
                curObject = _collection[curIndex];
            }
            return true;
        }

        public void Reset()     //реализовали, потому что требует интерфейс
        {
            curIndex = -1;
        }


        public void Dispose()     //ут нихуя не происходит
        {

        }



        //Реализации для двух ебаных интерфейсов, т.к IEnumerator<T> наследуется от IEnumerator (Возвращает object)

        public T Current
        {
            get { return curObject; }
        }

        object IEnumerator.Current => Current; // 


    }




    internal class Program
    {
        static void Main(string[] args)
        {
            var temp = new MyFuckinCollection<int>();
            for (var i = 0; i < 10; i++)
            {
                temp.Add(i);
            }

            Console.WriteLine("#########################################################################################");

            var temp2 = new MyFuckinCollection<int>();
            for (var i = 0; i < 10; i++)
            {
                temp2.Add(i + 999);
            }





            //temp[5] = int.Parse("1245");
            //temp.Clear();
            Console.WriteLine(temp.Contains(1245));
            Console.WriteLine("#########################################################################################");
            //temp.Add(temp2);   //???????????????????????

            //temp.Remove(0);
            temp.RemoveByIndex(10);


            foreach (var item in temp)
            {
                Console.WriteLine(item);
            }


            //Console.WriteLine("#########################################################################################");
            //foreach (var item in temp2)
            //{
            //    Console.WriteLine(item);
            //}
            //Console.WriteLine("#########################################################################################");


        }
    }



}
