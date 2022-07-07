using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace I4_QM_app.Helpers
{
    // https://stackoverflow.com/questions/19112922/sort-observablecollectionstring-through-c-sharp

    /// <summary>
    /// Extends an ObservableCollection with sorting funtionality.
    /// </summary>
    /// <typeparam name="T">Generic parameter of collection.</typeparam>
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Gets or sets the collection's sorting predicate.
        /// </summary>
        public Func<T, object> SortingSelector { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order of sorting is ascending or descending.
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// Handles sorting when collection changes.
        /// </summary>
        /// <param name="e">NotifyCollectionChangedEventArgs, notifying when collection changes.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (SortingSelector == null
                || e.Action == NotifyCollectionChangedAction.Remove
                || e.Action == NotifyCollectionChangedAction.Reset)
            {
                return;
            }

            var query = this
              .Select((item, index) => (Item: item, Index: index));
            query = Descending
              ? query.OrderBy(tuple => SortingSelector(tuple.Item))
              : query.OrderByDescending(tuple => SortingSelector(tuple.Item));

            var map = query.Select((tuple, index) => (OldIndex: tuple.Index, NewIndex: index))
             .Where(o => o.OldIndex != o.NewIndex);

            using (var enumerator = map.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    Move(enumerator.Current.OldIndex, enumerator.Current.NewIndex);
                }
            }
        }
    }
}
