using UnityEngine;

namespace BarthaSzabolcs.MarketPlace
{
    /// <summary>
    /// Placeholder class, to represent a marketplace item.
    /// </summary>
    public class Ingredient
    {
        public IngredientType Type { get; set; }

        public Sprite Icon { get; set; }

        public string Name { get; set; }

        public int Ammount { get; set; }

        public int Price { get; set; }
    }
}