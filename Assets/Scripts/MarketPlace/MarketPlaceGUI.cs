using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

using BarthaSzabolcs.PooledScrolledList;

namespace BarthaSzabolcs.MarketPlace
{
    public class MarketPlaceGUI : MonoBehaviour
    {
        #region Datamembers

        #region Editor Settings

        [Header("Components")]
        [SerializeField] private PooledScrollList<Ingredient, IngredientGUI> ingredientList;
        [SerializeField] private BargainPopUp bargainPopUp;
        [SerializeField] private TMP_Dropdown orderDropDown;

        [Header("Debug")]
        [SerializeField] private TMP_Text debugText;

        #endregion
        #region Public Properties

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

        private Dictionary<IngredientType, List<Ingredient>> groupedIngredients;

        // Filters
        private string filterGroup = string.Empty;
        private string filterName = string.Empty;
        private string orderProperty = nameof(Ingredient.Name);

        #endregion

        #endregion


        #region Methods

        #region Public

        public void SetFilterName(string filter)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            filterName = filter;

            RefreshScrollList();

            stopwatch.Stop();
            Debug.Log($"Ordered By: {orderProperty}, time: {stopwatch.ElapsedMilliseconds} ms.");
            debugText.text = $"{stopwatch.ElapsedMilliseconds} ms";
        }

        public void SetFilterOrder()
        {
            orderProperty = orderDropDown.options[orderDropDown.value].text;
            
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            foreach (var key in groupedIngredients.Keys.ToArray())
            {
                groupedIngredients[key] = groupedIngredients[key]
                    .OrderBy(orderProperty)
                    .ToList();
            }

            _ingredients = _ingredients
                .OrderBy(orderProperty)
                .ToList();

            stopwatch.Stop();
            Debug.Log($"Ordered By: {orderProperty}, time: {stopwatch.ElapsedMilliseconds} ms.");
            debugText.text = $"{stopwatch.ElapsedMilliseconds} ms";

            RefreshScrollList();
        }

        public void SetFilterGroup(string ingredientType)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            filterGroup = ingredientType;

            RefreshScrollList();
            
            stopwatch.Stop();
            Debug.Log($"Ordered By: {orderProperty}, time: {stopwatch.ElapsedMilliseconds} ms.");
            debugText.text = $"{stopwatch.ElapsedMilliseconds} ms";
        }

        #endregion

        private void Start()
        {
            ingredientList.OnItemClick += ShowPopUp;

            bargainPopUp.OnPurchaseAttempt += HandleItemPurchaseAttempt;
            bargainPopUp.Show(false);
        }

        private void PopulateOrderDropDown()
        {
            orderDropDown.ClearOptions();

            orderDropDown.AddOptions(new List<TMP_Dropdown.OptionData>()
            {
                new TMP_Dropdown.OptionData()
                {
                    text = nameof(Ingredient.Name)
                },
                new TMP_Dropdown.OptionData()
                {
                    text = nameof(Ingredient.Price)
                },
                new TMP_Dropdown.OptionData()
                {
                    text = nameof(Ingredient.Ammount)
                },
            });
        }

        private void SetIngredients(IList<Ingredient> ingredients)
        {
            _ingredients = ingredients
                .OrderBy(orderProperty)
                .ToList();

            groupedIngredients = _ingredients
                .GroupByType();

            RefreshScrollList();
        }

        private void RefreshScrollList()
        {
            ingredientList.Items =
                GetCurrentGroup(filterGroup)
                .FilterName(filterName)
                .ToList();
        }

        private IList<Ingredient> GetCurrentGroup(string groupFilter)
        {
            if (string.IsNullOrEmpty(groupFilter))
            {
                return _ingredients;
            }
            else 
            {
                if (Enum.TryParse<IngredientType>(groupFilter, out var ingredientType) &&
                    groupedIngredients.ContainsKey(ingredientType))
                {
                    return groupedIngredients[ingredientType];
                }
                else
                {
                    return Array.Empty<Ingredient>();
                }
            }
        }

        private void ShowPopUp(Ingredient ingredient)
        {
            bargainPopUp.Show(true);
            bargainPopUp.Model = ingredient;
        }

        private void HandleItemPurchaseAttempt(Ingredient ingredient)
        {
            if (true/*Random.value > 0.5f*/)
            {
                ingredient.Ammount = 0;
            }
            else
            {
                ingredient.Price = Mathf.CeilToInt(ingredient.Price * 1.1f);
            }

            bargainPopUp.Refresh();
        }

        #endregion
    }
}