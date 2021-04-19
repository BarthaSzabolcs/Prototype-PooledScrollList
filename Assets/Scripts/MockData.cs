using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BarthaSzabolcs.MarketPlace.Test
{
    [System.Serializable]
    public class MockData
    {
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private IngredientType type;
        [SerializeField] private string name;
        [SerializeField] private string[] adjectives;
        [SerializeField] private int minPrice;
        [SerializeField] private int maxPrice;
        [SerializeField] private int minAmmount;
        [SerializeField] private int maxAmmount;

        public Ingredient Generate()
        {
            return new Ingredient()
            {
                Type = type,
                Icon = sprites[Random.Range(0, sprites.Length)],
                Name = $"{adjectives[Random.Range(0, adjectives.Length)]} {name}",
                Ammount = Random.Range(minAmmount, maxAmmount),
                Price = Random.Range(minPrice, maxPrice)
            };
        }
    }
}
