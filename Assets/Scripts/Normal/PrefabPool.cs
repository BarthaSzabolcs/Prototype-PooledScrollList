using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace BarthaSzabolcs.PooledScrolledList
{
    public class PrefabPool
    {
        private Queue<GameObject> queue = new Queue<GameObject>();
        
        private Transform poolTransform;
        private GameObject prefab;

        public PrefabPool(GameObject prefab, Transform poolTransform, uint capacity)
        {
            this.poolTransform = poolTransform;
            this.prefab = prefab;

            Check();

            for (var i = 0; i < capacity; i++)
            {
                var item = GameObject.Instantiate(prefab, poolTransform);
                item.SetActive(false);

                queue.Enqueue(item);
            }
        }

        public GameObject Get(Transform parent)
        {
            if (queue.Count > 0)
            {
                var item = queue.Dequeue();
                item.transform.parent = parent;
                item.gameObject.SetActive(true);

                return item;
            }
            else
            {
                return GameObject.Instantiate(prefab, parent);
            }
        }

        public void Return(GameObject item)
        {
            item.gameObject.SetActive(false);
            item.transform.position = Vector3.zero;
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.SetParent(poolTransform, false);

            queue.Enqueue(item);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void Check()
        {
            Assert.IsNotNull(prefab, $"{nameof(prefab)} is null.");
            Assert.IsNotNull(poolTransform, $"{nameof(poolTransform)} is null.");
        }
    }
}