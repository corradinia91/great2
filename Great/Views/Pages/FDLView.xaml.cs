﻿using Great.ViewModels;
using Microsoft.Practices.ServiceLocation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Great.Views.Pages
{
    /// <summary>
    /// Interaction logic for FDLView.xaml
    /// </summary>
    public partial class FDLView : Page
    {
        private bool runonce = true;
        private FDLViewModel _viewModel { get { return DataContext as FDLViewModel; } }        

        public FDLView()
        {
            InitializeComponent();

            _viewModel.OnFactoryLink += OnFactoryLink;
        }
        
        private void fdlDataGridView_Loaded(object sender, RoutedEventArgs e)
        {
            // hack for correctly show all default selected FDL data on the side panel
            if(runonce)
            {
                if(fdlDataGridView.Items.Count > 0)
                {
                    fdlDataGridView.SelectedIndex = -1;
                    fdlDataGridView.SelectedIndex = 0;
                }

                runonce = false;
            }

            // hack for selecting the first datagrid row by default in a hidden page
            if (fdlDataGridView.SelectedIndex == -1 && fdlDataGridView.Items.Count > 0)
                fdlDataGridView.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_viewModel?.SelectedFDLClone != null)
                _viewModel.SelectedFDLClone.NotifyFDLPropertiesChanged();
        }

        private void FactoryHyperlink_OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;
        }

        private void OnFactoryLink(long factoryId)
        {
            Window wnd = Window.GetWindow(this);
            FactoriesViewModel factoriesVM = ServiceLocator.Current.GetInstance<FactoriesViewModel>();

            if (wnd is MainView && factoriesVM != null)
            {
                MainView mainView = wnd as MainView;
                TabItem factoriesTabItem = mainView.NavigationTabControl.Items.Cast<TabItem>().SingleOrDefault(item => (string)item.Header == "Factories");

                if (factoriesTabItem != null)
                {
                    factoriesVM.SelectedFactory = factoriesVM.Factories.SingleOrDefault(f => f.Id == factoryId);
                    factoriesVM.ZoomOnFactoryRequest(factoriesVM.SelectedFactory);
                    factoriesTabItem.IsSelected = true;
                }                
            }
        }
    }
}
