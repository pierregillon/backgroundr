﻿using System.Windows;
using backgroundr.application;
using backgroundr.view.viewmodels;

namespace backgroundr.view
{
    public partial class FlickrAuthenticationDialog : Window
    {
        private readonly FlickrAuthenticationViewModel _viewModel;

        public FlickrAuthenticationDialog(FlickrAuthenticationViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            _viewModel = viewModel;

        }

        public new FlickrPrivateAccess ShowDialog()
        {
            FlickrPrivateAccess result = null;
            _viewModel.Close += privateAccess => {
                result = privateAccess;
                Close();
            };
            _viewModel.Initialize();
            base.ShowDialog();
            return result;
        }
    }
}
