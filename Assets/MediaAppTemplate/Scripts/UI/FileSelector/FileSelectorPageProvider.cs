// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DaydreamElements.Common;

namespace Daydream.MediaAppTemplate
{

    /// This script provides pages that represent the file system to a PagedScrollRect
    /// It also integrates with BreadcrumbTrail to support hierarchical navigation.
    public class FileSelectorPageProvider : MonoBehaviour, IPageProvider
    {
        public delegate void FileChosenDelegate(DVDFileInfo file, int fileIndex);

        public event FileChosenDelegate OnFileChosen;

        /// The spacing between pages in local coordinates.
        [Tooltip("The spacing between pages.")]
        public float spacing = 2000.0f;

        [SerializeField]
        private string[] allowedExtensions;

        [SerializeField]
        private GameObject pagePrefab;

        [SerializeField]
        private BreadcrumbTrail breadcrumbTrail;

        private DVDDirectoryInfo workingDirectory;
        private DVDDirectoryInfo[] directories;
        private DVDFileInfo[] files;
        private int childCount;
        private int pageCount;
        private string pagePrefabName;

        private const string HOME_NAME = "Home";

        public int MaxChildrenPerPage
        {
            get
            {
                if (pagePrefab == null)
                {
                    Debug.LogError("pagePrefab is not set.");
                    return 0;
                }

                FileSelectorPage page = pagePrefab.GetComponent<FileSelectorPage>();
                if (page == null)
                {
                    Debug.LogError("pagePrefab does not contain FileSelectorPage component.");
                    return 0;
                }

                return page.MaxTileCount;
            }
        }

        public string[] AllowedExtensions
        {
            get
            {
                return allowedExtensions;
            }
            set
            {
                allowedExtensions = value;
                Refresh();
            }
        }

        public DVDDirectoryInfo WorkingDirectory
        {
            get
            {
                return workingDirectory;
            }
            private set
            {
                workingDirectory = value;
                Refresh();
            }
        }

        public DVDDirectoryInfo[] SubDirectories
        {
            get
            {
                return directories;
            }
        }

        public DVDFileInfo[] SubFiles
        {
            get
            {
                return files;
            }
        }

        private GameObjectPool PagePool
        {
            get
            {
                ObjectPoolManager poolManager = ObjectPoolManager.Instance;
                Assert.IsNotNull(poolManager);

                GameObjectPool pool =
                  poolManager.GetPool<GameObjectPool>(pagePrefabName);

                if (pool != null)
                {
                    return pool;
                }

                pool = new GameObjectPool(pagePrefab, 2);
                poolManager.AddPool(pagePrefab.name, pool);

                return pool;
            }
        }

        public float GetSpacing()
        {
            return spacing;
        }

        public int GetNumberOfPages()
        {
            return pageCount;
        }

        public RectTransform ProvidePage(int index)
        {
            Debug.Log("5555555555555555555555ProvidePageindex"+ index);
            GameObject pageObj = PagePool.Borrow();
            RectTransform page = pageObj.GetComponent<RectTransform>();

            Vector2 middleAnchor = new Vector2(0.5f, 0.5f);
            page.anchorMax = middleAnchor;
            page.anchorMin = middleAnchor;

            FileSelectorPage fileSelectorPage = page.GetComponent<FileSelectorPage>();
            int maxChildrenPerPage = MaxChildrenPerPage;
            int startingChildIndex = index * maxChildrenPerPage;
            int childIndex = startingChildIndex;
            Debug.Log("directories.Length" + directories.Length + "files.Length" + files.Length);
            while (childIndex < startingChildIndex + maxChildrenPerPage && childIndex < childCount)
            {
                FileSelectorTile tile;
                if (childIndex < directories.Length)
                {
                    Debug.Log("directories[childIndex]:name" + directories[childIndex].name);
                    tile = fileSelectorPage.AddDirectoryTile(directories[childIndex]);
                    tile.SetToDirectory(directories[childIndex]);
                    tile.OnDirectorySelected += OnDirectorySelected;
                }
                else
                {
                    int fileIndex = childIndex - directories.Length;
                    tile = fileSelectorPage.AddFileTile(files[fileIndex], fileIndex);
                    tile.OnFileSelected += OnFileSelected;
                }

                childIndex++;
            }

            return page;
        }

