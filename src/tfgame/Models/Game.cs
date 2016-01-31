using System;
using System.Collections.Generic;

namespace TT.Web.Models
{

    #region a

    public class Game
    {
        public List<Scene> Scenes { get; set; }
        public List<Connection> Connections { get; set; }
        public List<Character> Characters { get; set; }
        public List<Form> Forms { get; set; }
    }

    public class TimeMessage
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }

    public class Scene
    {
        public string dbName { get; set; }
        public string CasualName { get; set; }
        public List<TimeMessage> SceneLog { get; set; }
        public List<Connection> Connections { get; set; }
        public List<Event> Events { get; set; }
        public List<Requirement> EntryRequirements { get; set; }
        public List<Action> EntrySuccessActions { get; set; }
        public List<Action> EntryFailActions { get; set; }
        public string Img { get; set; }
        public string Description { get; set; }
    }

    public class Character
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Form Form { get; set; }
        public decimal Health { get; set; }
        public decimal Mana { get; set; }
        public decimal HealthMax { get; set; }
        public decimal ManaMax { get; set; }
        public string AtScene { get; set; }
    }

    public class Form
    {
        public string Img { get; set; }
        public string FormName { get; set; }
        public string FormNameCasual { get; set; }
        public string Description1st { get; set; }
        public string Description3rd { get; set; }
        public string Traits { get; set; }
    }

    public class Connection
    {
        public string ParentSceneDbName { get; set; }
        public string ChildSceneDbName { get; set; }
        public string DestinationName { get; set; }
        public string Position { get; set; }
    }

    public class Event
    {
        public string EventId { get; set; }
        public Trigger Trigger { get; set; }
        public List<Requirement> Requirements { get; set; }
        public List<Action> PassActions { get; set; }
        public List<Action> FailActions { get; set; }
    }

    public class Trigger
    {
        public string TriggerType { get; set; }
        public string Value { get; set; }
        public string Amount { get; set; }
    }

    public class PlogEntry
    {
        public string Message { get; set; }
        public string Img { get; set; }
        public string PortraitImg { get; set; }
    }

    public class Requirement {
        List<OrRequirement> OrRequirements { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Amount { get; set; }
        public Boolean Inverse { get; set; }
        public PlogEntry FailMessage { get; set; }
        public PlogEntry PassMessage { get; set; }
    }

    public class OrRequirement
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Amount { get; set; }
        public Boolean Inverse { get; set; }
        public PlogEntry FailMessage { get; set; }
        public PlogEntry PassMessage { get; set; }
    }

    public class Action
    {
        public List<Requirement> Requirements { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Amount { get; set; }
        public PlogEntry PassMessage { get; set; }
    }

    #endregion

    #region helper only

    public class SceneImgDescription
    {
        public string Img { get; set; }
        public string Description { get; set; }
    }

    public class SelfQuery
    {
        public List<PlogEntry> pLog { get; set; }
        public Character character { get; set; }

        public SelfQuery()
        {
            pLog = new List<PlogEntry>();
        }

    }

    public class SceneData
    {
        public List<Connection> Connections { get; set; }
        public List<Character> Characters { get; set; }
        public string SceneName { get; set; }
        public string Img { get; set; }
        public Character Me { get; set; }
    }

    public class BooleanPlogEntry
    {
        public Boolean Passed { get; set; }
        public PlogEntry Plog { get; set; }
    }

    

#endregion

    
}