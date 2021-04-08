using System.Collections.Generic;

using UnityEngine;

namespace BarthaSzabolcs.MarketPlace
{
    public class MarketTest : MonoBehaviour
    {
        #region Datamembers

        #region Editor Settings

        [Header("Components")]
        [SerializeField] private MarketPlaceGUI marketPlaceGUI;
        
        [Header("Parameters")]
        [SerializeField] private int testLength;

        #endregion
        #region Private Fields

        private IList<Ingredient> shopItems;

        #endregion

        #endregion


        private void Start()
        {
            marketPlaceGUI.Ingredients = RandomListOfModels();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                marketPlaceGUI.Ingredients = RandomListOfModels();
            }
        }

        private Ingredient[] RandomListOfModels()
        {
            var array = new Ingredient[testLength];

            for (int i = 0; i < testLength; i++)
            {
                array[i] = RandomModel();
            }

            return array;
        }

        private Ingredient RandomModel()
        {
            return new Ingredient()
            {
                Type = (IngredientType)Random.Range(0, 3),
                Icon = null,
                Name = RandomString(),
                Ammount = Random.Range(1, 101),
                Price = Random.Range(50, 5001)
            }; 
        }

        private string RandomString()
        {
            const string characters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "abcdefghijklmnopqrstuvwxyz" +
            "0123456789";

            int length = Random.Range(3, 9);

            string result = "";

            for (int i = 0; i < length; i++)
            {
                result += characters[Random.Range(0, characters.Length)];
            }

            return result;
        }
    }
}