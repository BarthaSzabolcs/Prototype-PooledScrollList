using System.Collections.Generic;

using UnityEngine;

using BarthaSzabolcs.MarketPlace.Test;

namespace BarthaSzabolcs.MarketPlace
{
    /// <summary>
    /// Placeholder Singleton to test the interaction with <see cref="MarketPlaceGUI"/>.
    /// </summary>
    public class MarketPlace : MonoBehaviour
    {
        #region Datamembers

        #region Editor Settings

        [Header("Components")]
        [SerializeField] private MarketPlaceGUI marketPlaceGUI;

        [Header("Parameters")]
        [SerializeField] private int testLength;

        [SerializeField] private MockData[] mockDatas;

        #endregion
        #region Public Properties

        public static MarketPlace Instance
        {
            get;
            set;
        }

        #endregion
        #region Private Fields

        private List<Ingredient> shopItems;

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Attempt to buy the <see cref="Ingredient"/>.
        /// </summary>
        public void Bargain(Ingredient model, int price, int ammount)
        {
            if (CalculateBargainChance(model, price) == 1)
            {
                model.Ammount -= ammount;

                if (model.Ammount == 0)
                {
                    shopItems.Remove(model);
                    marketPlaceGUI.Ingredients = shopItems;
                }
                else
                {
                    marketPlaceGUI.ForceUpdate();
                }
            }
            else
            {
                model.Price *= 2;
                marketPlaceGUI.ForceUpdate();
            }
        }

        /// <summary>
        /// Returns the chance of a successful bargain.
        /// </summary>
        public float CalculateBargainChance(Ingredient model, int price)
        {
            if (model.Price <= price)
            {
                return 1f;
            }
            else
            {
                return 0;
            }
        }

        #endregion
        #region Private

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        private void Start()
        {
            shopItems = RandomListOfModels();
            marketPlaceGUI.Ingredients = shopItems;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                marketPlaceGUI.Ingredients = RandomListOfModels();
            }
        }
        
        private List<Ingredient> RandomListOfModels()
        {
            var list = new List<Ingredient>
            {
                Capacity = testLength
            };

            for (int i = 0; i < testLength; i++)
            {
                list.Add(RandomModel());
            }

            return list;
        }

        private Ingredient RandomModel()
        {
            return mockDatas[Random.Range(0, mockDatas.Length)].Generate();
        }

        #endregion

        #endregion
    }
}