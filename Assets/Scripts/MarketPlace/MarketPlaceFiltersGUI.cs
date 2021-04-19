using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace BarthaSzabolcs.MarketPlace
{
    /// <summary>
    /// Provides a convinient interface to the Ingredient filters 
    /// and manages the dynamic population of the header buttons.
    /// </summary>
    public class MarketPlaceFiltersGUI : MonoBehaviour
    {
        #region Datamembers

        #region Events

        public delegate void FiltersChanged();

        /// <summary>
        /// Use this to react to filter changes.
        /// </summary>
        public event FiltersChanged OnFiltersChanged;

        #endregion
        #region Editor Settings

        [Header("Header buttons")]
        [SerializeField] private GameObject filterPrefab;
        [SerializeField] private GameObject simplePrefab;

        [SerializeField] private Transform buttonParent;
        [SerializeField] private int maxButtonCount;

        [Header("Popup buttons")]
        [SerializeField] private Transform popupButtonsParent;
        [SerializeField] private Transform popupCanvas;

        #endregion
        #region Public Properties

        /// <summary>
        /// The current state of the filters.
        /// </summary>
        public Dictionary<IngredientType, bool> ActiveFilters { get; set; } = new Dictionary<IngredientType, bool>();

        #endregion
        #region Private Fields

        private Dictionary<IngredientType, IngredientFilterGUI> buttons = new Dictionary<IngredientType, IngredientFilterGUI>();

        #endregion

        #endregion


        #region Methods

        #region Public

        /// <summary>
        /// Toggles On/Off the filter for the given type.
        /// </summary>
        public void ToggleFilter(IngredientType filter)
        {
            if (ActiveFilters.TryGetValue(filter, out var toggle))
            {
                ActiveFilters[filter] = !toggle;

                if (buttons.TryGetValue(filter, out var button))
                {
                    button.On = !toggle;
                }

                OnFiltersChanged?.Invoke();
            }
        }

        /// <summary>
        /// Set the <see cref="IngredientFilterGUI.Count"/> of each filter.
        ///
        /// <para>
        /// If there is no information for a given type, 
        /// the corresponding filter will be set to 0.
        ///</para>    
        ///
        /// </summary>
        public void SetGroupCounts(Dictionary<IngredientType, int> countByType)
        {
            foreach (var button in buttons)
            {
                if (countByType.ContainsKey(button.Key))
                {
                    button.Value.Count = countByType[button.Key];
                }
                else
                {
                    button.Value.Count = 0;
                }
            }
        }

        #endregion
        #region Private

        private void Awake()
        {
            foreach (var type in Enum.GetValues(typeof(IngredientType)).Cast<IngredientType>())
            {
                ActiveFilters.Add(type, false);
            }

            SpawnButtons();
        }

        private void SpawnButtons()
        {
            // The user friendly names of the buttons.
            Dictionary<IngredientType, string> buttonNames = new Dictionary<IngredientType, string>
            {
                { IngredientType.Vegetable, "Zöldség"},
                { IngredientType.Fruit, "Gyümölcs"},
                { IngredientType.Meat, "Hús"},
                { IngredientType.Backyard, "Háztáji"},
                { IngredientType.KitchenGarden, "Konyhakert"}
            };

            var showAllButton = SpawnButton("Mind");
            showAllButton.onClick.AddListener(ResetFilters);

            // Calculate how many button is needed in the header.
            var filters = ActiveFilters.Keys.ToArray();
            var visibleButtons = Mathf.Min(maxButtonCount, filters.Length);

            // Spawn buttons visible in the header.
            for (int i = 0; i < visibleButtons; i++)
            {
                // type is not just convinience, it is needed,
                // so the lambda function to properly copy the parameter.
                var type = filters[i];

                var text = buttonNames.ContainsKey(type) ?
                       buttonNames[type] : type.ToString();

                var button = SpawnFilterGUI(text, buttonParent);

                button.onClick.AddListener(() => ToggleFilter(type));
                buttons.Add(type, button);
            }

            // If there are buttons left...
            if (visibleButtons < filters.Length)
            {
                // Spawn the rest of them on the popup.
                for (int i = visibleButtons; i < filters.Length; i++)
                {
                    var type = filters[i];
                    var text = buttonNames.ContainsKey(type) ?
                           buttonNames[type] : type.ToString();

                    var button = SpawnFilterGUI(text, popupButtonsParent);

                    button.onClick.AddListener(() => ToggleFilter(type));
                    buttons.Add(type, button);
                }

                // Spawn the show more button in the header.
                var showMoreButton = SpawnButton("...");
                showMoreButton.onClick.AddListener(ShowPopup);
            }
        }

        private IngredientFilterGUI SpawnFilterGUI(string buttonName, Transform parent)
        {
            var buttonInstance = Instantiate(filterPrefab, parent);
            var button = buttonInstance.GetComponent<IngredientFilterGUI>();

            button.Text = buttonName;

            return button;
        }

        /// <summary>
        /// Spawn a button in the header and set its name.
        /// </summary>
        private Button SpawnButton(string buttonName)
        {
            var buttonInstance = Instantiate(simplePrefab, buttonParent);
            var button = buttonInstance.GetComponent<Button>();

            var textTransform = buttonInstance.transform.GetChild(0);
            var textComponent = textTransform.GetComponent<Text>();
            textComponent.text = buttonName;

            return button;
        }

        private void ResetFilters()
        {
            foreach (var key in ActiveFilters.Keys.ToList())
            {
                ActiveFilters[key] = false;
            }

            foreach (var button in buttons)
            {
                button.Value.On = false;
            }

            OnFiltersChanged?.Invoke();
        }

        private void ShowPopup()
        {
            popupCanvas.gameObject.SetActive(true);
        }

        #endregion

        #endregion
    }
}