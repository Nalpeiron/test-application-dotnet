using System.Collections.Generic;

namespace ZentitleOnPremDemo.Models
{
    public class ListItemCollectionView<T> : List<ListItem<T>>
    {
        public ListItem<T>? CurrentItem { get; set; } 
    }
}
