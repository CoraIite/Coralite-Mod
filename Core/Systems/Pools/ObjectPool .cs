using System;
using System.Collections;

namespace Coralite.Core.Systems.Pools
{
    public sealed class ObjectPool
    {
        private int _nCapacity;
        private int _nCurrentSize;
        private Hashtable _listObjects;
        private ArrayList _listFreeIndex;
        private ArrayList _listUsingIndex;
        private Type _typeObject;
        private object _objCreateParam;

        public ObjectPool(Type type, object create_param, int init_size, int capacity)
        {
            if (init_size < 0 || capacity < 1 || init_size > capacity)
            {
                throw new Exception("Invalid parameter!");
            }

            NCapacity = capacity;
            ListObjects = new Hashtable(capacity);
            ListFreeIndex = new ArrayList(capacity);
            ListUsingIndex = new ArrayList(capacity);
            TypeObject = type;
            ObjCreateParam = create_param;

            for (int i = 0; i < init_size; i++)
            {
                PoolItem pitem = new(type, create_param);
                ListObjects.Add(pitem.InnerObjectHashcode, pitem);
                ListFreeIndex.Add(pitem.InnerObjectHashcode);
            }

            NCurrentSize = ListObjects.Count;
        }

        public void Release()
        {
            lock (this)
            {
                foreach (DictionaryEntry de in ListObjects)
                {
                    ((PoolItem)de.Value).Release();
                }
                ListObjects.Clear();
                ListFreeIndex.Clear();
                ListUsingIndex.Clear();
            }
        }

        public int CurrentSize
        {
            get { return NCurrentSize; }
        }

        public int ActiveCount
        {
            get { return ListUsingIndex.Count; }
        }

        public int NCapacity { get => _nCapacity; set => _nCapacity = value; }
        public int NCurrentSize { get => _nCurrentSize; set => _nCurrentSize = value; }
        public Hashtable ListObjects { get => _listObjects; set => _listObjects = value; }
        public ArrayList ListFreeIndex { get => _listFreeIndex; set => _listFreeIndex = value; }
        public ArrayList ListUsingIndex { get => _listUsingIndex; set => _listUsingIndex = value; }
        public Type TypeObject { get => _typeObject; set => _typeObject = value; }
        public object ObjCreateParam { get => _objCreateParam; set => _objCreateParam = value; }

        public object GetOne()
        {
            lock (this)
            {
                if (ListFreeIndex.Count == 0)
                {
                    if (NCurrentSize == NCapacity)
                    {
                        return null;
                    }
                    PoolItem pnewitem = new(TypeObject, ObjCreateParam);
                    ListObjects.Add(pnewitem.InnerObjectHashcode, pnewitem);
                    ListFreeIndex.Add(pnewitem.InnerObjectHashcode);
                    NCurrentSize++;
                }

                int nFreeIndex = (int)ListFreeIndex[0];
                PoolItem pitem = (PoolItem)ListObjects[nFreeIndex];
                ListFreeIndex.RemoveAt(0);
                ListUsingIndex.Add(nFreeIndex);

                if (!pitem.IsValidate)
                {
                    pitem.Recreate();
                }

                pitem.Using = true;
                return pitem.InnerObject;
            }
        }

        public void FreeObject(object obj)
        {
            lock (this)
            {
                int key = obj.GetHashCode();
                if (ListObjects.ContainsKey(key))
                {
                    PoolItem item = (PoolItem)ListObjects[key];
                    item.Using = false;
                    ListUsingIndex.Remove(key);
                    ListFreeIndex.Add(key);
                }
            }
        }

        public int DecreaseSize(int size)
        {
            int nDecrease = size;
            lock (this)
            {
                if (nDecrease <= 0)
                {
                    return 0;
                }
                if (nDecrease > ListFreeIndex.Count)
                {
                    nDecrease = ListFreeIndex.Count;
                }

                for (int i = 0; i < nDecrease; i++)
                {
                    ListObjects.Remove(ListFreeIndex[i]);
                }

                ListFreeIndex.Clear();
                ListUsingIndex.Clear();

                foreach (DictionaryEntry de in ListObjects)
                {
                    PoolItem pitem = (PoolItem)de.Value;
                    if (pitem.Using)
                    {
                        ListUsingIndex.Add(pitem.InnerObjectHashcode);
                    }
                    else
                    {
                        ListFreeIndex.Add(pitem.InnerObjectHashcode);
                    }
                }
            }
            NCurrentSize -= nDecrease;
            return nDecrease;
        }
    }

    public class PoolItem
    {
        private IDynamicObject _object;
        private bool _bUsing;
        private Type _type;
        private object _CreateParam;

        public PoolItem(Type type, object param)
        {
            _type = type;
            _CreateParam = param;
            Create();
        }

        private void Create()
        {
            _bUsing = false;
            _object = (IDynamicObject)Activator.CreateInstance(_type);
            _object.Create(_CreateParam);
        }

        public void Recreate()
        {
            _object.Release();
            Create();
        }

        public void Release()
        {
            _object.Release();
        }

        public object InnerObject
        {
            get { return _object.GetInnerObject(); }
        }

        public int InnerObjectHashcode
        {
            get { return InnerObject.GetHashCode(); }
        }

        public bool IsValidate
        {
            get { return _object.IsValidate(); }
        }

        public bool Using
        {
            get { return _bUsing; }
            set { _bUsing = value; }
        }
    }

    public interface IDynamicObject
    {
        void Create(object param);
        object GetInnerObject();
        bool IsValidate();
        void Release();
    }
}
