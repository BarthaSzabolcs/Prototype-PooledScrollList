using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

using BarthaSzabolcs.PooledScrolledList;

namespace BarthaSzabolcs.MarketPlace
{
    public class IngredientGUI : MonoBehaviour, IPointerClickHandler, IModelGUI<Ingredient>
    {
        #region Datamembers

        #region Events

        public event IModelGUI<Ingredient>.Click OnClick;

        #endregion
        #region Editor Settings

        [Header("Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI ammountText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI typeText;

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
                if (_model != value)
                {
                    _model = value;
                    Refresh();
                }
            }
        }

        #endregion
        #region Backing Fields

        private Ingredient _model;

        #endregion

        #endregion


        #region Methods

        public void Refresh()
        {
            if (Model != null)
            {
                iconImage.sprite = Model.Icon;
                nameText.text = Model.Name;
                ammountText.text = Model.Ammount.ToString();
                priceText.text = Model.Price.ToString();
                typeText.text = Model.Type.ToString();
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
            Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
            OnClick?.Invoke(Model);
        }

        #endregion
    }
}