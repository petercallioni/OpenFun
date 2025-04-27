using OpenFun.PageModels;

namespace OpenFun.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage(HomePageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
        private void CollectionView_SizeChanged(object sender, EventArgs e)
        {
            if (sender is CollectionView collectionView && collectionView.Width > 0)
            {
                ((GridItemsLayout)collectionView.ItemsLayout).Span = 3;
            }
        }
    }
}