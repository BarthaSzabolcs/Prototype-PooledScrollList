using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using BarthaSzabolcs.PooledScrolledList;

namespace BarthaSzabolcs.MarketPlace
{
    public class MarketPlaceGUI : MonoBehaviour
    {
        #region Datamembers

        #region Enums

        public enum OrderProperty { Name, Ammount, Price };

        #endregion
        #region Editor Settings

        [SerializeField] private IngredientScrollList scrolledList;
        [SerializeField] private MarketPlaceFiltersGUI filters;
        [SerializeField] private BargainPopUp bargainPopUp;
        [SerializeField] private Dropdown orderDropDown;
        [SerializeField] private Text pageNumberText;

        #endregion
        #region Public Properties

        /// <summary>
        /// The <see cref="Ingredient"/>s displayed.
        /// </summary>
        public IList<Ingredient> Ingredients 
        { 
            get
            {
                return _ingredients;
            }
            set
            {
                _ingredients = value;
                SetIngredients(_ingredients);
            }
        }

        #endregion
        #region Backing Fields

        private IList<Ingredient> _ingredients;

        #endregion
        #region Private Fields

        private string filterString = string.Empty;
        private OrderProperty orderProperty = OrderProperty.Name;
        private bool orderAscending = true;

        #endregion

        #endregion


        #region Methods

        #region Public

        /// <summary>
        /// Set the search word to filter the <see cref="Ingredients"/> with.
        /// 
        /// <para>
        /// Hook this to <see cref="InputField.onValueChanged"/>.
        /// </para>
        ///
        /// </summary>
        public void SetFilterString(string filter)
        {
            filterString = filter;

            RefreshScrollList();
        }

        /// <summary>
        /// Set the property to use when ordering <see cref="Ingredients"/>.
        /// 
        /// <para>
        /// Hook this to <see cref="Dropdown.onValueChanged"/>.
        /// </para>
        ///
        /// </summary>
        public void SetOrderProperty()
        {
            if (Enum.IsDefined(typeof(OrderProperty), orderDropDown.value))
            {
                orderProperty = (OrderProperty)orderDropDown.value;
            }
            else
            {
                orderProperty = OrderProperty.Name;
            }

            _ingredients = _ingredients
                .OrderByProperty(orderProperty, orderAscending)
                .ToList();

            RefreshScrollList();
        }

        /// <summary>
        /// Set the order direction used when ordering <see cref="Ingredients"/>.
        /// 
        /// <para>
        /// Hook this to <see cref="Button.onClick"/>.
        /// </para>
        ///
        /// </summary>
        public void SetAscendingingOrder()
        {
            orderAscending = !orderAscending;

            _ingredients = _ingredients
                .OrderByProperty(orderProperty, orderAscending)
                .ToList();

            RefreshScrollList();
        }

        /// <summary>
        /// Force an update of the scroll list.
        /// </summary>
        public void ForceUpdate()
        {
            scrolledList.UpdateContent(force: true);
        }

        #endregion
        #region Private

        private void Awake()
        {
            scrolledList.ItemClickAction += ShowPopUp;
            scrolledList.OnPageChange += RefreshPageNumber;

            filters.OnFiltersChanged += RefreshScrollList;

            bargainPopUp.Hide();
        }

        private void SetIngredients(IList<Ingredient> ingredients)
        {
            _ingredients = ingredients
                .OrderByProperty(orderProperty, orderAscending)
                .ToList();

            filters.SetGroupCounts(_ingredients.CountByType());
            
            RefreshScrollList();
        }

        private void RefreshScrollList()
        {
            scrolledList.Items = Ingredients
                .FilterByType(filters.ActiveFilters)
                .FilterByName(filterString)
                .ToList();
        }

        private void ShowPopUp(Ingredient ingredient)
        {
            bargainPopUp.Show(ingredient);
        }

        private void RefreshPageNumber(int currentPage, int pageCount)
        {
            pageNumberText.text = $"{currentPage}/{Mathf.Max(pageCount, 1)}";
        }
        
        #endregion

        #endregion
    }
}