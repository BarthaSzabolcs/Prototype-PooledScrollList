using UnityEngine;

using TMPro;

namespace BarthaSzabolcs.MarketPlace
{
    public class BargainPopUp : MonoBehaviour
    {
        #region Events

        public delegate void PurchaseAttempt(Ingredient ingredient);
        public event PurchaseAttempt OnPurchaseAttempt;

        #endregion
        #region Datamembers

        #region Editor Settings

        [Header("Components")]
        [SerializeField] private GameObject canvas;
        [SerializeField] private TextMeshProUGUI ingredientText;
        [SerializeField] private TextMeshProUGUI ammountText;
        [SerializeField] private TextMeshProUGUI priceText;

        #endregion
        #region Public Properties

        public Ingredient Model
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


        #endregion

        #endregion


        #region Methods

        public void Refresh()
        {
            if (Model != null)
            {
                ingredientText.text = Model.Name;
                ammountText.text = Model.Ammount.ToString();
                priceText.text = Model.Price.ToString();
            }
            else
            {
                ingredientText.text = nameof(Ingredient.Name);
                ammountText.text = nameof(Ingredient.Ammount);
                priceText.text = nameof(Ingredient.Price);
            }
        }

        public void HandleBuyClick()
        {
            OnPurchaseAttempt?.Invoke(Model);
        }

        public void Show(bool show)
        {
            canvas.SetActive(show);
        }

        #endregion
    }
}