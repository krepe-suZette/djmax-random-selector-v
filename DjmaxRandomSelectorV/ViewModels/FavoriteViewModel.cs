﻿using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FavoriteViewModel : Screen
    {
        private bool searchesSuggestion;
        private readonly List<string> favorite;
        private readonly List<string> titleList;
        private readonly Action<bool> setUpdated;

        public BindableCollection<string> FavoriteItems { get; set; }
        public BindableCollection<string> TitleSuggestions { get; set; }

        public FavoriteViewModel(List<string> favorite, List<string> titleList, Action<bool> setUpdated)
        {
            searchesSuggestion = true;
            this.favorite = favorite;
            this.titleList = titleList;
            this.setUpdated = setUpdated;

            FavoriteItems = new BindableCollection<string>(favorite);

            TitleSuggestions = new BindableCollection<string>();
            OpensSuggestionBox = false;
        }

        #region SearchBox
        private string _searchBox;
        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                NotifyOfPropertyChange(() => SearchBox);
                if (searchesSuggestion) { SearchTitle(); }
            }
        }

        private void SearchTitle()
        {
            TitleSuggestions.Clear();

            if (string.IsNullOrEmpty(SearchBox))
            {
                OpensSuggestionBox = false;
                return;
            }

            OpensSuggestionBox = true;

            var titles = from title in titleList
                         where title.StartsWith(SearchBox, true, null)
                         select title;
            titles = titles.Count() < 8 ? titles.Take(titles.Count()) : titles.Take(8);
            TitleSuggestions.AddRange(titles);

            if (TitleSuggestions.Count == 0)
            {
                OpensSuggestionBox = false;
            }
        }
        #endregion

        #region SuggestionBox
        private bool _opensSuggestionBox;
        public bool OpensSuggestionBox
        {
            get { return _opensSuggestionBox; }
            set 
            {
                _opensSuggestionBox = value;
                NotifyOfPropertyChange(() => OpensSuggestionBox);
            }
        }

        public void SelectSuggestion(string title)
        {
            searchesSuggestion = false;

            SearchBox = title;
            OpensSuggestionBox = false;

            searchesSuggestion = true;
        }
        #endregion

        #region Favorite Item Adjustment
        public void AddItem()
        {
            if (!titleList.Any(x => x.Equals(SearchBox)))
            {
                MessageBox.Show("Does not exist.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            else if (FavoriteItems.Contains(SearchBox))
            {
                MessageBox.Show("Already exists.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            FavoriteItems.Add(SearchBox);
            favorite.Add(SearchBox);

            setUpdated.Invoke(true);
            SearchBox = string.Empty;
        }
        public void RemoveItem(string child)
        {
            FavoriteItems.Remove(child);
            favorite.Remove(child);
            setUpdated.Invoke(true);
        }
        #endregion
    }
}
