using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using BarthaSzabolcs.ModelGUIPool;

namespace BarthaSzabolcs.PooledScrolledList
{
    /// <summary>
    /// Generic pooled, scrollable list.
    /// A derived class with concreate types is needed to use it as a component.
    /// </summary>
    /// <typeparam name="TModel">Type of the data to display.</typeparam>
    /// <typeparam name="TGUI">A compoent capable of displaying <see cref="TModel"/></typeparam>
    public class PooledScrollList<TModel, TGUI> : MonoBehaviour 
        where TGUI : MonoBehaviour, IModelGUI<TModel>
    {
        #region Datamembers

        #region Events

        public delegate void PageChange(int currentPage, int pageCount);

        /// <summary>
        /// Use this to display information about the current page.
        /// </summary>
        public event PageChange OnPageChange;

        #endregion
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
        [SerializeField] private uint poolCapacity;

        #endregion
        #region Public Properties

        /// <summary>
        /// The items displayed.
        /// </summary>
        public IList<TModel> Items
        {
            get
            {
                return _items;
            }
            set
            {
                SetItems(value);
            }
        }

        /// <summary>
        /// Items will call this action when they are clicked.
        /// </summary>
        public Action<TModel> ItemClickAction;

        #endregion
        #region Private Properties

        private int CurrentPage
        {
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPageChange?.Invoke(_currentPage, pageCount);
                }
            }
            get
            {
                return _currentPage;
            }
        }

        #endregion
        #region Backing Fields

        private IList<TModel> _items;
        private int _currentPage;

        #endregion
        #region Private Fields

        private Queue<TGUI> prefabs = new Queue<TGUI>();

        private PrefabPool pool;
        private float spacedItemHeight;

        private int firstItemIndex = -1;
        private int lastItemIndex = -1;

        // Paging
        private int pageSize;
        private int pageCount;

        #endregion

        #endregion


        #region Methods

        #region Public

        /// <summary>
        /// Updates the prefabs position and content.
        ///
        /// <para>
        /// Hook this to <see cref="ScrollRect.onValueChanged"/> to update on scroll.
        /// </para>
        /// 
        /// <para>
        /// Call it with <paramref name="force"/>: true, to refresh the list after a visible model changed.
        /// </para>
        /// 
        /// </summary>
        /// <param name="force">Update the GUIs, even if the same items are visible then in the previous update.</param>
        public void UpdateContent(bool force = false)
        {
            // Set the spacing element, to the height of the unvisible items.
            var culledItems = Mathf.FloorToInt(GetCulledPartHeight() / spacedItemHeight);
            UpdateSpacingElement(culledItems * spacedItemHeight - layoutGroup.spacing);

            // Refresh the number of prefabs used.
            var prefabsNeeded = Mathf.Min(pageSize + 1, Items.Count() - culledItems);
            UpdatePrefabCount(Mathf.Clamp(prefabsNeeded, 0, Items.Count()));

            // Cache the first and last visible index.
            var firstVisibleIndex = Mathf.Clamp(culledItems, 0, Items.Count);
            var lastVisibleIndex = Mathf.Clamp(culledItems + pageSize, 0, Items.Count);

            // If either the last or first index changed, or the update is forced...
            if (force || lastVisibleIndex != lastItemIndex || firstItemIndex != firstVisibleIndex)
            {
                firstItemIndex = firstVisibleIndex;
                lastItemIndex = lastVisibleIndex;

                // Reassaign the models, causing the GUIs to refresh.
                int i = firstVisibleIndex;
                foreach (var item in prefabs)
                {
                    item.Model = Items[i++];
                }

                // Refresh the current page number.
                CurrentPage = CalculateCurrentPage();
            }
        }

        public void JumpToFirstPage()
        {
            JumpToPage(0);
        }

        public void JumpToLastPage()
        {
            JumpToPage(pageCount);
        }

        public void JumpToNextPage()
        {
            JumpToPage(CurrentPage + 1);
        }

        public void JumpToPreviousPage()
        {
            JumpToPage(CurrentPage - 1);
        }

        #endregion
        #region Private

        private void Awake()
        {
            pool = new PrefabPool(prefab, poolTransform, poolCapacity);

            var prefabLayout = prefab.GetComponent<LayoutElement>();
            if (prefabLayout != null)
            {
                spacedItemHeight = prefabLayout.minHeight + layoutGroup.spacing;
            }
        }

        private void SetItems(IList<TModel> value)
        {
            _items = value;

            // Reset the cache of last visible items.
            firstItemIndex = -1;
            lastItemIndex = -1;

            InitPaging();
            UpdateContent();

            // Scroll to the top.
            scrollRect.normalizedPosition = Vector2.up;
        }

        /// <summary>
        /// Adds or removes prefabs to reach <paramref name="count"/> prefab in content.
        /// </summary>
        /// <param name="count"></param>
        private void UpdatePrefabCount(int count)
        {
            while (prefabs.Count > count)
            {
                pool.Return(prefabs.Dequeue().gameObject);
            }

            while (prefabs.Count < count)
            {
                var guiComponent = pool.Get(contentRect).GetComponent<TGUI>();

                // Unsubscribe first, so it will be only subscribed once.
                guiComponent.OnClick -= HandleItemClick;
                guiComponent.OnClick += HandleItemClick;

                prefabs.Enqueue(guiComponent);
            }
        }

        /// <summary>
        /// Updates the blank <see cref="LayoutElement"/> 
        /// to leave <paramref name="space"/> before the prefabs.
        /// </summary>
        /// <param name="space">The space left above the prefabs.</param>
        private void UpdateSpacingElement(float space)
        {
            // When set to ingoreLayout, the layoutGroup will not pad the element.
            spacingElement.ignoreLayout = space <= 0;

            spacingElement.preferredHeight = space;
            spacingElement.minHeight = space;
        }

        /// <summary>
        /// The height(in pixels) of the content, that is culled above the viewport.
        /// </summary>
        private float GetCulledPartHeight()
        {
            // Get the positions of the viewportCorners.
            Vector3[] corners = new Vector3[4];
            viewPortRect.GetLocalCorners(corners);
            
            // Get the top of the viewport rect.
            var viewPortTop = corners[2];

            // Get the top of the content rect.
            contentRect.GetLocalCorners(corners);
            var contentTop = corners[2];

            // Get the position of the viewport's top relative to the content rect.
            var viewPortTopWorld = viewPortRect.TransformPoint(viewPortTop);
            var viewPortTopInContent = contentRect.InverseTransformPoint(viewPortTopWorld);

            // Return the vertical distance.
            return (contentTop - viewPortTopInContent).y;
        }

        private void InitPaging()
        {
            var viewPortHeight = viewPortRect.rect.height;

            // How many prefab should be placed on one page.
            pageSize = (int)(viewPortHeight / spacedItemHeight);

            // How many page is needed, counting the last fractional page as a whole.
            pageCount = Mathf.CeilToInt(Items.Count / (float)pageSize);

            // Set the content height, treating the last page as a whole page.
            var contentHeight = pageCount * pageSize * spacedItemHeight;
            contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);

            _currentPage = 0;
            OnPageChange?.Invoke(_currentPage, pageCount);
        }

        /// <summary>
        /// Returns the currently visible page, based on the content position.
        /// </summary>
        private int CalculateCurrentPage()
        {
            var scrollRectPosition = scrollRect.content.anchoredPosition.y;
            var pageHeight = pageSize * spacedItemHeight;

            return (int)Math.Round(scrollRectPosition / pageHeight, 1) + 1;
        }
 
        /// <summary>
        /// Repositions the content, to show the nth page (index starts with 1).
        /// </summary>
        /// <param name="page"></param>
        private void JumpToPage(int page)
        {
            page = Mathf.Clamp(page - 1, 0, pageCount);

            var pagePosition = page * pageSize * spacedItemHeight;
            scrollRect.content.anchoredPosition = Vector2.up * pagePosition;
        }

        private void HandleItemClick(TModel model)
        {
            ItemClickAction?.Invoke(model);
        }

        #endregion

        #endregion
    }
}