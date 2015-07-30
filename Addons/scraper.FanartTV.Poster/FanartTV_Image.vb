﻿' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################
' ################################################################################
' # This file is part of Ember Media Manager.                                    #
' #                                                                              #
' # Ember Media Manager is free software: you can redistribute it and/or modify  #
' # it under the terms of the GNU General Public License as published by         #
' # the Free Software Foundation, either version 3 of the License, or            #
' # (at your option) any later version.                                          #
' #                                                                              #
' # Ember Media Manager is distributed in the hope that it will be useful,       #
' # but WITHOUT ANY WARRANTY; without even the implied warranty of               #
' # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                #
' # GNU General Public License for more details.                                 #
' #                                                                              #
' # You should have received a copy of the GNU General Public License            #
' # along with Ember Media Manager.  If not, see <http://www.gnu.org/licenses/>. #
' ################################################################################

Imports System.IO
Imports EmberAPI
Imports RestSharp
Imports ScraperModule.FanartTVs
Imports NLog
Imports System.Diagnostics

Public Class FanartTV_Image
    Implements Interfaces.ScraperModule_Image_Movie
    Implements Interfaces.ScraperModule_Image_MovieSet
    Implements Interfaces.ScraperModule_Image_TV


#Region "Fields"
    Shared logger As Logger = NLog.LogManager.GetCurrentClassLogger()
    Public Shared ConfigModifier_Movie As New Structures.ScrapeModifier
    Public Shared ConfigModifier_MovieSet As New Structures.ScrapeModifier
    Public Shared ConfigModifier_TV As New Structures.ScrapeModifier
    Public Shared _AssemblyName As String

    ''' <summary>
    ''' Scraping Here
    ''' </summary>
    ''' <remarks></remarks>
    Private _MySettings_Movie As New sMySettings
    Private _MySettings_MovieSet As New sMySettings
    Private _MySettings_TV As New sMySettings
    Private _Name As String = "FanartTV_Image"
    Private _ScraperEnabled_Movie As Boolean = False
    Private _ScraperEnabled_MovieSet As Boolean = False
    Private _ScraperEnabled_TV As Boolean = False
    Private _setup_Movie As frmSettingsHolder_Movie
    Private _setup_MovieSet As frmSettingsHolder_MovieSet
    Private _setup_TV As frmSettingsHolder_TV
    Private _scraper As New FanartTVs.Scraper

#End Region 'Fields

#Region "Events"

    Public Event ModuleSettingsChanged_Movie() Implements Interfaces.ScraperModule_Image_Movie.ModuleSettingsChanged

    Public Event MovieScraperEvent_Movie(ByVal eType As Enums.ScraperEventType_Movie, ByVal Parameter As Object) Implements Interfaces.ScraperModule_Image_Movie.ScraperEvent

    Public Event SetupScraperChanged_Movie(ByVal name As String, ByVal State As Boolean, ByVal difforder As Integer) Implements Interfaces.ScraperModule_Image_Movie.ScraperSetupChanged

    Public Event SetupNeedsRestart_Movie() Implements Interfaces.ScraperModule_Image_Movie.SetupNeedsRestart

    Public Event ImagesDownloaded_Movie(ByVal Posters As List(Of MediaContainers.Image)) Implements Interfaces.ScraperModule_Image_Movie.ImagesDownloaded

    Public Event ProgressUpdated_Movie(ByVal iPercent As Integer) Implements Interfaces.ScraperModule_Image_Movie.ProgressUpdated


    Public Event ModuleSettingsChanged_MovieSet() Implements Interfaces.ScraperModule_Image_MovieSet.ModuleSettingsChanged

    Public Event MovieScraperEvent_MovieSet(ByVal eType As Enums.ScraperEventType_MovieSet, ByVal Parameter As Object) Implements Interfaces.ScraperModule_Image_MovieSet.ScraperEvent

    Public Event SetupScraperChanged_MovieSet(ByVal name As String, ByVal State As Boolean, ByVal difforder As Integer) Implements Interfaces.ScraperModule_Image_MovieSet.ScraperSetupChanged

    Public Event SetupNeedsRestart_MovieSet() Implements Interfaces.ScraperModule_Image_MovieSet.SetupNeedsRestart

    Public Event ImagesDownloaded_MovieSet(ByVal Posters As List(Of MediaContainers.Image)) Implements Interfaces.ScraperModule_Image_MovieSet.ImagesDownloaded

    Public Event ProgressUpdated_MovieSet(ByVal iPercent As Integer) Implements Interfaces.ScraperModule_Image_MovieSet.ProgressUpdated


    Public Event ModuleSettingsChanged_TV() Implements Interfaces.ScraperModule_Image_TV.ModuleSettingsChanged

    Public Event MovieScraperEvent_TV(ByVal eType As Enums.ScraperEventType_TV, ByVal Parameter As Object) Implements Interfaces.ScraperModule_Image_TV.ScraperEvent

    Public Event SetupScraperChanged_TV(ByVal name As String, ByVal State As Boolean, ByVal difforder As Integer) Implements Interfaces.ScraperModule_Image_TV.ScraperSetupChanged

    Public Event SetupNeedsRestart_TV() Implements Interfaces.ScraperModule_Image_TV.SetupNeedsRestart

    Public Event ImagesDownloaded_TV(ByVal Posters As List(Of MediaContainers.Image)) Implements Interfaces.ScraperModule_Image_TV.ImagesDownloaded

    Public Event ProgressUpdated_TV(ByVal iPercent As Integer) Implements Interfaces.ScraperModule_Image_TV.ProgressUpdated

