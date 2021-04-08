using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace BarthaSzabolcs.PooledScrolledList
{
    public class PooledScrollList<TModel, TGUI> : MonoBehaviour 
        where TGUI : MonoBehaviour, IModelGUI<TModel>
    {
        #region Datamembers

        #region Editor Settings

        [Header("Components")]
        [SerializeField] private RectTransform contentRect;
        [SerializeField] private RectTransform viewPortRect;
        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] private LayoutElement spacingElement;
        [SerializeField] private HorizontalOrVerticalLayoutGroup layoutGroup;

        [Header("Pool")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform poolTransform;
        [SerializeField, Range(1, 10)] private uint poolCapacity;

        #endregion
        #region Public Properties

        public IList<TModel> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;

                Init();
            }
        }

        public Action<TModel> OnItemClick;

        #endregion
        #region Backing Fields

        private IList<TModel> _items;

        #endregion
        #region Private Fields

        private Queue<TGUI> prefabs = new Queue<TGUI>();

        private PrefabPool pool;
        private float itemHeight;

        private int firstItemIndex = -1;
        private int lastItemIndex = -1;

        #endregion

        #endregion

        #region Methods

        #region Unity Callbacks

        private void Awake()
        {
            pool = new PrefabPool(prefab, poolTransform, poolCapacity);

            var prefabLayout = prefab.GetComponent<LayoutElement>();
            if (prefabLayout != null)
            {
                itemHeight = prefabLayout.minHeight;
            }
        }

        #endregion

        private void ResizeContent()
        {
            // Sum of the prefabs height.
            float prefabsHeight = itemHeight * Items.Count;

            // Sum of the spaces between the prefabs.
            float spacing = layoutGroup.spacing * Mathf.Clamp(Items.Count - 1, 0, float.MaxValue);

            // Sum of top and bottom padding.
            float padding = layoutGroup.padding.top + layoutGroup.padding.bottom;

            var contentHeight = prefabsHeight + spacing + padding;
            contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
        }

        private void Init()
        {
            firstItemIndex = -1;
            lastItemIndex = -1;

            ResizeContent();
            UpdateContent();

            scrollRect.normalizedPosition = Vector2.up;
        }

        public void UpdateContent()
        {
            var spacedItemHeight = itemHeight + layoutGroup.spacing;

            var culledItems = Mathf.FloorToInt(CulledPartHeight() / spacedItemHeight);

            UpdateSpacingElement(culledItems * spacedItemHeight - layoutGroup.spacing);

            var itemsInViewport = Mathf.CeilToInt(viewPortRect.rect.height / spacedItemHeight);
            var prefabsNeeded = Mathf.Min(itemsInViewport + 1, Items.Count() - culledItems);
            UpdatePrefabCount(Mathf.Clamp(prefabsNeeded, 0, Items.Count()));

            var firstVisibleIndex = Mathf.Clamp(culledItems, 0, Items.Count);
            var lastVisibleIndex = Mathf.Clamp(culledItems + itemsInViewport, 0, Items.Count);

            if (lastVisibleIndex != lastItemIndex || firstItemIndex != firstVisibleIndex)
            {
                firstItemIndex = firstVisibleIndex;
                lastItemIndex = lastVisibleIndex;

                int i = firstVisibleIndex;
                foreach (var item in prefabs)
                {
                    item.Model = Items[i++];
                }
            }
        }

        private void UpdatePrefabCount(int count)
        {
            while (prefabs.Count > count)
            {
                pool.Return(prefabs.Dequeue().gameObject);
            }

            while (prefabs.Count < count)
            {
                var guiComponent = pool.Get(contentRect).GetComponent<TGUI>();

                guiComponent.OnClick -= HandleItemClick;
                guiComponent.OnClick += HandleItemClick;

                prefabs.Enqueue(guiComponent);
            }
        }

        private void UpdateSpacingElement(float height)
        {
            spacingElement.ignoreLayout = height <= 0;
            spacingElement.preferredHeight = height;
            spacingElement.minHeight = height;
        }

        private float CulledPartHeight()
        {
            Vector3[] corners = new Vector3[4];
            viewPortRect.GetLocalCorners(corners);
            var viewPortTop = corners[2];

            contentRect.GetLocalCorners(corners);
            var contentTop = corners[2];

            var viewPortTopWorld = viewPortRect.TransformPoint(viewPortTop);
            var viewPortTopInContent = contentRect.InverseTransformPoint(viewPortTopWorld);

            return (contentTop - viewPortTopInContent).y;
        }

        private void HandleItemClick(TModel model)
        {
            OnItemClick?.Invoke(model);
        }

        #endregion
    }
}