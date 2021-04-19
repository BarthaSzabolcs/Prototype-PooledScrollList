using UnityEngine;
using UnityEngine.UI;

namespace BarthaSzabolcs.MarketPlace
{
    /// <summary>
    /// A button functioning as a toggle, also displaying information about the filter for the user.
    /// </summary>
    public class IngredientFilterGUI : Button
    {
        #region Datamembers

        #region Public Properties

        /// <summary>
        /// Is the filter pressed in.
        /// </summary>
        public bool On
        {
            get
            {
                return _on;
            }
            set
            {
                if (_on != value)
                {
                    _on = value;
                    SetOn(_on);
                }
            }
        }

        /// <summary>
        /// Name of the filter.
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    SetText(_text, _count);
                }
            }
        }

        /// <summary>
        /// Number of items for the filter.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (_count != value)
                {
                    _count = value;
                    SetText(_text, _count);
                }
            }
        }

        #endregion
        #region Backing Fields

        private bool _on;
        private string _text;
        private int _count;

        #endregion

        #endregion


        #region Methods

        #region Private

        protected override void Start()
        {
            base.Start();

            SetOn(false);
        }

        private void SetOn(bool value)
        {
            if (value)
            {
                image.color = Color.gray;
            }
            else
            {
                image.color = Color.white;
            }
        }

        private void SetText(string name, int count)
        {
            var textTransform = transform.GetChild(0);
            var textComponent = textTransform.GetComponent<Text>();
            textComponent.text = $"({count}) {name}";
        }

        #endregion

        #endregion
    }
}