#End Region 'Events

#Region "Properties"

    ReadOnly Property ModuleName() As String Implements Interfaces.ScraperModule_Image_Movie.ModuleName, Interfaces.ScraperModule_Image_MovieSet.ModuleName, Interfaces.ScraperModule_Image_TV.ModuleName
        Get
            Return _Name
        End Get
    End Property

    ReadOnly Property ModuleVersion() As String Implements Interfaces.ScraperModule_Image_Movie.ModuleVersion, Interfaces.ScraperModule_Image_MovieSet.ModuleVersion, Interfaces.ScraperModule_Image_TV.ModuleVersion
        Get
            Return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location).FileVersion.ToString
        End Get
    End Property

    Property ScraperEnabled_Movie() As Boolean Implements Interfaces.ScraperModule_Image_Movie.ScraperEnabled
        Get
            Return _ScraperEnabled_Movie
        End Get
        Set(ByVal value As Boolean)
            _ScraperEnabled_Movie = value
        End Set
    End Property

    Property ScraperEnabled_MovieSet() As Boolean Implements Interfaces.ScraperModule_Image_MovieSet.ScraperEnabled
        Get
            Return _ScraperEnabled_MovieSet
        End Get
        Set(ByVal value As Boolean)
            _ScraperEnabled_MovieSet = value
        End Set
    End Property

    Property ScraperEnabled_TV() As Boolean Implements Interfaces.ScraperModule_Image_TV.ScraperEnabled
        Get
            Return _ScraperEnabled_TV
        End Get
        Set(ByVal value As Boolean)
            _ScraperEnabled_TV = value
        End Set
    End Property

#End Region 'Properties

