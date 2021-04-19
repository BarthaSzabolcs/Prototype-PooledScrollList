using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using BarthaSzabolcs.ModelGUIPool;
using System;

namespace BarthaSzabolcs.MarketPlace
{
    public class IngredientGUI : MonoBehaviour, IModelGUI<Ingredient>
    {
        #region Datamembers

        #region Events

        public event Action<Ingredient> OnClick;

        #endregion
        #region Editor Settings

        [Header("Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Text nameText;
        [SerializeField] private Text ammountText;
        [SerializeField] private Text priceText;

        #endregion
        #region Public Properties

        /// <summary>
        /// The Model displayed.
        /// 
        /// <para>
        /// Calls <see cref="Refresh"/> on set.
        /// </para>
        /// 
        /// <para>
        /// There is no other bond, you have to call <see cref="Refresh"/> to keep in sync with the changes.
        /// </para>
        /// 
        /// </summary>
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

        #endregion


        #region Methods

        /// <summary>
        /// Refresh the UI to match the <see cref="Model"/>.
        /// </summary>
        public void Refresh()
        {
            if (Model != null)
            {
                iconImage.sprite = Model.Icon;
                nameText.text = Model.Name;
                ammountText.text = Model.Ammount.ToString();
                priceText.text = Model.Price.ToString();
            }
            else
            {
                iconImage.sprite = null;
                nameText.text = nameof(nameText);
                ammountText.text = nameof(ammountText);
                priceText.text = nameof(priceText);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(Model);
        }

        #endregion
    }
}