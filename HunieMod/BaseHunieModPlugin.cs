﻿using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using UnityEngine;

namespace HunieMod
{
    /// <summary>
    /// The base plugin type that adds HuniePop-specific functionality over the default <see cref="BaseUnityPlugin"/>
    /// </summary>
    [BepInPlugin("com.lounger.huniemod", "HunieMod", "1.0.0.0")]
    public class BaseHunieModPlugin : BaseUnityPlugin
    {
        #region Game core

        /// <summary>
        /// General game manager containing all other managers and general game settings
        /// </summary>
        protected static GameManager Game => GameManager.System;

        /// <summary>
        /// General data class containing nearly all of the game's assets definitions
        /// </summary>
        protected static GameData Data => GameManager.Data;

        /// <summary>
        /// The game's main camera
        /// </summary>
        protected static Camera MainCam => GameManager.System.gameCamera.mainCamera;

        /// <summary>
        /// The root container on which all visible elements are placed
        /// </summary>
        protected static Stage GameStage => GameManager.Stage;

        /// <summary>
        /// Manages location traveling, arrivals and departures
        /// </summary>
        protected static LocationManager Location => Game.Location;

        /// <summary>
        /// The main player's settings, stats and active game variables
        /// </summary>
        protected static PlayerManager Player => Game.Player;

        /// <summary>
        /// Manages going to puzzle locations and puzzle game logic, whereas <see cref="PuzzleManager.Game"/>
        /// manages the more visual aspects of the puzzle game
        /// </summary>
        protected static PuzzleManager Puzzle => Game.Puzzle;

        #endregion

        #region Locations

        /// <summary>
        /// The definition of the location that is currently active
        /// </summary>
        protected static LocationDefinition CurrentLocationDef => Player?.currentLocation;

        /// <summary>
        /// The ID of the location that is currently active
        /// </summary>
        protected static LocationId? CurrentLocation => (LocationId?)CurrentLocationDef?.id;

        /// <summary>
        /// Instances of all the location definitions in the game
        /// </summary>
        protected static LocationDefinition[] AllLocations => Data.Locations.GetLocationsByType(LocationType.DATE)
                                                                .Concat(Data.Locations.GetLocationsByType(LocationType.NORMAL))
                                                                .ToArray();

        #endregion

        #region Girls

        /// <summary>
        /// Instances of all the cellphone app definitions in the game
        /// </summary>
        protected static CellAppDefinition[] AllCellApps => Resources.FindObjectsOfTypeAll(typeof(CellAppDefinition)) as CellAppDefinition[];

        /// <summary>
        /// The definition of the girl that is currently active
        /// </summary>
        protected static GirlDefinition CurrentGirlDef => Player?.currentGirl;

        /// <summary>
        /// The visual object of the main girl currently on the stage
        /// </summary>
        protected static Girl CurrentStageGirlObject => GameStage.girl;

        /// <summary>
        /// The visual object of the alt. girl currently on the stage
        /// </summary>
        protected static Girl CurrentStageAltGirlObject => GameStage.altGirl;

        /// <summary>
        /// The ID of the girl that is currently active
        /// </summary>
        protected static GirlId? CurrentGirl => (GirlId?)CurrentGirlDef?.id;

        /// <summary>
        /// The ID of the main girl currently on the stage
        /// </summary>
        protected static GirlId? CurrentStageGirl => (GirlId?)CurrentStageGirlObject?.definition.id;

        /// <summary>
        /// The ID of the alt. girl currently on the stage
        /// </summary>
        protected static GirlId? CurrentStageAltGirl => (GirlId?)CurrentStageAltGirlObject?.definition.id;

        /// <summary>
        /// Instances of all the girl definitions in the game
        /// </summary>
        protected static List<GirlDefinition> AllGirls => Data.Girls.GetAll();

        /// <summary>
        /// Tries to find an instance of a girl's definition that matches with the specified ID
        /// </summary>
        /// <param name="girlId">The ID of the girl to find</param>
        /// <returns>The definition of the girl, or default if not found</returns>
        public static GirlDefinition GetGirl(GirlId girlId) => AllGirls.FirstOrDefault(girl => (GirlId)girl.id == girlId);

        /// <summary>
        /// Tries to find an instance of a girl's definition with the specified name, case-insensitive
        /// </summary>
        /// <param name="firstName">The first name of the girl to find</param>
        /// <returns>The definition of the girl, or default if not found</returns>
        public static GirlDefinition GetGirl(string firstName) => AllGirls.FirstOrDefault(girl => string.Equals(girl.firstName, firstName, StringComparison.OrdinalIgnoreCase));

        #endregion

        #region Events

        private static EventManager events;

        /// <summary>
        /// Event helper that wraps certain key game events
        /// </summary>
        protected static EventManager Events => events = events ?? new EventManager();

        /// <summary>
        /// Event helper that wraps certain key game events
        /// </summary>
        public class EventManager
        {
            /// <summary>
            /// Fires after <see cref="GameManager.Pause"/> has frozen all game elements but the cellphone
            /// </summary>
            public event GameManager.GameManagerDelegate GamePause
            {
                add { Game.GamePauseEvent += value; }
                remove { Game.GamePauseEvent -= value; }
            }

            /// <summary>
            /// Fires after <see cref="GameManager.Unpause"/> has unfrozen all game elements
            /// </summary>
            public event GameManager.GameManagerDelegate GameUnpause
            {
                add { Game.GameUnpauseEvent += value; }
                remove { Game.GameUnpauseEvent -= value; }
            }

            /// <summary>
            /// Fires after LocationManager.OnLocationArrival() has initialized the arrival sequence and before the location is "settled"
            /// </summary>
            public event LocationManager.LocationDelegate LocationArrive
            {
                add { Location.LocationArriveEvent += value; }
                remove { Location.LocationArriveEvent -= value; }
            }

            /// <summary>
            /// Fires after LocationManager.OnLocationDeparture() has set up the new location and transition screen.
            /// Shortly before LocationManager.ArriveLocation() fires
            /// </summary>
            public event LocationManager.LocationDelegate LocationDepart
            {
                add { Location.LocationDepartEvent += value; }
                remove { Location.LocationDepartEvent -= value; }
            }

            /// <summary>
            /// Fires when <see cref="Stage.OnStart"/> has setup all it's child elements
            /// </summary>
            public event Stage.StageDelegate StageStarted
            {
                add { GameStage.StageStartedEvent += value; }
                remove { GameStage.StageStartedEvent -= value; }
            }
        }

        #endregion
    }
}