#Region "Methods"

    Function QueryScraperCapabilities_Movie(ByVal cap As Enums.ScraperCapabilities_Movie_MovieSet) As Boolean Implements Interfaces.ScraperModule_Image_Movie.QueryScraperCapabilities
        Select Case cap
            Case Enums.ScraperCapabilities_Movie_MovieSet.Banner
                Return ConfigModifier_Movie.MainBanner
            Case Enums.ScraperCapabilities_Movie_MovieSet.ClearArt
                Return ConfigModifier_Movie.MainClearArt
            Case Enums.ScraperCapabilities_Movie_MovieSet.ClearLogo
                Return ConfigModifier_Movie.MainClearLogo
            Case Enums.ScraperCapabilities_Movie_MovieSet.DiscArt
                Return ConfigModifier_Movie.MainDiscArt
            Case Enums.ScraperCapabilities_Movie_MovieSet.Fanart
                Return ConfigModifier_Movie.MainFanart
            Case Enums.ScraperCapabilities_Movie_MovieSet.Landscape
                Return ConfigModifier_Movie.MainLandscape
            Case Enums.ScraperCapabilities_Movie_MovieSet.Poster
                Return ConfigModifier_Movie.MainPoster
        End Select
        Return False
    End Function
    Function QueryScraperCapabilities_MovieSet(ByVal cap As Enums.ScraperCapabilities_Movie_MovieSet) As Boolean Implements Interfaces.ScraperModule_Image_MovieSet.QueryScraperCapabilities
        Select Case cap
            Case Enums.ScraperCapabilities_Movie_MovieSet.Banner
                Return ConfigModifier_MovieSet.MainBanner
            Case Enums.ScraperCapabilities_Movie_MovieSet.ClearArt
                Return ConfigModifier_MovieSet.MainClearArt
            Case Enums.ScraperCapabilities_Movie_MovieSet.ClearLogo
                Return ConfigModifier_MovieSet.MainClearLogo
            Case Enums.ScraperCapabilities_Movie_MovieSet.DiscArt
                Return ConfigModifier_MovieSet.MainDiscArt
            Case Enums.ScraperCapabilities_Movie_MovieSet.Fanart
                Return ConfigModifier_MovieSet.MainFanart
            Case Enums.ScraperCapabilities_Movie_MovieSet.Landscape
                Return ConfigModifier_MovieSet.MainLandscape
            Case Enums.ScraperCapabilities_Movie_MovieSet.Poster
                Return ConfigModifier_MovieSet.MainPoster
        End Select
        Return False
    End Function

    Function QueryScraperCapabilities_TV(ByVal cap As Enums.ScraperCapabilities_TV) As Boolean Implements Interfaces.ScraperModule_Image_TV.QueryScraperCapabilities
        Select Case cap
            Case Enums.ScraperCapabilities_TV.SeasonBanner
                Return ConfigModifier_TV.SeasonBanner
            Case Enums.ScraperCapabilities_TV.SeasonLandscape
                Return ConfigModifier_TV.SeasonLandscape
            Case Enums.ScraperCapabilities_TV.SeasonPoster
                Return ConfigModifier_TV.SeasonPoster
            Case Enums.ScraperCapabilities_TV.ShowBanner
                Return ConfigModifier_TV.MainBanner
            Case Enums.ScraperCapabilities_TV.ShowCharacterArt
                Return ConfigModifier_TV.MainCharacterArt
            Case Enums.ScraperCapabilities_TV.ShowClearArt
                Return ConfigModifier_TV.MainClearArt
            Case Enums.ScraperCapabilities_TV.ShowClearLogo
                Return ConfigModifier_TV.MainClearLogo
            Case Enums.ScraperCapabilities_TV.ShowFanart
                Return ConfigModifier_TV.MainFanart
            Case Enums.ScraperCapabilities_TV.ShowLandscape
                Return ConfigModifier_TV.MainLandscape
            Case Enums.ScraperCapabilities_TV.ShowPoster
                Return ConfigModifier_TV.MainPoster
        End Select
        Return False
    End Function

    Private Sub Handle_ModuleSettingsChanged_Movie()
        RaiseEvent ModuleSettingsChanged_Movie()
    End Sub

    Private Sub Handle_ModuleSettingsChanged_MovieSet()
        RaiseEvent ModuleSettingsChanged_MovieSet()
    End Sub

    Private Sub Handle_ModuleSettingsChanged_TV()
        RaiseEvent ModuleSettingsChanged_TV()
    End Sub

    Private Sub Handle_SetupNeedsRestart_Movie()
        RaiseEvent SetupNeedsRestart_Movie()
    End Sub

    Private Sub Handle_SetupNeedsRestart_MovieSet()
        RaiseEvent SetupNeedsRestart_MovieSet()
    End Sub

    Private Sub Handle_SetupNeedsRestart_TV()
        RaiseEvent SetupNeedsRestart_TV()
    End Sub

    Private Sub Handle_SetupScraperChanged_Movie(ByVal state As Boolean, ByVal difforder As Integer)
        ScraperEnabled_Movie = state
        RaiseEvent SetupScraperChanged_Movie(String.Concat(Me._Name, "_Movie"), state, difforder)
    End Sub

    Private Sub Handle_SetupScraperChanged_MovieSet(ByVal state As Boolean, ByVal difforder As Integer)
        ScraperEnabled_MovieSet = state
        RaiseEvent SetupScraperChanged_MovieSet(String.Concat(Me._Name, "_MovieSet"), state, difforder)
    End Sub

    Private Sub Handle_SetupScraperChanged_TV(ByVal state As Boolean, ByVal difforder As Integer)
        ScraperEnabled_TV = state
        RaiseEvent SetupScraperChanged_TV(String.Concat(Me._Name, "_TV"), state, difforder)
    End Sub

    Sub Init_Movie(ByVal sAssemblyName As String) Implements Interfaces.ScraperModule_Image_Movie.Init
        _AssemblyName = sAssemblyName
        LoadSettings_Movie()
    End Sub

    Sub Init_MovieSet(ByVal sAssemblyName As String) Implements Interfaces.ScraperModule_Image_MovieSet.Init
        _AssemblyName = sAssemblyName
        LoadSettings_MovieSet()
    End Sub

    Sub Init_TV(ByVal sAssemblyName As String) Implements Interfaces.ScraperModule_Image_TV.Init
        _AssemblyName = sAssemblyName
        LoadSettings_TV()
    End Sub

    Function InjectSetupScraper_Movie() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Image_Movie.InjectSetupScraper
        Dim Spanel As New Containers.SettingsPanel
        _setup_Movie = New frmSettingsHolder_Movie
        LoadSettings_Movie()
        _setup_Movie.chkEnabled.Checked = _ScraperEnabled_Movie
        _setup_Movie.chkScrapePoster.Checked = ConfigModifier_Movie.MainPoster
        _setup_Movie.chkScrapeFanart.Checked = ConfigModifier_Movie.MainFanart
        _setup_Movie.chkScrapeBanner.Checked = ConfigModifier_Movie.MainBanner
        _setup_Movie.chkScrapeClearArt.Checked = ConfigModifier_Movie.MainClearArt
        _setup_Movie.chkScrapeClearArtOnlyHD.Checked = _MySettings_Movie.ClearArtOnlyHD
        _setup_Movie.chkScrapeClearLogo.Checked = ConfigModifier_Movie.MainClearLogo
        _setup_Movie.chkScrapeClearLogoOnlyHD.Checked = _MySettings_Movie.ClearLogoOnlyHD
        _setup_Movie.chkScrapeDiscArt.Checked = ConfigModifier_Movie.MainDiscArt
        _setup_Movie.chkScrapeLandscape.Checked = ConfigModifier_Movie.MainLandscape
        _setup_Movie.txtApiKey.Text = _MySettings_Movie.ApiKey

        If Not String.IsNullOrEmpty(_MySettings_Movie.ApiKey) Then
            _setup_Movie.btnUnlockAPI.Text = Master.eLang.GetString(443, "Use embedded API Key")
            _setup_Movie.lblEMMAPI.Visible = False
            _setup_Movie.txtApiKey.Enabled = True
        End If

        _setup_Movie.orderChanged()

        Spanel.Name = String.Concat(Me._Name, "_Movie")
        Spanel.Text = "FanartTV"
        Spanel.Prefix = "FanartTVMovieMedia_"
        Spanel.Order = 110
        Spanel.Parent = "pnlMovieMedia"
        Spanel.Type = Master.eLang.GetString(36, "Movies")
        Spanel.ImageIndex = If(Me._ScraperEnabled_Movie, 9, 10)
        Spanel.Panel = Me._setup_Movie.pnlSettings

        AddHandler _setup_Movie.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_Movie
        AddHandler _setup_Movie.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_Movie
        AddHandler _setup_Movie.SetupNeedsRestart, AddressOf Handle_SetupNeedsRestart_Movie
        Return Spanel
    End Function

    Function InjectSetupScraper_MovieSet() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Image_MovieSet.InjectSetupScraper
        Dim Spanel As New Containers.SettingsPanel
        _setup_MovieSet = New frmSettingsHolder_MovieSet
        LoadSettings_MovieSet()
        _setup_MovieSet.chkEnabled.Checked = _ScraperEnabled_MovieSet
        _setup_MovieSet.chkScrapePoster.Checked = ConfigModifier_MovieSet.MainPoster
        _setup_MovieSet.chkScrapeFanart.Checked = ConfigModifier_MovieSet.MainFanart
        _setup_MovieSet.chkScrapeBanner.Checked = ConfigModifier_MovieSet.MainBanner
        _setup_MovieSet.chkScrapeClearArt.Checked = ConfigModifier_MovieSet.MainClearArt
        _setup_MovieSet.chkScrapeClearArtOnlyHD.Checked = _MySettings_MovieSet.ClearArtOnlyHD
        _setup_MovieSet.chkScrapeClearLogo.Checked = ConfigModifier_MovieSet.MainClearLogo
        _setup_MovieSet.chkScrapeClearLogoOnlyHD.Checked = _MySettings_MovieSet.ClearLogoOnlyHD
        _setup_MovieSet.chkScrapeDiscArt.Checked = ConfigModifier_MovieSet.MainDiscArt
        _setup_MovieSet.chkScrapeLandscape.Checked = ConfigModifier_MovieSet.MainLandscape
        _setup_MovieSet.txtApiKey.Text = _MySettings_MovieSet.ApiKey

        If Not String.IsNullOrEmpty(_MySettings_MovieSet.ApiKey) Then
            _setup_MovieSet.btnUnlockAPI.Text = Master.eLang.GetString(443, "Use embedded API Key")
            _setup_MovieSet.lblEMMAPI.Visible = False
            _setup_MovieSet.txtApiKey.Enabled = True
        End If

        _setup_MovieSet.orderChanged()

        Spanel.Name = String.Concat(Me._Name, "_MovieSet")
        Spanel.Text = "FanartTV"
        Spanel.Prefix = "FanartTVMovieSetMedia_"
        Spanel.Order = 110
        Spanel.Parent = "pnlMovieSetMedia"
        Spanel.Type = Master.eLang.GetString(1203, "MovieSets")
        Spanel.ImageIndex = If(Me._ScraperEnabled_MovieSet, 9, 10)
        Spanel.Panel = Me._setup_MovieSet.pnlSettings

        AddHandler _setup_MovieSet.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_MovieSet
        AddHandler _setup_MovieSet.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_MovieSet
        AddHandler _setup_MovieSet.SetupNeedsRestart, AddressOf Handle_SetupNeedsRestart_MovieSet
        Return Spanel
    End Function

    Function InjectSetupScraper_TV() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Image_TV.InjectSetupScraper
        Dim Spanel As New Containers.SettingsPanel
        _setup_TV = New frmSettingsHolder_TV
        LoadSettings_TV()
        _setup_TV.chkEnabled.Checked = _ScraperEnabled_TV
        _setup_TV.chkScrapeSeasonBanner.Checked = ConfigModifier_TV.SeasonBanner
        _setup_TV.chkScrapeSeasonLandscape.Checked = ConfigModifier_TV.SeasonLandscape
        _setup_TV.chkScrapeSeasonPoster.Checked = ConfigModifier_TV.SeasonPoster
        _setup_TV.chkScrapeShowBanner.Checked = ConfigModifier_TV.MainBanner
        _setup_TV.chkScrapeShowCharacterArt.Checked = ConfigModifier_TV.MainCharacterArt
        _setup_TV.chkScrapeShowClearArt.Checked = ConfigModifier_TV.MainClearArt
        _setup_TV.chkScrapeShowClearArtOnlyHD.Checked = _MySettings_TV.ClearArtOnlyHD
        _setup_TV.chkScrapeShowClearLogo.Checked = ConfigModifier_TV.MainClearLogo
        _setup_TV.chkScrapeShowClearLogoOnlyHD.Checked = _MySettings_TV.ClearLogoOnlyHD
        _setup_TV.chkScrapeShowFanart.Checked = ConfigModifier_TV.MainFanart
        _setup_TV.chkScrapeShowLandscape.Checked = ConfigModifier_TV.MainLandscape
        _setup_TV.chkScrapeShowPoster.Checked = ConfigModifier_TV.MainPoster
        _setup_TV.txtApiKey.Text = _MySettings_TV.ApiKey

        If Not String.IsNullOrEmpty(_MySettings_TV.ApiKey) Then
            _setup_TV.btnUnlockAPI.Text = Master.eLang.GetString(443, "Use embedded API Key")
            _setup_TV.lblEMMAPI.Visible = False
            _setup_TV.txtApiKey.Enabled = True
        End If

        _setup_TV.orderChanged()

        Spanel.Name = String.Concat(Me._Name, "_TV")
        Spanel.Text = "FanartTV"
        Spanel.Prefix = "FanartTVTVMedia_"
        Spanel.Order = 110
        Spanel.Parent = "pnlTVMedia"
        Spanel.Type = Master.eLang.GetString(653, "TV Shows")
        Spanel.ImageIndex = If(Me._ScraperEnabled_TV, 9, 10)
        Spanel.Panel = Me._setup_TV.pnlSettings

        AddHandler _setup_TV.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_TV
        AddHandler _setup_TV.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_TV
        AddHandler _setup_TV.SetupNeedsRestart, AddressOf Handle_SetupNeedsRestart_TV
        Return Spanel
    End Function

    Sub LoadSettings_Movie()
        _MySettings_Movie.ApiKey = clsAdvancedSettings.GetSetting("ApiKey", "", , Enums.Content_Type.Movie)
        _MySettings_Movie.ClearArtOnlyHD = clsAdvancedSettings.GetBooleanSetting("ClearArtOnlyHD", False, , Enums.Content_Type.Movie)
        _MySettings_Movie.ClearLogoOnlyHD = clsAdvancedSettings.GetBooleanSetting("ClearLogoOnlyHD", False, , Enums.Content_Type.Movie)

        ConfigModifier_Movie.MainPoster = clsAdvancedSettings.GetBooleanSetting("DoPoster", True, , Enums.Content_Type.Movie)
        ConfigModifier_Movie.MainFanart = clsAdvancedSettings.GetBooleanSetting("DoFanart", True, , Enums.Content_Type.Movie)
        ConfigModifier_Movie.MainBanner = clsAdvancedSettings.GetBooleanSetting("DoBanner", True, , Enums.Content_Type.Movie)
        ConfigModifier_Movie.MainClearArt = clsAdvancedSettings.GetBooleanSetting("DoClearArt", True, , Enums.Content_Type.Movie)
        ConfigModifier_Movie.MainClearLogo = clsAdvancedSettings.GetBooleanSetting("DoClearLogo", True, , Enums.Content_Type.Movie)
        ConfigModifier_Movie.MainDiscArt = clsAdvancedSettings.GetBooleanSetting("DoDiscArt", True, , Enums.Content_Type.Movie)
        ConfigModifier_Movie.MainLandscape = clsAdvancedSettings.GetBooleanSetting("DoLandscape", True, , Enums.Content_Type.Movie)
        ConfigModifier_Movie.MainEFanarts = ConfigModifier_Movie.MainFanart
        ConfigModifier_Movie.MainEThumbs = ConfigModifier_Movie.MainFanart
    End Sub

    Sub LoadSettings_MovieSet()
        _MySettings_MovieSet.ApiKey = clsAdvancedSettings.GetSetting("ApiKey", "", , Enums.Content_Type.MovieSet)
        _MySettings_MovieSet.ClearArtOnlyHD = clsAdvancedSettings.GetBooleanSetting("ClearArtOnlyHD", False, , Enums.Content_Type.MovieSet)
        _MySettings_MovieSet.ClearLogoOnlyHD = clsAdvancedSettings.GetBooleanSetting("ClearLogoOnlyHD", False, , Enums.Content_Type.MovieSet)

        ConfigModifier_MovieSet.MainPoster = clsAdvancedSettings.GetBooleanSetting("DoPoster", True, , Enums.Content_Type.MovieSet)
        ConfigModifier_MovieSet.MainFanart = clsAdvancedSettings.GetBooleanSetting("DoFanart", True, , Enums.Content_Type.MovieSet)
        ConfigModifier_MovieSet.MainBanner = clsAdvancedSettings.GetBooleanSetting("DoBanner", True, , Enums.Content_Type.MovieSet)
        ConfigModifier_MovieSet.MainClearArt = clsAdvancedSettings.GetBooleanSetting("DoClearArt", True, , Enums.Content_Type.MovieSet)
        ConfigModifier_MovieSet.MainClearLogo = clsAdvancedSettings.GetBooleanSetting("DoClearLogo", True, , Enums.Content_Type.MovieSet)
        ConfigModifier_MovieSet.MainDiscArt = clsAdvancedSettings.GetBooleanSetting("DoDiscArt", True, , Enums.Content_Type.MovieSet)
        ConfigModifier_MovieSet.MainLandscape = clsAdvancedSettings.GetBooleanSetting("DoLandscape", True, , Enums.Content_Type.MovieSet)
        ConfigModifier_MovieSet.MainEFanarts = ConfigModifier_MovieSet.MainFanart
        ConfigModifier_MovieSet.MainEThumbs = ConfigModifier_MovieSet.MainFanart
    End Sub

    Sub LoadSettings_TV()
        _MySettings_TV.ApiKey = clsAdvancedSettings.GetSetting("ApiKey", "", , Enums.Content_Type.TV)
        _MySettings_TV.ClearArtOnlyHD = clsAdvancedSettings.GetBooleanSetting("ClearArtOnlyHD", False, , Enums.Content_Type.TV)
        _MySettings_TV.ClearLogoOnlyHD = clsAdvancedSettings.GetBooleanSetting("ClearLogoOnlyHD", False, , Enums.Content_Type.TV)

        ConfigModifier_TV.SeasonBanner = clsAdvancedSettings.GetBooleanSetting("DoSeasonBanner", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.SeasonLandscape = clsAdvancedSettings.GetBooleanSetting("DoSeasonLandscape", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.SeasonPoster = clsAdvancedSettings.GetBooleanSetting("DoSeasonPoster", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.MainBanner = clsAdvancedSettings.GetBooleanSetting("DoShowBanner", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.MainCharacterArt = clsAdvancedSettings.GetBooleanSetting("DoShowCharacterArt", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.MainClearArt = clsAdvancedSettings.GetBooleanSetting("DoShowClearArt", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.MainClearLogo = clsAdvancedSettings.GetBooleanSetting("DoShowClearLogo", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.MainFanart = clsAdvancedSettings.GetBooleanSetting("DoShowFanart", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.MainLandscape = clsAdvancedSettings.GetBooleanSetting("DoShowLandscape", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.MainPoster = clsAdvancedSettings.GetBooleanSetting("DoShowPoster", True, , Enums.Content_Type.TV)
        ConfigModifier_TV.MainEFanarts = ConfigModifier_TV.MainFanart
    End Sub

    Sub SaveSettings_Movie()
        Using settings = New clsAdvancedSettings()
            settings.SetBooleanSetting("ClearArtOnlyHD", _MySettings_Movie.ClearArtOnlyHD, , , Enums.Content_Type.Movie)
            settings.SetBooleanSetting("ClearLogoOnlyHD", _MySettings_Movie.ClearLogoOnlyHD, , , Enums.Content_Type.Movie)
            settings.SetBooleanSetting("DoPoster", ConfigModifier_Movie.MainPoster, , , Enums.Content_Type.Movie)
            settings.SetBooleanSetting("DoFanart", ConfigModifier_Movie.MainFanart, , , Enums.Content_Type.Movie)
            settings.SetBooleanSetting("DoBanner", ConfigModifier_Movie.MainBanner, , , Enums.Content_Type.Movie)
            settings.SetBooleanSetting("DoClearArt", ConfigModifier_Movie.MainClearArt, , , Enums.Content_Type.Movie)
            settings.SetBooleanSetting("DoClearLogo", ConfigModifier_Movie.MainClearLogo, , , Enums.Content_Type.Movie)
            settings.SetBooleanSetting("DoDiscArt", ConfigModifier_Movie.MainDiscArt, , , Enums.Content_Type.Movie)
            settings.SetBooleanSetting("DoLandscape", ConfigModifier_Movie.MainLandscape, , , Enums.Content_Type.Movie)

            settings.SetSetting("ApiKey", _setup_Movie.txtApiKey.Text, , , Enums.Content_Type.Movie)
        End Using
    End Sub

    Sub SaveSettings_MovieSet()
        Using settings = New clsAdvancedSettings()
            settings.SetBooleanSetting("ClearArtOnlyHD", _MySettings_MovieSet.ClearArtOnlyHD, , , Enums.Content_Type.MovieSet)
            settings.SetBooleanSetting("ClearLogoOnlyHD", _MySettings_MovieSet.ClearLogoOnlyHD, , , Enums.Content_Type.MovieSet)
            settings.SetBooleanSetting("DoPoster", ConfigModifier_MovieSet.MainPoster, , , Enums.Content_Type.MovieSet)
            settings.SetBooleanSetting("DoFanart", ConfigModifier_MovieSet.MainFanart, , , Enums.Content_Type.MovieSet)
            settings.SetBooleanSetting("DoBanner", ConfigModifier_MovieSet.MainBanner, , , Enums.Content_Type.MovieSet)
            settings.SetBooleanSetting("DoClearArt", ConfigModifier_MovieSet.MainClearArt, , , Enums.Content_Type.MovieSet)
            settings.SetBooleanSetting("DoClearLogo", ConfigModifier_MovieSet.MainClearLogo, , , Enums.Content_Type.MovieSet)
            settings.SetBooleanSetting("DoDiscArt", ConfigModifier_MovieSet.MainDiscArt, , , Enums.Content_Type.MovieSet)
            settings.SetBooleanSetting("DoLandscape", ConfigModifier_MovieSet.MainLandscape, , , Enums.Content_Type.MovieSet)

            settings.SetSetting("ApiKey", _setup_MovieSet.txtApiKey.Text, , , Enums.Content_Type.MovieSet)
        End Using
    End Sub

    Sub SaveSettings_TV()
        Using settings = New clsAdvancedSettings()
            settings.SetBooleanSetting("ClearArtOnlyHD", _MySettings_TV.ClearArtOnlyHD, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("ClearLogoOnlyHD", _MySettings_TV.ClearLogoOnlyHD, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoSeasonBanner", ConfigModifier_TV.SeasonBanner, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoSeasonLandscape", ConfigModifier_TV.SeasonLandscape, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoSeasonPoster", ConfigModifier_TV.SeasonPoster, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoShowBanner", ConfigModifier_TV.MainBanner, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoShowCharacterArt", ConfigModifier_TV.MainCharacterArt, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoShowClearArt", ConfigModifier_TV.MainClearArt, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoShowClearLogo", ConfigModifier_TV.MainClearLogo, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoShowFanart", ConfigModifier_TV.MainFanart, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoShowLandscape", ConfigModifier_TV.MainLandscape, , , Enums.Content_Type.TV)
            settings.SetBooleanSetting("DoShowPoster", ConfigModifier_TV.MainPoster, , , Enums.Content_Type.TV)

            settings.SetSetting("ApiKey", _setup_TV.txtApiKey.Text, , , Enums.Content_Type.TV)
        End Using
    End Sub

    Sub SaveSetupScraper_Movie(ByVal DoDispose As Boolean) Implements Interfaces.ScraperModule_Image_Movie.SaveSetupScraper
        _MySettings_Movie.ClearArtOnlyHD = _setup_Movie.chkScrapeClearArtOnlyHD.Checked
        _MySettings_Movie.ClearLogoOnlyHD = _setup_Movie.chkScrapeClearLogoOnlyHD.Checked
        ConfigModifier_Movie.MainPoster = _setup_Movie.chkScrapePoster.Checked
        ConfigModifier_Movie.MainFanart = _setup_Movie.chkScrapeFanart.Checked
        ConfigModifier_Movie.MainBanner = _setup_Movie.chkScrapeBanner.Checked
        ConfigModifier_Movie.MainClearArt = _setup_Movie.chkScrapeClearArt.Checked
        ConfigModifier_Movie.MainClearLogo = _setup_Movie.chkScrapeClearLogo.Checked
        ConfigModifier_Movie.MainDiscArt = _setup_Movie.chkScrapeDiscArt.Checked
        ConfigModifier_Movie.MainLandscape = _setup_Movie.chkScrapeLandscape.Checked
        SaveSettings_Movie()
        If DoDispose Then
            RemoveHandler _setup_Movie.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_Movie
            RemoveHandler _setup_Movie.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_Movie
            RemoveHandler _setup_Movie.SetupNeedsRestart, AddressOf Handle_SetupNeedsRestart_Movie
            _setup_Movie.Dispose()
        End If
    End Sub

    Sub SaveSetupScraper_MovieSet(ByVal DoDispose As Boolean) Implements Interfaces.ScraperModule_Image_MovieSet.SaveSetupScraper
        _MySettings_MovieSet.ClearArtOnlyHD = _setup_MovieSet.chkScrapeClearArtOnlyHD.Checked
        _MySettings_MovieSet.ClearLogoOnlyHD = _setup_MovieSet.chkScrapeClearLogoOnlyHD.Checked
        ConfigModifier_MovieSet.MainPoster = _setup_MovieSet.chkScrapePoster.Checked
        ConfigModifier_MovieSet.MainFanart = _setup_MovieSet.chkScrapeFanart.Checked
        ConfigModifier_MovieSet.MainBanner = _setup_MovieSet.chkScrapeBanner.Checked
        ConfigModifier_MovieSet.MainClearArt = _setup_MovieSet.chkScrapeClearArt.Checked
        ConfigModifier_MovieSet.MainClearLogo = _setup_MovieSet.chkScrapeClearLogo.Checked
        ConfigModifier_MovieSet.MainDiscArt = _setup_MovieSet.chkScrapeDiscArt.Checked
        ConfigModifier_MovieSet.MainLandscape = _setup_MovieSet.chkScrapeLandscape.Checked
        SaveSettings_MovieSet()
        If DoDispose Then
            RemoveHandler _setup_MovieSet.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_MovieSet
            RemoveHandler _setup_MovieSet.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_MovieSet
            RemoveHandler _setup_MovieSet.SetupNeedsRestart, AddressOf Handle_SetupNeedsRestart_MovieSet
            _setup_MovieSet.Dispose()
        End If
    End Sub

    Sub SaveSetupScraper_TV(ByVal DoDispose As Boolean) Implements Interfaces.ScraperModule_Image_TV.SaveSetupScraper
        _MySettings_TV.ClearArtOnlyHD = _setup_TV.chkScrapeShowClearArtOnlyHD.Checked
        _MySettings_TV.ClearLogoOnlyHD = _setup_TV.chkScrapeShowClearLogoOnlyHD.Checked
        ConfigModifier_TV.SeasonBanner = _setup_TV.chkScrapeSeasonBanner.Checked
        ConfigModifier_TV.SeasonLandscape = _setup_TV.chkScrapeSeasonLandscape.Checked
        ConfigModifier_TV.SeasonPoster = _setup_TV.chkScrapeSeasonPoster.Checked
        ConfigModifier_TV.MainBanner = _setup_TV.chkScrapeShowBanner.Checked
        ConfigModifier_TV.MainCharacterArt = _setup_TV.chkScrapeShowCharacterArt.Checked
        ConfigModifier_TV.MainClearArt = _setup_TV.chkScrapeShowClearArt.Checked
        ConfigModifier_TV.MainClearLogo = _setup_TV.chkScrapeShowClearLogo.Checked
        ConfigModifier_TV.MainFanart = _setup_TV.chkScrapeShowFanart.Checked
        ConfigModifier_TV.MainLandscape = _setup_TV.chkScrapeShowLandscape.Checked
        ConfigModifier_TV.MainPoster = _setup_TV.chkScrapeShowPoster.Checked
        SaveSettings_TV()
        If DoDispose Then
            RemoveHandler _setup_TV.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_TV
            RemoveHandler _setup_TV.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_TV
            RemoveHandler _setup_TV.SetupNeedsRestart, AddressOf Handle_SetupNeedsRestart_TV
            _setup_TV.Dispose()
        End If
    End Sub

    Function Scraper(ByRef DBMovie As Structures.DBMovie, ByRef ImagesContainer As MediaContainers.SearchResultsContainer_Movie_MovieSet, ByVal ScrapeModifier As Structures.ScrapeModifier) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Image_Movie.Scraper
        logger.Trace("Started scrape FanartTV")

        LoadSettings_Movie()

        Dim Settings As New FanartTVs.Scraper.MySettings
        Settings.ApiKey = _MySettings_Movie.ApiKey
        Settings.ClearArtOnlyHD = _MySettings_Movie.ClearArtOnlyHD
        Settings.ClearLogoOnlyHD = _MySettings_Movie.ClearLogoOnlyHD

        Dim filterModifier As Structures.ScrapeModifier = Functions.ScrapeModifierAndAlso(ScrapeModifier, ConfigModifier_Movie)

        If Not String.IsNullOrEmpty(DBMovie.Movie.ID) Then
            ImagesContainer = _scraper.GetImages_Movie_MovieSet(DBMovie.Movie.ID, filterModifier, Settings)
        ElseIf Not String.IsNullOrEmpty(DBMovie.Movie.TMDBID) Then
            ImagesContainer = _scraper.GetImages_Movie_MovieSet(DBMovie.Movie.TMDBID, filterModifier, Settings)
        Else
            logger.Trace(String.Concat("No IMDB and TMDB ID exist to search: ", DBMovie.ListTitle))
        End If

        logger.Trace(New StackFrame().GetMethod().Name, "Finished scrape FanartTV")
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Function Scraper(ByRef DBMovieset As Structures.DBMovieSet, ByRef ImagesContainer As MediaContainers.SearchResultsContainer_Movie_MovieSet, ByVal ScrapeModifier As Structures.ScrapeModifier) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Image_MovieSet.Scraper
        logger.Trace("Started scrape FanartTV")

        LoadSettings_MovieSet()

        If String.IsNullOrEmpty(DBMovieset.MovieSet.ID) Then
            If DBMovieset.Movies IsNot Nothing AndAlso DBMovieset.Movies.Count > 0 Then
                DBMovieset.MovieSet.ID = ModulesManager.Instance.GetMovieCollectionID(DBMovieset.Movies.Item(0).Movie.ID)
            End If
        End If

        If Not String.IsNullOrEmpty(DBMovieset.MovieSet.ID) Then
            Dim Settings As New FanartTVs.Scraper.MySettings
            Settings.ApiKey = _MySettings_MovieSet.ApiKey
            Settings.ClearArtOnlyHD = _MySettings_MovieSet.ClearArtOnlyHD
            Settings.ClearLogoOnlyHD = _MySettings_MovieSet.ClearLogoOnlyHD

            Dim filterModifier As Structures.ScrapeModifier = Functions.ScrapeModifierAndAlso(ScrapeModifier, ConfigModifier_MovieSet)

            ImagesContainer = _scraper.GetImages_Movie_MovieSet(DBMovieset.MovieSet.ID, filterModifier, Settings)
        End If

        logger.Trace("Finished scrape FanartTV")
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Function Scraper(ByRef DBTV As Structures.DBTV, ByRef ImagesContainer As MediaContainers.SearchResultsContainer_TV, ByVal ScrapeModifier As Structures.ScrapeModifier) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Image_TV.Scraper
        logger.Trace("Started scrape FanartTV")

        LoadSettings_TV()

        Dim Settings As FanartTVs.Scraper.MySettings
        Settings.ApiKey = _MySettings_TV.ApiKey
        Settings.ClearArtOnlyHD = _MySettings_TV.ClearArtOnlyHD
        Settings.ClearLogoOnlyHD = _MySettings_TV.ClearLogoOnlyHD

        Dim filterModifier As Structures.ScrapeModifier = Functions.ScrapeModifierAndAlso(ScrapeModifier, ConfigModifier_TV)

        If DBTV.TVEp IsNot Nothing Then
            Return Nothing
        Else
            If Not String.IsNullOrEmpty(DBTV.TVShow.TVDB) Then
                ImagesContainer = _scraper.GetImages_TV(DBTV.TVShow.TVDB, filterModifier, Settings)
            Else
                logger.Trace(String.Concat("No TVDB ID exist to search: ", DBTV.ListTitle))
            End If
        End If

        logger.Trace(New StackFrame().GetMethod().Name, "Finished scrape FanartTV")
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Public Sub ScraperOrderChanged_Movie() Implements EmberAPI.Interfaces.ScraperModule_Image_Movie.ScraperOrderChanged
        _setup_Movie.orderChanged()
    End Sub

    Public Sub ScraperOrderChanged_MovieSet() Implements EmberAPI.Interfaces.ScraperModule_Image_MovieSet.ScraperOrderChanged
        _setup_MovieSet.orderChanged()
    End Sub

    Public Sub ScraperOrderChanged_TV() Implements EmberAPI.Interfaces.ScraperModule_Image_TV.ScraperOrderChanged
        _setup_TV.orderChanged()
    End Sub

#End Region 'Methods

#Region "Nested Types"

    Structure sMySettings

#Region "Fields"
        Dim ApiKey As String
        Dim ClearArtOnlyHD As Boolean
        Dim ClearLogoOnlyHD As Boolean
        Dim GetEnglishImages As Boolean
        Dim GetBlankImages As Boolean
        Dim PrefLanguage As String
        Dim PrefLanguageOnly As Boolean
#End Region 'Fields

    End Structure

#End Region 'Nested Types

End Class