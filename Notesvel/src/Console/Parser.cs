﻿//********************************
// (c) 2020 Ada Maynek
// This software is released under the MIT License.
//********************************

namespace Maynek.Notesvel.Console
{
    public enum OptionType : int
    {
        NoValue = 0,
        RequireValue = 1,
    }

    public enum ErrorType : int
    {
        None = 0,
        NoValue = 1,
    }

    public enum WarningType : int
    {
        None = 0,
        UndefinedOption = 1,
    }

    public class ArgumentEventArgs
    {
        private readonly List<string> list = [];
        public string[] Args { get { return this.list.ToArray(); } }

        public void Add(string arg)
        {
            this.list.Add(arg);
        }
    }

    public class ErrorEventArgs
    {
        public ErrorType Type { get; set; } = ErrorType.None;
        public string OptionName { get; set; } = string.Empty;
    }

    public class WarningEventArgs
    {
        public WarningType Type { get; set; } = WarningType.None;
        public string OptionName = string.Empty;
    }

    public class OptionEventArgs
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public delegate void ArgumentEventHandler(object sender, ArgumentEventArgs e);
    public delegate void ErrorEventHandler(object sender, ErrorEventArgs warning);
    public delegate void WarningEventHandler(object sender, WarningEventArgs warning);
    public delegate void OptionEventHandler(object sender, OptionEventArgs e);

    public class OptionDefinition
    {
        public string[] Names { get; private set; }
        public OptionType Type { get; set; } = OptionType.NoValue;
        public OptionEventHandler EventHandler { get; set; }

        private OptionDefinition() { }

        public OptionDefinition(params string[] names) : this()
        {
            this.Names = names;
        }
    }

    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    public class Parser
    {
        //================================
        // Definitions 
        //================================
        private const uint STATE_DEFAULT    = 0x80000000;
        private const uint STATE_NEUTRAL    = 0x80000001;
        private const uint STATE_SUCCESSFUL = 0x80000010;
        private const uint STATE_FAIL       = 0x80000011;
        private const uint STATE_MAX = STATE_DEFAULT - 1;


        //================================
        // Fields
        //================================
        private uint stateCount;
        private uint currentState;
        private string currentOptionName;
        private readonly Dictionary<uint, OptionEventHandler> noArgOptionEvents;
        private readonly Dictionary<uint, OptionEventHandler> oneArgOptionEvents;
        private readonly Dictionary<string, uint> nextStates;

        public ArgumentEventHandler? ArgumentEvent;
        public ErrorEventHandler? ErrorEvent;
        public WarningEventHandler? WarningEvent;


        //================================
        // Properties
        //================================
        public bool Successful { get { return (this.currentState == STATE_SUCCESSFUL); } }


        //================================
        // Constructor
        //================================
        public Parser()
        {
            this.stateCount = 0;
            this.currentState = STATE_DEFAULT;
            this.currentOptionName = string.Empty;
            this.noArgOptionEvents = [];
            this.oneArgOptionEvents = [];
            this.nextStates = [];

            this.ArgumentEvent = null;
            this.WarningEvent = null;
        }


        //================================
        // Methods
        //================================
        private static bool isOptionName(string arg)
        {
            return (arg[0] == '-');
        }

        private void updateStateCount()
        {
            this.stateCount++;

            if (this.stateCount > STATE_MAX)
            {
                throw new Exception();
            }
        }

        private void finishOption(string arg)
        {
            var optionArg = new OptionEventArgs()
            {
                Name = this.currentOptionName,
                Value = arg
            };

            if (this.noArgOptionEvents.TryGetValue(this.currentState, out OptionEventHandler? noArgOptionEventHandler))
            {
                noArgOptionEventHandler?.Invoke(this, optionArg);
            }

            if (this.oneArgOptionEvents.TryGetValue(this.currentState, out OptionEventHandler? oneArgOptionEventsHandler))
            {
                oneArgOptionEventsHandler?.Invoke(this, optionArg);
            }

            this.currentState = STATE_NEUTRAL;
            this.currentOptionName = string.Empty;
        }

        public void AddOptionDefinition(OptionDefinition definition)
        {
            if (definition == null)
            {
                throw new Exception("OptionDefinition is null.");
            }

            if (definition.EventHandler == null)
            {
                throw new Exception("OptionDefinition needs EventHandler.");
            }

            foreach (var name in definition.Names)
            {
                if (!Parser.isOptionName(name))
                {
                    throw new Exception("\"" + name + "\" is invalid option name.");
                }

                this.updateStateCount();
                if (definition.Type == OptionType.RequireValue)
                {
                    this.nextStates.Add(name, this.stateCount);
                    this.oneArgOptionEvents.Add(this.stateCount, definition.EventHandler);
                }
                else
                {
                    this.nextStates.Add(name, this.stateCount);
                    this.noArgOptionEvents.Add(this.stateCount, definition.EventHandler);
                }
            }
        }

        public void Parse(string[] args)
        {
            var commandArgs = new ArgumentEventArgs();

            this.currentState = STATE_NEUTRAL;
            foreach (string arg in args)
            {
                if (Parser.isOptionName(arg))
                {
                    if (this.currentState != STATE_NEUTRAL)
                    {
                        if (this.ErrorEvent != null)
                        {
                            var e = new ErrorEventArgs()
                            {
                                Type = ErrorType.NoValue,
                                OptionName = this.currentOptionName
                            };
                            this.ErrorEvent?.Invoke(this, e);
                        }
                        this.currentState = STATE_FAIL;
                        return;
                    }

                    if (this.nextStates.TryGetValue(arg, out uint nextState))
                    {
                        this.currentState = nextState;
                        this.currentOptionName = arg;

                        if (this.noArgOptionEvents.ContainsKey(this.currentState))
                        {
                            this.finishOption(string.Empty);
                        }
                    }
                    else
                    {
                        if (this.WarningEvent != null)
                        {
                            var warning = new WarningEventArgs()
                            {
                                Type = WarningType.UndefinedOption,
                                OptionName = arg
                            };
                            this.WarningEvent?.Invoke(this, warning);
                        }
                    }
                }
                else
                {
                    if (this.currentState == STATE_NEUTRAL)
                    {
                        commandArgs.Add(arg);
                    }
                    else
                    {
                        this.finishOption(arg);
                    }                    
                }
            }

            this.ArgumentEvent?.Invoke(this, commandArgs);

            this.currentState = STATE_SUCCESSFUL;
        }
    }
}
