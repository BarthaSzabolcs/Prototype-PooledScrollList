using System;
using UnityEngine.EventSystems;

namespace BarthaSzabolcs.ModelGUIPool
{
    /// <summary>
    /// Represents a GUI item, capable of displaying <typeparamref name="TModel"/>.
    /// </summary>
    /// <typeparam name="TModel">The model displayed.</typeparam>
    public interface IModelGUI<TModel> : IPointerClickHandler
    {
        TModel Model { get; set; }

        event Action<TModel> OnClick;
    }
}