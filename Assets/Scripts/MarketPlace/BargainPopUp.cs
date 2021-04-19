using UnityEngine;
using UnityEngine.UI;

namespace BarthaSzabolcs.MarketPlace
{
    public class BargainPopUp : MonoBehaviour
    {
        #region Datamembers

        #region Editor Settings

        [Header("Components")]
        [SerializeField] private GameObject canvas;
        [SerializeField] private Text ingredientNameText;
        [SerializeField] private Text bargainChanceText;
        [SerializeField] private InputField priceInput;
        [SerializeField] private InputField ammountInput;

        #endregion
        #region Private Properties

        private Ingredient Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                Refresh();
            }
        }

        #endregion
        #region Backing Fields

        private Ingredient _model;

        #endregion
        #region Private Fields

        private int ammount;
        private int price;

        #endregion

        #endregion


        #region Methods

        #region Public

        /// <summary>
        /// Show the popup to bargain for <paramref name="ingredient"/>.
        /// </summary>
        public void Show(Ingredient ingredient)
        {
            Model = ingredient;
            canvas.SetActive(true);
        }

        /// <summary>
        /// Hide the popup.
        /// </summary>
        public void Hide()
        {
            canvas.SetActive(false);
        }

        /// <summary>
        /// Set the item price to bargain for.
        /// 
        /// <para>
        /// Hook this to <see cref="InputField.onValueChanged"/>.
        /// </para>
        ///
        /// </summary>
        public void SetPrice(string value)
        {
            price = ParseInput(value);

            RefreshBargainChance();
        }

        /// <summary>
        /// Set the ammount to bargain for.
        /// 
        /// <para>
        /// Hook this to <see cref="InputField.onValueChanged"/>.
        /// </para>
        ///
        /// </summary>
        public void SetAmmount(string value)
        {
            ammount = ParseInput(value);

            RefreshBargainChance();
        }

        /// <summary>
        /// Start the interaction with the <see cref="MarketPlace"/>.
        /// </summary>
        public void Submit()
        {
            MarketPlace.Instance.Bargain(Model, price, ammount);

            if (Model.Ammount == 0)
            {
                Model = null;
                Hide();
            }
            else
            {
                Refresh();
            }
        }

        #endregion
        #region Private
        
        private int ParseInput(string value)
        {
            if (int.TryParse(value, out int number))
            {
                return Mathf.Max(number, 0);
            }
            else
            {
                return 0;
            }
        }

        private void Refresh()
        {
            if (Model != null)
            {
                ingredientNameText.text = Model.Name;

                price = Model.Price;
                priceInput.text = price.ToString();

                ammount = Model.Ammount;
                ammountInput.text = ammount.ToString();

                RefreshBargainChance();
            }
            else
            {
                ingredientNameText.text = nameof(Ingredient.Name);
            }
        }

        private void RefreshBargainChance()
        {
            var bargainChance = MarketPlace.Instance.CalculateBargainChance(Model, price);
            bargainChanceText.text = $"{bargainChance * 100}%";
        }

        #endregion

        #endregion
    }
}