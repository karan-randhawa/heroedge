﻿using HeroEdge.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HeroEdge
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Character> MarvelCharacters { get; set; }
        public ObservableCollection<ComicBook> MarvelComics { get; set; }
        public MainPage()
        {
            this.InitializeComponent();

            MarvelCharacters = new ObservableCollection<Character>();
            MarvelComics = new ObservableCollection<ComicBook>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(
                new Uri("ms-appx:///VoiceCommandDictionary.xml"));
            await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.
                InstallCommandDefinitionsFromStorageFileAsync(storageFile);

            RefreshAsync();
        }

        public async void RefreshAsync()
        {
            APICallProgressRing.IsActive = true;
            APICallProgressRing.Visibility = Visibility.Visible;

            MarvelCharacters.Clear();
            while (MarvelCharacters.Count < 10)
            {
                Task t = MarvelFacade.PopulateMarvelCharactersAsync(MarvelCharacters);
                await t;
            }

            APICallProgressRing.IsActive = false;
            APICallProgressRing.Visibility = Visibility.Collapsed;
        }

        private async void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            APICallProgressRing.IsActive = true;
            APICallProgressRing.Visibility = Visibility.Visible;

            ComicDetailImage.Source = null;
            ComicDetailNameTextBlock.Text = "";
            ComicDetailDescriptionTextBlock.Text = "";

            var selectedCharacter = (Character)e.ClickedItem;

            DetailNameTextBlock.Text = selectedCharacter.name;
            DetailDescriptionTextBlock.Text = selectedCharacter.description;

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedCharacter.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;

            DetailImage.Source = largeImage;

            MarvelComics.Clear();

            //while (MarvelComics.Count < 10)
            //{
            //    Task t = MarvelFacade.PopulateMarvelComicsAsync(selectedCharacter.id, MarvelComics);
            //    await t;
            //}

            await MarvelFacade.PopulateMarvelComicsAsync(selectedCharacter.id, MarvelComics);

            APICallProgressRing.IsActive = false;
            APICallProgressRing.Visibility = Visibility.Collapsed;
        }

        private void ComicsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedComic = (ComicBook)e.ClickedItem;

            ComicDetailNameTextBlock.Text = selectedComic.title;
            ComicDetailDescriptionTextBlock.Text = (selectedComic.description == null) ? "No Description, Sorry!" : selectedComic.description;

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedComic.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;

            ComicDetailImage.Source = largeImage;
        }
    }
}