        public void RemovePage(int index, RectTransform page)
        {
            FileSelectorPage fileSelectorPage = page.GetComponent<FileSelectorPage>();
            fileSelectorPage.Reset();
            PagePool.Return(page.gameObject);
        }

        void Awake()
        {
            breadcrumbTrail.OnBreadcrumbChosen += OnBreadcrumbChosen;
            DvDInterface.GetInstance().DiskReadyCallBack += Refresh;
            pagePrefabName = pagePrefab.name;
            workingDirectory = new DVDDirectoryInfo("Home");
        }

        void Start()
        {
            BreadcrumbData breadcrumbData = new BreadcrumbData();
            breadcrumbData.displayName = HOME_NAME;
            breadcrumbData.name = WorkingDirectory.fullName;
            breadcrumbTrail.AddBreadcrumb(breadcrumbData);
        }

        void OnDestroy()
        {
            if (breadcrumbTrail != null)
            {
                breadcrumbTrail.OnBreadcrumbChosen -= OnBreadcrumbChosen;
            }
            DvDInterface.GetInstance().DiskReadyCallBack -= Refresh;
        }

        public void Refresh()
        {
            if (workingDirectory.javaObj == null)
            {
                DvDInterface.GetInstance().AnalyzeDirectorys();
                directories = DvDInterface.GetInstance().GetDirectorys();
                files = DvDInterface.GetInstance().GetFiles();
            }
            else
            {
                workingDirectory.AnalyzeDirectorys();
                directories = workingDirectory.GetDirectorys();
                //if (allowedExtensions.Length != 0) {
                //  files = allowedExtensions.SelectMany(filter =>
                //    workingDirectory.GetFiles("*" + filter)).OrderBy(x => x.name).ToArray();
                //} else {
                files = workingDirectory.GetFiles();
            }
            childCount = directories.Length + files.Length;
            pageCount = Mathf.CeilToInt((float)childCount / (float)MaxChildrenPerPage);
            pageCount = Mathf.Max(pageCount, 1);
            // If the PagedScrollRect already has active pages we need to Reset it
            // So that it will pick up the change to the working directories.
            PagedScrollRect scrollRect = GetComponent<PagedScrollRect>();
            if (scrollRect != null && scrollRect.ActivePage != null)
            {
                scrollRect.Reset();
            }
        }

        private void OnDirectorySelected(DVDDirectoryInfo directory)
        {
            WorkingDirectory = directory;
            BreadcrumbData breadcrumbData = new BreadcrumbData();
            breadcrumbData.displayName = directory.name;
            breadcrumbData.name = directory.fullName;
            breadcrumbTrail.AddBreadcrumb(breadcrumbData);
        }

        private void OnFileSelected(DVDFileInfo file, int fileIndex)
        {
            if (OnFileChosen != null)
            {
                OnFileChosen(file, fileIndex);
            }
        }

        private void OnBreadcrumbChosen(BreadcrumbData data)
        {
            DVDDirectoryInfo directory = new DVDDirectoryInfo(data.name);
            WorkingDirectory = directory;
        }

        public void OnDiskUpdateCD()
        {
            breadcrumbTrail.ClearBreadcrumbs();
            BreadcrumbData breadcrumbData = new BreadcrumbData();
            breadcrumbData.displayName = HOME_NAME;
            breadcrumbData.name = WorkingDirectory.fullName;
            breadcrumbTrail.AddBreadcrumb(breadcrumbData);
            WorkingDirectory = new DVDDirectoryInfo("Home");
        }
    }
}
