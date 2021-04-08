using System;

namespace BarthaSzabolcs.PooledScrolledList
{
    public interface IModelGUI<TModel>
    {
        TModel Model { get; set; }

        delegate void Click(TModel model);
        event Click OnClick;
    }
}