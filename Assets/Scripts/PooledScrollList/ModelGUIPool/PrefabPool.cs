using System.Collections.Generic;

using UnityEngine;

namespace BarthaSzabolcs.ModelGUIPool
{
    /// <summary>
    /// Simple <see cref="GameObject"/> pool.
    /// </summary>
    public class PrefabPool
    {
        #region Datamembers

        #region Private Fields

        private Queue<GameObject> queue = new Queue<GameObject>();
        
        private Transform poolTransform;
        private GameObject prefab;

        #endregion

        #endregion


        #region Methods

        public PrefabPool(GameObject prefab, Transform poolTransform, uint capacity)
        {
            this.poolTransform = poolTransform;
            this.prefab = prefab;

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
                item.transform.SetParent(parent, false);
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
        
        #endregion
    }
}