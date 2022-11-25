namespace com.wao.core
{
    using System;
    using System.Linq;
    using System.Collections.Concurrent;

    public class FactoryManager : MonobehaviourSingleton<FactoryManager>
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<MonoProduct>> _pools = new ConcurrentDictionary<string, ConcurrentQueue<MonoProduct>>();
        private readonly ConcurrentDictionary<string, int> _numberOfProductCreate = new ConcurrentDictionary<string, int>();
        private readonly ConcurrentDictionary<string, MonoProduct> _sample = new ConcurrentDictionary<string, MonoProduct>();
        private readonly ConcurrentDictionary<string, string> _pathToId = new ConcurrentDictionary<string, string>();
        public void GetObjectFromPool<T>(string path, Utility.AssetFrom from = Utility.AssetFrom.Resources, Action<T> oncomplete = null) where T : MonoProduct
        {

            if (!string.IsNullOrEmpty(path))
            {
                string id = string.Empty;
                if (_pathToId.ContainsKey(path))
                {
                    id = _pathToId[path];
                }
                if (!_numberOfProductCreate.ContainsKey(id))
                {
                    _numberOfProductCreate.TryAdd(id, 1);
                }
                else
                {
                    _numberOfProductCreate[id]++;
                }
                if (!string.IsNullOrEmpty(id) && _pools.ContainsKey(id))
                {
                    var smallPool = _pools[id];
                    if (smallPool.Count > 0)
                    {
                        MonoProduct res = null;
                        smallPool.TryDequeue(out res);
                        if (res != null && res.gameObject != null)
                        {
                            res.gameObject.SetActive(true);
                            oncomplete?.Invoke(res as T);
                            return;
                        }
                    }
                    else
                    {
                        if (_sample.ContainsKey(id))
                        {
                            var proc = Instantiate(_sample[id]);
                            oncomplete?.Invoke(proc as T);
                            return;
                        }
                    }
                }
                Utility.AssetManager.Instance.Load<T>(path, from).SetOnComplete((product) =>
                 {
                     if (product != null)
                     {
                         var proc = Instantiate(product);
                         oncomplete?.Invoke(proc);
                         var id = proc.productId;
                         if (!string.IsNullOrEmpty(id) && !_pools.ContainsKey(id))
                         {
                             _pools.TryAdd(id, new ConcurrentQueue<MonoProduct>());
                             _sample.TryAdd(id, product);
                         }
                         if (!_pathToId.ContainsKey(path))
                         {
                             _pathToId.TryAdd(path, id);
                         }

                     }
                     else
                     {
                         UnityEngine.Debug.LogError("Couldn't load resource from " + path);
                     }
                 }).Excute();
            }

        }
        public T GetObject<T>(T monoProduct) where T : MonoProduct
        {
            var id = monoProduct.productId;
            MonoProduct res = null;
            if (!string.IsNullOrEmpty(id))
            {
                if (!_pools.ContainsKey(id))
                {
                    _pools.TryAdd(id, new ConcurrentQueue<MonoProduct>());
                }
                var smallPool = _pools[id];

                if (smallPool.Count > 0)
                {

                    smallPool.TryDequeue(out res);
                    return res as T;
                }
            }
            res = Instantiate(monoProduct);
            return res as T;
        }
        public void ReturnProduct<T>(T product, string id) where T : MonoProduct
        {
            if (!_pools.ContainsKey(id))
            {
                _pools.TryAdd(id, new ConcurrentQueue<MonoProduct>());
            }
            var smallPool = _pools[id];
            smallPool.Enqueue(product);
            if (_numberOfProductCreate.ContainsKey(id))
            {
                _numberOfProductCreate[id]--;
                if (_numberOfProductCreate[id] == 0)
                {
                    //release memories;
                    Clear(id);
                }
            }

        }

        public void RemoveProduct<T>(T product, string id) where T : MonoProduct
        {
            if (!_pools.ContainsKey(id))
            {
                _pools.TryAdd(id, new ConcurrentQueue<MonoProduct>());
            }
            var smallPool = _pools[id];
            _pools.TryRemove(id, out smallPool);
            smallPool = new ConcurrentQueue<MonoProduct>(smallPool.Where(x => x != product));
            _pools.TryAdd(id, smallPool);
        }
        public void Register<T>(T product, string id) where T : MonoProduct
        {
            if (!_pools.ContainsKey(id))
            {
                _pools.TryAdd(id, new ConcurrentQueue<MonoProduct>());
            }
            var smallPool = _pools[id];
            smallPool.Enqueue(product);
        }

        public void Clear(string id)
        {
            if (_pools.ContainsKey(id))
            {
                var smallPool = _pools[id];
                while (smallPool.Count > 0)
                {
                    MonoProduct obj = null;
                    smallPool.TryDequeue(out obj);
                    Destroy(obj.gameObject);
                }
                if (_sample.ContainsKey(id))
                {
                    Destroy(_sample[id]);
                }
            }
        }

        public void CleanAllCreatedObjects()
        {
            foreach (var id in _pools.Keys)
            {
                var queue = _pools[id];
                while (queue.Count > 0)
                {
                    MonoProduct p = null;
                    queue.TryDequeue(out p);
                    if (p != null && p.gameObject != null)
                    {
                        Destroy(p.gameObject);
                    }
                }
            }
            _pools.Clear();
            _sample.Clear();
        }

    }

}