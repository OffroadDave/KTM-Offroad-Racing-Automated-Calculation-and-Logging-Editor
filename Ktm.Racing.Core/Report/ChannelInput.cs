namespace Ktm.Racing.Core;

/// <summary>Ein aus der Input-Excel aktivierter Kanal: VCH-Kanalname, Ausgabename (Alias) und Einheit.</summary>
public sealed record ChannelInput(string Channel, string Alias, string Unit);
