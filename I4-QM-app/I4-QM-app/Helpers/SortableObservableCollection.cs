using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace I4_QM_app.Helpers
{
    // https://stackoverflow.com/questions/19112922/sort-observablecollectionstring-through-c-sharp
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        public Func<T, object> SortingSelector { get; set; }
        public bool Descending { get; set; }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (SortingSelector == null
                || e.Action == NotifyCollectionChangedAction.Remove
                || e.Action == NotifyCollectionChangedAction.Reset)
                return;

            var query = this
              .Select((item, index) => (Item: item, Index: index));
            query = Descending
              ? query.OrderBy(tuple => SortingSelector(tuple.Item))
              : query.OrderByDescending(tuple => SortingSelector(tuple.Item));

            var map = query.Select((tuple, index) => (OldIndex: tuple.Index, NewIndex: index))
             .Where(o => o.OldIndex != o.NewIndex);

            using (var enumerator = map.GetEnumerator())
                if (enumerator.MoveNext())
                    Move(enumerator.Current.OldIndex, enumerator.Current.NewIndex);


        }
    }
}
