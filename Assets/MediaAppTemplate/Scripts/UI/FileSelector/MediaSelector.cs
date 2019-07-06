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
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DaydreamElements.Common;

namespace Daydream.MediaAppTemplate
{

    /// This script instantiates the video player and plays the selected video when
    /// the file selector is used to select a video file.
    [RequireComponent(typeof(FileSelectorPageProvider))]
    public class MediaSelector : MonoBehaviour
    {
        [SerializeField]
        private GameObject mediaSelectorContainer;

        [SerializeField]
        private GameObject videoPlayerPrefab;

        [SerializeField]
        private GameObject photoPlayerPrefab;

        [SerializeField]
        private MediaScreenController mediaScreenController;

        private PagedScrollRect scrollRect;
        private GameObject currentPlayerObject;
        private BaseMediaPlayer currentPlayer;
        private int currentFileIndex = -1;

        private const string PATH_PREFIX = "file://";
        public FileSelectorPageProvider fileSelector;

        [SerializeField]
        private GameObject PlayerRoot;
        void Awake()
        {
            scrollRect = GetComponent<PagedScrollRect>();
            fileSelector = GetComponent<FileSelectorPageProvider>();
            Assert.IsNotNull(fileSelector);
            fileSelector.OnFileChosen += OnFileChosen;
            //fileSelector.AllowedExtensions = MediaHelpers.EXTENSIONS_TO_MEDIA_TYPE.Keys.ToArray();
            MediaPlayerEventDispatcher.OnExitMedia += OnExitMedia;
            MediaPlayerEventDispatcher.OnNextFile += OnNextFile;
            MediaPlayerEventDispatcher.OnPreviousFile += OnPreviousFile;
            PlayerDataControl.GetInstance().StopPlayCallBack += DestroyMediaPlayer;
        }

        void OnDestroy()
        {
            fileSelector = GetComponent<FileSelectorPageProvider>();
            if (fileSelector != null)
            {
                fileSelector.OnFileChosen -= OnFileChosen;
            }
            MediaPlayerEventDispatcher.OnExitMedia -= OnExitMedia;
            MediaPlayerEventDispatcher.OnNextFile -= OnNextFile;
            MediaPlayerEventDispatcher.OnPreviousFile -= OnPreviousFile;
            PlayerDataControl.GetInstance().StopPlayCallBack -= DestroyMediaPlayer;
        }

        void Update()
        {
            if (scrollRect == null)
            {
                return;
            }

            if (mediaScreenController == null)
            {
                return;
            }

            scrollRect.scrollingEnabled = !mediaScreenController.IsMovingScreen;
        }

        private void OnFileChosen(DVDFileInfo file, int fileIndex)
        {
            MediaHelpers.MediaType mediaType = MediaHelpers.GetMediaType(file);
            if (mediaType == MediaHelpers.MediaType.Invalid)
            {
                return;
            }
            //CreateMediaPlayer(mediaType);
            //TryPlayMedia(mediaType, file, currentPlayer);
            if (mediaType == MediaHelpers.MediaType.Video)
            {
                JVideoDescriptionInfo jVideo = new JVideoDescriptionInfo(-1, file.fileName, file.fileUrl, file.fileUrl,
                    0, 0, (int)StereoType.ST3D_LR, 0, 0, System.DateTime.Now, null, null);
                PlayerDataControl.GetInstance().SetJVideoDscpInfoByLiveUrl(jVideo);
                PlayerRoot.SetActive(true);
            }
            else
                return;

            mediaSelectorContainer.SetActive(false);
            currentFileIndex = fileIndex;
        }

        private void DestroyMediaPlayer()
        {
            //if (currentPlayerObject != null)
            //{
            //    Destroy(currentPlayerObject);
            //    mediaSelectorContainer.SetActive(true);
            //    currentFileIndex = -1;
            //}
            mediaSelectorContainer.SetActive(true);
            currentFileIndex = -1;
            PlayerRoot.SetActive(false);
        }

        private void CreateMediaPlayer(MediaHelpers.MediaType mediaType)
        {
            DestroyMediaPlayer();

            switch (mediaType)
            {
                case MediaHelpers.MediaType.Video:
                    currentPlayerObject = GameObject.Instantiate(videoPlayerPrefab);
                    BaseVideoPlayer videoPlayer = currentPlayerObject.GetComponentInChildren<BaseVideoPlayer>();
                    currentPlayer = videoPlayer;
                    break;
                case MediaHelpers.MediaType.Photo:
                    currentPlayerObject = GameObject.Instantiate(photoPlayerPrefab);
                    BasePhotoPlayer photoPlayer = currentPlayerObject.GetComponentInChildren<BasePhotoPlayer>();
                    currentPlayer = photoPlayer;
                    break;
                default:
                    Debug.LogError("Invalid Media Type.");
                    break;
            }
        }

        private static bool TryPlayMedia(MediaHelpers.MediaType mediaType, DVDFileInfo file, BaseMediaPlayer player)
        {
            switch (mediaType)
            {
                case MediaHelpers.MediaType.Video:
                    BaseVideoPlayer videoPlayer = player as BaseVideoPlayer;
                    if (videoPlayer != null)
                    {
                        PlayOptions options = new PlayOptions();
                        options.Path = file.fileUrl;
                        options.Type = BaseVideoPlayer.VideoType.Other;
                        videoPlayer.Play(options);
                        return true;
                    }
                    break;
                case MediaHelpers.MediaType.Photo:
                    //BasePhotoPlayer photoPlayer = player as BasePhotoPlayer;
                    //if (photoPlayer != null) {
                    //  photoPlayer.ShowPhoto(file);
                    //  return true;
                    //}
                    break;
                default:
                    Debug.LogError("Invalid Media Type.");
                    break;
            }

            return false;
        }

        private void OnExitMedia()
        {
            DestroyMediaPlayer();
        }

        private void OnNextFile()
        {
            IncrementFile(false);
        }

        private void OnPreviousFile()
        {
            IncrementFile(true);
        }

        private void IncrementFile(bool previousDirection)
        {
            fileSelector = GetComponent<FileSelectorPageProvider>();
            Assert.IsNotNull(fileSelector);

            DVDFileInfo[] files = fileSelector.SubFiles;

            if (files == null || files.Length <= 1)
            {
                return;
            }

            if (currentPlayer == null)
            {
                return;
            }

            if (currentFileIndex == -1)
            {
                return;
            }

            int nextIndex;
            if (previousDirection)
            {
                nextIndex = currentFileIndex - 1;
            }
            else
            {
                nextIndex = currentFileIndex + 1;
            }

            if (nextIndex >= files.Length)
            {
                nextIndex = 0;
            }
            else if (nextIndex < 0)
            {
                nextIndex = files.Length - 1;
            }

            DVDFileInfo nextFile = files[nextIndex];
            MediaHelpers.MediaType mediaType = MediaHelpers.GetMediaType(nextFile);

            bool success = TryPlayMedia(mediaType, nextFile, currentPlayer);

            // Wrong media player type.
            if (!success)
            {
                CreateMediaPlayer(mediaType);
                TryPlayMedia(mediaType, nextFile, currentPlayer);
                PlaybackControlsManager playbackControlsManager =
                  currentPlayerObject.GetComponentInChildren<PlaybackControlsManager>();
                if (playbackControlsManager != null)
                {
                    playbackControlsManager.SetPlaybackControlsOpen(true);
                }
            }

            mediaSelectorContainer.SetActive(false);
            currentFileIndex = nextIndex;
        }
    }
}
