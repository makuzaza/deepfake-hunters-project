// Enums.cs — ALL game-wide enumerations (replaces your existing Enums.cs)
public enum ScreenId    { None, OnbLogin, OnbPortrait, Dashboard, HRForm, Chat, BriefQueue, Result, Phone }
public enum Motivation  { Money, Impact, Recognition }
public enum InboxAction { OpenHRForm, OpenChat, OpenBriefQueue, OpenNews, Dismiss }
public enum RiskLevel   { None, Low, Medium, High }
public enum EndingType  { Complicit, Whistleblower, PassiveResistance }

[System.Obsolete("Use Motivation instead")]
public enum ProfileType { Money, Impact, Recognition }
